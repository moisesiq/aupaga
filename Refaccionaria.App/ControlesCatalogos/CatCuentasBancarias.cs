using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

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
            oColBanco.CargarDatos("BancoID", "NombreBanco", General.GetListOf<Banco>(c => c.Estatus));
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

            var oDatos = General.GetListOf<BancoCuenta>();
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
                            oReg = General.GetEntity<BancoCuenta>(c => c.BancoCuentaID == iId);

                        // Se llenan los datos
                        oReg.NumeroDeCuenta = Helper.ConvertirCadena(oFila.Cells["NumeroDeCuenta"].Value);
                        oReg.NombreDeCuenta = Helper.ConvertirCadena(oFila.Cells["NombreDeCuenta"].Value);
                        oReg.BancoID = Helper.ConvertirEntero(oFila.Cells["BancoID"].Value);
                        oReg.UsoProveedores = Helper.ConvertirBool(oFila.Cells["UsoProveedores"].Value);
                        oReg.UsoClientes = Helper.ConvertirBool(oFila.Cells["UsoClientes"].Value);
                        oReg.EsCpcp = Helper.ConvertirBool(oFila.Cells["EsCpcp"].Value);

                        Guardar.Generico<BancoCuenta>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<BancoCuenta>(c => c.BancoCuentaID == iId);
                        Guardar.Eliminar<BancoCuenta>(oReg);
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
                if (Helper.ConvertirCadena(oFila.Cells["NumeroDeCuenta"].Value) == "")
                {
                    oFila.Cells["NumeroDeCuenta"].ErrorText = "Número de cuenta inválido.";
                    bError = true;
                }
                if (Helper.ConvertirCadena(oFila.Cells["NombreDeCuenta"].Value) == "")
                {
                    oFila.Cells["NombreDeCuenta"].ErrorText = "Nombre de cuenta inválido.";
                    bError = true;
                }
                if (Helper.ConvertirEntero(oFila.Cells["BancoID"].Value) <= 0)
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
