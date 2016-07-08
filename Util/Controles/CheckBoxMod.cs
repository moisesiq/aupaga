using System.Windows.Forms;

namespace LibUtil
{
    public partial class CheckBoxMod : CheckBox
    {
        bool KeyDownEnControl;

        public CheckBoxMod()
        {
            this.InitializeComponent();
        }
        private void InitializeComponent()
        {
            // 
            // CheckBox
            // 
            this.KeyDown += new KeyEventHandler(CheckBoxMod_KeyDown);
            this.KeyUp += new KeyEventHandler(CheckBoxMod_KeyUp);
        }

        #region [ Propiedades ]



        #endregion

        #region [ Eventos ]

        void CheckBoxMod_KeyDown(object sender, KeyEventArgs e)
        {
            this.KeyDownEnControl = true;
        }

        void CheckBoxMod_KeyUp(object sender, KeyEventArgs e)
        {
            if (!this.KeyDownEnControl) return;
            this.KeyDownEnControl = false;

            if (e.KeyCode == Keys.ShiftKey && !e.KeyData.HasFlag(Keys.Tab))
                this.Checked = !this.Checked;
        }

        #endregion
    }
}
