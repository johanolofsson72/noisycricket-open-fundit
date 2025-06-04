using Microsoft.AspNetCore.Builder;

namespace Shared.Global.Interface;

public interface IEndpoint
{
    void RegisterEndpoints(WebApplication app);
}