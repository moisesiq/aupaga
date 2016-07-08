using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

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
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));

            // Se cargan los combos del grid
            this.mesUsuarioID.ValueMember = "UsuarioID";
            this.mesUsuarioID.DisplayMember = "NombreUsuario";
            this.mesUsuarioID.DataSource = Datos.GetListOf<Usuario>(c => c.Estatus);
            this.mesMarcaID.ValueMember = "MarcaParteID";
            this.mesMarcaID.DisplayMember = "NombreMarcaParte";
            this.mesMarcaID.DataSource = Datos.GetListOf<MarcaParte>(c => c.Estatus);
            this.mesLineaID.ValueMember = "LineaID";
            this.mesLineaID.DisplayMember = "NombreLinea";
            this.mesLineaID.DataSource = Datos.GetListOf<Linea>(c => c.Estatus);

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
            if (Util.Entero(oFila.Cells["mesCambio"].Value) == Cat.TiposDeAfectacion.NoEspecificado)
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
                if (Util.Entero(this.dgvEspecificas.CurrentRow.Cells["mesCambio"].Value) == Cat.TiposDeAfectacion.Agregar)
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
            
            if (Util.Entero(oFila.Cells["mesCambio"].Value) == Cat.TiposDeAfectacion.SinCambios)
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

            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);

            // Se llenan la metas específicas
            this.dgvEspecificas.Rows.Clear();
            var oMetasEsp = Datos.GetListOf<MetasView>(c => c.SucursalID == iSucursalID);
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

            string sNumeroDeParte = Util.Cadena(oFila.Cells[sColNumeroDeParte].Value);
            var oPartes = Datos.GetListOf<Parte>(c => c.NumeroParte == sNumeroDeParte && c.Estatus);
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
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);

            // Se guardan las metas específicas
            Meta oMeta;
            foreach (DataGridViewRow oFila in this.dgvEspecificas.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iMetaID = Util.Entero(oFila.Cells["mesMetaID"].Value);
                int iCambio = Util.Entero(oFila.Cells["mesCambio"].Value);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oMeta = new Meta() { SucursalID = iSucursalID };
                        else
                            oMeta = Datos.GetEntity<Meta>(c => c.MetaID == iMetaID);
                        oMeta.VendedorID = (Util.Entero(oFila.Cells["mesUsuarioID"].Value) > 0 ?
                            (int?)Util.Entero(oFila.Cells["mesUsuarioID"].Value) : null);
                        oMeta.MarcaParteID = (Util.Entero(oFila.Cells["mesMarcaID"].Value) > 0 ? 
                            (int?)Util.Entero(oFila.Cells["mesMarcaID"].Value): null);
                        oMeta.LineaID = (Util.Entero(oFila.Cells["mesLineaID"].Value) > 0 ? 
                            (int?)Util.Entero(oFila.Cells["mesLineaID"].Value) : null);
                        oMeta.ParteID = (Util.Entero(oFila.Cells["mesParteID"].Value) > 0 ?
                            (int?)Util.Entero(oFila.Cells["mesParteID"].Value) : null);
                        oMeta.NombreMeta = Util.Cadena(oFila.Cells["mesNombre"].Value);
                        oMeta.Cantidad = Util.Decimal(oFila.Cells["mesCantidad"].Value);
                        oMeta.RutaImagen = Util.Cadena(oFila.Cells["mesRutaImagen"].Value);
                        Datos.Guardar<Meta>(oMeta);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oMeta = Datos.GetEntity<Meta>(c => c.MetaID == iMetaID);
                        Datos.Eliminar<Meta>(oMeta);
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
