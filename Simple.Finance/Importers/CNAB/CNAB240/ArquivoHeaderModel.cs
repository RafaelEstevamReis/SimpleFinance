namespace Simple.Finance.Importers.CNAB.CNAB240;

using System;
using TextSerializer;
using TextSerializer.Attributes;

[RegistrySize(240)]
public record ArquivoHeaderModel
{
    [Index(1), Type(DataType.N), Length(3)]
    public string CBanco { get; set; } = string.Empty;
    [Index(2), Type(DataType.N), Length(4)]
    public string CLote { get; set; } = string.Empty;
    [Index(3), Type(DataType.N), Length(1)]
    public string CRegistro { get; set; } = string.Empty; // "0" para Header Arquivo

    [Index(4), Type(DataType.C), Length(9)]
    public string Filler1 { get; set; } = string.Empty;

    [Index(5), Type(DataType.N), Length(1)]
    public string ETipoInscricao { get; set; } = string.Empty;
    [Index(6), Type(DataType.N), Length(14)]
    public string ENumeroInscricao { get; set; } = string.Empty;
    [Index(7), Type(DataType.C), Length(20)]
    public string ECodigoConvenio { get; set; } = string.Empty;
    [Index(8), Type(DataType.N), Length(5)]
    public string EAgencia { get; set; } = string.Empty;
    [Index(9), Type(DataType.C), Length(1)]
    public string EAgenciaDV { get; set; } = string.Empty;
    [Index(10), Type(DataType.N), Length(12)]
    public string EConta { get; set; } = string.Empty;
    [Index(11), Type(DataType.C), Length(1)]
    public string EContaDV { get; set; } = string.Empty;
    [Index(12), Type(DataType.C), Length(1)]
    public string EDVAC { get; set; } = string.Empty;
    [Index(13), Type(DataType.C), Length(30)]
    public string ENome { get; set; } = string.Empty;
    [Index(14), Type(DataType.C), Length(30)]
    public string NomeBanco { get; set; } = string.Empty;

    [Index(15), Type(DataType.C), Length(10)]
    public string CNAB_FILLER2 { get; set; } = string.Empty;

    [Index(16), Type(DataType.C), Length(1)]
    public string ACodigoRemessaRetorno { get; set; } = string.Empty;
    [Index(17), Type(DataType.N), Length(8)]
    public string ADataGeracao { get; set; } = string.Empty;
    [Index(18), Type(DataType.N), Length(6)]
    public string AHoraGeracao { get; set; } = string.Empty;
    [Index(19), Type(DataType.N), Length(6)]
    public string ASequenciaNSA { get; set; } = string.Empty;
    [Index(20), Type(DataType.N), Length(3)]
    public string ANVersaoLeiaute { get; set; } = string.Empty;
    [Index(21), Type(DataType.N), Length(5)]
    public string ADensidade { get; set; } = string.Empty;
    [Index(22), Type(DataType.N), Length(20)]
    public string CNAB_FILLER3 { get; set; } = string.Empty;
    [Index(23), Type(DataType.N), Length(20)]
    public string CNAB_FILLER4 { get; set; } = string.Empty;

    [Index(24), Type(DataType.C), Length(29)]
    public string CNAB_FILLER5 { get; set; } = string.Empty;
}
