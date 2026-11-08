namespace Simple.Finance.Importers.CNAB.CNAB400;

using System;
using TextSerializer;
using TextSerializer.Attributes;

[RegistrySize(400)]
public record CobrancaDetalheModelo
{
    // Usado de Base: SICRED

    [Index(1), Type(DataType.N), Length(1)]
    public string IdDetalhe { get; set; } = string.Empty; // 1
    [Index(2), Type(DataType.C), Length(1)]
    public string TipoCarteira { get; set; } = string.Empty;
    [Index(3), Type(DataType.C), Length(11)]
    public string FILLER1 { get; set; } = string.Empty;

    [Index(4), Type(DataType.C), Length(1)]
    public string TipoCobranca { get; set; } = string.Empty;
    [Index(5), Type(DataType.C), Length(5)]
    public string CodigoPagadorBanco { get; set; } = string.Empty;
    [Index(6), Type(DataType.C), Length(5)]
    public string CodigoPagadorEmpresa { get; set; } = string.Empty;
    [Index(7), Type(DataType.N), Length(1)]
    public string BoletoDDA { get; set; } = string.Empty;
    [Index(8), Type(DataType.N), Length(22)]
    public string FILLER2 { get; set; } = string.Empty;
    [Index(9), Type(DataType.N), Length(15)]
    public string NossoNumero { get; set; } = string.Empty;

    [Index(10), Type(DataType.C), Length(46)]
    public string FILLER3 { get; set; } = string.Empty;
    [Index(11), Type(DataType.C), Length(2)]
    public string Ocorrencia { get; set; } = string.Empty;
    [Index(12), Type(DataType.N), Length(6)]
    public string DataOcorrencia { get; set; } = string.Empty;
    [Index(13), Type(DataType.C), Length(10)]
    public string SeuNumero { get; set; } = string.Empty;
    [Index(14), Type(DataType.C), Length(20)]
    public string FILLER4 { get; set; } = string.Empty;
    [Index(15), Type(DataType.N), Length(6)]
    public string DataVencimento { get; set; } = string.Empty;
    [Index(16), Type(DataType.N), Length(13, 2)]
    public decimal ValorTitulo { get; set; }
    [Index(17), Type(DataType.C), Length(9)]
    public string FILLER5 { get; set; } = string.Empty;
    [Index(18), Type(DataType.C), Length(1)]
    public string Especie { get; set; } = string.Empty;
    [Index(19), Type(DataType.N), Length(13, 2)]
    public decimal DespesasCobranca { get; set; }
    [Index(20), Type(DataType.N), Length(13, 2)]
    public decimal DespesasProcesso { get; set; }

    [Index(21), Type(DataType.C), Length(26)]
    public string FILLER6 { get; set; } = string.Empty;

    [Index(22), Type(DataType.N), Length(13, 2)]
    public decimal AbatimentoConcedido { get; set; }
    [Index(23), Type(DataType.N), Length(13, 2)]
    public decimal DescontoConcedido { get; set; }
    [Index(24), Type(DataType.N), Length(13, 2)]
    public decimal ValorPago { get; set; }
    [Index(25), Type(DataType.N), Length(13, 2)]
    public decimal JurosMora { get; set; }
    [Index(26), Type(DataType.N), Length(13, 2)]
    public decimal Multa { get; set; }
    [Index(27), Type(DataType.C), Length(2)]
    public string FILLER7 { get; set; } = string.Empty;
    [Index(28), Type(DataType.C), Length(1)]
    public string ExOcorrencia19 { get; set; } = string.Empty;
    [Index(29), Type(DataType.C), Length(23)]
    public string FILLER8 { get; set; } = string.Empty;
    [Index(30), Type(DataType.C), Length(10)]
    public string MotivoOcorrencia { get; set; } = string.Empty;
    [Index(31), Type(DataType.N), Length(8)]
    public string DataPrevisaoLancamento { get; set; } = string.Empty;
    [Index(32), Type(DataType.C), Length(58)]
    public string FILLER9 { get; set; } = string.Empty;
    [Index(33), Type(DataType.N), Length(6)]
    public string SeqRegistro { get; set; } = string.Empty;


}