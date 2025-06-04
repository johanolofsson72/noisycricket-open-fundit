using System;
using Microsoft.JSInterop;

namespace Shared.Global.Services;

public class BrowserService
{
    private IJSRuntime JS = null!;
    public event EventHandler<int> Resize = null!;
    private int browserWidth = 0;
    private int browserHeight = 0;
    
    public async void Init(IJSRuntime js)
    {
        try
        {
            // enforce single invocation            
            if (JS is not null) return;
            this.JS = js;
            await JS.InvokeAsync<string>("resizeListener", DotNetObjectReference.Create(this));
        }
        catch
        {
            // ignored
        }
    }
        
    [JSInvokable]
    public void SetBrowserDimensions(int jsBrowserWidth, int jsBrowserHeight) {
        browserWidth = jsBrowserWidth;
        browserHeight = jsBrowserHeight;
        // For simplicity, we're just using the new width
        this.Resize?.Invoke(this,jsBrowserWidth);
    }
}
