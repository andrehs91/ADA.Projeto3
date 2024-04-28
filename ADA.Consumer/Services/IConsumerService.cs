using ADA.Consumer.Entities;

namespace ADA.Consumer.Services;

public interface IConsumerService
{
    Task<Transacao> ProcessarTransacaoAsync(Transacao transacao);
}
