namespace Simple.Finance.Importers.CNAB.CNAB240;

using System;
using TextSerializer;
using TextSerializer.Attributes;

[RegistrySize(240)]
public record SegExtratoDetalheModel
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
    public string SCodSegmento { get; set; } = string.Empty; // "E" para Extrato

    [Index(6), Type(DataType.C), Length(3)]
    public string FILLER1 { get; set; } = string.Empty;

    [Index(7), Type(DataType.N), Length(1)]
    public string ETipoInscricao { get; set; } = string.Empty;
    [Index(8), Type(DataType.N), Length(14)]
    public string ENumeroInscricao { get; set; } = string.Empty;
    [Index(9), Type(DataType.C), Length(20)]
    public string ECodigoConvenio { get; set; } = string.Empty;
    [Index(10), Type(DataType.N), Length(5)]
    public string EAgencia { get; set; } = string.Empty;
    [Index(11), Type(DataType.C), Length(1)]
    public string EAgenciaDV { get; set; } = string.Empty;
    [Index(12), Type(DataType.N), Length(12)]
    public string EConta { get; set; } = string.Empty;
    [Index(13), Type(DataType.C), Length(1)]
    public string EContaDV { get; set; } = string.Empty;
    [Index(14), Type(DataType.C), Length(1)]
    public string EDVAC { get; set; } = string.Empty;
    [Index(15), Type(DataType.C), Length(30)]
    public string ENome { get; set; } = string.Empty;

    [Index(16), Type(DataType.C), Length(40)]
    public string FILLER2 { get; set; } = string.Empty;
    // PULA CAMPOS VARIA DE BANCO PRA BANCO

    [Index(22), Type(DataType.N), Length(8)]
    public DateTime LData { get; set; }
    [Index(23), Type(DataType.N), Length(16, 2, LengthAttribute.CountMode.DecimalsExclusive)]
    public decimal LValor { get; set; }
    [Index(24), Type(DataType.C), Length(1)]
    public string LTipo { get; set; } = string.Empty;
    [Index(25), Type(DataType.N), Length(3)]
    public string LCategoria { get; set; } = string.Empty;
    [Index(26), Type(DataType.C), Length(4)]
    public string LCodigoHistorico { get; set; } = string.Empty;
    [Index(27), Type(DataType.C), Length(25)]
    public string LHistorico { get; set; } = string.Empty;
    [Index(28), Type(DataType.C), Length(39)]
    public string LNumDoc { get; set; } = string.Empty;
}
