using System.Windows.Forms;
using System.ComponentModel;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class AplicarPartesAVehiculo : Form
    {
        Vehiculo oVehiculo;
        BindingList<ProductoVenta> Partes;

        public AplicarPartesAVehiculo(Vehiculo oVehiculo, BindingList<ProductoVenta> Partes)
        {
            InitializeComponent();

            this.oVehiculo = oVehiculo;
            this.Partes = Partes;
        }

        private void AplicarPartesAVehiculo_Load(object sender, System.EventArgs e)
        {
            this.lblVehiculo.Text =
                oVehiculo.Marca
                + " - " + oVehiculo.Modelo
                + (oVehiculo.Anio > 0 ? " " + oVehiculo.Anio.ToString() : "")
                + (oVehiculo.MotorID > 0 ? " Motor " + oVehiculo.Motor : "")
            ;

            foreach (ProductoVenta Parte in Partes)
            {
                this.dgvPartes.Rows.Add(true, Parte.ParteID, Parte.NumeroDeParte, Parte.NombreDeParte);
            }
            this.dgvPartes.AutoResizeRows();
        }

        private void dgvPartes_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvPartes.CurrentRow == null) return;

            if (e.KeyCode == Keys.ShiftKey)
            {
                // Para corregir extraño detalle de que si está seleccionada la celda de "Aplicar", no se cambia el valor
                if (this.dgvPartes.CurrentCell.OwningColumn.Name == "Aplicar")
                    this.dgvPartes.EndEdit();

                this.dgvPartes.CurrentRow.Cells["Aplicar"].Value = !Helper.ConvertirBool(this.dgvPartes.CurrentRow.Cells["Aplicar"].Value);
            }
        }

        private void btnAplicar_Click(object sender, System.EventArgs e)
        {
            // Se solicita contraseña del Usuario
            var ValUsr = UtilDatos.ValidarObtenerUsuario("AplicarPartes.Agregar");
            if (ValUsr.Error)
                return;

            // Se procede a guardar los datos
            int? oMotorID = null, oAnio = null;
            ParteVehiculo ParteV;
            for (int iFila = 0; iFila < this.dgvPartes.Rows.Count; iFila++)
            {
                if (Helper.ConvertirBool(this.dgvPartes["Aplicar", iFila].Value))
                {
                    if (this.oVehiculo.MotorID > 0)
                        oMotorID = this.oVehiculo.MotorID;
                    if (this.oVehiculo.Anio > 0)
                        oAnio = this.oVehiculo.Anio;

                    ParteV = new ParteVehiculo() {
                         ParteID = Helper.ConvertirEntero(this.dgvPartes["ParteID", iFila].Value)
                         , ModeloID = this.oVehiculo.ModeloID
                         , MotorID = oMotorID
                         , Anio = oAnio
                         , TipoFuenteID = Cat.TipoDeFuentes.Mostrador
                         , RegistroUsuarioID = ValUsr.Respuesta.UsuarioID
                    };
                    var Res = Guardar.ParteVehiculo(ParteV);

                    if (Res.Exito)
                        UtilLocal.MostrarNotificacion("Aplicación guardada exitosamente.");
                    else
                        UtilLocal.MostrarNotificacion(Res.Mensaje);
                }
            }

            this.Close();
        }

        private void btnCancelar_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        
    }
}
