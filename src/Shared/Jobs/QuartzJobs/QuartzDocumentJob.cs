using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Quartz;
using Shared.Data.DbContext;
using Shared.Notifications;
using Telerik.Windows.Documents.Common.FormatProviders;
using Telerik.Windows.Documents.Flow.FormatProviders.Pdf;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.FormatProviders.Rtf;
using Telerik.Windows.Documents.Flow.FormatProviders.Txt;
using Telerik.Windows.Documents.Flow.Model;

namespace Shared.Jobs.QuartzJobs;

public class QuartzDocumentJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzDocument", "SingleJob");

    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);
            var cache = scope.ServiceProvider.GetRequiredService<IEasyCachingProvider>();
            var data = jobExecutionContext.JobDetail.JobDataMap;
            var documentId = data.GetInt("documentId");

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzDocument started for id: {documentId}" + $", at: {DateTime.UtcNow:hh:mm:ss}");
        
            // Get document
            var document = await context.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Phrases.Length < 5 && x.Summarize.Length < 5 && x.Id == documentId);

            // Update document
            if (document != null)
            {
                if (document.Path.Length > 0 || document.Binary.Length > 0)
                {
                    await SummarizeDocument(context, document);
                }
                
                // Clear Cache
                await cache.RemoveByPrefixAsync(CacheKeyPrefix.Documents.ToDescriptionString());
            }
            
            Console.WriteLine($"<= QuartzDocument succeeded for id: {documentId}" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzDocument failed " + ex);
        }
    }

    private async Task SummarizeDocument(ApplicationDbContext context, Document document)
    {
        try
        {
            var copy = new RadFlowDocument();
            var path = string.Empty;
            if (document.Binary.Length > 0)
            {
                copy = ReadText(new MemoryStream(document.Binary));
            }
            else
            {
                path = document.Path;

                var file = new FileInfo(document.Path);

                if (!file.Exists) return;

                copy = ReadFile(file.FullName);
            }

            if (copy is null) return;

            var metaData = document.Metadata;
            var index = 0;
            foreach (var md in metaData)
            {
                md.DocumentMetaDataIdentifier = index++;
            }
            document.Metadata = metaData;

            var provider = new TxtFormatProvider();
            var txt = provider.Export(copy, TimeSpan.FromSeconds(1));
            var ai = TextSummarizer.Summarize.Execute(txt);

            if (ai.Phrases.Any() || ai.SummarizedSentence.Length > 0)
            {
                var updateResult = await context.Documents
                    .Where(a => a.Id == document.Id)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(a => a.Phrases, string.Join(",", ai.Phrases))
                        .SetProperty(a => a.Summarize, ai.SummarizedSentence)) > 0;
                
                if (updateResult) Console.WriteLine($@"<= updated document: {document.Id}, phrases: {ai.Phrases.Count()}, summarized: {ai.SummarizedSentence.Length}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
    }

    private static Stream GetStreamWithStreamWriter(string sampleString, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var stream = new MemoryStream(encoding.GetByteCount(sampleString));
        using var writer = new StreamWriter(stream, encoding, -1, true);
        writer.Write(sampleString);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    private RadFlowDocument ReadFile(string path)
    {
        IFormatProvider<RadFlowDocument>? fileFormatProvider = GetImportFormatProvider(Path.GetExtension(path));

        RadFlowDocument document;

        if (fileFormatProvider is null) return new RadFlowDocument();

        using var input = new FileStream(path, FileMode.Open, FileAccess.Read);
#pragma warning disable CS0618 // Type or member is obsolete
        document = fileFormatProvider.Import(input);
#pragma warning restore CS0618 // Type or member is obsolete

        return document;
    }

    private RadFlowDocument ReadText(Stream stream)
    {
        IFormatProvider<RadFlowDocument>? fileFormatProvider = GetImportFormatProvider(".html");

#pragma warning disable CS0618 // Type or member is obsolete
        RadFlowDocument document = fileFormatProvider!.Import(stream);
#pragma warning restore CS0618 // Type or member is obsolete

        return document;
    }

    private IFormatProvider<RadFlowDocument>? GetImportFormatProvider(string extension)
    {
        IFormatProvider<RadFlowDocument> fileFormatProvider;
        switch (extension)
        {
            case ".docx":
                fileFormatProvider = new DocxFormatProvider();
                break;
            case ".rtf":
                fileFormatProvider = new RtfFormatProvider();
                break;
            case ".html":
                fileFormatProvider = new HtmlFormatProvider();
                break;
            case ".txt":
                fileFormatProvider = new TxtFormatProvider();
                break;
            case ".pdf":
                fileFormatProvider = new PdfFormatProvider();
                break;
            default:
                fileFormatProvider = null!;
                break;
        }

        if (fileFormatProvider == null)
        {
            Console.WriteLine(@"The chosen file cannot be read with the supported providers.");
            //throw new NotSupportedException("The chosen file cannot be read with the supported providers.");
        }

        return fileFormatProvider;
    }

    private IFormatProvider<RadFlowDocument> GetExportFormatProvider(string fileName, out string mimeType)
    {
        IFormatProvider<RadFlowDocument> fileFormatProvider;
        string extension = Path.GetExtension(fileName);
        switch (extension)
        {
            case ".docx":
                fileFormatProvider = new DocxFormatProvider();
                mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                break;
            case ".rtf":
                fileFormatProvider = new RtfFormatProvider();
                mimeType = "application/rtf";
                break;
            case ".html":
                fileFormatProvider = new HtmlFormatProvider();
                mimeType = "text/html";
                break;
            case ".txt":
                fileFormatProvider = new TxtFormatProvider();
                mimeType = "text/plain";
                break;
            case ".pdf":
                fileFormatProvider = new PdfFormatProvider();
                mimeType = "application/pdf";
                break;
            default:
                fileFormatProvider = null!;
                mimeType = string.Empty;
                break;
        }

        if (fileFormatProvider == null)
        {
            throw new NotSupportedException("The chosen format cannot be exported with the supported providers.");
        }

        return fileFormatProvider;
    }
    
    protected static string Decompress(string compressedString)
    {
        byte[] decompressedBytes;

        var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

        using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
        {
            using (var decompressedStream = new MemoryStream())
            {
                decompressorStream.CopyTo(decompressedStream);

                decompressedBytes = decompressedStream.ToArray();
            }
        }

        return Encoding.UTF8.GetString(decompressedBytes);
    }
    
}