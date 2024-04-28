using ADA.Producer.DTO;

namespace ADA.Producer.Services;

public interface ITransacaoService
{
    void EnviarTransacao(TransacaoDTO transacaoDTO);
}
