using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CatMetas : UserControl
    {
        // Para el Singleton
        private static CatMetas _Instance;
        public static CatMetas Instance
        {
            get
            {
                if (CatMetas._Instance == null || CatMetas._Instance.IsDisposed)
                    CatMetas._Instance = new CatMetas();
                return CatMetas._Instance;
            }
        }
        //

        ControlError ctlError = new ControlError();

        public CatMetas()
        {
            InitializeComponent();

            // this.dgvEspecificas.DefaultCellStyle.ForeColor = Color.Black;
            this.dgvEspecificas.DefaultCellStyle.ForeColor = Color.Black;
        }

        #region [ Eventos ]

        private void MetasCat_Load(object sender, EventArgs e)
        {
            // Se carga el combo de sucursal
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));

            // Se cargan los combos del grid
            this.mesUsuarioID.ValueMember = "UsuarioID";
            this.mesUsuarioID.DisplayMember = "NombreUsuario";
            this.mesUsuarioID.DataSource = General.GetListOf<Usuario>(c => c.Estatus);
            this.mesMarcaID.ValueMember = "MarcaParteID";
            this.mesMarcaID.DisplayMember = "NombreMarcaParte";
            this.mesMarcaID.DataSource = General.GetListOf<MarcaParte>(c => c.Estatus);
            this.mesLineaID.ValueMember = "LineaID";
            this.mesLineaID.DisplayMember = "NombreLinea";
            this.mesLineaID.DataSource = General.GetListOf<Linea>(c => c.Estatus);

            (this.mesUsuarioID.DataSource as List<Usuario>).Insert(0, new Usuario() { NombreUsuario = "(TODOS)" });
            (this.mesMarcaID.DataSource as List<MarcaParte>).Insert(0, new MarcaParte() { NombreMarcaParte = "" });
            (this.mesLineaID.DataSource as List<Linea>).Insert(0, new Linea() { NombreLinea = "" });

            // Se mandan cargar los datos
            // this.CargarDatos();
        }

        private void cmbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused)
                this.CargarDatos();
        }

        private void dgvEspecificas_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var oFila = this.dgvEspecificas.Rows[e.RowIndex];
            if (Helper.ConvertirEntero(oFila.Cells["mesCambio"].Value) == Cat.TiposDeAfectacion.NoEspecificado)
            {
                oFila.Cells["mesCambio"].Value = Cat.TiposDeAfectacion.Agregar;
                oFila.DefaultCellStyle.ForeColor = Color.Blue;
            }
        }

        private void dgvEspecificas_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvEspecificas.CurrentRow == null || this.dgvEspecificas.CurrentRow.IsNewRow) return;

            if (e.KeyCode == Keys.Delete)
            {
                if (Helper.ConvertirEntero(this.dgvEspecificas.CurrentRow.Cells["mesCambio"].Value) == Cat.TiposDeAfectacion.Agregar)
                {
                    this.dgvEspecificas.Rows.Remove(this.dgvEspecificas.CurrentRow);
                }
                else
                {
                    this.dgvEspecificas.CurrentRow.Cells["mesCambio"].Value = Cat.TiposDeAfectacion.Borrar;
                    this.dgvEspecificas.CurrentRow.DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
        }

        private void dgvEspecificas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var oFila = this.dgvEspecificas.Rows[e.RowIndex];

            // Se verifica el número de parte, si aplica
            if (this.dgvEspecificas.Columns[e.ColumnIndex].Name == "mesNumeroDeParte")
                this.VerNumeroDeParte(oFila);
            
            if (Helper.ConvertirEntero(oFila.Cells["mesCambio"].Value) == Cat.TiposDeAfectacion.SinCambios)
            {
                oFila.Cells["mesCambio"].Value = Cat.TiposDeAfectacion.Modificar;
                oFila.DefaultCellStyle.ForeColor = Color.Orange;
            }
        }

        private void dgvEspecificas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvEspecificas.Columns[e.ColumnIndex].Name == "mesRutaImagen")
            {
                var oArc = new OpenFileDialog();
                oArc.Filter = "*.png|*.png";
                oArc.InitialDirectory = UtilLocal.RutaImagenes();
                if (oArc.ShowDialog(Principal.Instance) == DialogResult.OK)
                {
                    this.dgvEspecificas.CurrentCell.Value = oArc.FileName.Replace(oArc.InitialDirectory, "");
                    this.dgvEspecificas.EndEdit();
                }
                oArc.Dispose();
            }
        }
        
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            this.AccionGuardar();
        }

        #endregion

        #region [ Métodos ]

        private void CargarDatos()
        {
            Cargando.Mostrar();

            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);

            // Se llenan la metas específicas
            this.dgvEspecificas.Rows.Clear();
            var oMetasEsp = General.GetListOf<MetasView>(c => c.SucursalID == iSucursalID);
            foreach (var oMeta in oMetasEsp)
            {
                this.dgvEspecificas.Rows.Add(oMeta.MetaID, Cat.TiposDeAfectacion.SinCambios, oMeta.ParteID
                    , oMeta.VendedorID, oMeta.MarcaID, oMeta.LineaID, oMeta.NumeroDeParte, oMeta.NombreMeta, oMeta.Cantidad, oMeta.RutaImagen);
            }
            
            Cargando.Cerrar();
        }

        private void VerNumeroDeParte(DataGridViewRow oFila)
        {
            string sColNumeroDeParte = ((oFila.DataGridView == this.dgvEspecificas ? "mes" : "mad") + "NumeroDeParte");
            string sColParteID = ((oFila.DataGridView == this.dgvEspecificas ? "mes" : "mad") + "ParteID");

            string sNumeroDeParte = Helper.ConvertirCadena(oFila.Cells[sColNumeroDeParte].Value);
            var oPartes = General.GetListOf<Parte>(c => c.NumeroParte == sNumeroDeParte && c.Estatus);
            if (oPartes.Count > 1)
            {
                var frmLista = new ObtenerElementoLista("Selecciona la parte que deseas usar:", oPartes);
                // frmLista.Text = "Número de parte o código repetido";
                frmLista.MostrarColumnas("NumeroParte", "NombreParte");
                frmLista.ShowDialog(Principal.Instance);
                if (frmLista.DialogResult == DialogResult.OK)
                    oFila.Cells[sColParteID].Value = (frmLista.Seleccion as Parte).ParteID;
                frmLista.Dispose();
            }
            else
            {
                oFila.Cells[sColParteID].Value = (oPartes.Count == 1 ? oPartes[0].ParteID : 0);
            }
        }
        
        private bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            Cargando.Mostrar();
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);

            // Se guardan las metas específicas
            Meta oMeta;
            foreach (DataGridViewRow oFila in this.dgvEspecificas.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iMetaID = Helper.ConvertirEntero(oFila.Cells["mesMetaID"].Value);
                int iCambio = Helper.ConvertirEntero(oFila.Cells["mesCambio"].Value);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oMeta = new Meta() { SucursalID = iSucursalID };
                        else
                            oMeta = General.GetEntity<Meta>(c => c.MetaID == iMetaID);
                        oMeta.VendedorID = (Helper.ConvertirEntero(oFila.Cells["mesUsuarioID"].Value) > 0 ?
                            (int?)Helper.ConvertirEntero(oFila.Cells["mesUsuarioID"].Value) : null);
                        oMeta.MarcaParteID = (Helper.ConvertirEntero(oFila.Cells["mesMarcaID"].Value) > 0 ? 
                            (int?)Helper.ConvertirEntero(oFila.Cells["mesMarcaID"].Value): null);
                        oMeta.LineaID = (Helper.ConvertirEntero(oFila.Cells["mesLineaID"].Value) > 0 ? 
                            (int?)Helper.ConvertirEntero(oFila.Cells["mesLineaID"].Value) : null);
                        oMeta.ParteID = (Helper.ConvertirEntero(oFila.Cells["mesParteID"].Value) > 0 ?
                            (int?)Helper.ConvertirEntero(oFila.Cells["mesParteID"].Value) : null);
                        oMeta.NombreMeta = Helper.ConvertirCadena(oFila.Cells["mesNombre"].Value);
                        oMeta.Cantidad = Helper.ConvertirDecimal(oFila.Cells["mesCantidad"].Value);
                        oMeta.RutaImagen = Helper.ConvertirCadena(oFila.Cells["mesRutaImagen"].Value);
                        Guardar.Generico<Meta>(oMeta);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oMeta = General.GetEntity<Meta>(c => c.MetaID == iMetaID);
                        Guardar.Eliminar<Meta>(oMeta);
                        break;
                }
            }
                                    
            Cargando.Cerrar();
            this.CargarDatos();
            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();

            if (this.cmbSucursal.SelectedValue == null)
                this.ctlError.PonerError(this.cmbSucursal, "Debes especificar la Sucursal.");
            
            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion
               
                
    }
}
