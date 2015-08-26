using System;
using System.Windows.Forms;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CatNominaOficialUsuarios : ListadoEditable
    {
        // Para el Singleton
        private static CatNominaOficialUsuarios _Instance;
        public static CatNominaOficialUsuarios Instance
        {
            get
            {
                if (CatNominaOficialUsuarios._Instance == null || CatNominaOficialUsuarios._Instance.IsDisposed)
                    CatNominaOficialUsuarios._Instance = new CatNominaOficialUsuarios();
                return CatNominaOficialUsuarios._Instance;
            }
        }
        //

        ControlError ctlError = new ControlError();

        public CatNominaOficialUsuarios()
        {
            InitializeComponent();

            // Se agregan las columnas al grid
            var oCol = base.AgregarColumna("ContaCuentaDeMayorID", "", 20);
            oCol.Visible = false;
            oCol = base.AgregarColumna("CuentaDeMayor", "Cuenta", 320);
            oCol.ReadOnly = true;
            base.AgregarColumnaImporte("Importe", "Importe");
            this.dgvDatos.AllowUserToAddRows = false;
            this.dgvDatos.AllowUserToDeleteRows = false;
            this.dgvDatos.PermitirBorrar = false;
        }
        
        #region [ Eventos ]

        private void CatNominaOficialUsuarios_Load(object sender, EventArgs e)
        {
            this.cmbUsuario.CargarDatos("UsuarioID", "NombreUsuario", General.GetListOf<Usuario>(c => c.Activo && c.Estatus).OrderBy(c => c.NombreUsuario).ToList());
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
        }

        private void cmbUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbUsuario.Focused)
                this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            this.dgvDatos.Rows.Clear();
            int iUsuarioID = Helper.ConvertirEntero(this.cmbUsuario.SelectedValue);
            if (iUsuarioID <= 0) return;

            Cargando.Mostrar();

            // Se cargan los importes de cuentas
            var oCuentas = General.GetListOf<NominaOficialCuentasView>();
            var oDatos = General.GetListOf<UsuarioNominaOficial>(c => c.IdUsuario == iUsuarioID);
            foreach (var oReg in oCuentas)
            {
                // Se busca el registro del usuario y la cuenta correspondiente
                var oRegUs = oDatos.FirstOrDefault(c => c.ContaCuentaDeMayorID == oReg.ContaCuentaDeMayorID);
                if (oRegUs == null)
                    this.dgvDatos.AgregarFila(0, Cat.TiposDeAfectacion.Agregar, oReg.ContaCuentaDeMayorID, oReg.CuentaDeMayor, 0);
                else
                    this.dgvDatos.AgregarFila(oRegUs.UsuarioNominaOficialID, Cat.TiposDeAfectacion.SinCambios, oReg.ContaCuentaDeMayorID, oReg.CuentaDeMayor
                        , oRegUs.Importe);
            }

            // Se carga el sueldo fijo
            var oNomina = General.GetEntity<UsuarioNomina>(c => c.IdUsuario == iUsuarioID);
            if (oNomina == null)
            {
                this.cmbSucursal.SelectedIndex = -1;
                this.txtSueldoFijo.Text = "";
            }
            else
            {
                this.cmbSucursal.SelectedValue = oNomina.SucursalID;
                this.txtSueldoFijo.Text = oNomina.SueldoFijo.ToString();
            }

            Cargando.Cerrar();
        }

        protected override bool AccionGuardar()
        {
            // Se valida
            if (!this.Validar())
                return false;

            int iUsuarioID = Helper.ConvertirEntero(this.cmbUsuario.SelectedValue);
            if (iUsuarioID <= 0) return false;

            Cargando.Mostrar();

            UsuarioNominaOficial oReg;
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
                            oReg = new UsuarioNominaOficial() { IdUsuario = iUsuarioID };
                        else
                            oReg = General.GetEntity<UsuarioNominaOficial>(c => c.UsuarioNominaOficialID == iId);

                        // Se llenan los datos
                        oReg.ContaCuentaDeMayorID = Helper.ConvertirEntero(oFila.Cells["ContaCuentaDeMayorID"].Value);
                        oReg.Importe = Helper.ConvertirDecimal(oFila.Cells["Importe"].Value);

                        Guardar.Generico<UsuarioNominaOficial>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<UsuarioNominaOficial>(c => c.UsuarioNominaOficialID == iId);
                        Guardar.Eliminar<UsuarioNominaOficial>(oReg);
                        break;
                }
            }

            // Se guarda el dato de Sueldo Fijo
            var oNomina = General.GetEntity<UsuarioNomina>(c => c.IdUsuario == iUsuarioID);
            if (oNomina == null)
                oNomina = new UsuarioNomina() { IdUsuario = iUsuarioID };
            oNomina.SucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            oNomina.SueldoFijo = Helper.ConvertirDecimal(this.txtSueldoFijo.Text);
            Guardar.Generico<UsuarioNomina>(oNomina);

            Cargando.Cerrar();
            this.CargarDatos();
            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (Helper.ConvertirEntero(this.cmbSucursal.SelectedValue) == 0)
                this.ctlError.PonerError(this.cmbSucursal, "Debes especificar la sucursal.");
            return this.ctlError.Valido;
        }

        #endregion

    }
}
