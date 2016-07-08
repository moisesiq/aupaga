using System;
using System.Windows.Forms;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

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
            oColLinea.CargarDatos("LineaID", "NombreLinea", Datos.GetListOf<Linea>(c => c.Estatus));
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

            var oDatos = Datos.GetListOf<VentaTicketLeyenda>();
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

                int iId = this.dgvDatos.ObtenerId(oFila); // Util.ConvertirEntero(oFila.Cells["__Id"].Value);
                int iCambio = this.dgvDatos.ObtenerIdCambio(oFila); // Util.ConvertirEntero(oFila.Cells["__Cambio"].Value);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new VentaTicketLeyenda();
                        else
                            oReg = Datos.GetEntity<VentaTicketLeyenda>(c => c.VentaTicketLeyendaID == iId);

                        // Se llenan los datos
                        oReg.NombreLeyenda = Util.Cadena(oFila.Cells["Nombre"].Value);
                        oReg.Leyenda = Util.Cadena(oFila.Cells["Leyenda"].Value);
                        oReg.LineaID = Util.Entero(oFila.Cells["LineaID"].Value);
                        oReg.LineaID = (oReg.LineaID > 0 ? oReg.LineaID : null);

                        Datos.Guardar<VentaTicketLeyenda>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = Datos.GetEntity<VentaTicketLeyenda>(c => c.VentaTicketLeyendaID == iId);
                        Datos.Eliminar<VentaTicketLeyenda>(oReg);
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
