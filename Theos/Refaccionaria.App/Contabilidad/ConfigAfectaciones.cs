using System;
using System.Windows.Forms;
using System.Data;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class ConfigAfectaciones : UserControl
    {
        ControlError ctlError = new ControlError();
        DataTable oListado;
        enum TiposDeCuenta { CuentaAuxiliar, CuentaDeMayor, CasoFijo }

        public ConfigAfectaciones()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ConfigAfectaciones_Load(object sender, EventArgs e)
        {
            this.cmbOperacion.CargarDatos("ContaConfigAfectacionID", "Operacion", Datos.GetListOf<ContaConfigAfectacion>().OrderBy(c => c.Operacion).ToList());
            this.cmbTipoDePoliza.CargarDatos("ContaTipoPolizaID", "TipoDePoliza", Datos.GetListOf<ContaTipoPoliza>());
            // this.CuentaAuxiliarID.CargarDatos("ContaCuentaAuxiliarID", "CuentaAuxiliar", General.GetListOf<ContaCuentaAuxiliar>());
            this.CargoAbono.Items.AddRange("Cargo", "Abono");
            this.AsigSucursalID.CargarDatos("ContaPolizaAsigSucursalID", "Sucursal", Datos.GetListOf<ContaPolizaAsigSucursal>());
            this.dgvAfectaciones.Inicializar();

            // Se cargan los datos para el grid de búsqueda
            var oDatos = Datos.GetListOf<ContaCuentaAuxiliar>();
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Clientes, CuentaAuxiliar = "* CLIENTES" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Proveedores, CuentaAuxiliar = "* PROVEEDORES" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Bancos, CuentaAuxiliar = "* BANCOS" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Agua, CuentaAuxiliar = "* GASTOS" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.InteresesBancarios, CuentaAuxiliar = "* INTERESES BANCARIOS" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo, CuentaAuxiliar = "* CUENTAS POR PAGAR CORTO PLAZO" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.ReparteDeUtilidades, CuentaAuxiliar = "* REPARTO DE UTILIDADES" });

            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Salarios, CuentaAuxiliar = "* SALARIOS" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.TiempoExtra, CuentaAuxiliar = "* TIEMPO EXTRA" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.PremioDeAsistencia, CuentaAuxiliar = "* PREMIO DE ASISTENCIA" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.PremioDePuntualidad, CuentaAuxiliar = "* PREMIO DE PUNTUALIDAD" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Vacaciones, CuentaAuxiliar = "* VACACIONES" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.PrimaVacacional, CuentaAuxiliar = "* PRIMA VACACIONAL" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Aguinaldo, CuentaAuxiliar = "* AGUINALDO" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Ptu, CuentaAuxiliar = "* PTU" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Imss, CuentaAuxiliar = "* IMSS" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Ispt, CuentaAuxiliar = "* ISPT" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Infonavit, CuentaAuxiliar = "* INFONAVIT" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.RetencionImss, CuentaAuxiliar = "* RETENCIÓN IMSS" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.SubsidioAlEmpleo, CuentaAuxiliar = "* SUBSIDIO AL EMPLEO" });
            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.RetencionInfonavit, CuentaAuxiliar = "* RETENCIÓN INFONAVIT" });

            oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasDeMayor.Nomina2Por, CuentaAuxiliar = "* 2 % SOBRE NÓMINA" });

            oDatos.Add(new ContaCuentaAuxiliar() { RelacionID = Cat.ContaAfectacionesCasosFijos.GerenteDeSucursal, CuentaAuxiliar = "= GERENTE DE SUCURSAL" });
            // oDatos.Add(new ContaCuentaAuxiliar() { ContaCuentaDeMayorID = Cat.ContaCuentasAuxiliares., CuentaAuxiliar = "* Clientes" });
            this.oListado = Util.ListaEntityADataTable(oDatos);
        }

        private void cmbOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.cmbOperacion.Focused) return;

            int iAfectacionID = Util.Entero(this.cmbOperacion.SelectedValue);
            this.CargarAfectacion(iAfectacionID);
        }

        private void dgvAfectaciones_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F3)
            {
                var frmSel = new SeleccionListado(this.oListado);
                frmSel.dgvListado.MostrarColumnas("CuentaAuxiliar", "CuentaContpaq", "CuentaSat");
                if (frmSel.ShowDialog(Principal.Instance) == DialogResult.OK)
                {
                    int iCuenta;
                    ConfigAfectaciones.TiposDeCuenta eTipo;
                    if (Util.Entero(frmSel.Seleccion["ContaCuentaAuxiliarID"]) > 0)
                    {
                        eTipo = ConfigAfectaciones.TiposDeCuenta.CuentaAuxiliar;
                        iCuenta = Util.Entero(frmSel.Seleccion["ContaCuentaAuxiliarID"]);
                    }
                    else if (Util.Entero(frmSel.Seleccion["RelacionID"]) > 0)
                    {
                        eTipo = ConfigAfectaciones.TiposDeCuenta.CasoFijo;
                        iCuenta = Util.Entero(frmSel.Seleccion["RelacionID"]);
                    }
                    else
                    {
                        eTipo = ConfigAfectaciones.TiposDeCuenta.CuentaDeMayor;
                        iCuenta = Util.Entero(frmSel.Seleccion["ContaCuentaDeMayorID"]);
                    }
                    this.dgvAfectaciones.CurrentRow.Cells["TipoDeCuenta"].Value = eTipo;
                    this.dgvAfectaciones.CurrentRow.Cells["CuentaID"].Value = iCuenta;
                    this.dgvAfectaciones.CurrentRow.Cells["Cuenta"].Value = frmSel.Seleccion["CuentaAuxiliar"];
                    this.dgvAfectaciones.VerAgregarFilaNueva();
                }
                frmSel.Dispose();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar())
            {
                int iAfectacionID = Util.Entero(this.cmbOperacion.SelectedValue);
                this.CargarAfectacion(iAfectacionID);
            }
        }

        #endregion

        #region [ Métodos ]

        private void CargarAfectacion(int iAfectacionID)
        {
            var oAfectacion = Datos.GetEntity<ContaConfigAfectacion>(c => c.ContaConfigAfectacionID == iAfectacionID);
            if (oAfectacion == null) return;
            this.cmbTipoDePoliza.SelectedValue = oAfectacion.ContaTipoPolizaID;

            var oDetalle = Datos.GetListOf<ContaConfigAfectacionDetalle>(c => c.ContaConfigAfectacionID == iAfectacionID);
            this.dgvAfectaciones.Rows.Clear();
            foreach (var oReg in oDetalle)
            {
                string sCuenta;
                ConfigAfectaciones.TiposDeCuenta eTipo;
                if (oReg.EsCasoFijo)
                {
                    sCuenta = ("= GERENTE DE SUCURSAL");
                    eTipo = ConfigAfectaciones.TiposDeCuenta.CasoFijo;
                }
                else if (oReg.EsCuentaDeMayor)
                {
                    var oCuentaM = Datos.GetEntity<ContaCuentaDeMayor>(c => c.ContaCuentaDeMayorID == oReg.CuentaID);
                    sCuenta = ("* " + oCuentaM.CuentaDeMayor);
                    eTipo = ConfigAfectaciones.TiposDeCuenta.CuentaDeMayor;
                }
                else
                {
                    var oCuentaA = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == oReg.CuentaID);
                    sCuenta = oCuentaA.CuentaAuxiliar;
                    eTipo = ConfigAfectaciones.TiposDeCuenta.CuentaAuxiliar;
                }

                this.dgvAfectaciones.AgregarFila(oReg.ContaConfigAfectacionDetalleID, Cat.TiposDeAfectacion.SinCambios, eTipo
                    , oReg.CuentaID, sCuenta, (oReg.EsCargo ? "Cargo" : "Abono"), oReg.ContaPolizaAsigSucursalID, oReg.Observacion);
            }
        }

        private bool AccionGuardar()
        {
            // Se realiza la validación correspondiente
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            // Se guarda la afectación
            int iAfectacionID = Util.Entero(this.cmbOperacion.SelectedValue);
            var oAfectacion = Datos.GetEntity<ContaConfigAfectacion>(c => c.ContaConfigAfectacionID == iAfectacionID);
            oAfectacion.ContaTipoPolizaID = Util.Entero(this.cmbTipoDePoliza.SelectedValue);
            Datos.Guardar<ContaConfigAfectacion>(oAfectacion);

            // Se procede a guardar el detalle
            ContaConfigAfectacionDetalle oReg;
            foreach (DataGridViewRow oFila in this.dgvAfectaciones.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iId = this.dgvAfectaciones.ObtenerId(oFila);
                int iCambio = this.dgvAfectaciones.ObtenerIdCambio(oFila);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new ContaConfigAfectacionDetalle() { ContaConfigAfectacionID = iAfectacionID };
                        else
                            oReg = Datos.GetEntity<ContaConfigAfectacionDetalle>(c => c.ContaConfigAfectacionDetalleID == iId);

                        ConfigAfectaciones.TiposDeCuenta eTipo = (ConfigAfectaciones.TiposDeCuenta)oFila.Cells["TipoDeCuenta"].Value;
                        int iCuentaID = Util.Entero(oFila.Cells["CuentaID"].Value);
                        oReg.EsCuentaDeMayor = (eTipo == ConfigAfectaciones.TiposDeCuenta.CuentaDeMayor);
                        oReg.EsCasoFijo = (eTipo == ConfigAfectaciones.TiposDeCuenta.CasoFijo);
                        oReg.CuentaID = (iCuentaID);
                        oReg.EsCargo = (Util.Cadena(oFila.Cells["CargoAbono"].Value) == "Cargo");
                        oReg.ContaPolizaAsigSucursalID = Util.Entero(oFila.Cells["AsigSucursalID"].Value);
                        oReg.Observacion = Util.Cadena(oFila.Cells["Observacion"].Value);
                        
                        Datos.Guardar<ContaConfigAfectacionDetalle>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = Datos.GetEntity<ContaConfigAfectacionDetalle>(c => c.ContaConfigAfectacionDetalleID == iId);
                        Datos.Eliminar<ContaConfigAfectacionDetalle>(oReg);
                        break;
                }
            }

            Cargando.Cerrar();
            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (Util.Entero(this.cmbOperacion.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbOperacion, "No se ha seleccionado ninguna operación.");
            if (Util.Entero(this.cmbTipoDePoliza.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbTipoDePoliza, "No se ha seleccionado un tipo de poliza válido.");
            // Se validan las filas del grid
            bool bErrorGrid = false;
            foreach (DataGridViewRow oFila in this.dgvAfectaciones.Rows)
            {
                if (oFila.IsNewRow) continue;
                oFila.ErrorText = "";
                if (Util.Entero(oFila.Cells["AsigSucursalID"].Value) <= 0)
                {
                    oFila.ErrorText = "Sucursal inválida.";
                    bErrorGrid = true;
                    break;
                }
            }
            if (bErrorGrid)
                this.ctlError.PonerError(this.btnGuardar, "Existen errores de validación. Verificar.", ErrorIconAlignment.MiddleLeft);

            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
