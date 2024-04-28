using ADA.Producer.Configurations;
using Minio;
using Minio.DataModel.Args;
using StackExchange.Redis;

namespace ADA.Producer.Services;

public class RelatorioService : IRelatorioService
{
    private readonly ILogger<RelatorioService> _logger;
    private readonly IDatabase _db;
    private readonly string _endPoint;
    private readonly string _accessKey;
    private readonly string _secretKey;
    private readonly bool _isSecure = false;
    private readonly int _port = 9000;
    private readonly string _appHostName;
    private readonly string _bucketName = "relatorios";

    public RelatorioService(ILogger<RelatorioService> logger, IAppSettings appSettings)
    {
        _logger = logger;

        string connectionString = appSettings.GetValue("ConnectionStrings:Redis");
        var redis = ConnectionMultiplexer.Connect(connectionString);
        _db = redis.GetDatabase();

        _endPoint = appSettings.GetValue("Minio:EndPoint");
        _accessKey = appSettings.GetValue("Minio:AccessKey");
        _secretKey = appSettings.GetValue("Minio:SecretKey");
        if (bool.TryParse(appSettings.GetValue("Minio:IsSecure"), out bool isSecure)) _isSecure = isSecure;
        if (int.TryParse(appSettings.GetValue("Minio:Port"), out int port)) _port = port;
        _appHostName = appSettings.GetValue("AppHostName");
    }

    private IMinioClient GetClient()
    {
        return new MinioClient()
            .WithEndpoint(_endPoint, _port)
            .WithCredentials(_accessKey, _secretKey)
            .WithSSL(_isSecure)
            .Build();
    }

    private async void CreateBucketIfNotExistsAsync()
    {
        using var minio = GetClient();

        bool bucketExists = await minio.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucketName)
        ).ConfigureAwait(false);
        _logger.LogDebug("Bucket Existe: " + (bucketExists ? "Sim" : "Não"));
        
        if (!bucketExists)
        {
            await minio.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucketName)
            ).ConfigureAwait(false);

            string policyJson = $"{{\"Version\":\"2012-10-17\",\"Statement\":[{{\"Effect\":\"Allow\",\"Principal\":{{\"AWS\":[\"*\"]}},\"Action\":[\"s3:ListBucket\",\"s3:ListBucketMultipartUploads\",\"s3:GetBucketLocation\"],\"Resource\":[\"arn:aws:s3:::{_bucketName}\"]}},{{\"Effect\":\"Allow\",\"Principal\":{{\"AWS\":[\"*\"]}},\"Action\":[\"s3:AbortMultipartUpload\",\"s3:DeleteObject\",\"s3:GetObject\",\"s3:ListMultipartUploadParts\",\"s3:PutObject\"],\"Resource\":[\"arn:aws:s3:::{_bucketName}/*\"]}}]}}";
            await minio.SetPolicyAsync(
                new SetPolicyArgs().WithBucket(_bucketName).WithPolicy(policyJson)
            ).ConfigureAwait(false);

            _logger.LogDebug("Bucket Criado.");
        }

        //string policy = await minio.GetPolicyAsync(
        //    new GetPolicyArgs().WithBucket(_bucketName)
        //).ConfigureAwait(false);
        //_logger.LogDebug("Policy: " + policy);
    }

    public async Task<string> GerarRelatorioAsync(string contaOrigem)
    {
        string chaveTransacaoInvalida = "invalida." + contaOrigem;
        var transacoesInvalidas = await _db.ListRangeAsync(chaveTransacaoInvalida);

        if (transacoesInvalidas.Length == 0)
            return "Conta não possui registro de transações fraudulentas ou todos os registros já foram enviados para o relatório.";

        string content = "[";
        foreach (var transacao in transacoesInvalidas) content += transacao + ",";
        content = content.Remove(content.Length - 1, 1) + "]";
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);
        streamWriter.Write(content);
        streamWriter.Flush();
        memoryStream.Position = 0;

        CreateBucketIfNotExistsAsync();

        string objectName = $"{contaOrigem}_{DateTime.Now:yyyyMMddHHmmss}.txt";
        using var minio = GetClient();
        _ = await minio.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(memoryStream)
                .WithObjectSize(memoryStream.Length)
                .WithContentType("application/octet-stream")
        ).ConfigureAwait(false);

        streamWriter.Dispose();
        memoryStream.Dispose();

        await _db.KeyDeleteAsync(chaveTransacaoInvalida);

        string link = $"{_appHostName}/api/relatorio/baixar-relatorio/{objectName}";
        //var headers = new Dictionary<string, string>(StringComparer.Ordinal)
        //{
        //    { "response-content-type", "application/json" }
        //};
        //var link = await minio.PresignedGetObjectAsync(
        //    new PresignedGetObjectArgs()
        //        .WithBucket(_bucketName)
        //        .WithObject(objectName)
        //        .WithExpiry(1000)
        //        .WithHeaders(headers)
        //).ConfigureAwait(false);

        _db.SetAdd("relatorios." + contaOrigem, link);

        return link;
    }

    public async Task<List<string>?> ListarRelatoriosAsync(string contaOrigem)
    {
        var cache = await _db.SetMembersAsync("relatorios." + contaOrigem);
        if (cache.Length == 0) return null;
        List<string> links = cache.Select(c => c.ToString()).ToList();
        return links;
    }

    public async Task<MemoryStream> BaixarRelatorioAsync(string nomeDoArquivo)
    {
        using var minio = GetClient();
        var memoryStream = new MemoryStream();
        await minio.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(nomeDoArquivo)
            .WithCallbackStream((stream) => stream.CopyTo(memoryStream)));
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}
