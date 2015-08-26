using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ReportarParteFaltante : UserControl
    {
        Parte oParte;

        public ReportarParteFaltante(int iParteID)
        {
            InitializeComponent();
            this.oParte = General.GetEntity<Parte>(q => q.ParteID == iParteID);
        }

        private void ReporteDeFaltante_Load(object sender, System.EventArgs e)
        {
            this.txtNumeroDeParte.Text = this.oParte.NumeroParte;
            this.txtDescripcion.Text = this.oParte.NombreParte;
        }

        private void btnGuardar_Click(object sender, System.EventArgs e)
        {
            // Se solicita contraseña del Usuario
            var Res = UtilDatos.ValidarObtenerUsuario("Ventas.ReporteDeFaltante.Agregar");
            if (Res.Error)
                return;

            ReporteDeFaltante Faltante = new ReporteDeFaltante()
            {
                SucursalID = GlobalClass.SucursalID
                , RealizoUsuarioID = Res.Respuesta.UsuarioID
                , ParteID = this.oParte.ParteID
                , CantidadRequerida = (int)this.nudCantidad.Value
                , Comentario = this.txtComentario.Text
            };

            Guardar.Generico<ReporteDeFaltante>(Faltante);

            UtilLocal.MostrarNotificacion("Reporte guardado correctamente.");

            this.Dispose();
        }

        private void btnCerrar_Click(object sender, System.EventArgs e)
        {
            this.Dispose();
        }


    }
}
