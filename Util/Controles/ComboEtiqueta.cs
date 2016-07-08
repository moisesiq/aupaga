using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibUtil
{
    public partial class ComboEtiqueta : UserControl
    {
        public ComboEtiqueta()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public string Etiqueta
        {
            get { return this.lblEtiqueta.Text; }
            set { this.lblEtiqueta.Text = value; }
        }
        public Size tam { get { return this.cmbCombo.Size; } }
        public Color EtiquetaColor
        {
            get { return this.lblEtiqueta.ForeColor; }
            set { this.lblEtiqueta.ForeColor = value; }
        }

        public ComboBoxStyle DropDownStyle
        {
            get { return this.cmbCombo.DropDownStyle; }
            set { this.cmbCombo.DropDownStyle = value; }
        }

        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get { return this.cmbCombo.AutoCompleteCustomSource; }
            set { this.cmbCombo.AutoCompleteCustomSource = value; }
        }

        public AutoCompleteMode AutoCompleteMode
        {
            get { return this.cmbCombo.AutoCompleteMode; }
            set { this.cmbCombo.AutoCompleteMode = value; }
        }

        public AutoCompleteSource AutoCompleteSource
        {
            get { return this.cmbCombo.AutoCompleteSource; }
            set { this.cmbCombo.AutoCompleteSource = value; }
        }

        public object DataSource
        {
            get { return this.cmbCombo.DataSource; }
            set { this.cmbCombo.DataSource = value; }
        }

        public string DisplayMember
        {
            get { return this.cmbCombo.DisplayMember; }
            set { this.cmbCombo.DisplayMember = value; }
        }

        public string ValueMember
        {
            get { return this.cmbCombo.ValueMember; }
            set { this.cmbCombo.ValueMember = value; }
        }

        public override string Text
        {
            get { return this.cmbCombo.Text; }
            set { this.cmbCombo.Text = value; }
        }

        public int SelectedIndex
        {
            get { return this.cmbCombo.SelectedIndex; }
            set { this.cmbCombo.SelectedIndex = value; }
        }

        public object SelectedItem
        {
            get { return this.cmbCombo.SelectedItem; }
            set { this.cmbCombo.SelectedItem = value; }
        }

        public string SelectedText
        {
            get { return this.cmbCombo.SelectedText; }
            set { this.cmbCombo.SelectedText = value; }
        }

        public object SelectedValue
        {
            get { return this.cmbCombo.SelectedValue; }
            set { this.cmbCombo.SelectedValue = value; }
        }

        public override bool Focused
        {
            get { return this.cmbCombo.Focused; }
        }

        public ComboBox.ObjectCollection Items
        {
            get { return this.cmbCombo.Items; }
        }

        public ComboBox Combo { get { return this.cmbCombo; } }

        #endregion

        #region [ Manejadores de Eventos ]

        public event EventHandler SelectedIndexChanged;
        public new event EventHandler TextChanged;

        #endregion

        #region [ Eventos ]

        private void lblEtiqueta_Click(object sender, EventArgs e)
        {
            this.cmbCombo.Focus();
            this.cmbCombo_Enter(sender, e);
        }

        private void cmbCombo_Enter(object sender, EventArgs e)
        {
            this.lblEtiqueta.Visible = false;
        }

        private void cmbCombo_Leave(object sender, EventArgs e)
        {
            this.lblEtiqueta.Visible = (this.cmbCombo.Text == "");
        }

        private void cmbCombo_DropDownStyleChanged(object sender, EventArgs e)
        {
            if (this.cmbCombo.DropDownStyle == ComboBoxStyle.DropDownList)
                this.lblEtiqueta.SendToBack();
            else
                this.lblEtiqueta.BringToFront();
        }

        private void cmbCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblEtiqueta.Visible = (this.cmbCombo.Text == "");
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged.Invoke(sender, e);
        }

        private void cmbCombo_TextChanged(object sender, EventArgs e)
        {
            this.lblEtiqueta.Visible = (!this.cmbCombo.Focused && this.cmbCombo.Text == "");

            if (this.TextChanged != null)
                this.TextChanged.Invoke(sender, e);
        }

        #endregion

    }
}
