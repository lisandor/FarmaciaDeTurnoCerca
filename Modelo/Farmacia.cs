using GeoCoordinatePortable;

namespace FarmaciaDeTurnoCerca.Modelo;

public class Farmacia
{
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string Ubicacion { get; set; }
    public GeoCoordinate Geo { get; set; }
    public double Distancia { get; set; }
}