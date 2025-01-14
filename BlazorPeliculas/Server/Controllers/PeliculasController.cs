﻿using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "peliculas";


        public PeliculasController(ApplicationDbContext context,
            IAlmacenadorArchivos almacenadorArchivos) {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
        }
        [HttpGet]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            var limite = 6;
            var peliculasEnCartelera = await context.Peliculas
                .Where(pelicula => pelicula.EnCartelera).Take(limite)
                .OrderByDescending(pelicula => pelicula.Lanzamiento)
                .ToListAsync();
            var fechaActual = DateTime.Today;
            var proximosEstrenos = await context.Peliculas
                .Where(pelicula => pelicula.Lanzamiento > fechaActual)
                .OrderBy(pelicula => pelicula.Lanzamiento).Take(limite)
                .ToListAsync();
            var resultado = new HomePageDTO
            {
                PeliculasEnCartelera = peliculasEnCartelera,
                ProximosEstrenos = proximosEstrenos
            };
            return resultado;

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeliculaVisualizarDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas.Where(pelicula => pelicula.Id == id)
                .Include(pelicula => pelicula.GenerosPelicula)
                    .ThenInclude(gp => gp.Genero)
                .Include(pelicula => pelicula.PeliculasActor.OrderBy(pa => pa.Orden))
                    .ThenInclude(pa => pa.Actor)
                .FirstOrDefaultAsync();
            if (pelicula is null)
            {
                return NotFound();
            }

            // TODO: Sistema de votación
            var promedioVotos = 4;
            var votoUsuario = 5;
            var modelo = new PeliculaVisualizarDTO();
            modelo.Pelicula = pelicula;
            modelo.Generos = pelicula.GenerosPelicula.Select(gp => gp.Genero!).ToList();
            modelo.Actores = pelicula.PeliculasActor.Select(pa => new Actor
            {
                Nombre = pa.Actor!.Nombre,
                Foto =  pa.Actor.Foto,
                Personaje = pa.Personaje,
                Id = pa.Actor.Id,
            }).ToList();
            modelo.PromedioVotos = promedioVotos;
            modelo.VotoUsuario = votoUsuario;
            return modelo;
        }   
        
        [HttpPost]
        public async Task<ActionResult<int>> Post(Pelicula pelicula)
        {
            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                var poster = Convert.FromBase64String(pelicula.Poster);
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(poster, ".jpg", contenedor);
            }
            if(pelicula.PeliculasActor is not null)
            {
                for(int i = 0; i < pelicula.PeliculasActor.Count; i++)
                {
                    pelicula.PeliculasActor[i].Orden = i + 1;
                }
            }
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }
    }
}
