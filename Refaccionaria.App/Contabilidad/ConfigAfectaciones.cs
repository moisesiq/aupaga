using System;
using System.Windows.Forms;
using System.Data;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

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
            this.cmbOperacion.CargarDatos("ContaConfigAfectacionID", "Operacion", General.GetListOf<ContaConfigAfectacion>().OrderBy(c => c.Operacion).ToList());
            this.cmbTipoDePoliza.CargarDatos("ContaTipoPolizaID", "TipoDePoliza", General.GetListOf<ContaTipoPoliza>());
            // this.CuentaAuxiliarID.CargarDatos("ContaCuentaAuxiliarID", "CuentaAuxiliar", General.GetListOf<ContaCuentaAuxiliar>());
            this.CargoAbono.Items.AddRange("Cargo", "Abono");
            this.AsigSucursalID.CargarDatos("ContaPolizaAsigSucursalID", "Sucursal", General.GetListOf<ContaPolizaAsigSucursal>());
            this.dgvAfectaciones.Inicializar();

            // Se cargan los datos para el grid de búsqueda
            var oDatos = General.GetListOf<ContaCuentaAuxiliar>();
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
            this.oListado = Helper.ListaEntityADataTable(oDatos);
        }

        private void cmbOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.cmbOperacion.Focused) return;

            int iAfectacionID = Helper.ConvertirEntero(this.cmbOperacion.SelectedValue);
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
                    if (Helper.ConvertirEntero(frmSel.Seleccion["ContaCuentaAuxiliarID"]) > 0)
                    {
                        eTipo = ConfigAfectaciones.TiposDeCuenta.CuentaAuxiliar;
                        iCuenta = Helper.ConvertirEntero(frmSel.Seleccion["ContaCuentaAuxiliarID"]);
                    }
                    else if (Helper.ConvertirEntero(frmSel.Seleccion["RelacionID"]) > 0)
                    {
                        eTipo = ConfigAfectaciones.TiposDeCuenta.CasoFijo;
                        iCuenta = Helper.ConvertirEntero(frmSel.Seleccion["RelacionID"]);
                    }
                    else
                    {
                        eTipo = ConfigAfectaciones.TiposDeCuenta.CuentaDeMayor;
                        iCuenta = Helper.ConvertirEntero(frmSel.Seleccion["ContaCuentaDeMayorID"]);
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
                int iAfectacionID = Helper.ConvertirEntero(this.cmbOperacion.SelectedValue);
                this.CargarAfectacion(iAfectacionID);
            }
        }

        #endregion

        #region [ Métodos ]

        private void CargarAfectacion(int iAfectacionID)
        {
            var oAfectacion = General.GetEntity<ContaConfigAfectacion>(c => c.ContaConfigAfectacionID == iAfectacionID);
            if (oAfectacion == null) return;
            this.cmbTipoDePoliza.SelectedValue = oAfectacion.ContaTipoPolizaID;

            var oDetalle = General.GetListOf<ContaConfigAfectacionDetalle>(c => c.ContaConfigAfectacionID == iAfectacionID);
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
                    var oCuentaM = General.GetEntity<ContaCuentaDeMayor>(c => c.ContaCuentaDeMayorID == oReg.CuentaID);
                    sCuenta = ("* " + oCuentaM.CuentaDeMayor);
                    eTipo = ConfigAfectaciones.TiposDeCuenta.CuentaDeMayor;
                }
                else
                {
                    var oCuentaA = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == oReg.CuentaID);
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
            int iAfectacionID = Helper.ConvertirEntero(this.cmbOperacion.SelectedValue);
            var oAfectacion = General.GetEntity<ContaConfigAfectacion>(c => c.ContaConfigAfectacionID == iAfectacionID);
            oAfectacion.ContaTipoPolizaID = Helper.ConvertirEntero(this.cmbTipoDePoliza.SelectedValue);
            Guardar.Generico<ContaConfigAfectacion>(oAfectacion);

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
                            oReg = General.GetEntity<ContaConfigAfectacionDetalle>(c => c.ContaConfigAfectacionDetalleID == iId);

                        ConfigAfectaciones.TiposDeCuenta eTipo = (ConfigAfectaciones.TiposDeCuenta)oFila.Cells["TipoDeCuenta"].Value;
                        int iCuentaID = Helper.ConvertirEntero(oFila.Cells["CuentaID"].Value);
                        oReg.EsCuentaDeMayor = (eTipo == ConfigAfectaciones.TiposDeCuenta.CuentaDeMayor);
                        oReg.EsCasoFijo = (eTipo == ConfigAfectaciones.TiposDeCuenta.CasoFijo);
                        oReg.CuentaID = (iCuentaID);
                        oReg.EsCargo = (Helper.ConvertirCadena(oFila.Cells["CargoAbono"].Value) == "Cargo");
                        oReg.ContaPolizaAsigSucursalID = Helper.ConvertirEntero(oFila.Cells["AsigSucursalID"].Value);
                        oReg.Observacion = Helper.ConvertirCadena(oFila.Cells["Observacion"].Value);
                        
                        Guardar.Generico<ContaConfigAfectacionDetalle>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<ContaConfigAfectacionDetalle>(c => c.ContaConfigAfectacionDetalleID == iId);
                        Guardar.Eliminar<ContaConfigAfectacionDetalle>(oReg);
                        break;
                }
            }

            Cargando.Cerrar();
            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (Helper.ConvertirEntero(this.cmbOperacion.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbOperacion, "No se ha seleccionado ninguna operación.");
            if (Helper.ConvertirEntero(this.cmbTipoDePoliza.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbTipoDePoliza, "No se ha seleccionado un tipo de poliza válido.");
            // Se validan las filas del grid
            bool bErrorGrid = false;
            foreach (DataGridViewRow oFila in this.dgvAfectaciones.Rows)
            {
                if (oFila.IsNewRow) continue;
                oFila.ErrorText = "";
                if (Helper.ConvertirEntero(oFila.Cells["AsigSucursalID"].Value) <= 0)
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
