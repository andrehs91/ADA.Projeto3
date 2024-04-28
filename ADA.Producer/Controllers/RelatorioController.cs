using ADA.Producer.DTO;
using ADA.Producer.Services;
using Microsoft.AspNetCore.Mvc;
using Minio.Exceptions;
using System.Text.RegularExpressions;

namespace ADA.Producer.Controllers;

[ApiController]
[Route("api/relatorio")]
[Produces("application/json")]
public class RelatorioController(
    ILogger<RelatorioController> logger,
    IRelatorioService relatorioService
) : ControllerBase
{
    private readonly ILogger<RelatorioController> _logger = logger;
    private readonly IRelatorioService _relatorioService = relatorioService;

    [HttpGet]
    [Route("gerar-relatorio")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> GerarRelatorio(string contaOrigem)
    {
        if (!FormatoContaValido(contaOrigem))
        {
            return BadRequest(RespostaDTO.Aviso("Informe a conta no formato 0000.00000000."));
        }
        try
        {
            return Ok(await _relatorioService.GerarRelatorioAsync(contaOrigem));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "RelatorioController.ConsultarRelatorio");
            return StatusCode(500, RespostaDTO.Erro("Entre em contato com o suporte."));
        }
    }

    [HttpGet]
    [Route("listar-relatorios")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<string>>> ListarRelatorios(string contaOrigem)
    {
        if (!FormatoContaValido(contaOrigem))
        {
            return BadRequest(RespostaDTO.Aviso("Informe a conta no formato 0000.00000000."));
        }
        try
        {
            var links = await _relatorioService.ListarRelatoriosAsync(contaOrigem);
            if (links is null) return Ok(RespostaDTO.Sucesso("Nenhum relatório foi encontrado para esta conta."));
            return Ok(links);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "RelatorioController.ListarRelatorios");
            return StatusCode(500, RespostaDTO.Erro("Entre em contato com o suporte."));
        }
    }

    [HttpGet]
    [Route("baixar-relatorio/{nomeDoArquivo}")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK, "application/octet-stream")]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> BaixarRelatorio(string nomeDoArquivo)
    {
        try
        {
            var arquivo = await _relatorioService.BaixarRelatorioAsync(nomeDoArquivo);
            return new FileStreamResult(arquivo, "application/octet-stream");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "RelatorioController.BaixarRelatorio");
            if (e is InvalidObjectNameException) return BadRequest(RespostaDTO.Erro("O nome do arquivo não é válido."));
            if (e is ObjectNotFoundException) return NotFound(RespostaDTO.Aviso("O arquivo não foi encontrado."));
            return StatusCode(500, RespostaDTO.Erro("Entre em contato com o suporte."));
        }
    }

    private static bool FormatoContaValido(string? conta)
    {
        if (string.IsNullOrEmpty(conta)) return false;
        if (!Regex.IsMatch(conta, @"^\d{4}\.\d{8}$")) return false;
        return true;
    }
}
