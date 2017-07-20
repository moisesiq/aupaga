using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App.Administracion.Formas
{
    public partial class ImprimirEtiquetas : Form
    {
        public ImprimirEtiquetas()
        {
            InitializeComponent();
        }

        private void ImprimirEtiquetas_Load(object sender, EventArgs e)
        {
            this.cmbLinea.CargarDatos("LineaID", "NombreLinea", Datos.GetListOf<Linea>(q => q.Estatus).OrderBy(o => o.NombreLinea).ToList());
            this.cmbSucursales.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cargando.Mostrar();
            int LineaId = Util.Entero(cmbLinea.SelectedValue);
            int SucursalID = Util.Entero(cmbSucursales.SelectedValue);
            dgvExistencias.DataSource = Datos.GetListOf<PartesExistenciasView>(c => c.Existencia > 0 && c.LineaID == LineaId && c.SucursalID == SucursalID);
            Util.OcultarColumnas(this.dgvExistencias, new string[]{"Registro","ParteExistenciaID","ProveedorID","Proveedor","MarcaID","Marca","LineaID","Linea","Costo","CostoConDescuento","SucursalID","UltimaVenta","UltimaCompra","FolioFactura"});
            Cargando.Cerrar();
        }

        private void lblNombreParte_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cargando.Mostrar();
            var etiquetas = new List<Etiquetas>();
            foreach (DataGridViewRow item in this.dgvExistencias.Rows)
            {
                int pID = Util.Entero(item.Cells["ParteID"].Value);
                var oParte = Datos.GetEntity<Parte>(c => c.ParteID == pID);
                decimal test = (decimal)item.Cells["Existencia"].Value;
                for (var x = 0; x < (decimal)item.Cells["Existencia"].Value; x++)
                {
                    var etiqueta = new Etiquetas()
                    {

                        ParteID = pID,
                        NumeroParte = oParte.NumeroParte,
                        sParteID = oParte.ParteID.ToString("D8"),
                        NombreParte = oParte.NombreParte,
                        CodigoBarra = oParte.CodigoBarra,
                        NumeroEtiquetas = Convert.ToInt32(Math.Round(test, 0))
                    };
                    etiquetas.Add(etiqueta);
                }
            }
            Cargando.Cerrar();
            IEnumerable<Etiquetas> detalleE = etiquetas;
            using (FastReport.Report report = new FastReport.Report())
            {
                report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteEtiquetas.frx"));
                report.RegisterData(detalleE, "etiquetas", 3);
                report.SetParameterValue("FolioFactura", 0);
                report.GetDataSource("etiquetas").Enabled = true;
                report.Show(true);
                //report.Design(true);
            }
            
        }

        
    }
}
