namespace Simple.Finance.Importers.CNAB.CNAB240;

using System;
using TextSerializer;
using TextSerializer.Attributes;

[RegistrySize(240)]
public record SegCobrancaHeaderLoteModel
{
    [Index(1), Type(DataType.N), Length(3)]
    public string CBanco { get; set; } = string.Empty;
    [Index(2), Type(DataType.N), Length(4)]
    public string CLote { get; set; } = string.Empty;
    [Index(3), Type(DataType.N), Length(1)]
    public string CRegistro { get; set; } = string.Empty; // "1" para Header Lote

    [Index(4), Type(DataType.C), Length(1)]
    public string SOperacao { get; set; } = string.Empty; // "E" para extrato
    [Index(5), Type(DataType.N), Length(2)]
    public string SServico { get; set; } = string.Empty;
    [Index(6), Type(DataType.C), Length(2)]
    public string FILLER1 { get; set; } = string.Empty;
    [Index(7), Type(DataType.N), Length(3)]
    public string SLayoutLote { get; set; } = string.Empty;
    [Index(8), Type(DataType.C), Length(1)]
    public string FILLER2 { get; set; } = string.Empty;

    [Index(9), Type(DataType.N), Length(1)]
    public string ETipoInscricao { get; set; } = string.Empty;
    [Index(10), Type(DataType.N), Length(15)]
    public string ENumeroInscricao { get; set; } = string.Empty;
    [Index(11), Type(DataType.C), Length(20)]
    public string ECodigoConvenio { get; set; } = string.Empty;
    [Index(12), Type(DataType.N), Length(5)]
    public string EAgencia { get; set; } = string.Empty;
    [Index(13), Type(DataType.C), Length(1)]
    public string EAgenciaDV { get; set; } = string.Empty;
    [Index(14), Type(DataType.N), Length(12)]
    public string EConta { get; set; } = string.Empty;
    [Index(15), Type(DataType.C), Length(1)]
    public string EContaDV { get; set; } = string.Empty;
    [Index(16), Type(DataType.C), Length(1)]
    public string EDVAC { get; set; } = string.Empty;
    [Index(17), Type(DataType.C), Length(30)]
    public string ENome { get; set; } = string.Empty;

    [Index(18), Type(DataType.C), Length(40)]
    public string Mensagem1 { get; set; } = string.Empty;

    [Index(19), Type(DataType.C), Length(40)]
    public string Mensagem2 { get; set; } = string.Empty;

    [Index(20), Type(DataType.N), Length(8)]
    public string NumeroRemessaRetorno { get; set; } = string.Empty;
    [Index(21), Type(DataType.N), Length(8)]
    public string DataRemessaRetorno { get; set; } = string.Empty;
    [Index(22), Type(DataType.N), Length(8)]
    public string DataCredito { get; set; } = string.Empty;

    [Index(23), Type(DataType.C), Length(33)]
    public string FILLER3 { get; set; } = string.Empty;
}