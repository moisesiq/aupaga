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
    public partial class DetalleProveedorProntoPago : DetalleBase
    {
        public int ProveedorId;
        ProveedorProntoPago ProveedorProntoPago;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleProveedorProntoPago Instance
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

            internal static readonly DetalleProveedorProntoPago instance = new DetalleProveedorProntoPago();
        }

        public DetalleProveedorProntoPago()
        {
            InitializeComponent();
        }

        public DetalleProveedorProntoPago(int Id)
        {
            InitializeComponent();
            try
            {
                ProveedorProntoPago = Negocio.General.GetEntityById<ProveedorProntoPago>("ProveedorProntoPago", "ProveedorProntoPagoID", Id);
                if (ProveedorProntoPago == null)
                    throw new EntityNotFoundException(Id.ToString(), "ProveedorProntoPago");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleProveedorProntoPago_Load(object sender, EventArgs e)
        {
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Negocio.Helper.ClearTextBoxes(this);
                this.nudNumeroDias.Focus();
            }
            else
            {
                if (ProveedorProntoPago.ProveedorProntoPagoID > 0)
                {
                    this.Text = "Modificar";

                    var pp = Negocio.General.GetEntity<ProveedorProntoPago>(p => p.ProveedorProntoPagoID.Equals(ProveedorProntoPago.ProveedorProntoPagoID));
                    if (pp != null)
                    {
                        this.nudNumeroDias.Value = pp.NumeroDias;
                        this.txtPorcentaje.Text = Negocio.Helper.ConvertirCadena(pp.PorcentajeDescuento);
                        this.txtComentario.Text = pp.Comentario;
                    }
                }
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            try
            {
                if (EsNuevo)
                {
                    var prontoPago = new ProveedorProntoPago() 
                    {
                        ProveedorID = ProveedorId,
                        NumeroDias = Negocio.Helper.ConvertirEntero(this.nudNumeroDias.Value),
                        PorcentajeDescuento = Negocio.Helper.ConvertirDecimal(this.txtPorcentaje.Text),
                        Comentario = this.txtComentario.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Negocio.General.SaveOrUpdate<ProveedorProntoPago>(prontoPago, prontoPago);
                }
                else
                {            
                    ProveedorProntoPago.NumeroDias = Negocio.Helper.ConvertirEntero(this.nudNumeroDias.Value);
                    ProveedorProntoPago.PorcentajeDescuento = Negocio.Helper.ConvertirDecimal(this.txtPorcentaje.Text);
                    ProveedorProntoPago.Comentario = this.txtComentario.Text;
                    ProveedorProntoPago.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    ProveedorProntoPago.FechaModificacion = DateTime.Now;
                    Negocio.General.SaveOrUpdate<ProveedorProntoPago>(ProveedorProntoPago, ProveedorProntoPago);
                }
                new Notificacion("Pronto Pago guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                catalogosProveedores.Instance.CustomInvoke<catalogosProveedores>(m => m.CargarProntoPago(ProveedorId));
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleProveedorProntoPago_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Metodos ]

        private bool Validaciones()
        {
            try
            {
                var numeroDias = Negocio.Helper.ConvertirEntero(this.nudNumeroDias.Value);
                var item = Negocio.General.GetEntity<ProveedorProntoPago>(p => p.ProveedorID.Equals(ProveedorId) && p.NumeroDias.Equals(numeroDias));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Negocio.Helper.MensajeError("Ya existe un registro con ese número de días, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.ProveedorProntoPagoID != ProveedorProntoPago.ProveedorProntoPagoID)
                {
                    Negocio.Helper.MensajeError("Ya existe un registro con ese número de días, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }

            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.nudNumeroDias.Value == 0)
                this.cntError.PonerError(this.nudNumeroDias, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            if (this.txtPorcentaje.Text == "")
                this.cntError.PonerError(this.txtPorcentaje, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion
                
    }
}
