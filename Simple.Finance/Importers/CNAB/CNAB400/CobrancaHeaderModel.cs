namespace Simple.Finance.Importers.CNAB.CNAB400;

using System;
using TextSerializer;
using TextSerializer.Attributes;

[RegistrySize(400)]
public record CobrancaHeaderModel
{
    // Usado de Base: SICRED

    [Index(1), Type(DataType.N), Length(1)]
    public string IdHeader { get; set; } = string.Empty; // 0
    [Index(2), Type(DataType.N), Length(1)]
    public string IdRemessa { get; set; } = string.Empty; // 2
    [Index(3), Type(DataType.C), Length(7)]
    public string LiteralRetorno { get; set; } = string.Empty; // RETORNO
    [Index(4), Type(DataType.N), Length(2)]
    public string CodServicoCobranca { get; set; } = string.Empty; // 01
    [Index(5), Type(DataType.C), Length(15)]
    public string LiteralCobranca { get; set; } = string.Empty; // COBRANCA
    [Index(6), Type(DataType.C), Length(5)]
    public string CodBeneficiario { get; set; } = string.Empty;
    [Index(7), Type(DataType.C), Length(14)]
    public string DocBeneficiario { get; set; } = string.Empty;
    [Index(8), Type(DataType.C), Length(31)]
    public string Filler1 { get; set; } = string.Empty;
    [Index(9), Type(DataType.N), Length(3)]
    public string NumeroBanco { get; set; } = string.Empty;
    [Index(10), Type(DataType.C), Length(15)]
    public string NomeBanco { get; set; } = string.Empty;
    [Index(11), Type(DataType.N), Length(8)]
    public string DataArquivo { get; set; } = string.Empty; // yyyyMMdd
    [Index(13), Type(DataType.C), Length(8)]
    public string Filler2 { get; set; } = string.Empty;
    [Index(14), Type(DataType.C), Length(7)]
    public string NumeroRetorno { get; set; } = string.Empty;
    [Index(15), Type(DataType.C), Length(272)]
    public string Filler3 { get; set; } = string.Empty;
    [Index(16), Type(DataType.N), Length(5)]
    public string Versao { get; set; } = string.Empty;
    [Index(17), Type(DataType.N), Length(6)]
    public string SeqRegistro { get; set; } = string.Empty;
}
