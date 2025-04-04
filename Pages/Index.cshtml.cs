using FarmaciaDeTurnoCerca.Modelo;
using GeoCoordinatePortable;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FarmaciaDeTurnoCerca.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    internal List<Farmacia> Farmacias = [];
    internal List<Ciudad> Ciudades = [new Ciudad()
    {
        Nombre = "La Plata",
        Url = "https://www.colfarmalp.org.ar/turnos-la-plata/"
    }];

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        
    }

    public void OnPost(string ciudad, string latitud, string longitud)
    {
        var miposicion = new GeoCoordinate(double.Parse(latitud.Replace(".",",")), double.Parse(longitud.Replace(".",",")));
        const string url = "https://www.colfarmalp.org.ar/turnos-la-plata/";
        var response = CallUrl(ciudad).Result;
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(response);
        var trNodes = htmlDoc.DocumentNode
            .SelectNodes("//div[contains(@class, 'turnos')]//div[contains(@class, 'tr')]").ToList();
        foreach (var htmlNode in trNodes.Slice(1,trNodes.Count-1))
        {
            var nombre =  htmlNode.SelectNodes("div[contains(@class, 'td')]")[0].InnerText.Trim().Replace("Farmacia ", "");;
            var direccion = htmlNode.SelectNodes("div[contains(@class, 'td')]")[1].InnerText.Trim().Replace("Direcci&oacute;n ","");
            var ubicacion = htmlNode.SelectNodes("div[contains(@class, 'td')]//a").First().Attributes["href"].Value;
            var coordenadas = GetCoordenadas(ubicacion);
            var f = new Farmacia()
            {
                Direccion = direccion, 
                Nombre = nombre,
                Ubicacion = ubicacion,
                Geo = coordenadas,
                Distancia = miposicion.GetDistanceTo(coordenadas)
               
            };
            Farmacias.Add(f);
        }

        Farmacias = Farmacias.OrderBy(f => f.Distancia).ToList().Slice(0,5);
    }

    private static GeoCoordinate GetCoordenadas(string ubicacion)
    {
        var coord = ubicacion.Split("=").Last();
        return new GeoCoordinate(double.Parse(coord.Split(",")[0].Replace(".", ",")),
            double.Parse(coord.Split(",")[1].Replace(".", ",")));
    }

    private static async Task<string> CallUrl(string fullUrl)
    {
        HttpClient client = new HttpClient();
        var response = await client.GetStringAsync(fullUrl);
        return response;
    }
}