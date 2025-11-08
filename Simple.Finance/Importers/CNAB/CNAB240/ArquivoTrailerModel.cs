namespace Simple.Finance.Importers.CNAB.CNAB240;

using System;
using TextSerializer;
using TextSerializer.Attributes;

[RegistrySize(240)]
public record ArquivoTrailerModel
{
    [Index(1), Type(DataType.N), Length(3)]
    public string CBanco { get; set; } = string.Empty;
    [Index(2), Type(DataType.N), Length(4)]
    public string CLote { get; set; } = string.Empty;
    [Index(3), Type(DataType.N), Length(1)]
    public string CRegistro { get; set; } = string.Empty; // "9" para Trailer Arquivo

    [Index(4), Type(DataType.C), Length(9)]
    public string Filler1 { get; set; } = string.Empty;

    [Index(5), Type(DataType.N), Length(6)]
    public string QtdLotes { get; set; } = string.Empty;
    [Index(6), Type(DataType.N), Length(6)]
    public string QtdRegistros { get; set; } = string.Empty;
    [Index(7), Type(DataType.N), Length(6)]
    public string QtdContas { get; set; } = string.Empty;

    [Index(8), Type(DataType.C), Length(205)]
    public string Filler2 { get; set; } = string.Empty;
}
