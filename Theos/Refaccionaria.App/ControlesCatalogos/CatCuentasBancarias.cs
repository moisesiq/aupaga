using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CatCuentasBancarias : ListadoEditable
    {
        // Para el Singleton
        private static CatCuentasBancarias _Instance;
        public static CatCuentasBancarias Instance
        {
            get
            {
                if (CatCuentasBancarias._Instance == null || CatCuentasBancarias._Instance.IsDisposed)
                    CatCuentasBancarias._Instance = new CatCuentasBancarias();
                return CatCuentasBancarias._Instance;
            }
        }
        //

        public CatCuentasBancarias()
        {
            InitializeComponent();

            // Se agregan las columnas al grid
            this.AgregarColumna("CuentaBancariaID", "Id", 60).ReadOnly = true;
            this.AgregarColumna("NumeroDeCuenta", "No. Cuenta", 120);
            this.AgregarColumna("NombreDeCuenta", "Nombre", 160);
            var oColBanco = this.AgregarColumnaCombo("BancoID", "Banco", 160);
            oColBanco.CargarDatos("BancoID", "NombreBanco", Datos.GetListOf<Banco>(c => c.Estatus));
            this.AgregarColumnaCheck("UsoProveedores", "Proveedores", 80);
            this.AgregarColumnaCheck("UsoClientes", "Clientes", 80);
            this.AgregarColumnaCheck("EsCpcp", "Cpcp", 80);
        }
        
        #region [ Eventos ]



        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oDatos = Datos.GetListOf<BancoCuenta>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                this.dgvDatos.AgregarFila(oReg.BancoCuentaID, Cat.TiposDeAfectacion.SinCambios, oReg.BancoCuentaID, oReg.NumeroDeCuenta, oReg.NombreDeCuenta
                    , oReg.BancoID, oReg.UsoProveedores, oReg.UsoClientes, oReg.EsCpcp);
            }

            Cargando.Cerrar();
        }

        protected override bool AccionGuardar()
        {
            // Se valida
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            BancoCuenta oReg;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iId = this.dgvDatos.ObtenerId(oFila);
                int iCambio = this.dgvDatos.ObtenerIdCambio(oFila);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new BancoCuenta();
                        else
                            oReg = Datos.GetEntity<BancoCuenta>(c => c.BancoCuentaID == iId);

                        // Se llenan los datos
                        oReg.NumeroDeCuenta = Util.Cadena(oFila.Cells["NumeroDeCuenta"].Value);
                        oReg.NombreDeCuenta = Util.Cadena(oFila.Cells["NombreDeCuenta"].Value);
                        oReg.BancoID = Util.Entero(oFila.Cells["BancoID"].Value);
                        oReg.UsoProveedores = Util.Logico(oFila.Cells["UsoProveedores"].Value);
                        oReg.UsoClientes = Util.Logico(oFila.Cells["UsoClientes"].Value);
                        oReg.EsCpcp = Util.Logico(oFila.Cells["EsCpcp"].Value);

                        Datos.Guardar<BancoCuenta>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = Datos.GetEntity<BancoCuenta>(c => c.BancoCuentaID == iId);
                        Datos.Eliminar<BancoCuenta>(oReg);
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
                // Se limpian los errores primero
                foreach (DataGridViewCell oCelda in oFila.Cells)
                    oCelda.ErrorText = "";
                // Se realiza la validación
                if (Util.Cadena(oFila.Cells["NumeroDeCuenta"].Value) == "")
                {
                    oFila.Cells["NumeroDeCuenta"].ErrorText = "Número de cuenta inválido.";
                    bError = true;
                }
                if (Util.Cadena(oFila.Cells["NombreDeCuenta"].Value) == "")
                {
                    oFila.Cells["NombreDeCuenta"].ErrorText = "Nombre de cuenta inválido.";
                    bError = true;
                }
                if (Util.Entero(oFila.Cells["BancoID"].Value) <= 0)
                {
                    oFila.Cells["BancoID"].ErrorText = "Debes seleccionar un Banco.";
                    bError = true;
                }
            }
            if (bError)
                this.ctlError.PonerError(this.btnGuardar, "Existen errores de validación en uno o más celdas.", ErrorIconAlignment.MiddleLeft);

            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion
    }
}
