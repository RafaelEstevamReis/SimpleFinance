namespace DemoProject;

using System;
using System.Windows.Forms;

internal static class Extensions
{
    public static void SaveConfig(this Form form, DateTimePicker control)
        => SaveConfig(form, control, o => o.Value);
    public static void SaveConfig(this Form form, CheckBox control)
        => SaveConfig(form, control, o => o.Checked);

    public static void SaveConfig<C, T>(this Form form, C control, Func<C, T> selector) where C : Control
    {
        Program.Config.SetConfig<T>(control.Name, form.Name, selector(control));
    }

    public static void GetConfig(this Form form, DateTimePicker control, DateTime def)
    {
        control.Value = Program.Config.GetConfig(control.Name, form.Name, def);
    }
    public static void GetConfig(this Form form, CheckBox control, bool def)
    {
        control.Checked = Program.Config.GetConfig(control.Name, form.Name, def);
    }
    public static void GetConfig(this Form form, ComboBox control, int def)
    {
        control.SelectedIndex = Program.Config.GetConfig(control.Name, form.Name, def);
    }
    public static void GetConfig(this Form form, ComboBox control, object def)
    {
        control.SelectedValue = Program.Config.GetConfig(control.Name, form.Name, def);
    }
}
