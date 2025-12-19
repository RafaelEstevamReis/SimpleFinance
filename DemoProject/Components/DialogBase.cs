using System;
using System.Windows.Forms;

namespace DemoProject.Components
{
    public class DialogBase : Form
    {
        protected override void OnLoad(EventArgs e)
        {
            KeyPreview = true;
            base.OnLoad(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;

                var r = MessageBox.Show("Close without saving?", "Close?", MessageBoxButtons.YesNo);
                if (r == DialogResult.Yes)
                {
                    DialogResult = DialogResult.Cancel;
                }
            }
            base.OnKeyDown(e);
        }

    }
}
