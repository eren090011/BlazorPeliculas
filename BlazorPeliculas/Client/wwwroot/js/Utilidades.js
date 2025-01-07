function pruebaPuntoNetStatic(){
    DotNet.invokeMethodAsync("BlazorPeliculas.Client","ObtenerCurrentCount")
    .then(resultado =>{
        console.log('Conteo desde javascript: '+resultado);   
    })
}

function pruebaPuntoNetInstancia(dotnetHelper){
    dotnetHelper.invokeMethodAsync("IncrementCount")
}