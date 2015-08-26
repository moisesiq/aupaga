using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Negocio;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class DetalleEquivalenteSeleccion : DetalleBase
    {
        public string NumeroParte { get; set; }
        public bool Seleccionado = false;
        public Dictionary<string, object> Sel;

        public static DetalleEquivalenteSeleccion Instance
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

            internal static readonly DetalleEquivalenteSeleccion instance = new DetalleEquivalenteSeleccion();
        }

        public DetalleEquivalenteSeleccion()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void DetalleEquivalenteSeleccion_Load(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.NumeroParte))
                {
                    this.dgvDatos.DataSource = null;
                    this.Text = string.Format("{0} {1}", "Seleccione un equivalente para el Número de parte: ", this.NumeroParte);
                    var lista = General.GetListOf<PartesView>(p => p.NumeroDeParte == this.NumeroParte).ToList();
                    if (lista != null)
                    {
                        this.dgvDatos.DataSource = lista;
                        Helper.ColumnasToHeaderText(this.dgvDatos);                        
                        Helper.OcultarColumnas(this.dgvDatos, new string[] { "Marca", "FaltanCaracteristicas", "ParteEstatusID", "LineaID", "Aplicacion", "Equivalente",	"FechaRegistro", "Busqueda" });

                        if (!this.dgvDatos.Columns.Contains("X"))
                        {
                            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                            checkColumn.Name = "X";
                            checkColumn.HeaderText = " ";
                            checkColumn.Width = 50;
                            checkColumn.ReadOnly = false;
                            checkColumn.FillWeight = 10;
                            this.dgvDatos.Columns.Add(checkColumn);
                            this.dgvDatos.Columns["X"].DisplayIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            int cont = 0;
            foreach (DataGridViewRow row in this.dgvDatos.Rows)
            if (Convert.ToBoolean(row.Cells["X"].Value).Equals(true))                
                    cont++;

            if (cont == 0)
            {
                Helper.MensajeAdvertencia("Debe seleccionar un registro.", GlobalClass.NombreApp);
                return;
            }

            if (cont > 1)
            {
                Helper.MensajeAdvertencia("Debe seleccionar un solo registro.", GlobalClass.NombreApp);
                return;
            }

            this.Sel = new Dictionary<string, object>();
            foreach (DataGridViewRow row in this.dgvDatos.Rows)
            {
                if (Convert.ToBoolean(row.Cells["X"].Value).Equals(true))
                {
                    var detalle = new PartesView()
                    {
                        ParteID = Helper.ConvertirEntero(row.Cells["ParteID"].Value),
                        NumeroDeParte = row.Cells["NumeroDeParte"].Value.ToString(),
                        Descripcion = row.Cells["Descripcion"].Value.ToString(),
                        LineaID = Helper.ConvertirEntero(row.Cells["LineaID"].Value)
                    };

                    Sel.Add("PartesView", detalle);
                    this.Seleccionado = true;
                }
            }

            if (Seleccionado)
                this.Close();
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Seleccionado = false;
            this.Close();
        }

        #endregion

    }
}
