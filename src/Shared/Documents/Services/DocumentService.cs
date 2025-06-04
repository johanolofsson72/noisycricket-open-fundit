

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Data.DbContext;
using Shared.Documents.DTOs;
using Shared.Documents.Entities;
using Shared.Global.Structs;

namespace Shared.Documents.Services;

public class DocumentService(IDbContextFactory<ApplicationDbContext> factory, IConfiguration configuration, IHttpClientFactory httpClientFactory, IEasyCachingProvider cache, ApplicationService applicationService)
{
    public static IEnumerable<int> GetDocumentTypes()
    {
        return Enumerable.Range(0, Enum.GetNames(typeof(DocumentType)).Length).ToList();
    }
    
    public async Task<Result<bool, Exception>> AggregateAsync(CancellationToken ct, int documentId = 0)
    {
        try
        {
            var emptyPayload = new {};
            var httpClient = httpClientFactory.CreateClient("api");
            var response = await httpClient.PostAsJsonAsync($"/api/v1/jobs/aggregate/documents/" + documentId, emptyPayload, ct);

            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<DocumentDto, Exception>> CreateDocumentAsync(CreateDocumentDto dto, CancellationToken ct, bool aggregate = true)
    {
        try
        {
            if (dto.ApplicationId == 0) throw new Exception("SchemaVersion is required");
            if (dto.RequirementTypeId == 0) throw new Exception("RequirementTypeId is required");
            if (dto.FileName == string.Empty) throw new Exception("FileName is required");
            if (dto.MimeType == string.Empty) throw new Exception("MimeType is required");
            if (dto.Extension == string.Empty) throw new Exception("Extension is required");

            var document = new Document
            {
                ApplicationId = dto.ApplicationId,
                StatusId = 2,
                RequirementTypeId = dto.RequirementTypeId,
                DeliveryTypeId = dto.DeliveryTypeId,
                FileName = dto.FileName,
                MimeType = dto.MimeType,
                Extension = dto.Extension,
                Path = dto.Path,
                Phrases = dto.Phrases,
                Summarize = dto.Summarize,
                Binary = dto.Binary,
                Metadata = dto.Metadata.Select(x => new DocumentMetaData{ DocumentMetaDataIdentifier = x.DocumentMetaDataIdentifier, Key = x.Key, Value = x.Value }).ToList(),
                CreatedDate = DateTime.UtcNow,
                IsDelivered = false,
                IsSigned = false,
                IsCertified = false,
                IsLocked = false
            };

            await using var context = await factory.CreateDbContextAsync(ct);
            await context.Documents.AddAsync(document, ct);
            await context.SaveChangesAsync(ct);

            await applicationService.AddApplicationAuditAsync(dto.ApplicationId, "Document Created", [ document.Id.ToString()], "DocumentService.CreateDocumentAsync", ct);

            if (aggregate) await AggregateAsync(ct, document.Id);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Documents.ToDescriptionString(), ct);

            return document.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> UpdateDocumentAsync(int documentId, UpdateDocumentDto dto, CancellationToken ct)
    {
        try
        {
            if (documentId == 0) throw new Exception("documentId is required");
            if (dto.StatusId == 0) throw new Exception("StatusId is required");
            if (dto.RequirementTypeId == 0) throw new Exception("RequirementTypeId is required");
            if (dto.FileName == string.Empty) throw new Exception("FileName is required");
            if (dto.MimeType == string.Empty) throw new Exception("MimeType is required");
            if (dto.Extension == string.Empty) throw new Exception("Extension is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var document = await context.Documents
                .AsTracking()
                .Where(x => x.Id == documentId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Document not found");

            document.StatusId = dto.StatusId;
            document.RequirementTypeId = dto.RequirementTypeId;
            document.DeliveryTypeId = dto.DeliveryTypeId;
            document.FileName = dto.FileName;
            document.MimeType = dto.MimeType;
            document.Extension = dto.Extension;
            document.Path = dto.Path;
            document.Phrases = dto.Phrases;
            document.Summarize = dto.Summarize;
            document.Binary = dto.Binary;
            document.Metadata = dto.Metadata.Select(x => new DocumentMetaData{ Key = x.Key, Value = x.Value }).ToList();
            document.IsDelivered = dto.IsDelivered;
            document.IsSigned = dto.IsSigned;
            document.IsCertified = dto.IsCertified;
            document.IsLocked = dto.IsLocked;
            document.DeliverDate = dto.DeliverDate;
            document.SignedDate = dto.SignedDate;
            document.CertifiedDate = dto.CertifiedDate;
            document.LockedDate = dto.LockedDate;
            
            context.Documents.Update(document);
            await context.SaveChangesAsync(ct);

            await applicationService.AddApplicationAuditAsync(document.ApplicationId, "Document Updated", [ document.Id.ToString()], "DocumentService.UpdateDocumentAsync", ct);

            await AggregateAsync(ct, document.Id);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Documents.ToDescriptionString(), ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteDocumentAsync(int documentId, CancellationToken ct)
    {
        try
        {
            if (documentId == 0) throw new Exception("documentId is required");
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Documents.ToDescriptionString(), ct);
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var document = await context.Documents
                .Where(x => x.Id == documentId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Document not found");
            
            document.StatusId = 19;
            
            context.Documents.Update(document);
            await context.SaveChangesAsync(ct);
            
            await applicationService.AddApplicationAuditAsync(document.ApplicationId, "Document Deleted", [ document.Id.ToString()], "DocumentService.DeleteDocumentAsync", ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<DocumentDto, Exception>> DocumentByIdAsync(int documentId, CancellationToken ct)
    {
        try
        {
            if (documentId == 0) throw new Exception("documentId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var document = await context.Documents
                .AsNoTracking()
                .Where(x => x.Id == documentId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Document not found");

            return document.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<DocumentsSummaryDto>, Exception>> DocumentsSummaryAsync(CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(ct);
            var summary = await context.Documents
                    .AsNoTracking()
                    .Where(x => x.StatusId != 19)
                    .Select(x => new DocumentsSummaryDto
                    {
                        Id = x.Id, Name = x.FileName
                    })
                    .ToListAsync(ct) ?? throw new Exception("Documents not found");

            return summary;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<DocumentDto>, Exception>> DocumentsByApplicationIdAndDocumentTypeAsync(int applicationId, int requirementTypeId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Documents.ToDescriptionString()}_DocumentsByApplicationIdAndDocumentTypeAsync_{applicationId}_{requirementTypeId}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(ct);
                    var documents = GetDocumentsByApplicationIdAndDocumentType(context, applicationId, requirementTypeId) ?? throw new Exception("Documents not found");
                    return (await documents.ToListAsync(cancellationToken: ct)).ToList();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<DocumentDto>, Exception>> DocumentsByApplicationIdAndFileNameAsync(int applicationId, string fileName, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Documents.ToDescriptionString()}_DocumentsByApplicationIdAndFileNameAsync_{applicationId}_{fileName}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(ct);
                    var documents = GetDocumentsByApplicationIdAndFileName(context, applicationId, fileName) ?? throw new Exception("Documents not found");
                    return (await documents.ToListAsync(cancellationToken: ct)).ToList();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<DocumentDto>, Exception>> DocumentsByApplicationIdAndRequiredDocumentIdsTypeAsync(int applicationId, int[] requirementTypeIds, CancellationToken ct)
    {
        /*try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");

            var documents = await context.Documents
                .AsNoTracking()
                .Where(x => x.ApplicationId == applicationId && requirementTypeIds.Contains(x.RequirementTypeId) && x.StatusId != 19)
                .ToListAsync(ct) ?? throw new Exception("Documents not found");

            return documents.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }*/
        
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            
            var cacheResult = await cache.GetAsync(
                $"{CacheKeyPrefix.Documents.ToDescriptionString()}_DocumentsByApplicationIdAndRequiredDocumentIdsTypeAsync_{applicationId}_{string.Join("_", requirementTypeIds)}", 
                async () =>
                {
                    await using var context = await factory.CreateDbContextAsync(ct);
                    var documents = GetDocumentsByApplicationIdAndRequiredDocumentIdsType(context, applicationId, requirementTypeIds) ?? throw new Exception("Documents not found");
                    return (await documents.ToListAsync(cancellationToken: ct)).ToList();
                }, TimeSpan.FromMinutes(configuration.GetValue<int>("EasyCaching:CacheTimeInMinutes")), ct);

            return cacheResult.Value.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private static readonly Func<ApplicationDbContext, int, string, IAsyncEnumerable<Document>> GetDocumentsByApplicationIdAndFileName = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId, string fileName) => 
            context.Documents
                .AsNoTracking()
                .TagWith("GetDocumentsByApplicationIdAndFileName")
                .Where(x => x.ApplicationId == applicationId && x.StatusId != 19 && x.FileName == fileName));
    
    private static readonly Func<ApplicationDbContext, int, int, IAsyncEnumerable<Document>> GetDocumentsByApplicationIdAndDocumentType = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId, int requirementTypeId) => 
            context.Documents
                .AsNoTracking()
                .TagWith("GetDocumentsByApplicationIdAndDocumentType")
                .Where(x => x.ApplicationId == applicationId && x.RequirementTypeId == requirementTypeId && x.StatusId != 19));
    
    private static readonly Func<ApplicationDbContext, int, int[], IAsyncEnumerable<Document>> GetDocumentsByApplicationIdAndRequiredDocumentIdsType = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int applicationId, int[] requirementTypeIds) => 
            context.Documents
                .AsNoTracking()
                .TagWith("GetDocumentsByApplicationIdAndRequiredDocumentIdsType")
                .Where(x => x.ApplicationId == applicationId && requirementTypeIds.Contains(x.RequirementTypeId) && x.StatusId != 19));
}
