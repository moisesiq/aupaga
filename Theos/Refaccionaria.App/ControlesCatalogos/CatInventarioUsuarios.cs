using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CatInventarioUsuarios : ListadoEditable
    {
        // Para el Singleton
        private static readonly CatInventarioUsuarios instance = new CatInventarioUsuarios();
        public static CatInventarioUsuarios Instance
        {
            get { return CatInventarioUsuarios.instance; }
        }
        //

        public CatInventarioUsuarios()
        {
            InitializeComponent();

            // Se agregan las columnas al grid
            var oColSucursal = this.AgregarColumnaCombo("SucursalID", "Sucursal", 120);
            oColSucursal.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));
            var oColUsuario = this.AgregarColumnaCombo("InvUsuarioID", "Usuario", 120);
            oColUsuario.CargarDatos("UsuarioID", "NombreUsuario", Datos.GetListOf<Usuario>(c => c.Estatus));
            this.AgregarColumna("ArticulosDiarios", "Art. diarios", 80);
        }

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oDatos = Datos.GetListOf<InventarioUsuario>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                this.dgvDatos.AgregarFila(oReg.InventarioUsuarioID, Cat.TiposDeAfectacion.SinCambios, oReg.SucursalID, oReg.InvUsuarioID, oReg.ArticulosDiarios);
            }

            Cargando.Cerrar();
        }

        protected override bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            InventarioUsuario oReg;
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
                            oReg = new InventarioUsuario();
                        else
                            oReg = Datos.GetEntity<InventarioUsuario>(c => c.InventarioUsuarioID == iId);

                        oReg.InvUsuarioID = Util.Entero(oFila.Cells["InvUsuarioID"].Value);
                        oReg.SucursalID = Util.Entero(oFila.Cells["SucursalID"].Value);
                        oReg.ArticulosDiarios = Util.Entero(oFila.Cells["ArticulosDiarios"].Value);

                        Datos.Guardar<InventarioUsuario>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = Datos.GetEntity<InventarioUsuario>(c => c.InventarioUsuarioID == iId);
                        Datos.Eliminar<InventarioUsuario>(oReg);
                        break;
                }
            }

            Cargando.Cerrar();
            this.CargarDatos();
            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();

            bool bError = false;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (oFila.IsNewRow) continue;
                if (Util.Entero(oFila.Cells["InvUsuarioID"].Value) < 1)
                {
                    oFila.ErrorText = "Usuario inválido.";
                    bError = true;
                }
                if (Util.Entero(oFila.Cells["SucursalID"].Value) < 1)
                {
                    oFila.ErrorText = "Sucursal inválida.";
                    bError = true;
                }
            }

            return (!bError);
        }

        #endregion

    }
}
