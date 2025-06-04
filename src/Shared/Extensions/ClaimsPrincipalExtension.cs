using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Shared.Extensions;

public static class ClaimsPrincipalExtension
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        try
        {
            return Convert.ToInt32(user.Claims.First(x => x.Type == "userId").Value);
        }
        catch
        {
            return 0;
        }
    }
    public static IEnumerable<string> GetApplicationRoles(this ClaimsPrincipal user)
    {
        try
        {
            return user.Claims.Where(x => x.Type == "applicationRole").Select(x => x.Value);
        }
        catch
        {
            return new List<string>();
        }
    }
    public static int GetOrganizationId(this ClaimsPrincipal user)
    {
        try
        {
            return Convert.ToInt32(user.Claims.First(x => x.Type == "organizationId").Value);
        }
        catch
        {
            return 0;
        }
    }
    public static string GetOrganizationNumber(this ClaimsPrincipal user)
    {
        try
        {
            return user.Claims.First(x => x.Type == "organizationNumber").Value;
        }
        catch
        {
            return string.Empty;
        }
    }
    public static string GetFirstName(this ClaimsPrincipal user)
    {
        try
        {
            return user.Claims.First(x => x.Type == "firstName").Value;
        }
        catch
        {
            return string.Empty;
        }
    }
    public static string GetLastName(this ClaimsPrincipal user)
    {
        try
        {
            return user.Claims.First(x => x.Type == "lastName").Value;
        }
        catch
        {
            return string.Empty;
        }
    }
    public static string GetFullName(this ClaimsPrincipal user)
    {
        try
        {
            return user.Claims.First(x => x.Type == "fullName").Value;
        }
        catch
        {
            return string.Empty;
        }
    }
    public static string GetStartPage(this ClaimsPrincipal user)
    {
        try
        {
            return user.Claims.First(x => x.Type == "startPage").Value;
        }
        catch
        {
            return string.Empty;
        }
    }
    public static bool GetIsAdministrator(this ClaimsPrincipal user)
    {
        try
        {
            return Convert.ToBoolean(user.Claims.First(x => x.Type == "isAdministrator").Value);
        }
        catch
        {
            return false;
        }
    }
    /*public static UserType GetUserType(this ClaimsPrincipal user)
    {
        try
        {
            return user.Claims.First(x => x.Type == "userType").Value switch 
            { 
                "Employee" => UserType.Admin,
                "Client" => UserType.Client,
                _ => UserType.Default
            };
        }
        catch
        {
            return UserType.Default;
        }
    }*/
}
