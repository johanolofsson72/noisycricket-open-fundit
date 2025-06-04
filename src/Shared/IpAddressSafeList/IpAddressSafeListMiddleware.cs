using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Shared.IpAddressSafeList;

public class IpAddressSafeListMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpAddressSafeListMiddleware> _logger;
    private readonly IConfiguration _configuration;
    private readonly byte[][] _safelist;

    public IpAddressSafeListMiddleware(
        RequestDelegate next,
        ILogger<IpAddressSafeListMiddleware> logger,
        IConfiguration configuration,
        string safelist)
    {
        var ips = safelist.Split(';');
        _safelist = new byte[ips.Length][];
        for (var i = 0; i < ips.Length; i++)
        {
            _safelist[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
        }

        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Method == HttpMethod.Get.Method && _configuration.GetValue<bool>("UseIpAddressSafeList"))
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            _logger.LogDebug("=> Request from Remote IP address: {RemoteIp}", remoteIp);

            var bytes = remoteIp?.GetAddressBytes();
            var badIp = _safelist.All(address => bytes != null && !address.SequenceEqual(bytes));

            if (badIp)
            {
                _logger.LogWarning("<= Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
            else
            {
                _logger.LogInformation("<= Request from Remote IP address: {RemoteIp}", remoteIp);
            }
        }

        await _next.Invoke(context);
    }
}
