namespace ADA.Producer.DTO;

public class RespostaDTO(string tipo, string mensagem)
{
    public string Tipo { get; set; } = tipo;
    public string Mensagem { get; set; } = mensagem;

    public static RespostaDTO Aviso(string mensagem) => new("Aviso", mensagem);
    public static RespostaDTO Erro(string mensagem) => new("Erro", mensagem);
    public static RespostaDTO Sucesso(string mensagem) => new("Sucesso", mensagem);
}
