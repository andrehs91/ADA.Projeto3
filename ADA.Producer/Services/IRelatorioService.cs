namespace ADA.Producer.Services;

public interface IRelatorioService
{
    Task<string> GerarRelatorioAsync(string contaOrigem);
    Task<List<string>?> ListarRelatoriosAsync(string contaOrigem);
    Task<MemoryStream> BaixarRelatorioAsync(string nomeDoArquivo);
}
