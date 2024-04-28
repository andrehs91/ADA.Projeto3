namespace ADA.Consumer.Entities;

public struct Coordenadas(double latitute, double longitude)
{
    public double Latitute { get; set; } = latitute;
    public double Longitude { get; set; } = longitude;
}
