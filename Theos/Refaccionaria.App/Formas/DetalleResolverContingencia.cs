using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

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
                //Contingencia = General.GetEntityById<MovimientoInventarioTraspasoContingencia>("MovimientoInventarioTraspasoContingencia", "MovimientoInventarioTraspasoContingenciaID", Id);
                Contingencia = Datos.GetEntity<MovimientoInventarioTraspasoContingencia>(c => c.MovimientoInventarioTraspasoContingenciaID == Id && c.Estatus);
                if (Contingencia == null)
                    throw new EntityNotFoundException(Id.ToString(), "MovimientoInventarioTraspasoContingencia");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                    var ResU = UtilLocal.ValidarObtenerUsuario(null, "Autorización");
                    if (ResU.Exito)
                        iAutorizoID = ResU.Respuesta.UsuarioID;
                    else
                    {
                        Util.MensajeError("Error al validar el usuario.", GlobalClass.NombreApp);
                        return;
                    }

                    SplashScreen.Show(new Splash());
                    this.btnGuardar.Enabled = false;

                    var ContingenciaCompleta = Datos.GetEntity<MovimientoInventarioContingenciasView>(c => c.MovimientoInventarioTraspasoContingenciaID == Contingencia.MovimientoInventarioTraspasoContingenciaID);
                    if (null == ContingenciaCompleta)
                        return;

                    //Actualiza la contingencia
                    Contingencia.FechaModificacion = DateTime.Now;
                    Contingencia.MovimientoInventarioEstatusContingenciaID = 1;
                    Contingencia.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Contingencia.UsuarioSolucionoID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Contingencia.FechaSoluciono = DateTime.Now;
                    Contingencia.TipoOperacionID = Util.Entero(this.cboTipoOperacion.SelectedValue);
                    Contingencia.TipoConceptoOperacionID = Util.Entero(this.cboConceptoOperacion.SelectedValue);
                    Contingencia.ObservacionSolucion = this.txtObservacion.Text;
                    if (Util.Entero(this.cboTipoOperacion.SelectedValue) == Cat.TiposDeOperacionMovimientos.SalidaInventario)
                        Contingencia.ObservacionSolucion += (" - SE CREA UNA ENTRADA PARA DEVOLVER EL TRASPASO A LA TIENDA ORIGEN Y EN EL SEGUNDO MOVIMIENTO UNA SALIDA PARA PODER HACER LA SALIDA DEL INVENTARIO");
                    Contingencia.FechaModificacion = DateTime.Now;
                    Datos.SaveOrUpdate<MovimientoInventarioTraspasoContingencia>(Contingencia);

                    int iSucursalID = ContingenciaCompleta.SucursalOrigenID.Valor();
                    string sOrigen = UtilDatos.NombreDeSucursal(ContingenciaCompleta.SucursalOrigenID.Valor());
                    string sDestino = UtilDatos.NombreDeSucursal(ContingenciaCompleta.SucursalDestinoID.Valor());
                    var oPrecio = Datos.GetEntity<PartePrecio>(c => c.ParteID == ContingenciaCompleta.ParteID && c.Estatus);

                    switch (Util.Entero(cboTipoOperacion.SelectedValue))
                    {
                        case 2:

                            #region [ Entrada Inventario ]

                            //Insertar Movimiento
                            var movimientoEntradaI = new MovimientoInventario()
                            {
                                TipoOperacionID = Util.Entero(this.cboTipoOperacion.SelectedValue),
                                TipoPagoID = 1,
                                ProveedorID = 1,
                                SucursalOrigenID = ContingenciaCompleta.SucursalOrigenID,
                                SucursalDestinoID = ContingenciaCompleta.SucursalDestinoID,
                                FechaFactura = DateTime.Now,
                                FechaRecepcion = DateTime.Now,
                                FolioFactura = null,
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
                            Datos.Guardar<MovimientoInventario>(movimientoEntradaI);

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
                                Cantidad = Util.Decimal(ContingenciaCompleta.Diferencia),
                                PrecioUnitario = 0,
                                Importe = 0,
                                FueDevolucion = false,
                                FechaRegistro = DateTime.Now,
                                Estatus = true
                            };
                            Datos.SaveOrUpdate<MovimientoInventarioDetalle>(detalleMovimiento);

                            //Actualizar ParteExistencia
                            /* var sucursalId = ContingenciaCompleta.SucursalOrigenID;
                            var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID == ContingenciaCompleta.ParteID && p.SucursalID == sucursalId);
                            if (existencia != null)
                            {
                                existencia.Existencia += Util.ConvertirDecimal(ContingenciaCompleta.Diferencia);
                                Datos.Guardar<ParteExistencia>(existencia);//dmod
                            }
                            */
                            // AdmonProc.AgregarExistencia(ContingenciaCompleta.ParteID, sucursalId.Valor(), ContingenciaCompleta.Diferencia.Valor()
                            //     , Cat.Tablas.MovimientoInventario, movimientoEntradaI.MovimientoInventarioID);
                            
                            // Se modifica la existencia y el kardex
                            AdmonProc.AfectarExistenciaYKardex(ContingenciaCompleta.ParteID, iSucursalID, Cat.OperacionesKardex.EntradaTraspaso
                                , Contingencia.MovimientoInventarioID.ToString(), Contingencia.UsuarioSolucionoID.Valor(), "Conflicto Resuelto Entrada"
                                , sOrigen, sDestino, ContingenciaCompleta.Diferencia.Valor()
                                , oPrecio.Costo.Valor(), Cat.Tablas.MovimientoInventario, movimientoEntradaI.MovimientoInventarioID);
                            
                            #endregion

                            break;

                        case 3:

                            #region [ Salida Inventario ]

                            //Insertar Movimiento
                            var movimientoSalida = new MovimientoInventario()
                            {
                                TipoOperacionID = Util.Entero(this.cboTipoOperacion.SelectedValue),
                                TipoPagoID = 1,
                                ProveedorID = 1,
                                SucursalOrigenID = ContingenciaCompleta.SucursalOrigenID,
                                SucursalDestinoID = ContingenciaCompleta.SucursalDestinoID,
                                FechaFactura = DateTime.Now,
                                FechaRecepcion = DateTime.Now,
                                FolioFactura = null,
                                Subtotal = null,
                                IVA = null,
                                ImporteTotal = 0,
                                FueLiquidado = false,
                                TipoConceptoOperacionID = Util.Entero(this.cboConceptoOperacion.SelectedValue),
                                Observacion = string.Format("{0}: {1}", "Traspaso por resolucion de un Conflicto. Movimiento: ", Contingencia.MovimientoInventarioID),
                                Articulos = null,
                                Unidades = null,
                                Seguro = null,
                                ImporteTotalSinDescuento = null
                            };
                            Datos.Guardar<MovimientoInventario>(movimientoSalida);

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
                                    Cantidad = Util.Decimal(ContingenciaCompleta.Diferencia),
                                    PrecioUnitario = 0,
                                    Importe = 0,
                                    FueDevolucion = false,
                                    FechaRegistro = DateTime.Now,
                                    Estatus = true
                                };
                            Datos.SaveOrUpdate<MovimientoInventarioDetalle>(detalleMovimientoS);

                            //No se descuenta por que ya se habia descontado en el traspaso anterior
                            // Ahora sí se generan dos registros en kardex, uno de entrada y luego uno de salida, para que al final quede igual
                            // Se hace así para mantener un registro de la operación
                            // Se hace primero la entrada
                            AdmonProc.AfectarExistenciaYKardex(ContingenciaCompleta.ParteID, iSucursalID, Cat.OperacionesKardex.EntradaTraspaso
                                , Contingencia.MovimientoInventarioID.ToString(), Contingencia.UsuarioSolucionoID.Valor(), "Conflicto Resuelto Salida"
                                , sOrigen, sDestino, ContingenciaCompleta.Diferencia.Valor()
                                , oPrecio.Costo.Valor(), Cat.Tablas.MovimientoInventario, movimientoSalida.MovimientoInventarioID);
                            // Luego la salida
                            AdmonProc.AfectarExistenciaYKardex(ContingenciaCompleta.ParteID, iSucursalID, Cat.OperacionesKardex.SalidaTraspaso
                                , Contingencia.MovimientoInventarioID.ToString(), Contingencia.UsuarioSolucionoID.Valor(), "Conflicto Resuelto Salida"
                                , sOrigen, sDestino, (ContingenciaCompleta.Diferencia.Valor() * -1)
                                , oPrecio.Costo.Valor(), Cat.Tablas.MovimientoInventario, movimientoSalida.MovimientoInventarioID);
                            
                            #endregion

                            break;

                        case 5:

                            #region [ Traspaso ]

                            //Almacenar traspaso
                            var traspaso = new MovimientoInventario()
                            {
                                TipoOperacionID = Util.Entero(this.cboTipoOperacion.SelectedValue),
                                SucursalOrigenID = ContingenciaCompleta.SucursalOrigenID,
                                SucursalDestinoID = ContingenciaCompleta.SucursalDestinoID,
                                ImporteTotal = 0,
                                FueLiquidado = false,
                                UsuarioSolicitoTraspasoID = Contingencia.UsuarioSolucionoID,
                                ExisteContingencia = false,
                                Observacion = string.Format("{0}: {1}", "Traspaso por resolucion de un Conflicto. Movimiento: ", Contingencia.MovimientoInventarioID)
                            };
                            Datos.Guardar<MovimientoInventario>(traspaso);

                            //Almacenar el detalle del traspaso
                            if (traspaso.MovimientoInventarioID > 0)
                            {

                                var detalleTraspaso = new MovimientoInventarioDetalle()
                                {
                                    MovimientoInventarioID = traspaso.MovimientoInventarioID,
                                    ParteID = ContingenciaCompleta.ParteID,
                                    Cantidad = Util.Decimal(ContingenciaCompleta.Diferencia),
                                    PrecioUnitario = 0,
                                    Importe = 0,
                                    FueDevolucion = false,
                                    FechaRegistro = DateTime.Now,
                                    Estatus = true
                                };
                                Datos.SaveOrUpdate<MovimientoInventarioDetalle>(detalleTraspaso);

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
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                this.cboTipoOperacion.DataSource = Datos.GetListOf<TipoOperacionesView>(s => s.TipoOperacionID == 2 || s.TipoOperacionID == 3 || s.TipoOperacionID == 5);
                this.cboTipoOperacion.DisplayMember = "NombreTipoOperacion";
                this.cboTipoOperacion.ValueMember = "TipoOperacionID";
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarConceptos(int id)
        {
            try
            {
                this.cboConceptoOperacion.DataSource = Datos.GetListOf<TipoConceptoOperacion>(t => t.TipoOperacionID == id
                    && t.Estatus && t.NombreConceptoOperacion != "OBSEQUIO");
                this.cboConceptoOperacion.DisplayMember = "NombreConceptoOperacion";
                this.cboConceptoOperacion.ValueMember = "TipoConceptoOperacionID";
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
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
