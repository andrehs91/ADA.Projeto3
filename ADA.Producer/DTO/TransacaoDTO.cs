using ADA.Producer.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ADA.Producer.DTO;

public class TransacaoDTO
{
    [Required(ErrorMessage = "DataHora: Campo obrigatório.")]
    public DateTime DataHora { get; set; }

    [Required(ErrorMessage = "Campo obrigatório.", AllowEmptyStrings = false)]
    [RegularExpression(@"^\d{4}\.\d{8}$", ErrorMessage = "ContaOrigem: Informe a conta no formato 0000.00000000.")]
    public string ContaOrigem { get; set; } = null!;

    [Required(ErrorMessage = "Campo obrigatório.", AllowEmptyStrings = false)]
    [RegularExpression(@"^\d{4}\.\d{8}$", ErrorMessage = "ContaDestino: Informe a conta no formato 0000.00000000.")]
    public string ContaDestino { get; set; } = null!;

    [Required(ErrorMessage = "Canal: Campo obrigatório.")]
    public Canal Canal { get; set; }

    [Required(ErrorMessage = "Valor: Campo obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor: O valor deve ser maior do que 0.")]
    public double Valor { get; set; }

    [Required(ErrorMessage = "Latitute: Campo obrigatório.")]
    [Range(-90, 90, ErrorMessage = "Latitute: Informe um valor entre -90º e 90º.")]
    [DefaultValue(0)]
    public double Latitute { get; set; }

    [Required(ErrorMessage = "Longitude: Campo obrigatório.")]
    [Range(-180, 180, ErrorMessage = "Longitude: Informe um valor entre -180º e 180º.")]
    [DefaultValue(0)]
    public double Longitude { get; set; }
}
