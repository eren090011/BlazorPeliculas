using BlazorPeliculas.Client.Helpers;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace BlazorPeliculas.Client.Pages
{
    public partial class Counter{
        private int currentCount = 0;
        [Inject] public IJSRuntime js{get; set;} = null!;
        public async Task IncrementCount()
        {
            
            var arreglo = new double[]{ 1, 2, 3, 4, 5 };
            var max = arreglo.Maximum();
            var min = arreglo.Maximum();
            currentCount++;
            await js.InvokeVoidAsync("alert",$"el max es {max} y el min es {min}");
        }
    }
}
