using ADA.Consumer.Entities;

namespace ADA.Consumer.DTO;

public class TransacaoDTO
{
    public DateTime DataHora { get; set; }
    public string ContaOrigem { get; set; } = null!;
    public string ContaDestino { get; set; } = null!;
    public Canal Canal { get; set; }
    public double Valor { get; set; }
    public double Latitute { get; set; }
    public double Longitude { get; set; }

    public Transacao MapearParaEntidade() => new(
        DataHora,
        ContaOrigem,
        ContaDestino,
        Canal,
        Valor,
        new Coordenadas(Latitute, Longitude)
    );
}
