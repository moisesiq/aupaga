using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class DetalleResolverContingencia : DetalleBase
    {
        MovimientoInventarioTraspasoContingencia Contingencia;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleResolverContingencia Instance
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

            internal static readonly DetalleResolverContingencia instance = new DetalleResolverContingencia();
        }

        public DetalleResolverContingencia()
        {
            InitializeComponent();
        }

        public DetalleResolverContingencia(int Id)
        {
            InitializeComponent();
            try
            {
                Contingencia = Negocio.General.GetEntityById<MovimientoInventarioTraspasoContingencia>("MovimientoInventarioTraspasoContingencia", "MovimientoInventarioTraspasoContingenciaID", Id);
                if (Contingencia == null)
                    throw new EntityNotFoundException(Id.ToString(), "MovimientoInventarioTraspasoContingencia");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleResolverContingencia_Load(object sender, EventArgs e)
        {
            this.CargaInicial();
            if (EsNuevo)
            {

            }
            else
            {
                if (Contingencia.MovimientoInventarioTraspasoContingenciaID > 0)
                {
                    this.Text = "Resolver";
                }
            }
        }

        private void cboTipoOperacion_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(this.cboTipoOperacion.SelectedValue.ToString(), out id))
            {
                this.CargarConceptos(id);
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validaciones())
                    return;

                if (EsNuevo)
                {

                }
                else
                {
                    int iAutorizoID = 0;
                    var ResU = UtilDatos.ValidarObtenerUsuario(null, "Autorización");
                    if (ResU.Exito)
                        iAutorizoID = ResU.Respuesta.UsuarioID;
                    else
                    {
                        Helper.MensajeError("Error al validar el usuario.", GlobalClass.NombreApp);
                        return;
                    }

                    SplashScreen.Show(new Splash());
                    this.btnGuardar.Enabled = false;

                    var ContingenciaCompleta = General.GetEntity<MovimientoInventarioContingenciasView>(c => c.MovimientoInventarioTraspasoContingenciaID == Contingencia.MovimientoInventarioTraspasoContingenciaID);
                    if (null == ContingenciaCompleta)
                        return;

                    //Actualiza la contingencia
                    Contingencia.FechaModificacion = DateTime.Now;
                    Contingencia.MovimientoInventarioEstatusContingenciaID = 1;
                    Contingencia.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Contingencia.UsuarioSolucionoID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Contingencia.FechaSoluciono = DateTime.Now;
                    Contingencia.TipoOperacionID = Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue);
                    Contingencia.TipoConceptoOperacionID = Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue);
                    Contingencia.ObservacionSolucion = this.txtObservacion.Text;
                    Contingencia.FechaModificacion = DateTime.Now;
                    General.SaveOrUpdate<MovimientoInventarioTraspasoContingencia>(Contingencia, Contingencia);

                    switch (Helper.ConvertirEntero(cboTipoOperacion.SelectedValue))
                    {
                        case 2:

                            #region [ Entrada Inventario ]

                            //Insertar Movimiento
                            var movimientoEntradaI = new MovimientoInventario()
                            {
                                TipoOperacionID = Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue),
                                TipoPagoID = 1,
                                ProveedorID = 1,
                                SucursalOrigenID = ContingenciaCompleta.SucursalOrigenID,
                                SucursalDestinoID = ContingenciaCompleta.SucursalDestinoID,
                                FechaFactura = DateTime.Now,
                                FechaRecepcion = DateTime.Now,
                                FolioFactura = null,

                                AplicaEnMovimientoInventarioID = null,
                                FechaAplicacion = null,

                                Subtotal = null,
                                IVA = null,
                                ImporteTotal = 0,
                                FueLiquidado = false,
                                TipoConceptoOperacionID = ContingenciaCompleta.TipoConceptoOperacionID,
                                Observacion = string.Format("{0}: {1}", "Traspaso por resolucion de un Conflicto. Movimiento: ", Contingencia.MovimientoInventarioID),
                                Articulos = null,
                                Unidades = null,
                                Seguro = null,
                                ImporteTotalSinDescuento = null
                            };
                            Guardar.Generico<MovimientoInventario>(movimientoEntradaI);

                            if (movimientoEntradaI.MovimientoInventarioID < 1)
                            {
                                new EntityNotFoundException("MovimientoInventarioID", "MovimientoInventario");
                                return;
                            }

                            //Insertar MovimientoDetalle
                            var detalleMovimiento = new MovimientoInventarioDetalle()
                            {
                                MovimientoInventarioID = movimientoEntradaI.MovimientoInventarioID,
                                ParteID = ContingenciaCompleta.ParteID,
                                Cantidad = Helper.ConvertirDecimal(ContingenciaCompleta.Diferencia),
                                PrecioUnitario = 0,
                                Importe = 0,
                                FueDevolucion = false,
                                FechaRegistro = DateTime.Now,
                                Estatus = true
                            };
                            General.SaveOrUpdate<MovimientoInventarioDetalle>(detalleMovimiento, detalleMovimiento);

                            //Actualizar ParteExistencia
                            var sucursalId = ContingenciaCompleta.SucursalOrigenID;
                            var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID == ContingenciaCompleta.ParteID && p.SucursalID == sucursalId);
                            if (existencia != null)
                            {
                                existencia.Existencia += Helper.ConvertirDecimal(ContingenciaCompleta.Diferencia);
                                Guardar.Generico<ParteExistencia>(existencia);
                            }

                            #endregion

                            break;

                        case 3:

                            #region [ Salida Inventario ]

                            //Insertar Movimiento
                            var movimientoSalida = new MovimientoInventario()
                            {
                                TipoOperacionID = Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue),
                                TipoPagoID = 1,
                                ProveedorID = 1,
                                SucursalOrigenID = ContingenciaCompleta.SucursalOrigenID,
                                SucursalDestinoID = ContingenciaCompleta.SucursalDestinoID,
                                FechaFactura = DateTime.Now,
                                FechaRecepcion = DateTime.Now,
                                FolioFactura = null,

                                AplicaEnMovimientoInventarioID = null,
                                FechaAplicacion = null,

                                Subtotal = null,
                                IVA = null,
                                ImporteTotal = 0,
                                FueLiquidado = false,
                                TipoConceptoOperacionID = Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue),
                                Observacion = string.Format("{0}: {1}", "Traspaso por resolucion de un Conflicto. Movimiento: ", Contingencia.MovimientoInventarioID),
                                Articulos = null,
                                Unidades = null,
                                Seguro = null,
                                ImporteTotalSinDescuento = null
                            };
                            Guardar.Generico<MovimientoInventario>(movimientoSalida);

                            if (movimientoSalida.MovimientoInventarioID < 1)
                            {
                                new EntityNotFoundException("MovimientoInventarioID", "MovimientoInventario");
                                return;
                            }

                            //Insertar MovimientoDetalle
                            var detalleMovimientoS = new MovimientoInventarioDetalle()
                                {
                                    MovimientoInventarioID = movimientoSalida.MovimientoInventarioID,
                                    ParteID = ContingenciaCompleta.ParteID,
                                    Cantidad = Helper.ConvertirDecimal(ContingenciaCompleta.Diferencia),
                                    PrecioUnitario = 0,
                                    Importe = 0,
                                    FueDevolucion = false,
                                    FechaRegistro = DateTime.Now,
                                    Estatus = true
                                };
                            General.SaveOrUpdate<MovimientoInventarioDetalle>(detalleMovimientoS, detalleMovimientoS);

                            //No se descuenta por que ya se habia descontado en el traspaso anterior
                            
                            #endregion

                            break;

                        case 5:

                            #region [ Traspaso ]

                            //Almacenar traspaso
                            var traspaso = new MovimientoInventario()
                            {
                                TipoOperacionID = Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue),
                                SucursalOrigenID = ContingenciaCompleta.SucursalOrigenID,
                                SucursalDestinoID = ContingenciaCompleta.SucursalDestinoID,
                                ImporteTotal = 0,
                                FueLiquidado = false,
                                UsuarioSolicitoTraspasoID = Contingencia.UsuarioSolucionoID,
                                ExisteContingencia = false,
                                Observacion = string.Format("{0}: {1}", "Traspaso por resolucion de un Conflicto. Movimiento: ", Contingencia.MovimientoInventarioID)
                            };
                            Guardar.Generico<MovimientoInventario>(traspaso);

                            //Almacenar el detalle del traspaso
                            if (traspaso.MovimientoInventarioID > 0)
                            {

                                var detalleTraspaso = new MovimientoInventarioDetalle()
                                {
                                    MovimientoInventarioID = traspaso.MovimientoInventarioID,
                                    ParteID = ContingenciaCompleta.ParteID,
                                    Cantidad = Helper.ConvertirDecimal(ContingenciaCompleta.Diferencia),
                                    PrecioUnitario = 0,
                                    Importe = 0,
                                    FueDevolucion = false,
                                    FechaRegistro = DateTime.Now,
                                    Estatus = true
                                };
                                General.SaveOrUpdate<MovimientoInventarioDetalle>(detalleTraspaso, detalleTraspaso);

                                //No se descuenta por que ya se habia descontado en el primer traspaso
                            }

                            #endregion

                            break;

                        default:
                            break;
                    }

                }
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                new Notificacion("Conflicto Resuelto exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                catalogosTraspasos.Instance.CustomInvoke<catalogosTraspasos>(m => m.ActualizarListadoConflictos());
            }
            catch (Exception ex)
            {

                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Metodos ]

        private void CargaInicial()
        {
            // Se validan los permisos
            //if (this.EsNuevo)
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Agregar", true))
            //    {
            //        this.Close();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Modificar", false))
            //        this.btnGuardar.Enabled = false;
            //}

            try
            {
                this.cboTipoOperacion.DataSource = Negocio.General.GetListOf<TipoOperacionesView>(s => s.TipoOperacionID == 2 || s.TipoOperacionID == 3 || s.TipoOperacionID == 5);
                this.cboTipoOperacion.DisplayMember = "NombreTipoOperacion";
                this.cboTipoOperacion.ValueMember = "TipoOperacionID";
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarConceptos(int id)
        {
            try
            {
                this.cboConceptoOperacion.DataSource = Negocio.General.GetListOf<TipoConceptoOperacion>(t => t.TipoOperacionID == id
                    && t.Estatus && t.NombreConceptoOperacion != "OBSEQUIO");
                this.cboConceptoOperacion.DisplayMember = "NombreConceptoOperacion";
                this.cboConceptoOperacion.ValueMember = "TipoConceptoOperacionID";
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool Validaciones()
        {
            this.cntError.LimpiarErrores();
            if (this.txtObservacion.Text == "")
                this.cntError.PonerError(this.txtObservacion, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
