using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using LibUtil;
using TheosProc;

namespace Refaccionaria.App
{
    public partial class DetalleBusquedaDesdeMovimientos : DetalleBase
    {
        public bool Seleccionado = false;
        public List<Dictionary<string, object>> Sel;
        
        public static DetalleBusquedaDesdeMovimientos Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly DetalleBusquedaDesdeMovimientos instance = new DetalleBusquedaDesdeMovimientos();
        }

        public DetalleBusquedaDesdeMovimientos()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public string BusquedaPredefinida { get; set; }

        #endregion

        #region [ Eventos ]

        private void DetalleBusquedaDesdeMovimientos_Load(object sender, EventArgs e)
        {
            Seleccionado = false;
            this.cargaInicial();

            if (!string.IsNullOrEmpty(this.BusquedaPredefinida))
            {
                this.txtDescripcion.Text = this.BusquedaPredefinida;
                this.ActiveControl = this.txtDescripcion;
            }
        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var dic = new Dictionary<string, object>();
                dic.Add("Codigo", this.txtCodigo.Text.Replace("'", ""));
                for (int x = 0; x < 9; x++)
                {
                    dic.Add(string.Format("{0}{1}", "Descripcion", x + 1), null);
                }

                var lst = Datos.ExecuteProcedure<pauPartesBusquedaEnMovimientos_Result>("pauPartesBusquedaEnMovimientos", dic);

                if (lst != null)
                {
                    this.dgvDatos.DataSource = null;
                    this.dgvDatos.DataSource = lst;
                    this.lblEncontrados.Text = string.Format("Encontrados: {0}", lst.Count);
                    Util.OcultarColumnas(this.dgvDatos, new string[] { "ParteID" });
                    UtilLocal.ColumnasToHeaderText(this.dgvDatos);
                    if (!this.dgvDatos.Columns.Contains("X"))
                    {
                        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                        checkColumn.Name = "X";
                        checkColumn.HeaderText = "";
                        checkColumn.Width = 50;
                        checkColumn.ReadOnly = false;
                        checkColumn.FillWeight = 10;
                        this.dgvDatos.Columns.Add(checkColumn);
                        this.dgvDatos.Columns["X"].DisplayIndex = 0;
                    }

                    foreach (DataGridViewColumn column in this.dgvDatos.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        column.ReadOnly = (column.Name != "X" && column.Name != "Costo" && column.Name != "Cantidad");
                    }

                    this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                }
                else
                {
                    lblEncontrados.Text = "Encontrados:";
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
                
        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.txtCodigo.Clear();
                this.txtDescripcion.Clear();
                this.cargaInicial();
            }
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvDatos.Focus();
            }
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string input = this.txtDescripcion.Text.Replace("'", "");
                string[] matches = Regex.Matches(input, @""".*?""|[^\s]+").Cast<Match>().Select(m => m.Value).ToArray();

                if (matches.Length > 0 && matches.Length < 10)
                {
                    var dic = new Dictionary<string, object>();
                    dic.Add("Codigo", null);
                    for (int x = 0; x < matches.Length; x++)
                    {
                        dic.Add(string.Format("{0}{1}", "Descripcion", x + 1), matches[x].ToString());
                    }

                    var lst = Datos.ExecuteProcedure<pauPartesBusquedaEnMovimientos_Result>("pauPartesBusquedaEnMovimientos", dic);

                    if (lst != null)
                    {
                        this.dgvDatos.DataSource = null;
                        this.dgvDatos.DataSource = lst;
                        this.lblEncontrados.Text = string.Format("Encontrados: {0}", lst.Count);
                        Util.OcultarColumnas(this.dgvDatos, new string[] { "ParteID" });
                        UtilLocal.ColumnasToHeaderText(this.dgvDatos);
                        if (!this.dgvDatos.Columns.Contains("X"))
                        {
                            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                            checkColumn.Name = "X";
                            checkColumn.HeaderText = "";
                            checkColumn.Width = 50;
                            checkColumn.ReadOnly = false;
                            checkColumn.FillWeight = 10;
                            this.dgvDatos.Columns.Add(checkColumn);
                            this.dgvDatos.Columns["X"].DisplayIndex = 0;
                        }

                        foreach (DataGridViewColumn column in this.dgvDatos.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                            column.ReadOnly = (column.Name != "X" && column.Name != "Costo" && column.Name != "Cantidad");
                        }

                        this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    }
                    else
                    {
                        lblEncontrados.Text = "Encontrados:";
                    }
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtDescripcion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.txtCodigo.Clear();
                this.txtDescripcion.Clear();
                this.cargaInicial();
            }
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvDatos.Focus();
            }
        }

        private void txtDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;

            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Space)
            {
                var fila = this.dgvDatos.CurrentRow.Index;
                if (Convert.ToBoolean(this.dgvDatos.Rows[fila].Cells["X"].Value).Equals(true))
                    this.dgvDatos.Rows[fila].Cells["X"].Value = false;
                else
                    this.dgvDatos.Rows[fila].Cells["X"].Value = true;
            }

            if (e.KeyCode == Keys.F5)
            {
                this.cargaInicial();
            }

            if (e.KeyCode == Keys.Enter)
            {
                var filasSel = 0;
                foreach (DataGridViewRow row in this.dgvDatos.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["X"].Value).Equals(true)) 
                    {
                        filasSel += 1;
                    }

                }
                if (filasSel == 0)
                {
                    var dic = new Dictionary<string, object>();
                    dic.Add("ParteID", this.dgvDatos.CurrentRow.Cells["ParteID"].Value);
                    Sel.Add(dic);
                    this.Seleccionado = true;
                }
                else
                {
                    this.btnGuardar_Click(sender, null);
                }
            }

            if (Seleccionado)
                this.Close();
        }
                        
        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dgvDatos.Rows)
            {
                if (Convert.ToBoolean(row.Cells["X"].Value).Equals(true))
                {
                    var dic = new Dictionary<string, object>();
                    dic.Add("ParteID", row.Cells["ParteID"].Value);
                    dic.Add("Costo", row.Cells["Costo"].Value);
                    dic.Add("Cantidad", row.Cells["Cantidad"].Value);
                    Sel.Add(dic);
                    this.Seleccionado = true;
                }
            }
            
            if (Seleccionado)
                this.Close();
        }

        private void DetalleBusquedaDesdeMovimientos_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.txtCodigo.Focus();
        }

        #endregion

        #region [ Metodos ]

        private void cargaInicial()
        {
            this.Sel = new List<Dictionary<string, object>>();
            try
            {
                this.txtCodigo.Clear();
                this.txtDescripcion.Clear();
                this.dgvDatos.DataSource = null;
                if (this.dgvDatos.Columns.Count > 0)
                    this.dgvDatos.Columns.Clear();
                if (this.dgvDatos.Rows.Count > 0)
                    this.dgvDatos.Rows.Clear();
                this.txtCodigo.Focus();
                this.txtDescripcion.Focus();
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

    }
}
