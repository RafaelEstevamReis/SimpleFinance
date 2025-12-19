using System;
using System.Windows.Forms;

namespace DemoProject.Components
{
    public class MoneyBox : TextBox
    {
        private decimal value = 0;
        public decimal Value
        {
            get => value;
            set
            {
                this.value = value;
                renderValue();
            }
        }

        public int DecimalPlaces { get; set; } = 2;
        public string MoneySign { get; set; } = "";

        protected override void OnCreateControl()
        {
            try
            {
                this.TextAlign = HorizontalAlignment.Right;
                renderValue();
            }
            catch { }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (ReadOnly) return;
            if (!Enabled) return;

            e.Handled = true;
            e.SuppressKeyPress = true;

            if (e.KeyCode == Keys.Back)
            {
                Value /= 10;
                Value = Math.Round(Value, DecimalPlaces, MidpointRounding.ToZero);
                base.OnKeyDown(e);
            }
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                Value *= 10;
                Value += (e.KeyCode - Keys.D0) * (1 / (decimal)Math.Pow(10, DecimalPlaces));
                base.OnKeyDown(e);
            }
            else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                Value *= 10;
                Value += (e.KeyCode - Keys.NumPad0) * (1 / (decimal)Math.Pow(10, DecimalPlaces)); ;
                base.OnKeyDown(e);
            }
            else
            {
                base.OnKeyDown(e);
                return;
            }

            if (Value > 10_000_000_000_000) Value = 10_000_000_000_000;

            renderValue();
        }
        void renderValue()
        {
            if (IsDisposed) return;
            if (Disposing) return;

            this.Text = (MoneySign + " " + Value.ToString("N" + DecimalPlaces)).Trim();
        }
    }
}
