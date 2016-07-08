
namespace Refaccionaria.App
{
    public partial class CajaRefuerzo : CajaMonedas
    {
        public CajaRefuerzo()
        {
            InitializeComponent();
        }

        public void ActualizarDatos()
        {
            this.LimpiarMonedas();
        }
    }
}
