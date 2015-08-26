using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CatNominaOficialCuentas : ListadoEditable
    {
        // Para el Singleton
        private static CatNominaOficialCuentas _Instance;
        public static CatNominaOficialCuentas Instance
        {
            get
            {
                if (CatNominaOficialCuentas._Instance == null || CatNominaOficialCuentas._Instance.IsDisposed)
                    CatNominaOficialCuentas._Instance = new CatNominaOficialCuentas();
                return CatNominaOficialCuentas._Instance;
            }
        }
        //

        public CatNominaOficialCuentas()
        {
            InitializeComponent();

            // Se agregan las columnas al grid
            var oCol = base.AgregarColumna("ContaCuentaDeMayorID", "", 20);
            oCol.Visible = false;
            oCol = base.AgregarColumna("CuentaDeMayor", "Cuenta", 320);
            oCol.ReadOnly = true;
            var oColSucursal = base.AgregarColumnaCombo("Suma", "Suma/Resta", 80);
            oColSucursal.Items.AddRange("Suma", "Resta");

            // Se asignan los eventos
            this.dgvDatos.KeyDown += new KeyEventHandler(dgvDatos_KeyDown);
        }

        #region [ Eventos ]

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            if (e.KeyData == Keys.F3)
            {
                var oListado = Helper.ListaEntityADataTable(General.GetListOf<ContaCuentaDeMayor>());
                var frmSel = new SeleccionListado(oListado);
                frmSel.dgvListado.MostrarColumnas("CuentaDeMayor", "CuentaContpaq", "CuentaSat");
                if (frmSel.ShowDialog(Principal.Instance) == DialogResult.OK)
                {
                    this.dgvDatos.CurrentRow.Cells["ContaCuentaDeMayorID"].Value = frmSel.Seleccion["ContaCuentaDeMayorID"];
                    this.dgvDatos.CurrentRow.Cells["CuentaDeMayor"].Value = frmSel.Seleccion["CuentaDeMayor"];
                }
                frmSel.Dispose();
            }
        }

        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oDatos = General.GetListOf<NominaOficialCuentasView>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                this.dgvDatos.AgregarFila(oReg.NominaOficialCuentaID, Cat.TiposDeAfectacion.SinCambios, oReg.ContaCuentaDeMayorID, oReg.CuentaDeMayor
                    , (oReg.Suma ? "Suma" : "Resta"));
            }

            Cargando.Cerrar();
        }

        protected override bool AccionGuardar()
        {
            // Se valida
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            NominaOficialCuenta oReg;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iId = this.dgvDatos.ObtenerId(oFila);
                int iCambio = this.dgvDatos.ObtenerIdCambio(oFila);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new NominaOficialCuenta();
                        else
                            oReg = General.GetEntity<NominaOficialCuenta>(c => c.NominaOficialCuentaID == iId);

                        // Se llenan los datos
                        oReg.ContaCuentaDeMayorID = Helper.ConvertirEntero(oFila.Cells["ContaCuentaDeMayorID"].Value);
                        oReg.Suma = (Helper.ConvertirCadena(oFila.Cells["Suma"].Value) == "Suma");

                        Guardar.Generico<NominaOficialCuenta>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<NominaOficialCuenta>(c => c.NominaOficialCuentaID == iId);
                        Guardar.Eliminar<NominaOficialCuenta>(oReg);
                        break;
                }
            }

            Cargando.Cerrar();
            this.CargarDatos();
            return true;
        }

        private bool Validar()
        {
            bool bValido = true;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (oFila.IsNewRow) continue;
                oFila.Cells["CuentaDeMayor"].ErrorText = "";

                if (Helper.ConvertirEntero(oFila.Cells["ContaCuentaDeMayorID"].Value) == 0)
                {
                    oFila.Cells["CuentaDeMayor"].ErrorText = "Cuenta inválida.";
                    bValido = false;
                }
            }

            return bValido;
        }

        #endregion
    }
}
