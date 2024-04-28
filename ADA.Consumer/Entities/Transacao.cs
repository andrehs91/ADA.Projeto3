namespace ADA.Consumer.Entities;

public class Transacao(DateTime dataHora, string contaOrigem, string contaDestino, Canal canal, double valor, Coordenadas coordenadas)
{
    public DateTime DataHora { get; set; } = dataHora;
    public string ContaOrigem { get; set; } = contaOrigem;
    public string ContaDestino { get; set; } = contaDestino;
    public Canal Canal { get; set; } = canal;
    public double Valor { get; set; } = valor;
    public Coordenadas Coordenadas { get; set; } = coordenadas;
    public bool Fraude = false;
}
