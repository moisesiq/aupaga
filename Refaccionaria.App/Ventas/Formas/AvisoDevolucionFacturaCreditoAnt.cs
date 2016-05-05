using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class AvisoDevolucionFacturaCreditoAnt : MensajeTexto
    {
        // Para el Singleton *
        private static AvisoDevolucionFacturaCreditoAnt instance;
        public static AvisoDevolucionFacturaCreditoAnt Instance
        {
            get
            {
                if (AvisoDevolucionFacturaCreditoAnt.instance == null || AvisoDevolucionFacturaCreditoAnt.instance.IsDisposed)
                    AvisoDevolucionFacturaCreditoAnt.instance = new AvisoDevolucionFacturaCreditoAnt();
                return AvisoDevolucionFacturaCreditoAnt.instance;
            }
        }
        //

        public AvisoDevolucionFacturaCreditoAnt()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria;
        }
    }
}
