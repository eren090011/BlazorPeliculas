using Microsoft.JSInterop;
namespace BlazorPeliculas.Client.Helpers
{
    public static class IJSRuntimeExtentionMethods
    {
        public static async ValueTask<bool> Confirm(this IJSRuntime js, string mensaje){
            await js.InvokeVoidAsync("console.log","prueba de consolelog");
            return await js.InvokeAsync<bool>("confirm",mensaje); 
        }
        
    } 
}
