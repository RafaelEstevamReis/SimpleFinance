namespace DemoProject;

using DemoProject.Components;
using Simple.BotUtils.DI;
using Simple.Finance;
using Simple.Finance.Helpers;
using Simple.Finance.Tables;
using Simple.Sqlite;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

internal static class Extensions
{
    public static void SaveConfig(this Form form, DateTimePicker control)
        => SaveConfig(form, control, o => o.Value);
    public static void SaveConfig(this Form form, CheckBox control)
        => SaveConfig(form, control, o => o.Checked);

    public static void SaveConfig<C, T>(this Form form, C control, Func<C, T> selector) where C : Control
    {
        Injector.Get<KeyValueStorage>().SetKey<T>(form.Name, control.Name, selector(control));
    }

    public static void GetConfig(this Form form, DateTimePicker control, DateTime def)
    {
        var kvs = Injector.Get<KeyValueStorage>();
        control.Value = kvs.GetKey<DateTime>(form.Name, control.Name, def);
    }
    public static void GetConfig(this Form form, CheckBox control, bool def)
    {
        control.Checked = Injector.Get<KeyValueStorage>().GetKey<bool>(form.Name, control.Name, def);
    }
    public static void GetConfig(this Form form, ComboBox control, int def)
    {
        control.SelectedIndex = Injector.Get<KeyValueStorage>().GetKey<int>(form.Name, control.Name, def);
    }
    public static void GetConfig(this Form form, ComboBox control, object def)
    {
        control.SelectedValue = Injector.Get<KeyValueStorage>().GetKey<object>(form.Name, control.Name) ?? def;
    }

    public static void FormatColumn(this DataGridViewTextBoxColumn column, Manager manager)
    {
        var wallets = manager.GetWalletsDict();
        var grid = column.DataGridView;
        if (grid is null) throw new ArgumentNullException(nameof(column), "Column must have a DataGridView");

        grid.CellFormatting += (object? sender, DataGridViewCellFormattingEventArgs e) =>
        {
            if (e.Value is not decimal dVal) return;

            var tx = grid.Rows[e.RowIndex].Tag as Transac;
            if (tx == null) return;

            var code = tx.GetTransactionCurrencyCode(wallets);
            if (string.IsNullOrEmpty(code)) return;

            e.FormattingApplied = true;
            e.Value = CurrencyHelpers.FormatFor(dVal, code);
        };
    }
    public static void SumMoneyBoxFor(this MoneyBox moneyBox, Manager manager, IEnumerable<Transac> txs)
        => SumMoneyBoxFor(moneyBox, manager, txs, o => o.EffectiveValue);
    public static void SumMoneyBoxFor(this MoneyBox moneyBox, Manager manager, IEnumerable<Transac> txs, Func<Transac, decimal> selector)
    {
        var transactionsByCurrency = manager.SumTransactionsByBaseCurrency(txs, selector).ToArray();

        moneyBox.MoneySign = "";
        moneyBox.DecimalPlaces = 2;
        moneyBox.Value = 0;
        if (transactionsByCurrency.Length == 1)
        {
            var code = CurrencyHelpers.GetCurrencyData(transactionsByCurrency[0].currencyCode);

            if (code != null)
            {
                moneyBox.MoneySign = code.NumberFormatInfo.CurrencySymbol;
                moneyBox.DecimalPlaces = code.NumberFormatInfo.CurrencyDecimalDigits;
            }
            moneyBox.Value = transactionsByCurrency[0].value;
        }
        else
        {
            moneyBox.Text = "-";
        }
    }
    public static void FormatFor(this MoneyBox moneyBox, string currencyCode)
    {
        var currValue = moneyBox.Value;

        moneyBox.MoneySign = "";
        moneyBox.DecimalPlaces = 2;

        var code = CurrencyHelpers.GetCurrencyData(currencyCode);
        if (code != null)
        {
            moneyBox.MoneySign = code.NumberFormatInfo.CurrencySymbol;
            moneyBox.DecimalPlaces = code.NumberFormatInfo.CurrencyDecimalDigits;
        }

        moneyBox.Value = currValue;
    }
}
