using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Controls.DTOs;
using Shared.Controls.Entities;
using Shared.Data.DbContext;
using Shared.Global.Structs;

namespace Shared.Controls.Services;

public class ControlService(ApplicationDbContext context)
{
    public async Task<Result<IEnumerable<ControlDto>, Exception>> GetControlsAsync()
    {
        try
        {
            var controls = GetControls(context) ?? throw new Exception("Controls not found");
            
            return (await controls.ToListAsync()).ToDto().ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<Control>> GetControls = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Controls
                .TagWith("GetSchemas"));
}