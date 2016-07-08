using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CambiosSistemaDetalle : DetalleBase
    {
        private CambioSistema oCambio;
        private bool EsMod = false;
        private ControlError ctlError = new ControlError();

        public CambiosSistemaDetalle()
        {
            InitializeComponent();
            this.oCambio = new CambioSistema();
        }

        public CambiosSistemaDetalle(int iCambioSistemaID)
        {
            this.InitializeComponent();
            this.oCambio = Datos.GetEntity<CambioSistema>(q => q.CambioSistemaID == iCambioSistemaID);
            this.EsMod = true;
        }

        #region [ Eventos ]

        private void CambiosSistemaDetalle_Load(object sender, EventArgs e)
        {
            if (this.EsMod)
            {
                this.dtpFecha.Value = this.oCambio.Fecha;
                this.txtVersion.Text = this.oCambio.Version;
                this.txtCategoria.Text = this.oCambio.Categoria;
                this.txtModificacion.Text = this.oCambio.Modificacion;
                this.txtObservaciones.Text = this.oCambio.Observaciones;
            }
            else
            {
                this.txtVersion.Text = Application.ProductVersion.Izquierda(Application.ProductVersion.Length - 2);
            }

            this.ActiveControl = this.txtVersion;
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!this.Validar())
                return;

            this.oCambio.Fecha = this.dtpFecha.Value;
            this.oCambio.Version = this.txtVersion.Text;
            this.oCambio.Categoria = this.txtCategoria.Text;
            this.oCambio.Modificacion = this.txtModificacion.Text;
            this.oCambio.Observaciones = this.txtObservaciones.Text;
            
            Datos.Guardar<CambioSistema>(this.oCambio);

            UtilLocal.MostrarNotificacion("Cambio guardado correctamente.");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
                
        #endregion

        #region [ Métodos ]

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();

            if (this.txtVersion.Text == "")
                this.ctlError.PonerError(this.txtVersion, "Debes especificar la versión.", ErrorIconAlignment.MiddleLeft);
            if (this.txtCategoria.Text == "")
                this.ctlError.PonerError(this.txtCategoria, "Debes especificar una categoria.", ErrorIconAlignment.MiddleLeft);
            if (this.txtModificacion.Text == "")
                this.ctlError.PonerError(this.txtModificacion, "Debes especificar una modificación.", ErrorIconAlignment.MiddleLeft);

            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion
    }
}
