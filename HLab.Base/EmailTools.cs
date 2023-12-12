using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HLab.Base;

public class EmailTools
{
    public static bool IsValid(String email)
    {
        if(String.IsNullOrEmpty(email))
            return true;

        // Use IdnMapping class to convert Unicode domain names.
        email = Regex.Replace(email, @"(@)(.+)$", DomainMapper);
        if(email == "")
            return false;

        // Return true if strIn is in valid e-mail format.
        return Regex.IsMatch(email, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase);
    }

    static string DomainMapper(Match match)
    {
        // IdnMapping class with default property values.
        IdnMapping idn = new IdnMapping();

        string domainName = match.Groups[2].Value;
        try
        {
            domainName = idn.GetAscii(domainName);
        }
        catch(ArgumentException)
        {
            return "";
        }
        return match.Groups[1].Value + domainName;
    }    }