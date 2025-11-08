namespace Simple.Finance.Importers.CNAB.CNAB240;

using System;
using TextSerializer;
using TextSerializer.Attributes;

[RegistrySize(240)]
public record SegCobrancaTDetalheModel
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

    [Index(8), Type(DataType.N), Length(5)]
    public string CAgencia { get; set; } = string.Empty;
    [Index(9), Type(DataType.C), Length(1)]
    public string CAgenciaDV { get; set; } = string.Empty;
    [Index(10), Type(DataType.N), Length(12)]
    public string CConta { get; set; } = string.Empty;
    [Index(11), Type(DataType.C), Length(1)]
    public string CContaDV { get; set; } = string.Empty;
    [Index(12), Type(DataType.C), Length(1)]
    public string CDVAC { get; set; } = string.Empty;

    [Index(13), Type(DataType.C), Length(20)]
    public string NossoNumero { get; set; } = string.Empty;
    [Index(14), Type(DataType.N), Length(1)]
    public string Carteira { get; set; } = string.Empty;
    [Index(15), Type(DataType.C), Length(15)]
    public string NumeroDocumento { get; set; } = string.Empty;
    [Index(16), Type(DataType.N), Length(8)]
    public string DataVencimento { get; set; } = string.Empty;
    [Index(17), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal ValorTitulo { get; set; }
    [Index(18), Type(DataType.N), Length(3)]
    public string BancoCobRec { get; set; } = string.Empty;
    [Index(19), Type(DataType.N), Length(5)]
    public string AgCobRec { get; set; } = string.Empty;
    [Index(20), Type(DataType.N), Length(1)]
    public string DvAgCobRec { get; set; } = string.Empty;
    [Index(21), Type(DataType.C), Length(25)]
    public string IdentificacaoTituloEmpresa { get; set; } = string.Empty;
    [Index(22), Type(DataType.N), Length(2)]
    public string CodMoeda { get; set; } = string.Empty;

    [Index(23), Type(DataType.N), Length(1)]
    public string PTipoInscricao { get; set; } = string.Empty;
    [Index(24), Type(DataType.N), Length(15)]
    public string PNumeroInscricao { get; set; } = string.Empty;
    [Index(25), Type(DataType.N), Length(40)]
    public string PNome { get; set; } = string.Empty;
    [Index(26), Type(DataType.N), Length(10)]
    public string NumeroContrato { get; set; } = string.Empty;
    [Index(27), Type(DataType.N), Length(13, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal ValorCustasTarifas { get; set; }
    [Index(28), Type(DataType.C), Length(10)]
    public string MotivoOcorrencia { get; set; } = string.Empty;

    [Index(29), Type(DataType.C), Length(17)]
    public string FILLER3 { get; set; } = string.Empty;
}