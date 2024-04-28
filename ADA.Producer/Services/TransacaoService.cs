using ADA.Producer.Configurations;
using ADA.Producer.DTO;
using RabbitMQ.Client;
using System.Text.Json;

namespace ADA.Producer.Services;

public class TransacaoService(IAppSettings appSettings) : ITransacaoService
{
    private readonly IAppSettings _appSettings = appSettings;

    public void EnviarTransacao(TransacaoDTO transacaoDTO)
    {
        ConnectionFactory factory = new()
        {
            HostName = _appSettings.GetValue("RabbitMQ:HostName"),
            UserName = _appSettings.GetValue("RabbitMQ:UserName"),
            Password = _appSettings.GetValue("RabbitMQ:Password")
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var basicProperties = channel.CreateBasicProperties();
        basicProperties.Persistent = true;

        channel.BasicPublish(exchange: "ada.transacao",
                             routingKey: "transacao",
                             basicProperties: basicProperties,
                             body: JsonSerializer.SerializeToUtf8Bytes(transacaoDTO));
    }
}
