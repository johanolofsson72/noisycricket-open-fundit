
using Microsoft.Extensions.Configuration;
using Shared.Email.Entities;

namespace Shared.Global.Configurations;

public static class SystemConfiguration
{
    public static IConfiguration? Configuration { get; set; }

    public static EmailConfiguration? EmailConfiguration { get; set; }

    public static bool IsDevelopmentEnvironment { get; set; }
}