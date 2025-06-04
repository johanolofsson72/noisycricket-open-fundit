using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace Shared.Extensions;

public static class StringExtensions
{
    public static bool IsDate(this string date)
    {
        return DateTime.TryParse(date, out _);
    }
    
    public static int CountWords(this string text)
    {
        var punctuationCharacters = text.Where(char.IsPunctuation).Distinct().ToArray();
        var words = text.Split().Select(x => x.Trim(punctuationCharacters));
        return words.Count(x => !string.IsNullOrWhiteSpace(x));
    }
    
    public static string ExtractCommaSeparatedTextFromHtml(this string? html)
    {
        if (html == null)
        {
            return "";
        }

        var plainText = Regex.Replace(html, "<[^>]+?>", ", ");
        plainText = System.Net.WebUtility.HtmlDecode(plainText).Trim();

        return plainText;
    }
    
    public static string ExtractListTextFromHtml(this string? html)
    {
        if (html == null)
        {
            return "";
        }

        var plainText = Regex.Replace(html, "<[^>]+?>", "\r\n");
        plainText = System.Net.WebUtility.HtmlDecode(plainText).Trim();

        return plainText;
    }
    
    public static string TruncateAtWord(this string? input, int length, bool appendDots = false)
    {
        if (input == null || input.Length < length)
            if (input != null)
                return input;
        var iNextSpace = input!.LastIndexOf(" ", length, StringComparison.Ordinal);
        var trimmedInput = $"{input[..((iNextSpace > 0) ? iNextSpace : length)].Trim()}";

        if (appendDots)
        {
            return trimmedInput + "...";
        }
        return trimmedInput;
    }
    
    public static bool IsValidEmailAddress(this string email)
    {
        bool valid = true;

        try
        {
            MailAddress emailAddress = new(email);
        }
        catch
        {
            valid = false;
        }

        return valid;
    }

    public static bool IsHtml(this string val)
    {
        Regex tagRegex = new(@"<[^>]+>");
        return tagRegex.IsMatch(val);
    }

    public static string HtmlDecode(this string? plain)
    {
        if (plain is null) return string.Empty;
        return HttpUtility.HtmlDecode(plain);
    }

    public static string HtmlEncode(this string plain)
    {
        return HttpUtility.HtmlEncode(plain);
    }

    public static string UrlDecode(this string plain)
    {
        return HttpUtility.UrlDecode(plain);
    }

    public static string UrlEncode(this string plain)
    {
        return HttpUtility.UrlEncode(plain);
    }

    public static string ParseAsHtml(this string plain)
    {
        Regex regex = new(@"(\n|\n\n|\r\n)");
        return regex.Replace(plain, "<br />");
    }

    public static string ParseAsPlain(this string html)
    {
        Regex regex = new(@"(<br  />|<br   />|<br    />|<br />|<br/>|</ br>|</  br>|</   br>|</    br>|</br>)");
        return Regex.Replace(regex.Replace(html, "\n"), "<.*?>", string.Empty);
    }

    private static readonly Regex _htmlRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
    public static string NormalizeText(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        // Remove html tags
        text = _htmlRegex.Replace(text, string.Empty);

        text = text.Replace("'", "´");
        text = text.Replace("{", " ");
        text = text.Replace("}", " ");
        text = text.Replace("[", " ");
        text = text.Replace("]", " ");
        text = text.Replace("/", " ");
        text = text.Replace("|", " ");

        return text;
    }
}
