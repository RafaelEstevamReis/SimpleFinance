namespace Simple.Finance.Importers.CNAB.CNAB240;

using System;
using TextSerializer;
using TextSerializer.Attributes;

[RegistrySize(240)]
public record SegCobrancaUDetalheModel
{
    [Index(1), Type(DataType.N), Length(3)]
    public string CBanco { get; set; } = string.Empty;
    [Index(2), Type(DataType.N), Length(4)]
    public string CLote { get; set; } = string.Empty;
    [Index(3), Type(DataType.N), Length(1)]
    public string CRegistro { get; set; } = string.Empty; // "3" para Detalhe
    [Index(4), Type(DataType.N), Length(5)]
    public string SSeqRegistro { get; set; } = string.Empty;
    [Index(5), Type(DataType.C), Length(1)]
    public string SCodSegmento { get; set; } = string.Empty; // "T"
    [Index(6), Type(DataType.C), Length(1)]
    public string FILLER1 { get; set; } = string.Empty;
    [Index(7), Type(DataType.C), Length(2)]
    public string SCodMovimentacao { get; set; } = string.Empty;

    [Index(8), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal TAcrescimos { get; set; }
    [Index(9), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal TValorDesconto { get; set; }
    [Index(10), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal TValorlAbatimento { get; set; }
    [Index(11), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal TValorlIOF { get; set; }
    [Index(12), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal TValorlPago { get; set; }
    [Index(13), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal TValorlLiquido { get; set; }
    [Index(14), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal OutrasDespesas { get; set; }
    [Index(15), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal OutrosCreditos { get; set; }
    [Index(16), Type(DataType.N), Length(8)]
    public string DataOcorrencia { get; set; } = string.Empty;
    [Index(17), Type(DataType.N), Length(8)]
    public string DataCredito { get; set; } = string.Empty;

    [Index(18), Type(DataType.C), Length(4)]
    public string OPagCodigoOcorrencia { get; set; } = string.Empty;
    [Index(19), Type(DataType.N), Length(8)]
    public string OPagDataOcorrencia { get; set; } = string.Empty;
    [Index(20), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal OPagValorOcorrencia { get; set; }
    [Index(21), Type(DataType.C), Length(30)]
    public string OPagComplementoOcorrencia { get; set; } = string.Empty;

    [Index(22), Type(DataType.C), Length(3)]
    public string CodBancoOcorrencia { get; set; } = string.Empty;
    [Index(23), Type(DataType.C), Length(20)]
    public string NossoNumeroBancoOcorrencia { get; set; } = string.Empty;
    [Index(24), Type(DataType.C), Length(7)]
    public string FILLER2 { get; set; } = string.Empty;

}