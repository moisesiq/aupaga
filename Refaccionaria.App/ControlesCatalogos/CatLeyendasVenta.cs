using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CatLeyendasVenta : ListadoEditable
    {
        // Para el Singleton
        private static CatLeyendasVenta _Instance;
        public static CatLeyendasVenta Instance
        {
            get
            {
                if (CatLeyendasVenta._Instance == null || CatLeyendasVenta._Instance.IsDisposed)
                    CatLeyendasVenta._Instance = new CatLeyendasVenta();
                return CatLeyendasVenta._Instance;
            }
        }
        //

        public CatLeyendasVenta()
        {
            InitializeComponent();

            // Se agregan las columnas al grid
            base.AgregarColumna("Nombre", "Nombre", 100);
            base.AgregarColumna("Leyenda", "Leyenda", 320);
            var oColLinea = base.AgregarColumnaCombo("LineaID", "Línea", 240);
            oColLinea.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Linea>(c => c.Estatus));
            (oColLinea.DataSource as List<Linea>).Insert(0, new Linea());
        }

        #region [ Eventos ]

        private void CatLeyendasVenta_Load(object sender, EventArgs e)
        {
            // Los datos se mandan cargar desde el "Load" la base, "ListadoEditable"
        }
                
        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oDatos = General.GetListOf<VentaTicketLeyenda>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                this.dgvDatos.AgregarFila(oReg.VentaTicketLeyendaID, Cat.TiposDeAfectacion.SinCambios, oReg.NombreLeyenda, oReg.Leyenda, oReg.LineaID);
            }

            Cargando.Cerrar();
        }

        protected override bool AccionGuardar()
        {
            // Se valida
            /* if (!this.Validar())
                return false;
            */

            Cargando.Mostrar();

            VentaTicketLeyenda oReg;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iId = this.dgvDatos.ObtenerId(oFila); // Helper.ConvertirEntero(oFila.Cells["__Id"].Value);
                int iCambio = this.dgvDatos.ObtenerIdCambio(oFila); // Helper.ConvertirEntero(oFila.Cells["__Cambio"].Value);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new VentaTicketLeyenda();
                        else
                            oReg = General.GetEntity<VentaTicketLeyenda>(c => c.VentaTicketLeyendaID == iId);

                        // Se llenan los datos
                        oReg.NombreLeyenda = Helper.ConvertirCadena(oFila.Cells["Nombre"].Value);
                        oReg.Leyenda = Helper.ConvertirCadena(oFila.Cells["Leyenda"].Value);
                        oReg.LineaID = Helper.ConvertirEntero(oFila.Cells["LineaID"].Value);
                        oReg.LineaID = (oReg.LineaID > 0 ? oReg.LineaID : null);

                        Guardar.Generico<VentaTicketLeyenda>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<VentaTicketLeyenda>(c => c.VentaTicketLeyendaID == iId);
                        Guardar.Eliminar<VentaTicketLeyenda>(oReg);
                        break;
                }
            }

            Cargando.Cerrar();
            this.CargarDatos();
            return true;
        }

        #endregion
    }
}
