using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class ReportarErrorParte : Form
    {
        ControlError ctlError = new ControlError();
        int ParteID;

        public ReportarErrorParte(int iParteID)
        {
            InitializeComponent();

            this.ParteID = iParteID;
        }

        #region [ Eventos ]

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            // Se solicita la validación de usuario
            var oResU = UtilLocal.ValidarObtenerUsuario();
            if (oResU.Error)
                return false;

            Cargando.Mostrar();

            // Se guarda el error
            var oError = new ParteError()
            {
                ParteID = this.ParteID,
                Fecha = DateTime.Now,
                Foto = this.chkFoto.Checked,
                Equivalente = this.chkEquivalente.Checked,
                Aplicacion = this.chkAplicacion.Checked,
                Alterno = this.chkAlterno.Checked,
                Complemento = this.chkComplemento.Checked,
                Otro = this.chkOtro.Checked,
                ComentarioError = this.txtComentario.Text,
                ErrorUsuarioID = oResU.Respuesta.UsuarioID
            };
            Datos.Guardar<ParteError>(oError);

            Cargando.Cerrar();

            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();

            if (!(this.chkFoto.Checked || this.chkEquivalente.Checked || this.chkAplicacion.Checked || this.chkAlterno.Checked || this.chkComplemento.Checked
                || this.chkOtro.Checked))
                this.ctlError.PonerError(this.btnAceptar, "Debes seleccionar al menos un error.", ErrorIconAlignment.MiddleLeft);
            if (this.chkOtro.Checked && this.txtComentario.Text == "")
                this.ctlError.PonerError(this.chkOtro, "Debes poner un comentario sobre el error.");

            return this.ctlError.Valido;
        }

        #endregion

    }
}
