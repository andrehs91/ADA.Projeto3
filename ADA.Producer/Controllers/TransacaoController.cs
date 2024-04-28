using ADA.Producer.DTO;
using ADA.Producer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADA.Producer.Controllers;

[ApiController]
[Route("api/transacao")]
[Produces("application/json")]
public class TransacaoController(
    ILogger<TransacaoController> logger,
    ITransacaoService transacaoService
) : ControllerBase
{
    private readonly ILogger<TransacaoController> _logger = logger;
    private readonly ITransacaoService _transacaoService = transacaoService;

    [HttpPost]
    [Route("enviar-transacao")]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(RespostaDTO), StatusCodes.Status400BadRequest)]
    public ActionResult<RespostaDTO> EnviarTransacao(TransacaoDTO transacaoDTO)
    {
        if (!ModelState.IsValid)
        {
            var mensagem = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(RespostaDTO.Aviso(mensagem));
        }
        try
        {
            _transacaoService.EnviarTransacao(transacaoDTO);
            return Accepted(RespostaDTO.Sucesso("Transação enviada com sucesso."));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TransacaoController.EnviarTransacao");
            return StatusCode(500, RespostaDTO.Erro("Entre em contato com o suporte."));
        }
    }
}
