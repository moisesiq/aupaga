using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
using System.IO;

namespace Refaccionaria.App
{
    public partial class catalogosPartes : UserControl
    {
        // Para el Singleton
        private static catalogosPartes _Instance;
        public static catalogosPartes Instance
        {
            get
            {
                if (catalogosPartes._Instance == null || catalogosPartes._Instance.IsDisposed)
                    catalogosPartes._Instance = new catalogosPartes();
                return catalogosPartes._Instance;
            }
        }
        //


        // Para la búsqueda
        const int BusquedaRetrasoTecla = 400;
        int BusquedaLlamada = 0;
        int BusquedaIntento = 0;

        BindingSource fuenteDatos;
        public bool Seleccionado = false;
        public Dictionary<string, object> Sel;
        BackgroundWorker bgworker;

        Parte oParte;
        ControlError cntError = new ControlError();
        ControlError ctlAdv = new ControlError() { Icon = Properties.Resources._16_Ico_Advertencia };
        bool EsNuevo = true;

        public catalogosPartes()
        {
            InitializeComponent();
        }
        
        #region [eventos]

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void catalogosPartes_Load(object sender, EventArgs e)
        {
            this.CargaInicial();
        }
                
        public void btnAgregar_Click(object sender, EventArgs e)
        {
            this.txtBusqueda.Clear();
            this.dgvDatos.Rows.Clear();
            this.LimpiarFormulario();
            this.EsNuevo = true;
            this.txtNumeroParte.Focus();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (e == null)
                return;
            if (this.dgvDatos.CurrentRow == null)
                return;
            try
            {
                var parteId = Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ParteID"].Value);
                this.CargarParte(parteId);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnImprimirTicket_Click(object sender, EventArgs e)
        {
            if (!EsNuevo && oParte != null)
            {
                int copias = 0;
                var frmCantidad = new MensajeObtenerValor("Número de etiquetas", "1", MensajeObtenerValor.Tipo.Entero);
                if (frmCantidad.ShowDialog(Principal.Instance) == DialogResult.OK)
                {
                    copias = Helper.ConvertirEntero(frmCantidad.Valor);
                }
                frmCantidad.Dispose();

                if (copias > 0)
                {
                    var etiquetas = new List<Etiquetas>();
                    for (int x = 0; x < copias; x++)
                    {
                        var etiqueta = new Etiquetas()
                        {
                            ParteID = oParte.ParteID,
                            NumeroParte = oParte.NumeroParte,
                            NombreParte = oParte.NombreParte,
                            CodigoBarra = oParte.CodigoBarra,
                            NumeroEtiquetas = copias
                        };
                        etiquetas.Add(etiqueta);
                    }

                    IEnumerable<Etiquetas> listaEtiquetas = etiquetas;
                    using (FastReport.Report report = new FastReport.Report())
                    {
                        report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteEtiquetas.frx"));
                        report.RegisterData(etiquetas, "etiquetas", 3);
                        report.GetDataSource("etiquetas").Enabled = true;
                        report.FindObject("Text1").Delete();
                        // report.Show(true);
                        UtilLocal.EnviarReporteASalida("Reportes.Partes.Etiqueta", report);
                    }
                }
            }
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;
            if (e.KeyCode == Keys.F5)
            {
                /* this.txtBusqueda.Clear();
                this.txtBusqueda.Focus();
                this.CargaInicial();
                this.IniciarActualizarListado();
                */
            }
            if (e.KeyCode == Keys.End)
            {
                this.dgvDatos.Rows[dgvDatos.Rows.Count - 1].Selected = true;
                this.dgvDatos.Rows[dgvDatos.Rows.Count - 1].Cells[0].Selected = true;
                //this.dgvDatos.FirstDisplayedScrollingRowIndex = dgvDatos.Rows.Count - 1;               
            }
        }

        private void dgvDatos_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvDatos.VerSeleccionNueva() && this.dgvDatos.CurrentRow != null)
            {
                int iParteID = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ParteID"].Value);
                this.CargarParte(iParteID);

                // Si está en la pestaña de kardex, éste se actualiza
                if (this.tabPartes.SelectedTab == this.tbpKardex)
                    this.CargarKardex(iParteID);
            }
        }

        private void cboFiltro_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //bgworker = new BackgroundWorker();
            //bgworker.DoWork += ActualizarListado;
            //bgworker.RunWorkerCompleted += Terminado;
            //bgworker.RunWorkerAsync();
            //progreso.Value = 0;
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (!this.txtBusqueda.Focused)
                return;
            if (this.txtBusqueda.Text == "")
            {
                this.BusquedaParte();
                return;
            }
            if (this.txtBusqueda.TextLength < 3)
                return;

            // Se implementa mecanismo de restraso para teclas, si se presionan demasiado rápido, no se hace la búsqueda
            this.BusquedaLlamada++;
            new System.Threading.Thread(this.BusquedaVerRetraso).Start();
        }

        private void txtBusqueda_Enter(object sender, EventArgs e)
        {
            txtBusqueda.BeginInvoke(new MethodInvoker(txtBusqueda.SelectAll));
        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    this.txtBusqueda.Clear();
                    break;
                case Keys.Down:
                    this.dgvDatos.Focus();
                    break;
                case Keys.Escape:
                    this.txtBusqueda.Clear();
                    this.BusquedaLlamada = this.BusquedaIntento = 0;
                    break;
            }
        }

        private void cboMarca_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(cboMarca.SelectedValue.ToString(), out id))
            {
                CargarLineas(id);
                SugerirDescripcionDeParte();
            }
        }

        private void tabPartes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabPartes.SelectedTab == null) return;

            switch (this.tabPartes.SelectedTab.Name)
            {
                case "tbpKardex":
                    if (this.oParte != null)
                        this.CargarKardex(this.oParte.ParteID);
                    break;
                case "tbpErrores":
                    if (this.dgvErrores.Rows.Count <= 0)
                        this.CargarErrores();
                    break;
            }
        }

        private void cboLinea_SelectedValueChanged(object sender, EventArgs e)
        {
            int id = Helper.ConvertirEntero(this.cboLinea.SelectedValue);
            if (id > 0)
            {
                this.CargarSistemas(id);
                this.CargarMachote(id);
                this.SugerirDescripcionDeParte();
                this.CargarSubsistemasLinea(id);
            }
            else if (this.oParte != null)
            {
                this.ctlAdv.PonerError(this.cboLinea, "No hay relación Marca - Línea.");
            }
        }

        private void cboSistema_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(cboSistema.SelectedValue.ToString(), out id))
            {
                CargarSubsistemas(id);
            }
        }

        private void txtRequiereDepositoDe_Leave(object sender, EventArgs e)
        {
            this.txtRequiereDepositoDe.Tag = null;
            if (this.txtRequiereDepositoDe.Text == "") return;
            var oPartes = General.GetListOf<Parte>(c => c.NumeroParte == this.txtRequiereDepositoDe.Text && c.Estatus);
            if (oPartes.Count > 1)
            {
                var frmLista = new ObtenerElementoLista("Selecciona el artículo de depósito correspondiente:", oPartes);
                frmLista.Text = "Número de parte o código repetido";
                frmLista.MostrarColumnas("CodigoBarra", "NumeroParte", "NombreParte");
                frmLista.dgvDatos.Columns["NombreParte"].DisplayIndex = 0;
                frmLista.dgvDatos.Columns["NumeroParte"].DisplayIndex = 1;
                frmLista.ShowDialog(Principal.Instance);
                if (frmLista.DialogResult == DialogResult.OK)
                    this.txtRequiereDepositoDe.Tag = ((frmLista.Seleccion as Parte) == null ? null : (int?)(frmLista.Seleccion as Parte).ParteID);
                frmLista.Dispose();
            }
            else if (oPartes.Count == 1)
            {
                this.txtRequiereDepositoDe.Tag = oPartes[0].ParteID;
            }
        }

        private void chkSiEtiqueta_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkSiEtiqueta.CheckState == CheckState.Checked)
            {
                this.chkSoloUna.Checked = false;
                this.chkSoloUna.Enabled = true;
            }
            else
            {
                this.chkSoloUna.Enabled = false;
                this.chkSoloUna.Checked = false;
            }
        }

        private void picBoxImagen_Click(object sender, EventArgs e)
        {
            if (this.oParte == null) return;
            var frmImagenes = new VerImagenesParte(this.oParte.ParteID);
            frmImagenes.Show(Principal.Instance);
            // frmImagenes.Dispose();
        }

        private void txtNumeroParte_Enter(object sender, EventArgs e)
        {
            txtNumeroParte.BeginInvoke(new MethodInvoker(txtNumeroParte.SelectAll));
        }

        private void txtNombreParte_Enter(object sender, EventArgs e)
        {
            //txtNombreParte.BeginInvoke(new MethodInvoker(txtNombreParte.SelectAll));
            txtNombreParte.Select(txtNombreParte.Text.Length, 0);
        }

        private void lstImagenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lstImagenes.SelectedIndex >= 0)
                {
                    if (Directory.Exists(GlobalClass.ConfiguracionGlobal.pathImagenes))
                        picBoxImagen.Image = new Bitmap(System.IO.Path.Combine(GlobalClass.ConfiguracionGlobal.pathImagenes, lstImagenes.SelectedItem.ToString()));
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void lstImagenes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var res = Negocio.Helper.MensajePregunta("¿Está seguro de que desea eliminar esta imagen?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        var nombreImagen = lstImagenes.SelectedItem.ToString();
                        // Se borra la imagen
                        if (File.Exists(GlobalClass.ConfiguracionGlobal.pathImagenes + nombreImagen))
                            File.Delete(GlobalClass.ConfiguracionGlobal.pathImagenes + nombreImagen);

                        /* var imagen = Negocio.General.GetEntity<ParteImagen>(p => p.ParteID.Equals(oParte.ParteID) && p.NombreImagen.Equals(nombreImagen));
                        if (imagen != null)
                        {
                            Negocio.General.Delete<ParteImagen>(imagen);
                            lstImagenes.Items.Remove(nombreImagen);
                            picBoxImagen.Image = null;
                        }
                        */
                    }
                    catch (Exception ex)
                    {
                        Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        private void btnAgregarImagenes_Click(object sender, EventArgs e)
        {
            if (oParte.ParteID < 0)
                return;
            /* DetalleImagen detalle = new DetalleImagen(oParte.ParteID);
            detalle.ShowDialog();
            this.CargarImagenes(oParte.ParteID);
            */
        }

        private void btnAgregarEquivalencia_Click(object sender, EventArgs e)
        {
            if (this.oParte == null || this.oParte.ParteID < 0)
                return;
            if (UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Equivalentes.Agregar", true))
            {
                DetalleEquivalente detalle = new DetalleEquivalente(oParte.ParteID);
                detalle.ShowDialog();
                this.CargarEquivalencias(oParte.ParteID);
            }
        }

        private void btnAgregarAplicacion_Click(object sender, EventArgs e)
        {
            if (oParte.ParteID < 0)
                return;
            if (UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Aplicaciones.Agregar", true))
            {
                DetalleAplicacion detalle = new DetalleAplicacion(oParte.ParteID);
                detalle.ShowDialog();
                this.CargarAplicaciones(oParte.ParteID);
            }
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            if (EsNuevo)
            {
                Calculadora calc = Calculadora.Instance;
                calc.ShowDialog();
                try
                {
                    if (calc.PreciosCalculadora != null)
                        if (calc.PreciosCalculadora.FueModificado.Equals(true))
                        {
                            CalcularPrecios(calc.PreciosCalculadora);
                        }
                }
                catch
                { }
            }
            else
            {
                if (oParte.ParteID < 0)
                    return;
                Calculadora calc = new Calculadora(oParte.ParteID);
                calc.ShowDialog();
                try
                {
                    if (calc.PreciosCalculadora != null)
                        if (calc.PreciosCalculadora.FueModificado.Equals(true))
                        {
                            CalcularPrecios(calc.PreciosCalculadora);
                        }
                }
                catch
                { }
            }
        }

        private void dgvExistencias_CurrentCellChanged(object sender, EventArgs e)
        {
            this.LlenarDescripcionMaxMin(this.dgvExistencias.CurrentRow);
        }

        private void dgvExistencias_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                for (int i = 0; i < dgvExistencias.Columns.Count; i++)
                {
                    if (dgvExistencias.Columns[i].Name != "Max" && dgvExistencias.Columns[i].Name != "Min" && dgvExistencias.Columns[i].Name != "UEmp")
                        dgvExistencias.Columns[i].ReadOnly = true;
                }

                if (dgvExistencias.Columns[e.ColumnIndex].Name == "Max" || dgvExistencias.Columns[e.ColumnIndex].Name == "Min" || dgvExistencias.Columns[e.ColumnIndex].Name == "UEmp")
                {
                    int value;
                    if (!int.TryParse(e.FormattedValue.ToString(), out value))
                    {
                        dgvExistencias.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida.";
                        e.Cancel = true;
                    }
                    else
                    {
                        if (value < 0)
                        {
                            dgvExistencias.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida mayor a 0.";
                            e.Cancel = true;
                        }
                        //else if (dgvExistencias.Columns[e.ColumnIndex].Name == "Max" && value < 1)
                        //{
                        //    dgvExistencias.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida mayor a 1.";
                        //    e.Cancel = true;
                        //}
                        else if (dgvExistencias.Columns[e.ColumnIndex].Name == "Max"
                        && value < Helper.ConvertirEntero(dgvExistencias.Rows[e.RowIndex].Cells["Min"].Value))
                        {
                            dgvExistencias.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida que sea mayor que el minimo.";
                            e.Cancel = true;
                        }
                        else if (dgvExistencias.Columns[e.ColumnIndex].Name == "Min"
                            && value > Helper.ConvertirEntero(dgvExistencias.Rows[e.RowIndex].Cells["Max"].Value))
                        {
                            dgvExistencias.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida que sea menor que el maximo.";
                            e.Cancel = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvExistencias_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvExistencias.CurrentRow == null) return;
            if (this.dgvExistencias.Columns[e.ColumnIndex].Name == "Max" || this.dgvExistencias.Columns[e.ColumnIndex].Name == "Min")
                this.dgvExistencias.CurrentRow.Cells["MaxMinFijo"].Value = true;
        }

        private void dgvEquivalentes_KeyDown(object sender, KeyEventArgs e)
        {
            if (UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Equivalentes.Eliminar", false))
            {
                if (this.dgvEquivalentes.CurrentRow == null) return;
                if (e.KeyCode == Keys.Delete)
                {
                    var res = Negocio.Helper.MensajePregunta("¿Está seguro de que desea eliminar esta equivalencia?", GlobalClass.NombreApp);
                    if (res == DialogResult.Yes)
                    {
                        try
                        {
                            var dg = (DataGridView)sender;
                            var fila = dg.SelectedRows[0].Index;
                            var ParteEquivalenteID = Negocio.Helper.ConvertirEntero(dgvEquivalentes.Rows[fila].Cells["equ_ParteEquivalenteID"].Value);
                            var parteEquivalente = Negocio.General.GetEntity<ParteEquivalente>(p => p.ParteEquivalenteID.Equals(ParteEquivalenteID));
                            if (parteEquivalente != null)
                            {
                                Negocio.General.Delete<ParteEquivalente>(parteEquivalente);
                                CargarEquivalencias(oParte.ParteID);
                            }
                        }
                        catch (Exception ex)
                        {
                            Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                        }
                    }
                }
            }
        }

        private void dgvEquivalentes_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvEquivalentes.VerSeleccionNueva())
            {
                this.pcbEqu_Imagen.Image = null;
                if (this.dgvEquivalentes.CurrentRow == null) return;
                int iParteID = Helper.ConvertirEntero(this.dgvEquivalentes.CurrentRow.Cells["equ_ParteIDEquivalente"].Value);
                string sImagen = AdmonProc.ObtenerImagenParte(iParteID);
                if (File.Exists(sImagen))
                    this.pcbEqu_Imagen.Image = Image.FromFile(sImagen);
            }
        }

        private void pcbEqu_Imagen_Click(object sender, EventArgs e)
        {
            if (this.dgvEquivalentes.CurrentRow == null) return;
            int iParteID = Helper.ConvertirEntero(this.dgvEquivalentes.CurrentRow.Cells["equ_ParteIDEquivalente"].Value);
            var frmImagenes = new VerImagenesParte(iParteID);
            frmImagenes.Show(Principal.Instance);
        }

        private void dgvAplicaciones_KeyDown(object sender, KeyEventArgs e)
        {
            if (UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Aplicaciones.Eliminar", false))
            {
                if (this.dgvAplicaciones.CurrentRow == null) return;
                if (e.KeyCode == Keys.Delete)
                {
                    var res = Helper.MensajePregunta("¿Está seguro de que desea eliminar esta aplicación?", GlobalClass.NombreApp);
                    if (res == DialogResult.Yes)
                    {
                        try
                        {
                            var fila = this.dgvAplicaciones.CurrentRow.Index;
                            var parteId = Helper.ConvertirEntero(dgvAplicaciones.Rows[fila].Cells["ParteID"].Value);
                            var modeloId = Helper.ConvertirEntero(dgvAplicaciones.Rows[fila].Cells["ModeloID"].Value);

                            //Pueden ser nulls //MotorID NombreMotor Anio
                            var motorId = Helper.ConvertirEntero(dgvAplicaciones.Rows[fila].Cells["MotorID"].Value);
                            var anios = Helper.ConvertirCadena(dgvAplicaciones.Rows[fila].Cells["Anio"].Value).Split(',');

                            var partesVehiculo = new List<ParteVehiculo>();

                            //Caso todos nulos
                            if (motorId == 0 && anios[0].Length == 0)
                            {
                                partesVehiculo = General.GetListOf<ParteVehiculo>(p => p.ParteID == parteId && p.ModeloID == modeloId
                                    && p.MotorID == null && p.Anio == null);

                                foreach (var parteVehiculo in partesVehiculo)
                                {
                                    General.Delete<ParteVehiculo>(parteVehiculo);
                                }
                            }

                            //Caso solo MotorID
                            if (motorId > 0 && anios[0].Length == 0)
                            {
                                partesVehiculo = General.GetListOf<ParteVehiculo>(p => p.ParteID == parteId && p.ModeloID == modeloId
                                    && p.MotorID == motorId && p.Anio == null);

                                foreach (var parteVehiculo in partesVehiculo)
                                {
                                    General.Delete<ParteVehiculo>(parteVehiculo);
                                }
                            }

                            //Caso solo Anio
                            if (motorId == 0 && anios[0].Length > 0)
                            {
                                foreach (var anio in anios)
                                {
                                    var ano = Helper.ConvertirEntero(anio);
                                    partesVehiculo = General.GetListOf<ParteVehiculo>(p => p.ParteID == parteId && p.ModeloID == modeloId
                                    && p.MotorID == null && p.Anio == ano);

                                    foreach (var parteVehiculo in partesVehiculo)
                                    {
                                        General.Delete<ParteVehiculo>(parteVehiculo);
                                    }
                                }
                            }

                            //Caso todos con valor
                            if (motorId > 0 && anios[0].Length > 0)
                            {
                                foreach (var anio in anios)
                                {
                                    var ano = Helper.ConvertirEntero(anio);
                                    partesVehiculo = General.GetListOf<ParteVehiculo>(p => p.ParteID == parteId && p.ModeloID == modeloId
                                    && p.MotorID == motorId && p.Anio == ano);

                                    foreach (var parteVehiculo in partesVehiculo)
                                    {
                                        General.Delete<ParteVehiculo>(parteVehiculo);
                                    }
                                }
                            }

                            this.CargarAplicaciones(oParte.ParteID);
                        }
                        catch (Exception ex)
                        {
                            Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                        }
                    }
                }
            }
        }

        private void dgvCodigosAlternos_KeyDown(object sender, KeyEventArgs e)
        {
            if (UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.CodigosAlternos.Eliminar", false))
            {
                if (this.dgvCodigosAlternos.CurrentRow == null) return;
                if (e.KeyCode == Keys.Delete)
                {
                    var res = Helper.MensajePregunta("¿Está seguro de que desea eliminar este Código Alterno?", GlobalClass.NombreApp);
                    if (res == DialogResult.Yes)
                    {
                        try
                        {
                            var dg = (DataGridView)sender;
                            var fila = dg.SelectedRows[0].Index;
                            var ParteCodigoAlternoID = Helper.ConvertirEntero(this.dgvCodigosAlternos.Rows[fila].Cells["ParteCodigoAlternoID"].Value);
                            var parteCodigoAlterno = General.GetEntity<ParteCodigoAlterno>(p => p.ParteCodigoAlternoID.Equals(ParteCodigoAlternoID));
                            if (parteCodigoAlterno != null)
                            {
                                General.Delete<ParteCodigoAlterno>(parteCodigoAlterno);
                                CargarCodigosAlternos(oParte.ParteID);
                            }
                        }
                        catch (Exception ex)
                        {
                            Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                        }
                    }
                }
            }
        }

        private void dgvComplementarios_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                // Validación
                if (!UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Complementarias.Eliminar", true))
                    return;
                // Confirmación
                if (UtilLocal.MensajePregunta("¿Estás seguro que deseas borrar la Parte Complementaria seleccionada?") == DialogResult.Yes)
                {
                    int iParteComID = Helper.ConvertirEntero(this.dgvComplementarios.CurrentRow.Cells["ParteComplementariaID"].Value);
                    var oParteCom = General.GetEntity<ParteComplementaria>(c => c.ParteComplementariaID == iParteComID);
                    Guardar.Eliminar<ParteComplementaria>(oParteCom);
                    this.CargarComplementarios(this.oParte.ParteID);
                }
            }
        }

        private void txtNumeroParte_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtNumeroParte.Text))
                {
                    this.SugerirDescripcionDeParte();
                    if (EsNuevo)
                    {
                        var parte = General.GetEntity<Parte>(p => p.NombreParte == this.txtNumeroParte.Text && p.Estatus);
                        if (parte != null)
                            Helper.MensajeAdvertencia("el número de parte ya existe", GlobalClass.NombreApp);
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
        
        private void txtCosto_Leave(object sender, EventArgs e)
        {
            this.txtPrecio1.Text = Helper.DecimalToCadenaMoneda(OperacionPrecio(Negocio.Helper.ConvertirDecimal(this.txtPorcentaje1.Text)));
            this.txtPrecio2.Text = Helper.DecimalToCadenaMoneda(OperacionPrecio(Negocio.Helper.ConvertirDecimal(this.txtPorcentaje2.Text)));
            this.txtPrecio3.Text = Helper.DecimalToCadenaMoneda(OperacionPrecio(Negocio.Helper.ConvertirDecimal(this.txtPorcentaje3.Text)));
            this.txtPrecio4.Text = Helper.DecimalToCadenaMoneda(OperacionPrecio(Negocio.Helper.ConvertirDecimal(this.txtPorcentaje4.Text)));
            this.txtPrecio5.Text = Helper.DecimalToCadenaMoneda(OperacionPrecio(Negocio.Helper.ConvertirDecimal(this.txtPorcentaje5.Text)));
        }

        private void txtPrecio1_Leave(object sender, EventArgs e)
        {
            if (Helper.ConvertirDecimal(this.txtPorcentaje1.Text).Equals(0)
                && Helper.ConvertirDecimal(this.txtPrecio1.Text) > 0
                && Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPorcentaje1.Text = string.Format("{0:0.00}", (Helper.ConvertirDecimal(this.txtPrecio1.Text) /
                    Helper.ConvertirDecimal(this.txtCosto.Text)));
            }

            this.txtPrecio1.Text = Helper.DecimalToCadenaMoneda(Helper.ConvertirDecimal(this.txtPrecio1.Text));
        }

        private void txtPrecio2_Leave(object sender, EventArgs e)
        {
            if (Helper.ConvertirDecimal(this.txtPorcentaje2.Text).Equals(0)
               && Helper.ConvertirDecimal(this.txtPrecio2.Text) > 0
               && Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPorcentaje2.Text = String.Format("{0:0.00}", (Helper.ConvertirDecimal(this.txtPrecio2.Text) /
                    Helper.ConvertirDecimal(this.txtCosto.Text)));
            }

            this.txtPrecio2.Text = Helper.DecimalToCadenaMoneda(Helper.ConvertirDecimal(this.txtPrecio2.Text));
        }

        private void txtPrecio3_Leave(object sender, EventArgs e)
        {
            if (Helper.ConvertirDecimal(this.txtPorcentaje3.Text).Equals(0)
                && Helper.ConvertirDecimal(this.txtPrecio3.Text) > 0
                && Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPorcentaje3.Text = String.Format("{0:0.00}", (Helper.ConvertirDecimal(this.txtPrecio3.Text) /
                    Helper.ConvertirDecimal(this.txtCosto.Text)));
            }
            this.txtPrecio3.Text = Helper.DecimalToCadenaMoneda(Helper.ConvertirDecimal(this.txtPrecio3.Text));
        }

        private void txtPrecio4_Leave(object sender, EventArgs e)
        {
            if (Helper.ConvertirDecimal(this.txtPorcentaje4.Text).Equals(0)
                && Helper.ConvertirDecimal(this.txtPrecio4.Text) > 0
                && Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPorcentaje4.Text = String.Format("{0:0.00}", (Helper.ConvertirDecimal(this.txtPrecio4.Text) /
                    Helper.ConvertirDecimal(this.txtCosto.Text)));
            }
            this.txtPrecio4.Text = Helper.DecimalToCadenaMoneda(Helper.ConvertirDecimal(this.txtPrecio4.Text));
        }

        private void txtPrecio5_Leave(object sender, EventArgs e)
        {
            if (Helper.ConvertirDecimal(this.txtPorcentaje5.Text).Equals(0)
                && Helper.ConvertirDecimal(this.txtPrecio5.Text) > 0
                && Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPorcentaje5.Text = String.Format("{0:0.00}", (Helper.ConvertirDecimal(this.txtPrecio5.Text) /
                    Helper.ConvertirDecimal(this.txtCosto.Text)));
            }
            this.txtPrecio5.Text = Helper.DecimalToCadenaMoneda(Helper.ConvertirDecimal(this.txtPrecio5.Text));
        }

        private void txtPorcentaje1_Leave(object sender, EventArgs e)
        {
            if (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje1.Text) > 0
                && Negocio.Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPrecio1.Text = String.Format("{0:0.00}", (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje1.Text) *
                    Negocio.Helper.ConvertirDecimal(this.txtCosto.Text)));
            }
        }

        private void txtPorcentaje2_Leave(object sender, EventArgs e)
        {
            if (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje2.Text) > 0
                && Negocio.Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPrecio2.Text = String.Format("{0:0.00}", (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje2.Text) *
                    Negocio.Helper.ConvertirDecimal(this.txtCosto.Text)));
            }
        }

        private void txtPorcentaje3_Leave(object sender, EventArgs e)
        {
            if (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje3.Text) > 0
                && Negocio.Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPrecio3.Text = String.Format("{0:0.00}", (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje3.Text) *
                    Negocio.Helper.ConvertirDecimal(this.txtCosto.Text)));
            }
        }

        private void txtPorcentaje4_Leave(object sender, EventArgs e)
        {
            if (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje4.Text) > 0
                && Negocio.Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPrecio4.Text = String.Format("{0:0.00}", (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje4.Text) *
                    Negocio.Helper.ConvertirDecimal(this.txtCosto.Text)));
            }
        }

        private void txtPorcentaje5_Leave(object sender, EventArgs e)
        {
            if (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje5.Text) > 0
               && Negocio.Helper.ConvertirDecimal(this.txtCosto.Text) > 0)
            {
                this.txtPrecio5.Text = String.Format("{0:0.00}", (Negocio.Helper.ConvertirDecimal(this.txtPorcentaje5.Text) *
                    Negocio.Helper.ConvertirDecimal(this.txtCosto.Text)));
            }
        }

        private void txtCosto_Enter(object sender, EventArgs e)
        {
            BeginInvoke((Action)delegate { txtCosto.SelectAll(); });
        }

        private void txtPorcentaje1_Enter(object sender, EventArgs e)
        {
            BeginInvoke((Action)delegate { txtPorcentaje1.SelectAll(); });
        }

        private void txtPorcentaje2_Enter(object sender, EventArgs e)
        {
            BeginInvoke((Action)delegate { txtPorcentaje2.SelectAll(); });
        }

        private void txtPorcentaje3_Enter(object sender, EventArgs e)
        {
            BeginInvoke((Action)delegate { txtPorcentaje3.SelectAll(); });
        }

        private void txtPorcentaje4_Enter(object sender, EventArgs e)
        {
            BeginInvoke((Action)delegate { txtPorcentaje4.SelectAll(); });
        }

        private void txtPorcentaje5_Enter(object sender, EventArgs e)
        {
            BeginInvoke((Action)delegate { txtPorcentaje5.SelectAll(); });
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            var caracteristicas = new StringBuilder();

            /* int id;
            if (int.TryParse(cboLinea.SelectedValue.ToString(), out id))
            {
                //Validaciones para las caracteristicas segun la linea
                var lineaId = Negocio.Helper.ConvertirEntero(this.cboLinea.SelectedValue);
                var linea = Negocio.General.GetEntity<Linea>(l => l.LineaID.Equals(lineaId));
                if (linea != null)
                {
                    if (linea.Alto.Equals(true) && this.txtAlto.Text == "")
                        caracteristicas.Append("Alto, ");
                    if (linea.Diametro.Equals(true) && this.txtDiametro.Text == "")
                        caracteristicas.Append("Diametro, ");
                    if (linea.Largo.Equals(true) && this.txtLargo.Text == "")
                        caracteristicas.Append("Largo, ");
                    if (linea.Dientes.Equals(true) && this.txtDientes.Text == "")
                        caracteristicas.Append("Dientes, ");
                    if (linea.Astrias.Equals(true) && this.txtAstrias.Text == "")
                        caracteristicas.Append("Astrias, ");
                    if (linea.Sistema.Equals(true) && Negocio.Helper.ConvertirEntero(this.cboParteSistema.SelectedValue).Equals(1))
                        caracteristicas.Append("Sistema, ");
                    if (linea.Amperaje.Equals(true) && this.txtAmperes.Text == "")
                        caracteristicas.Append("Amperaje, ");
                    if (linea.Voltaje.Equals(true) && this.txtVoltios.Text == "")
                        caracteristicas.Append("Voltaje, ");
                    if (linea.Watts.Equals(true) && this.txtWatts.Text == "")
                        caracteristicas.Append("Watts, ");
                    if (linea.Ubicacion.Equals(true) && Negocio.Helper.ConvertirEntero(this.cboParteUbicacion.SelectedValue).Equals(1))
                        caracteristicas.Append("Ubicacion, ");
                    if (linea.Terminales.Equals(true) && this.txtTerminales.Text == "")
                        caracteristicas.Append("Terminales, ");
                }
            }
            */

            var mensaje = string.Empty;

            if (string.IsNullOrEmpty(caracteristicas.ToString()))
            {
                mensaje = "¿Está seguro de que la información es correcta?";
            }
            else
            {
                mensaje = string.Format("{0}{1}{2}", "Faltan características por llenar, (", caracteristicas.ToString().Substring(0, caracteristicas.ToString().Length - 2), ") ¿Está seguro de que la información es correcta?");
            }

            var res = Negocio.Helper.MensajePregunta(mensaje, GlobalClass.NombreApp);
            if (res == DialogResult.No)
                return;

            try
            {
                SplashScreen.Show(new Splash());
                this.btnGuardar.Enabled = false;
                PartePrecio oPartePrecio;
                if (this.EsNuevo)
                {
                    this.oParte = new Parte();
                    oPartePrecio = new PartePrecio();
                }
                else //Modificación
                {
                    oPartePrecio = General.GetEntity<PartePrecio>(c => c.ParteID == this.oParte.ParteID && c.Estatus);
                }
                
                // Se llenan los datos
                oParte.NumeroParte = txtNumeroParte.Text;
                oParte.NombreParte = txtNombreParte.Text;
                oParte.LineaID = Negocio.Helper.ConvertirEntero(cboLinea.SelectedValue);
                oParte.MarcaParteID = Negocio.Helper.ConvertirEntero(cboMarca.SelectedValue);
                oParte.SubsistemaID = Negocio.Helper.ConvertirEntero(cboSubsistema.SelectedValue);
                oParte.ProveedorID = Negocio.Helper.ConvertirEntero(cboProveedor.SelectedValue);
                oParte.MedidaID = Negocio.Helper.ConvertirEntero(cboMedida.SelectedValue);
                oParte.ParteEstatusID = Negocio.Helper.ConvertirEntero(cboEstatus.SelectedValue);
                oParte.AplicaComision = chkAplicaComision.Checked;

                /* Parte.Alto = Negocio.Helper.ConvertirEntero(txtAlto.Text);
                Parte.Largo = Negocio.Helper.ConvertirEntero(txtLargo.Text);
                Parte.Diametro = Negocio.Helper.ConvertirEntero(txtDiametro.Text);
                Parte.Dientes = Negocio.Helper.ConvertirEntero(txtDientes.Text);
                Parte.Astrias = Negocio.Helper.ConvertirEntero(txtAstrias.Text);
                Parte.Amperes = Negocio.Helper.ConvertirEntero(txtAmperes.Text);
                Parte.Voltios = Negocio.Helper.ConvertirEntero(txtVoltios.Text);
                Parte.Watts = Negocio.Helper.ConvertirEntero(txtWatts.Text);
                Parte.Terminales = Negocio.Helper.ConvertirEntero(txtTerminales.Text);
                Parte.Litros = Negocio.Helper.ConvertirEntero(txtLitros.Text);
                */

                oParte.ParteSistemaID = Negocio.Helper.ConvertirEntero(cboParteSistema.SelectedValue);
                oParte.ParteUbicacionID = Negocio.Helper.ConvertirEntero(cboParteUbicacion.SelectedValue);

                // Control de Cascos
                this.oParte.EsCasco = (this.cmbEsCascoPara.SelectedValue != null);
                this.oParte.EsCascoPara = (this.oParte.EsCasco.Valor() ? (int?)Helper.ConvertirEntero(this.cmbEsCascoPara.SelectedValue) : null);
                this.oParte.RequiereCascoDe = (this.cmbRequiereCascoDe.SelectedValue == null ? null : (int?)Helper.ConvertirEntero(this.cmbRequiereCascoDe.SelectedValue));
                this.oParte.RequiereDepositoDe = (this.txtRequiereDepositoDe.Tag == null ? null : (int?)Helper.ConvertirEntero(this.txtRequiereDepositoDe.Tag));

                oParte.Es9500 = chk9500.Checked;
                oParte.EsServicio = chkServicio.Checked;

                // Parte.TipoCilindroID = Helper.ConvertirEntero(this.cmbCar01.SelectedValue);
                oParte.TipoGarantiaID = Helper.ConvertirEntero(this.cboTipoGarantia.SelectedValue);
                oParte.Etiqueta = this.chkSiEtiqueta.Checked;
                oParte.SoloUnaEtiqueta = this.chkSoloUna.Checked;

                oParte.CodigoBarra = this.txtCodigoBarra.Text;
                oParte.TiempoReposicion = Helper.ConvertirDecimal(this.txtTiempoReposicion.Text);

                oParte.UnidadEmpaque = Helper.ConvertirDecimal(this.txtUnidadEmpaque.Text);
                oParte.AGranel = this.chkAGranel.Checked;
                oParte.EsPar = this.chkEsPar.Checked;

                // Se llenan los datos del precio
                if (oPartePrecio != null)
                {
                    oPartePrecio.Costo = Negocio.Helper.ConvertirDecimal(txtCosto.Text);
                    oPartePrecio.PorcentajeUtilidadUno = Negocio.Helper.ConvertirDecimal(txtPorcentaje1.Text);
                    oPartePrecio.PorcentajeUtilidadDos = Negocio.Helper.ConvertirDecimal(txtPorcentaje2.Text);
                    oPartePrecio.PorcentajeUtilidadTres = Negocio.Helper.ConvertirDecimal(txtPorcentaje3.Text);
                    oPartePrecio.PorcentajeUtilidadCuatro = Negocio.Helper.ConvertirDecimal(txtPorcentaje4.Text);
                    oPartePrecio.PorcentajeUtilidadCinco = Negocio.Helper.ConvertirDecimal(txtPorcentaje5.Text);
                    oPartePrecio.PrecioUno = Negocio.Helper.ConvertirDecimal(txtPrecio1.Text);
                    oPartePrecio.PrecioDos = Negocio.Helper.ConvertirDecimal(txtPrecio2.Text);
                    oPartePrecio.PrecioTres = Negocio.Helper.ConvertirDecimal(txtPrecio3.Text);
                    oPartePrecio.PrecioCuatro = Negocio.Helper.ConvertirDecimal(txtPrecio4.Text);
                    oPartePrecio.PrecioCinco = Negocio.Helper.ConvertirDecimal(txtPrecio5.Text);
                }

                // Se guarda la Parte
                if (this.EsNuevo)
                {
                    Guardar.Parte(this.oParte, oPartePrecio);
                }
                else
                {
                    Guardar.Generico<Parte>(this.oParte);
                    Guardar.Generico<PartePrecio>(oPartePrecio);
                }

                // Se mandan guardar las características
                this.GuardarCaracteristicas();

                // Se guardan las existencias y los datos de máximos y mínimos
                foreach (DataGridViewRow row in dgvExistencias.Rows)
                {
                    var sucursalId = Negocio.Helper.ConvertirEntero(row.Cells["SucursalID"].Value);
                    var parteExistencia = Negocio.General.GetEntity<ParteExistencia>(p => p.ParteID.Equals(oParte.ParteID) && p.SucursalID.Equals(sucursalId));
                    if (parteExistencia != null)
                    {
                        parteExistencia.SucursalID = sucursalId;
                        parteExistencia.Maximo = Negocio.Helper.ConvertirEntero(row.Cells["Max"].Value);
                        parteExistencia.Minimo = Negocio.Helper.ConvertirEntero(row.Cells["Min"].Value);
                        // parteExistencia.UnidadEmpaque = Negocio.Helper.ConvertirEntero(row.Cells["UEmp"].Value);
                        parteExistencia.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                        parteExistencia.FechaModificacion = DateTime.Now;
                        parteExistencia.Estatus = true;
                        parteExistencia.Actualizar = true;
                        General.SaveOrUpdate<ParteExistencia>(parteExistencia, parteExistencia);
                    }

                    // Se guardan los datos de MaxMin, si fueron modificados
                    bool bMaxMinFijo = Helper.ConvertirBool(row.Cells["MaxMinFijo"].Value);
                    if (bMaxMinFijo)
                    {
                        var oParteMaxMin = General.GetEntity<ParteMaxMin>(q => q.ParteID == oParte.ParteID && q.SucursalID == sucursalId);
                        oParteMaxMin.Fijo = bMaxMinFijo;
                        oParteMaxMin.Maximo = Helper.ConvertirEntero(row.Cells["Max"].Value);
                        oParteMaxMin.Minimo = Helper.ConvertirEntero(row.Cells["Min"].Value);
                        Guardar.Generico<ParteMaxMin>(oParteMaxMin);
                    }
                }

                // Se verifica el máximo, para guardar como 9500 o no
                this.oParte.Es9500 = AdmonProc.VerGuardar9500(this.oParte.ParteID);

                // Si es nuevo, se carga la parte, para que ya no sea nuevo
                if (this.EsNuevo)
                    this.CargarParte(this.oParte.ParteID);

                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                new Notificacion("Parte Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                // this.IniciarActualizarListado();
            }
            catch (Exception ex)
            {
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnMigrarCodigos_Click(object sender, EventArgs e)
        {
            if (oParte.ParteID < 0)
                return;
            Migrador detalle = new Migrador(oParte.ParteID);
            detalle.oTipoMigrador = Migrador.MigradorType.Alternos;
            detalle.ShowDialog();
            this.CargarCodigosAlternos(oParte.ParteID);
        }

        private void btnMigrarAplicaciones_Click(object sender, EventArgs e)
        {
            if (oParte.ParteID < 0)
                return;
            Migrador detalle = new Migrador(oParte.ParteID);
            detalle.oTipoMigrador = Migrador.MigradorType.Aplicaciones;
            detalle.ShowDialog();
            this.CargarAplicaciones(oParte.ParteID);
        }

        private void btnAgregarCodigoAlterno_Click(object sender, EventArgs e)
        {
            if (oParte.ParteID < 0)
                return;
            if (UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.CodigosAlternos.Agregar", true))
            {
                DetalleCodigoAlterno detalle = new DetalleCodigoAlterno(oParte.ParteID);
                detalle.ShowDialog();
                this.CargarCodigosAlternos(oParte.ParteID);
            }
        }

        private void btnMigrarEquivalentes_Click(object sender, EventArgs e)
        {
            if (oParte == null || oParte.ParteID < 0)
                return;
            Migrador detalle = new Migrador(oParte.ParteID);
            detalle.oTipoMigrador = Migrador.MigradorType.Equivalentes;
            detalle.ShowDialog();
            this.CargarEquivalencias(oParte.ParteID);
        }

        private void btnAgregarComplementarios_Click(object sender, EventArgs e)
        {
            // Validación
            if (!UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Complementarias.Agregar", true))
                return;

            //
            if (this.oParte.ParteID <= 0)
            {
                UtilLocal.MensajeAdvertencia("Debes seleccionar una Parte.");
                return;
            }

            var frmPartes = new BusquedaPartes();
            frmPartes.HacerMultiple();
            if (frmPartes.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                int iParteID = this.oParte.ParteID;

                foreach (pauParteBusquedaGeneral_Result oReg in frmPartes.ListaSeleccion)
                {
                    int iParteIDComplementaria = oReg.ParteID;
                    Guardar.ParteComplementaria(iParteID, iParteIDComplementaria);
                    if (this.chkComplementariosGrupo.Checked)
                        Guardar.ParteComplementaria(iParteIDComplementaria, iParteID);
                }

                this.CargarComplementarios(iParteID);
            }
            frmPartes.Dispose();
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            var frmImportar = new PartesImportar();
            frmImportar.ShowDialog(Principal.Instance);
            frmImportar.Dispose();
        }

        private void btnAplicacionesCopiarEquiv_Click(object sender, EventArgs e)
        {
            if (this.oParte.ParteID <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ninguna Parte seleccionada");
                return;
            }
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas copiar estas Aplicaciones a las Partes Equivalentes?") == DialogResult.Yes)
            {
                AdmonProc.CopiarAplicacionesDeEquivalentes(this.oParte.ParteID);
                UtilLocal.MostrarNotificacion("Completado correctamente.");
            }
        }

        private void btnAlternosCopiarEquiv_Click(object sender, EventArgs e)
        {
            if (this.oParte.ParteID <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ninguna Parte seleccionada");
                return;
            }
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas copiar estos Códigos Alternos a las Partes Equivalentes?") == DialogResult.Yes)
            {
                AdmonProc.CopiarCodigosAlternosDeEquivalentes(this.oParte.ParteID);
                UtilLocal.MostrarNotificacion("Completado correctamente.");
            }
        }

        private void btnComplementariosCopiarEquiv_Click(object sender, EventArgs e)
        {
            if (this.oParte.ParteID <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ninguna Parte seleccionada");
                return;
            }
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas copiar estas Partes Complementarias a las Partes Equivalentes?") == DialogResult.Yes)
            {
                AdmonProc.CopiarPartesComplementariasDeEquivalentes(this.oParte.ParteID);
                UtilLocal.MostrarNotificacion("Completado correctamente.");
            }
        }

        private void btnEquivalentesCopiarMas_Click(object sender, EventArgs e)
        {
            if (this.oParte.ParteID <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ninguna Parte seleccionada");
                return;
            }

            var frmCopiaMas = new CopiarDeEquivalentes(this.oParte.LineaID);
            frmCopiaMas.ShowDialog(Principal.Instance);
            frmCopiaMas.Dispose();
        }

        private void btnActualizadorDeImagenes_Click(object sender, EventArgs e)
        {
            // Se verifica si existe el actualizador
            if (!File.Exists(Program.NombreActualizador))
            {
                UtilLocal.MensajeAdvertencia("No se encontró el ejecutable del Actualizador: Actualizador.exe");
                return;
            }

            string sRutaActImagenes = Helper.AgregarSeparadorDeCarpeta(Config.Valor("Actualizacion.RutaImagenes"));
            System.Diagnostics.Process.Start(Program.NombreActualizador, string.Format(" -t img -r {0}", sRutaActImagenes));
        }

        private void chkValCaracteristicas_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkValCaracteristicas.Focused && this.oParte != null)
                this.GuardarValidacionParte(this.oParte.ParteID, "Caracteristicas", this.chkValCaracteristicas.Checked);
        }

        private void chkValEquivalentes_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkValEquivalentes.Focused && this.oParte != null)
                this.GuardarValidacionParte(this.oParte.ParteID, "Equivalentes", this.chkValEquivalentes.Checked);
        }

        private void chkValAplicaciones_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkValAplicaciones.Focused && this.oParte != null)
                this.GuardarValidacionParte(this.oParte.ParteID, "Aplicaciones", this.chkValAplicaciones.Checked);
        }

        private void chkValAlternos_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkValAlternos.Focused && this.oParte != null)
                this.GuardarValidacionParte(this.oParte.ParteID, "Alternos", this.chkValAlternos.Checked);
        }

        private void chkValComplementarios_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkValComplementarios.Focused && this.oParte != null)
                this.GuardarValidacionParte(this.oParte.ParteID, "Complementarios", this.chkValComplementarios.Checked);
        }

        private void chkValFotos_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkValFotos.Focused && this.oParte != null)
                this.GuardarValidacionParte(this.oParte.ParteID, "Fotos", this.chkValFotos.Checked);
        }

        #endregion      
        
        #region [metodos]

        public void CargaInicial()
        {
            // Se validan los permisos (con variable "bAbregar", porque como se corre desde el constructor, los controles se comportan raro)
            this.btnAgregarEquivalencia.Visible = UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Equivalentes.Agregar"); ;
            this.btnMigrarEquivalentes.Visible = UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Equivalentes.Migrar");
            this.btnEquivalentesCopiarMas.Visible = this.btnAgregarEquivalencia.Visible;
            this.btnAgregarAplicacion.Visible = UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Aplicaciones.Agregar");
            this.btnMigrarAplicaciones.Visible = UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Aplicaciones.Migrar");
            this.btnAplicacionesCopiarEquiv.Visible = this.btnAgregarAplicacion.Visible;
            this.btnAgregarCodigoAlterno.Visible = UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.CodigosAlternos.Agregar");
            this.btnMigrarCodigos.Visible = UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.CodigosAlternos.Migrar");
            this.btnAlternosCopiarEquiv.Visible = this.btnAgregarCodigoAlterno.Visible;
            this.chkComplementariosGrupo.Visible = UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Complementarias.Agregar"); ;
            this.btnAgregarComplementarios.Visible = true;// this.chkComplementariosGrupo.Visible;
            this.btnComplementariosCopiarEquiv.Visible = this.chkComplementariosGrupo.Visible;

            this.chkValFotos.Visible = UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Validar"); ;
            this.chkValEquivalentes.Visible = this.chkValFotos.Visible;
            this.chkValAplicaciones.Visible = this.chkValFotos.Visible;
            this.chkValAlternos.Visible = this.chkValFotos.Visible;
            this.chkValComplementarios.Visible = this.chkValFotos.Visible;
            this.chkValCaracteristicas.Visible = this.chkValFotos.Visible;

            try
            {
                this.LimpiarFormulario();

                this.txtBusqueda.Clear();
                this.cboFiltro.DataSource = Negocio.General.GetListOf<ParteEstatus>(p => p.Estatus.Equals(true));
                this.cboFiltro.DisplayMember = "NombreParteEstatus";
                this.cboFiltro.ValueMember = "ParteEstatusID";

                var listaProveedor = Negocio.General.GetListOf<Proveedor>(p => p.Estatus.Equals(true)).OrderBy(c => c.NombreProveedor).ToList();
                this.cboProveedor.DataSource = listaProveedor;
                this.cboProveedor.DisplayMember = "NombreProveedor";
                this.cboProveedor.ValueMember = "ProveedorID";
                /* AutoCompleteStringCollection autProveedor = new AutoCompleteStringCollection();
                foreach (var proveedor in listaProveedor) autProveedor.Add(proveedor.NombreProveedor);
                this.cboProveedor.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboProveedor.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboProveedor.AutoCompleteCustomSource = autProveedor;
                this.cboProveedor.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
                */
 
                var listaMarcaParte = Negocio.General.GetListOf<MarcaParte>(m => m.Estatus.Equals(true));
                this.cboMarca.DataSource = listaMarcaParte;
                this.cboMarca.DisplayMember = "NombreMarcaParte";
                this.cboMarca.ValueMember = "MarcaParteID";
                AutoCompleteStringCollection autMarcaParte = new AutoCompleteStringCollection();
                foreach (var marcaParte in listaMarcaParte) autMarcaParte.Add(marcaParte.NombreMarcaParte);
                this.cboMarca.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboMarca.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboMarca.AutoCompleteCustomSource = autMarcaParte;
                this.cboMarca.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);

                var listaMedidas = Negocio.General.GetListOf<Medida>(m => m.Estatus.Equals(true));
                this.cboMedida.DataSource = listaMedidas;
                this.cboMedida.DisplayMember = "NombreMedida";
                this.cboMedida.ValueMember = "MedidaID";
                AutoCompleteStringCollection autMedida = new AutoCompleteStringCollection();
                foreach (var medida in listaMedidas) autMedida.Add(medida.NombreMedida);
                this.cboMedida.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboMedida.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboMedida.AutoCompleteCustomSource = autMedida;
                this.cboMedida.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
                if (this.cboMedida.DataSource != null)
                    this.cboMedida.SelectedValue = Cat.Medidas.Pieza;

                var listaParteSistema = Negocio.General.GetListOf<ParteSistema>(p => p.Estatus.Equals(true));
                this.cboParteSistema.DataSource = listaParteSistema;
                this.cboParteSistema.DisplayMember = "NombreParteSistema";
                this.cboParteSistema.ValueMember = "ParteSistemaID";
                AutoCompleteStringCollection autParteSistema = new AutoCompleteStringCollection();
                foreach (var sistema in listaParteSistema) autParteSistema.Add(sistema.NombreParteSistema);
                this.cboParteSistema.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboParteSistema.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboParteSistema.AutoCompleteCustomSource = autParteSistema;
                this.cboParteSistema.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);

                var listaParteUbicacion = Negocio.General.GetListOf<ParteUbicacion>(p => p.Estatus.Equals(true));
                this.cboParteUbicacion.DataSource = listaParteUbicacion;
                this.cboParteUbicacion.DisplayMember = "NombreParteUbicacion";
                this.cboParteUbicacion.ValueMember = "ParteUbicacionID";
                AutoCompleteStringCollection autParteUbicacion = new AutoCompleteStringCollection();
                foreach (var ubicacion in listaParteUbicacion) autParteUbicacion.Add(ubicacion.NombreParteUbicacion);
                this.cboParteUbicacion.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboParteUbicacion.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboParteUbicacion.AutoCompleteCustomSource = autParteUbicacion;
                this.cboParteUbicacion.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);

                var listaParteEstatus = Negocio.General.GetListOf<ParteEstatus>(p => p.Estatus.Equals(true));
                this.cboEstatus.DataSource = listaParteEstatus;
                this.cboEstatus.DisplayMember = "NombreParteEstatus";
                this.cboEstatus.ValueMember = "ParteEstatusID";
                AutoCompleteStringCollection autParteEstatus = new AutoCompleteStringCollection();
                foreach (var estatus in listaParteEstatus) autParteEstatus.Add(estatus.NombreParteEstatus);
                this.cboEstatus.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboEstatus.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboEstatus.AutoCompleteCustomSource = autParteEstatus;
                this.cboEstatus.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);

                // Control de Cascos
                this.cmbEsCascoPara.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Linea>(c => c.Estatus));

                this.cboTipoGarantia.DataSource = Negocio.General.GetListOf<TipoGarantia>(t => t.Estatus.Equals(true));
                this.cboTipoGarantia.DisplayMember = "NombreTipoGarantia";
                this.cboTipoGarantia.ValueMember = "TipoGarantiaID";
                this.cboTipoGarantia.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
                
                // Referente a Kardex

                // Se llena el combo de sucursales de Kardex
                this.cmbKardexSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
                //
                this.dtpKardexDesde.Value = DateTime.Now.AddMonths(-3);

                // Ya no se carga el listado al cargar el control, se va filtrando según la búsqueda
                // this.IniciarActualizarListado();

                // Más validaciones de permisos, después de haber cargado varios controles 
                if (!UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Parte.Ver"))
                    this.tabPartes.TabPages.Remove(this.tbpParte);
                if (!UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Kardex.Ver"))
                    this.tabPartes.TabPages.Remove(this.tbpKardex);
                if (!UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Avance.Ver"))
                    this.tabPartes.TabPages.Remove(this.tbpAvance);
                if (!UtilDatos.ValidarPermiso("Administracion.CatalogosPartes.Errores.Ver"))
                    this.tabPartes.TabPages.Remove(this.tbpErrores);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void CargarParte(int iParteID)
        {
            // Se limpia el formulario primero
            this.LimpiarFormulario();

            // Se obtiene la parte correspondiente
            this.oParte = Negocio.General.GetEntity<Parte>(c => c.ParteID == iParteID && c.Estatus);
            if (this.oParte == null)
            {
                UtilLocal.MensajeError("Hubo un error al carcar la Parte especificada.");
                return;
            }
            this.EsNuevo = false;

            // Se empiezan a cargar los datos
            this.txtNumeroParte.Text = oParte.NumeroParte;
            this.txtNombreParte.Text = oParte.NombreParte;
            this.cboProveedor.SelectedValue = oParte.ProveedorID;
            var subsitema = Negocio.General.GetEntity<Subsistema>(s => s.SubsistemaID.Equals(oParte.SubsistemaID));

            this.cboMarca.SelectedValue = oParte.MarcaParteID;
            this.cboLinea.SelectedValue = oParte.LineaID;
            // this.cboSistema.SelectedValue = subsitema.SistemaID;
            this.cboSubsistema.SelectedValue = oParte.SubsistemaID;

            this.cboMedida.SelectedValue = oParte.MedidaID;
            this.cboEstatus.SelectedValue = oParte.ParteEstatusID;

            // Control de Cascos
            if (this.oParte.EsCascoPara.HasValue)
                this.cmbEsCascoPara.SelectedValue = this.oParte.EsCascoPara;
            this.cmbRequiereCascoDe.CargarDatos("ParteID", "NumeroParte", General.GetListOf<Parte>(c => c.EsCascoPara == this.oParte.LineaID && c.Estatus));
            if (this.oParte.RequiereCascoDe.HasValue)
                this.cmbRequiereCascoDe.SelectedValue = this.oParte.RequiereCascoDe;
            this.txtRequiereDepositoDe.Tag = this.oParte.RequiereDepositoDe;
            if (this.oParte.RequiereDepositoDe.HasValue)
            {
                var oParteDep = General.GetEntity<Parte>(c => c.ParteID == this.oParte.RequiereDepositoDe.Value && c.Estatus);
                this.txtRequiereDepositoDe.Text = oParteDep.NumeroParte;
            }

            this.chkAplicaComision.Checked = Negocio.Helper.ConvertirBool(oParte.AplicaComision);
            this.chk9500.Checked = Negocio.Helper.ConvertirBool(oParte.Es9500);
            this.chkServicio.Checked = Negocio.Helper.ConvertirBool(oParte.EsServicio);

            this.CargaExistencias(oParte.ParteID);
            this.CargaPrecios(oParte.ParteID);

            this.CargarImagenes(oParte.ParteID);
            this.CargarEquivalencias(oParte.ParteID);
            this.CargarAplicaciones(oParte.ParteID);
            this.CargarCodigosAlternos(oParte.ParteID);
            this.CargarComplementarios(oParte.ParteID);
            this.CargarValidaciones(oParte.ParteID);

            /* this.txtAlto.Text = Parte.Alto.ToString().Equals("0") ? string.Empty : Parte.Alto.ToString();
            this.txtLargo.Text = Parte.Largo.ToString().Equals("0") ? string.Empty : Parte.Largo.ToString();
            this.txtDiametro.Text = Parte.Diametro.ToString().Equals("0") ? string.Empty : Parte.Diametro.ToString();
            this.txtDientes.Text = Parte.Dientes.ToString().Equals("0") ? string.Empty : Parte.Dientes.ToString();
            this.txtAstrias.Text = Parte.Astrias.ToString().Equals("0") ? string.Empty : Parte.Astrias.ToString();

            this.txtAmperes.Text = Parte.Amperes.ToString().Equals("0") ? string.Empty : Parte.Amperes.ToString();
            this.txtVoltios.Text = Parte.Voltios.ToString().Equals("0") ? string.Empty : Parte.Voltios.ToString();
            this.txtWatts.Text = Parte.Watts.ToString().Equals("0") ? string.Empty : Parte.Watts.ToString();
            this.txtTerminales.Text = Parte.Terminales.ToString().Equals("0") ? string.Empty : Parte.Terminales.ToString();
            this.txtLitros.Text = Parte.Litros.ToString().Equals("0") ? string.Empty : Parte.Litros.ToString();
            */

            // Se llenan las características
            this.CargarCaracteristicas();
            //

            if (oParte.ParteSistemaID == null)
                this.cboParteSistema.SelectedValue = 1;
            else
                this.cboParteSistema.SelectedValue = oParte.ParteSistemaID;

            if (oParte.ParteUbicacionID == null)
                this.cboParteUbicacion.SelectedValue = 1;
            else
                this.cboParteUbicacion.SelectedValue = oParte.ParteUbicacionID;

            if (oParte.TipoGarantiaID == null)
                this.cboTipoGarantia.SelectedValue = 1;
            else
                this.cboTipoGarantia.SelectedValue = oParte.TipoGarantiaID;

            if (oParte.TipoCilindroID == null)
                this.cboTipoGarantia.SelectedValue = 1;
            else
                this.cboTipoGarantia.SelectedValue = oParte.TipoGarantiaID;

            this.chkSiEtiqueta.Checked = Negocio.Helper.ConvertirBool(oParte.Etiqueta);
            this.chkSoloUna.Checked = Negocio.Helper.ConvertirBool(oParte.SoloUnaEtiqueta);

            this.dgvEquivalentes.ClearSelection();
            this.dgvAplicaciones.ClearSelection();
            this.dgvExistencias.ClearSelection();
            this.dgvCodigosAlternos.ClearSelection();

            this.dgvEquivalentes.CurrentCell = null;
            this.dgvAplicaciones.CurrentCell = null;
            this.dgvExistencias.CurrentCell = null;
            this.dgvCodigosAlternos.CurrentCell = null;

            // this.btnAgregarAplicacion.Enabled = true;
            // this.btnAgregarEquivalencia.Enabled = true;
            this.btnAgregarImagenes.Enabled = true;

            this.txtABC.Text = Helper.ConvertirCadena(oParte.CriterioABC);
            this.txtCodigoBarra.Text = Helper.ConvertirCadena(oParte.CodigoBarra);
            this.txtTiempoReposicion.Text = Helper.ConvertirCadena(oParte.TiempoReposicion);

            this.txtUnidadEmpaque.Text = oParte.UnidadEmpaque.ToString();
            this.chkAGranel.Checked = oParte.AGranel;
            this.chkEsPar.Checked = oParte.EsPar.Valor();

            // Para mostrar / ocultar etiqueta de no pedidos
            this.lblNoPedidos.Visible = General.Exists<ParteCaracteristicaTemporal>(c => c.ParteID == this.oParte.ParteID
                && c.Caracteristica == Cat.CaracTempPartes.NoPedidosPorEquivalentes);

            // Se llenan los datos de las ventas por mes
            this.ctlVentasPorMes.LlenarDatos(iParteID);
        }

        public void LimpiarFormulario()
        {
            this.cntError.LimpiarErrores();
            this.ctlAdv.LimpiarErrores();
            //Limpiar cajas de texto y grids
            this.txtNumeroParte.Clear();
            this.txtNombreParte.Clear();
            // Se limpia lo de Cascos
            this.cmbEsCascoPara.SelectedIndex = -1;
            this.cmbRequiereCascoDe.DataSource = null;
            this.txtRequiereDepositoDe.Clear();
            this.txtRequiereDepositoDe.Tag = null;
            // Se limpia lo de los costos
            this.txtCosto.Text = "0.00";
            this.txtPrecio1.Text = "0.00";
            this.txtPrecio2.Text = "0.00";
            this.txtPrecio3.Text = "0.00";
            this.txtPrecio4.Text = "0.00";
            this.txtPrecio5.Text = "0.00";
            this.txtPorcentaje1.Text = "0.00";
            this.txtPorcentaje2.Text = "0.00";
            this.txtPorcentaje3.Text = "0.00";
            this.txtPorcentaje4.Text = "0.00";
            this.txtPorcentaje5.Text = "0.00";
            /* this.txtLargo.Clear();
            this.txtDiametro.Clear();
            this.txtAlto.Clear();
            this.txtAstrias.Clear();
            this.txtDientes.Clear();
            this.txtTerminales.Clear();
            this.txtAmperes.Clear();
            this.txtVoltios.Clear();
            this.txtWatts.Clear();
            */
            // Se limpian los textos de características
            this.LimpiarCaracteristicas();
            //
            this.chkAplicaComision.Checked = true;
            this.chk9500.Checked = false;
            this.chkServicio.Checked = false;
            this.chkSiEtiqueta.Checked = true;
            this.chkSoloUna.Enabled = false;
            this.chkSoloUna.Checked = false;
            // this.btnAgregarAplicacion.Enabled = false;
            // this.btnAgregarEquivalencia.Enabled = false;
            this.btnAgregarImagenes.Enabled = false;
            this.ConfigurarGridExistencias();
            this.picBoxImagen.Image = null;
            this.dgvEquivalentes.DataSource = null;
            this.dgvAplicaciones.DataSource = null;
            this.dgvComplementarios.DataSource = null;
            if (this.lstImagenes.Items.Count > 0)
                this.lstImagenes.Items.Clear();
            // this.txtNumeroParte.Focus();

            this.chkValCaracteristicas.Checked = false;
            this.chkValEquivalentes.Checked = false;
            this.chkValAplicaciones.Checked = false;
            this.chkValAlternos.Checked = false;
            this.chkValComplementarios.Checked = false;
            this.chkValFotos.Checked = false;

            this.txtUnidadEmpaque.Text = "1";
            this.chkAGranel.Checked = false;
            this.chkEsPar.Checked = false;

            this.ctlVentasPorMes.LimpiarDatos();
        }

        private void LimpiarCaracteristicas()
        {
            this.flpCaracteristicas.Controls.Clear();
        }
                
        public void IniciarActualizarListado()
        {
            bgworker = new BackgroundWorker();
            bgworker.DoWork += ActualizarListado;
            bgworker.RunWorkerCompleted += Terminado;
            bgworker.RunWorkerAsync();
            progreso.Value = 0;
        }

        public void ActualizarListado(object o, DoWorkEventArgs e)
        {
            try
            {
                int estatusId = GetSelecetedValue();
                var dt = Negocio.Helper.newTable<PartesView>("Partes", Negocio.General.GetListOf<PartesView>(p => p.ParteID > 0 && p.ParteEstatusID.Equals(estatusId)));
                this.IniciarCarga(dt, "Partes");
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private int GetSelecetedValue()
        {
            if (cboFiltro.InvokeRequired)
                return (int)cboFiltro.Invoke(new Func<int>(GetSelecetedValue));
            else
                return (int)cboFiltro.SelectedValue;
        }

        public void Terminado(object o, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.dgvDatos.DataSource = this.fuenteDatos;
                this.dgvDatos.Rows[0].Selected = false;
                progreso.Step = 1;
                for (int i = progreso.Minimum; i < progreso.Maximum; i = i + progreso.Step)
                {
                    progreso.PerformStep();
                }
                Helper.OcultarColumnas(this.dgvDatos, new string[] { "ParteID", "ParteEstatusID", "LineaID", "Busqueda", "EntityState", "EntityKey" });
                Helper.ColumnasToHeaderText(this.dgvDatos);
                this.lblNumeroRegistros.Text = string.Format("Número de registros: {0}", dgvDatos.Rows.Count);
                this.dgvDatos.Columns[2].Width = 400;
                this.txtBusqueda.Focus();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void IniciarCarga(object FuenteDatos, string sTitulo)
        {
            this.fuenteDatos = new BindingSource();
            this.Sel = new Dictionary<string, object>();
            this.fuenteDatos.DataSource = FuenteDatos;
        }

        private void ConfigurarGridExistencias()
        {
            try
            {
                this.dgvExistencias.DataSource = null;
                if (dgvExistencias.Columns.Count > 0)
                    dgvExistencias.Columns.Clear();

                if (dgvExistencias.Rows.Count > 0)
                    dgvExistencias.Rows.Clear();

                /* Creo que esto no es necesario - Moi 2015-08-25
                
                DataTable dt = new DataTable();
                DataRow row;

                var colSucursalId = new DataColumn();
                colSucursalId.DataType = System.Type.GetType("System.Int32");
                colSucursalId.ColumnName = "SucursalID";

                var colSucursal = new DataColumn();
                colSucursal.DataType = System.Type.GetType("System.String");
                colSucursal.ColumnName = "Sucursal";

                var colMaximo = new DataColumn();
                colMaximo.DataType = System.Type.GetType("System.Int32");
                colMaximo.ColumnName = "Max";

                var colMinimo = new DataColumn();
                colMinimo.DataType = System.Type.GetType("System.Int32");
                colMinimo.ColumnName = "Min";

                /* var colUnidadEmpaque = new DataColumn();
                colUnidadEmpaque.DataType = System.Type.GetType("System.Int32");
                colUnidadEmpaque.ColumnName = "UEmp";
                * /

                dt.Columns.AddRange(new DataColumn[] { colSucursalId, colSucursal, colMaximo, colMinimo });
                                
                var sucursales = Negocio.General.GetListOf<Sucursal>(s => s.Estatus.Equals(true));

                foreach (var sucursal in sucursales)
                {
                    row = dt.NewRow();
                    row[0] = sucursal.SucursalID;
                    row[1] = sucursal.NombreSucursal;
                    row[2] = 0;
                    row[3] = 0;
                    //row[4] = 1;
                    dt.Rows.Add(row);
                }
                this.dgvExistencias.DataSource = dt;
                this.dgvExistencias.Columns["SucursalID"].Visible = false;

                // Se agrega columna de MaxMinFijo
                this.dgvExistencias.Columns.Add("MaxMinFijo", "MaxMinFijo");
                this.dgvExistencias.Columns["MaxMinFijo"].Visible = false;
                
                */
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargaExistencias(int parteId)
        {
            try
            {
                this.dgvExistencias.DataSource = null;
                if (dgvExistencias.Columns.Count > 0)
                    dgvExistencias.Columns.Clear();

                if (dgvExistencias.Rows.Count > 0)
                    dgvExistencias.Rows.Clear();

                this.dgvExistencias.DataSource = General.GetListOf<ExistenciasView>(ex => ex.ParteID.Equals(parteId));
                this.dgvExistencias.AutoResizeColumns();
                
                // Se agrega una columna para determinar si se cambió manualmente el valor de Máximo o Mínimo
                this.dgvExistencias.Columns.Add("MaxMinFijo", "MaxMinFijo");

                // Se oculta y ajustan las columnas
                // this.dgvExistencias.OcultarColumnas("ParteExistenciaID", "ParteID", "NumeroParte", "SucursalID", "UEmp", "MaxMinFijo");
                this.dgvExistencias.MostrarColumnas("Tienda", "Exist", "Max", "Min");
                Negocio.Helper.ColumnasToHeaderText(this.dgvExistencias);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargaPrecios(int parteId)
        {
            try
            {
                var precio = Negocio.General.GetEntity<PartePrecio>(p => p.ParteID.Equals(parteId));
                this.txtCosto.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.Costo);
                this.txtPrecio1.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PrecioUno);
                this.txtPrecio2.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PrecioDos);
                this.txtPrecio3.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PrecioTres);
                this.txtPrecio4.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PrecioCuatro);
                this.txtPrecio5.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PrecioCinco);
                this.txtPorcentaje1.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PorcentajeUtilidadUno);
                this.txtPorcentaje2.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PorcentajeUtilidadDos);
                this.txtPorcentaje3.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PorcentajeUtilidadTres);
                this.txtPorcentaje4.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PorcentajeUtilidadCuatro);
                this.txtPorcentaje5.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.PorcentajeUtilidadCinco);
                this.lblCostoConDescuento.Text = Negocio.Helper.DecimalToCadenaMoneda(precio.CostoConDescuento);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarImagenes(int parteId)
        {
            try
            {
                picBoxImagen.Image = null;

                if (lstImagenes.Items.Count > 0)
                    lstImagenes.Items.Clear();
                // var imagenes = Negocio.General.GetListOf<ParteImagen>(p => p.ParteID.Equals(parteId));
                var imagenes = AdmonProc.ObtenerImagenesParte(parteId);
                foreach (var imagen in imagenes)
                {
                    // lstImagenes.Items.Add(imagen.NombreImagen);
                    lstImagenes.Items.Add(Path.GetFileName(imagen));
                }
                if (lstImagenes.Items.Count > 0)
                    lstImagenes.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarEquivalencias(int parteId)
        {
            var oEquivalencias = General.GetListOf<PartesEquivalentesView>(e => e.ParteID.Equals(parteId));
            this.dgvEquivalentes.Rows.Clear();
            foreach (var oReg in oEquivalencias)
                this.dgvEquivalentes.Rows.Add(oReg.ParteEquivalenteID, oReg.ParteIDEquivalente, oReg.NumeroParte, oReg.Descripcion, oReg.CostoConDescuento
                    , oReg.Matriz, oReg.Suc02, oReg.Suc03);
        }

        private void CargarAplicaciones(int parteId)
        {
            try
            {
                this.dgvAplicaciones.DataSource = General.GetListOf<PartesVehiculosView>(e => e.ParteID.Equals(parteId));
                Helper.OcultarColumnas(this.dgvAplicaciones, new string[] { "GenericoID", "ParteID", "MotorID", "ModeloID" });
                Helper.ColumnasToHeaderText(this.dgvAplicaciones);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarCodigosAlternos(int parteId)
        {
            try
            {
                this.dgvCodigosAlternos.DataSource = General.GetListOf<PartesCodigosAlternosView>(e => e.ParteID.Equals(parteId));
                Helper.OcultarColumnas(this.dgvCodigosAlternos, new string[] { "ParteCodigoAlternoID", "ParteID", "MarcaParteID" });
                Helper.ColumnasToHeaderText(this.dgvCodigosAlternos);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarComplementarios(int iParteID)
        {
            this.dgvComplementarios.DataSource = General.GetListOf<PartesComplementariasView>(c => c.ParteID == iParteID);
            this.dgvComplementarios.OcultarColumnas("ParteComplementariaID", "ParteID", "ParteIDComplementaria", "NombreImagen");
        }

        private void CargarValidaciones(int iParteID)
        {
            var oParteVal = General.GetEntity<ParteCompletado>(c => c.ParteID == iParteID);
            bool bVal = (oParteVal != null);
            this.chkValCaracteristicas.Checked = (bVal && oParteVal.Caracteristicas);
            this.chkValEquivalentes.Checked = (bVal && oParteVal.Equivalentes);
            this.chkValAplicaciones.Checked = (bVal && oParteVal.Aplicaciones);
            this.chkValAlternos.Checked = (bVal && oParteVal.Alternos);
            this.chkValComplementarios.Checked = (bVal && oParteVal.Complementarios);
            this.chkValFotos.Checked = (bVal && oParteVal.Fotos);
        }

        public void CalcularPrecios(PreciosCalculadora valores)
        {
            txtCosto.Text = valores.Costo.ToString();
            txtPrecio1.Text = valores.PrecioUno.ToString();
            txtPrecio2.Text = valores.PrecioDos.ToString();
            txtPrecio3.Text = valores.PrecioTres.ToString();
            txtPrecio4.Text = valores.PrecioCuatro.ToString();
            txtPrecio5.Text = valores.PrecioCinco.ToString();
            txtPorcentaje1.Text = valores.GananciaUno.ToString();
            txtPorcentaje2.Text = valores.GananciaDos.ToString();
            txtPorcentaje3.Text = valores.GananciaTres.ToString();
            txtPorcentaje4.Text = valores.GananciaCuatro.ToString();
            txtPorcentaje5.Text = valores.GananciaCinco.ToString();
        }

        public void CargarClasificacionABC(int parteId)
        {
            try
            {
                //var parte = Negocio.General.GetEntity<Parte>(p => p.ParteID.Equals(parteId));
                //if (parte != null)
                //{
                //N	SI EL PRODUCTO TIENE MENOS DE UN AÑO Y SÓLO UNA VENTA.
                //A	SI EL ARTICULO SE HA VENDIDO MÁS DE 20 VECES AL AÑO
                //B	SI EL ARTICULO SE HA VENDIDO MÁS DE 12 Y MENOS DE 20 VECES AL AÑO 
                //C	SI EL ARTICULO SE HA VENDIDO MÁS DE 8 Y MENOS DE 12 VECES AL AÑO 
                //D	SI EL ARTICULO SE HA VENDIDO MÁS DE 1 Y MENOS DE 8 VECES AL AÑO 
                //Z	SI EL ARTICULO NO SE HA VENDIDO EN UN AÑO 
                //}
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarLineas(int marcaId)
        {
            try
            {
                var listaLineas = Negocio.General.GetListOf<LineaMarcaPartesView>(l => l.MarcaParteID.Equals(marcaId));
                cboLinea.DataSource = listaLineas;
                cboLinea.DisplayMember = "NombreLinea";
                cboLinea.ValueMember = "LineaID";
                AutoCompleteStringCollection autLinea = new AutoCompleteStringCollection();
                foreach (var listaLinea in listaLineas) autLinea.Add(listaLinea.NombreLinea);
                cboLinea.AutoCompleteMode = AutoCompleteMode.Suggest;
                cboLinea.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cboLinea.AutoCompleteCustomSource = autLinea;
                cboLinea.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarSistemas(int lineaId)
        {
            try
            {
                var sistemaId = Negocio.General.GetEntity<Linea>(l => l.LineaID.Equals(lineaId)).SistemaID;
                var listaSistemas = Negocio.General.GetListOf<Sistema>(s => s.SistemaID.Equals(sistemaId));
                cboSistema.DataSource = listaSistemas;
                cboSistema.DisplayMember = "NombreSistema";
                cboSistema.ValueMember = "SistemaID";

                AutoCompleteStringCollection autSistema = new AutoCompleteStringCollection();
                foreach (var listaSistema in listaSistemas) autSistema.Add(listaSistema.NombreSistema);
                cboSistema.AutoCompleteMode = AutoCompleteMode.Suggest;
                cboSistema.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cboSistema.AutoCompleteCustomSource = autSistema;
                cboSistema.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarSubsistemas(int sistemaId)
        {
            try
            {
                var listaSubsistemas = Negocio.General.GetListOf<Subsistema>(s => s.SistemaID.Equals(sistemaId));
                cboSubsistema.DataSource = listaSubsistemas;
                cboSubsistema.DisplayMember = "NombreSubsistema";
                cboSubsistema.ValueMember = "SubsistemaID";
                AutoCompleteStringCollection autSubsistema = new AutoCompleteStringCollection();
                foreach (var listaSubsistema in listaSubsistemas) autSubsistema.Add(listaSubsistema.NombreSubsistema);
                cboSubsistema.AutoCompleteMode = AutoCompleteMode.Suggest;
                cboSubsistema.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cboSubsistema.AutoCompleteCustomSource = autSubsistema;
                cboSubsistema.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarSubsistemasLinea(int lineaId)
        {
            try
            {
                var linea = General.GetEntity<Linea>(l => l.LineaID == lineaId);
                var listaSubsistemas = Negocio.General.GetListOf<Subsistema>(s => s.SubsistemaID == linea.SubsistemaID);
                cboSubsistema.DataSource = listaSubsistemas;
                cboSubsistema.DisplayMember = "NombreSubsistema";
                cboSubsistema.ValueMember = "SubsistemaID";
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarMachote(int lineaId)
        {
            try
            {
                var linea = Negocio.General.GetEntity<Linea>(l => l.LineaID.Equals(lineaId));
                this.lblMachote.Text = linea.Machote;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void SugerirDescripcionDeParte()
        {
            try
            {
                if (EsNuevo)
                {
                    var cadena = new StringBuilder();

                    if (Negocio.Helper.ConvertirEntero(cboLinea.SelectedValue) > 0)
                    {
                        var sel = Negocio.Helper.ConvertirEntero(cboLinea.SelectedValue);
                        var abreviacionLinea = Negocio.General.GetEntity<Linea>(l => l.LineaID.Equals(sel));
                        if (abreviacionLinea != null)
                            if (!string.IsNullOrEmpty(abreviacionLinea.Abreviacion))
                                cadena.Append(string.Format("{0} ", abreviacionLinea.Abreviacion));
                    }

                    if (Negocio.Helper.ConvertirEntero(cboMarca.SelectedValue) > 0)
                    {
                        var sel = Negocio.Helper.ConvertirEntero(cboMarca.SelectedValue);
                        var abreviacionMarca = Negocio.General.GetEntity<MarcaParte>(m => m.MarcaParteID.Equals(sel));
                        if (abreviacionMarca != null)
                            if (!string.IsNullOrEmpty(abreviacionMarca.Abreviacion))
                                cadena.Append(string.Format("{0} ", abreviacionMarca.Abreviacion));
                    }

                    if (!string.IsNullOrEmpty(txtNumeroParte.Text))
                        cadena.Append(string.Format("{0} ", txtNumeroParte.Text));

                    txtNombreParte.Text = cadena.ToString();
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private decimal OperacionPrecio(decimal valor)
        {
            var resultado = Negocio.Helper.ConvertirDecimal(this.txtCosto.Text) * valor;
            return resultado;
        }

        private bool Validaciones()
        {
            //var item = Negocio.General.GetEntity<Linea>(m => m.NombreLinea.Equals(txtNombreLinea.Text));
            //if (EsNuevo.Equals(true) && item != null)
            //{
            //    Negocio.Helper.MensajeError("Ya existe una Linea con ese nombre, intente con otro.", GlobalClass.NombreApp);
            //    return false;
            //}
            //else if ((EsNuevo.Equals(false) && item != null) && item.LineaID != Linea.LineaID)
            //{
            //    Negocio.Helper.MensajeError("Ya existe una Linea con ese nombre, intente con otro.", GlobalClass.NombreApp);
            //    return false;
            //}

            this.cntError.LimpiarErrores();
            if (this.txtNumeroParte.Text == "")
                this.cntError.PonerError(this.txtNumeroParte, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtNombreParte.Text == "")
                this.cntError.PonerError(this.txtNombreParte, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Negocio.Helper.ConvertirEntero(this.cboMarca.SelectedValue) < 1)
                this.cntError.PonerError(this.cboMarca, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Negocio.Helper.ConvertirEntero(this.cboLinea.SelectedValue) < 1)
                this.cntError.PonerError(this.cboLinea, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Negocio.Helper.ConvertirEntero(this.cboSistema.SelectedValue) < 1)
                this.cntError.PonerError(this.cboSistema, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Negocio.Helper.ConvertirEntero(this.cboSubsistema.SelectedValue) < 1)
                this.cntError.PonerError(this.cboSubsistema, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Negocio.Helper.ConvertirEntero(this.cboMedida.SelectedValue) < 1)
                this.cntError.PonerError(this.cboMedida, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Negocio.Helper.ConvertirEntero(this.cboEstatus.SelectedValue) < 1)
                this.cntError.PonerError(this.cboEstatus, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Negocio.Helper.ConvertirEntero(this.cboProveedor.SelectedValue) < 1)
                this.cntError.PonerError(this.cboProveedor, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtCosto.Text == "")
                this.cntError.PonerError(this.txtCosto, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            // Se valida la relación Proveedor - Marca - Línea
            int iProveedorID = Helper.ConvertirEntero(this.cboProveedor.SelectedValue);
            int iMarcaID = Helper.ConvertirEntero(this.cboMarca.SelectedValue);
            int iLineaID = Helper.ConvertirEntero(this.cboLinea.SelectedValue);
            if (!General.Exists<ProveedorMarcaParte>(q => q.ProveedorID == iProveedorID && q.MarcaParteID == iMarcaID && q.Estatus)
                || !General.Exists<LineaMarcaParte>(q => q.MarcaParteID == iMarcaID && q.LineaID == iLineaID && q.Estatus))
            {
                this.cntError.PonerError(this.cboProveedor, "La relación Proveedor - Marca - Línea es inválida.");
            }

            // Se valida el estatus, si se intenta dar de baja pero aún hay existencia
            if (this.oParte != null && Helper.ConvertirEntero(this.cboEstatus.SelectedValue) == Cat.PartesEstatus.Inactivo)
            {
                if (General.Exists<ParteExistencia>(c => c.ParteID == this.oParte.ParteID && c.Existencia > 0 && c.Estatus))
                    this.cntError.PonerError(this.cboEstatus, "No se puede marcar como Inactivo porque tiene existencias.");
            }

            //Validaciones para las caracteristicas segun la linea
            //var lineaId = Negocio.Helper.ConvertirEntero(this.cboLinea.SelectedValue);
            //var linea = Negocio.General.GetEntity<Linea>(l => l.LineaID.Equals(lineaId));
            //if (linea != null)
            //{
            //    if (linea.Alto.Equals(true) && this.txtAlto.Text == "")
            //        this.cntError.PonerError(this.txtAlto, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Diametro.Equals(true) && this.txtDiametro.Text == "")
            //        this.cntError.PonerError(this.txtDiametro, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Largo.Equals(true) && this.txtLargo.Text == "")
            //        this.cntError.PonerError(this.txtLargo, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Dientes.Equals(true) && this.txtDientes.Text == "")
            //        this.cntError.PonerError(this.txtDientes, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Astrias.Equals(true) && this.txtAstrias.Text == "")
            //        this.cntError.PonerError(this.txtAstrias, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Sistema.Equals(true) && Negocio.Helper.ConvertirEntero(this.cboParteSistema.SelectedValue).Equals(1))
            //        this.cntError.PonerError(this.cboParteSistema, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    //if (linea.Capacidad.Equals(true) && this.txtCapacidad.Text == "")
            //    //    this.cntError.PonerError(this.txtCapacidad, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Amperaje.Equals(true) && this.txtAmperes.Text == "")
            //        this.cntError.PonerError(this.txtAmperes, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Voltaje.Equals(true) && this.txtVoltios.Text == "")
            //        this.cntError.PonerError(this.txtVoltios, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Watts.Equals(true) && this.txtWatts.Text == "")
            //        this.cntError.PonerError(this.txtWatts, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Ubicacion.Equals(true) && Negocio.Helper.ConvertirEntero(this.cboParteUbicacion.SelectedValue).Equals(1))
            //        this.cntError.PonerError(this.cboParteUbicacion, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //    if (linea.Terminales.Equals(true) && this.txtTerminales.Text == "")
            //        this.cntError.PonerError(this.txtTerminales, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //}

            return (this.cntError.NumeroDeErrores == 0);
        }

        private void CargarCaracteristicas()
        {
            var oLineaCarV = General.GetListOf<LineasCaracteristicasView>(c => c.LineaID == this.oParte.LineaID);
            var oParteCars = General.GetListOf<ParteCaracteristica>(c => c.ParteID == this.oParte.ParteID);
            this.LimpiarCaracteristicas();
            foreach (var oReg in oLineaCarV)
            {
                // Se agrega un contenedor para la etiqueta y el control
                var oPanel = new Panel() { Width = 201, Height = 20 };
                this.flpCaracteristicas.Controls.Add(oPanel);
                // Se agrega la etiqueta
                oPanel.Controls.Add(new Label() { Text = oReg.Caracteristica, TextAlign = ContentAlignment.MiddleLeft, Width = 60 });
                // Se agrega el control
                if (oReg.Multiple.Valor())
                {
                    var oCombo = new ComboMultiSel() { Left = 61, Width = 140 };
                    oCombo.Items.AddRange(oReg.MultipleOpciones.Split(','));
                    oPanel.Controls.Add(oCombo);
                    // oCombo.Width = 140;
                }
                else
                {
                    oPanel.Controls.Add(new TextBox() { Left = 61, Width = 140 });
                }
                // Se llena el control
                var oParteCar = oParteCars.FirstOrDefault(c => c.CaracteristicaID == oReg.CaracteristicaID);
                oPanel.Controls[1].Text = (oParteCar == null ? "" : oParteCar.Valor);
                oPanel.Controls[1].Tag = oReg.CaracteristicaID;
            }
            
        }

        private void GuardarCaracteristicas()
        {
            foreach (Control oCaract in this.flpCaracteristicas.Controls)
            {
                var oControl = oCaract.Controls[1];
                if (oControl.Tag == null) continue;
                int iCaracteristicaID = Helper.ConvertirEntero(oControl.Tag);
                var oParteCar = General.GetEntity<ParteCaracteristica>(c => c.ParteID == this.oParte.ParteID && c.CaracteristicaID == iCaracteristicaID);
                if (oParteCar == null)
                    oParteCar = new ParteCaracteristica() { ParteID = this.oParte.ParteID, CaracteristicaID = iCaracteristicaID };
                if (oControl is ComboMultiSel)
                    oParteCar.Valor = string.Join(",", (oControl as ComboMultiSel).CheckedItems);
                else
                    oParteCar.Valor = oControl.Text;
                Guardar.Generico<ParteCaracteristica>(oParteCar);
            }
        }

        private void BusquedaVerRetraso()
        {
            System.Threading.Thread.Sleep(catalogosPartes.BusquedaRetrasoTecla);
            if (++this.BusquedaIntento == this.BusquedaLlamada)
                this.Invoke(new Action(this.BusquedaParte));
        }

        private void BusquedaParte()
        {
            // Experimental. Se restauran los valores de variables para retraso en búsqueda por descripción
            this.BusquedaLlamada = this.BusquedaIntento = 0;
            //

            // Si es filtro avanzado
            if (this.txtBusqueda.Text == "")
            {
                this.dgvDatos.Rows.Clear();
            }
            else
            {
                var oPalabras = this.txtBusqueda.Text.Split(' ');
                var oFiltros = new Dictionary<string, object>();
                for (int iCont = 0; iCont < oPalabras.Length && iCont < 9; iCont++)
                    oFiltros.Add(string.Format("Descripcion{0}", iCont + 1), oPalabras[iCont]);

                var oDatos = General.ExecuteProcedure<pauParteBusquedaGeneral_Result>("pauParteBusquedaGeneral", oFiltros);
                this.dgvDatos.Rows.Clear();
                foreach (var oReg in oDatos)
                {
                    int iFila = this.dgvDatos.Rows.Add(oReg.ParteID, oReg.NumeroDeParte, oReg.Descripcion, oReg.Proveedor, oReg.Marca, oReg.Linea, oReg.FechaRegistro);
                    if (oReg.ParteEstatusID == Cat.PartesEstatus.Inactivo)
                        this.dgvDatos.Rows[iFila].DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
            this.lblEncontrados.Text = string.Format("Encontrados: {0}", this.dgvDatos.Rows.Count.ToString(GlobalClass.FormatoEntero));
        }

        private void LlenarDescripcionMaxMin(DataGridViewRow oFila)
        {
            this.txtDescripcionMaxMin.Clear();
            if (oFila == null)
                return;
            this.txtDescripcionMaxMin.Text = string.Format("Condición: {0} Procesado: {1}\r\n{2}", Helper.ConvertirEntero(oFila.Cells["ParteMaxMinReglaID"].Value)
                , Helper.ConvertirFechaHora(oFila.Cells["FechaMaxMin"].Value), Helper.ConvertirCadena(oFila.Cells["DescripcionMaxMin"].Value));
        }

        #endregion
        
        #region [ Kardex ]

        #region [ Eventos ]

        private void cmbKardexSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbKardexSucursal.Focused)
                this.CargarKardex(this.oParte.ParteID);
        }

        private void dtpKardexDesde_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpKardexDesde.Focused)
                this.CargarKardex(this.oParte.ParteID);
        }

        private void dtpKardexHasta_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpKardexHasta.Focused)
                this.CargarKardex(this.oParte.ParteID);
        }

        private void btnDiferenciasExistencia_Click(object sender, EventArgs e)
        {
            this.MostrarDiferenciasExistencia();
        }

        private void txtKardexBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (this.txtKardexBusqueda.TextLength > 0)
                this.dgvKardex.FiltrarContiene(this.txtKardexBusqueda.Text, "Kardex_Fecha", "Kardex_Folio", "Kardex_Tipo", "Kardex_Operacion", "Kardex_Entidad"
                    , "Kardex_Usuario", "Kardex_Origen", "Kardex_Destino", "Kardex_Importe", "Kardex_Cantidad", "Kardex_ExistenciaNueva");
            else
                this.dgvKardex.QuitarFiltro();
        }

        private void dgvKardex_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvAplicaciones.CurrentRow != null && this.dgvKardex.VerSeleccionNueva())
            {
                int iKardexID = Helper.ConvertirEntero(this.dgvKardex.CurrentRow.Cells["Kardex_ParteKardexID"].Value);
                this.LlenarObservacionKardex(iKardexID);
            }
        }

        #endregion

        #region [ Métodos ]

        private void CargarKardex(int iParteID)
        {
            if (iParteID <= 0) return;
            if (iParteID == Helper.ConvertirEntero(this.tbpKardex.Tag)) return;

            Cargando.Mostrar();

            var oParams = new Dictionary<string, object>();
            oParams.Add("ParteID", iParteID);
            oParams.Add("Desde", this.dtpKardexDesde.Value);
            oParams.Add("Hasta", this.dtpKardexHasta.Value);
            int iSucursalID = Helper.ConvertirEntero(this.cmbKardexSucursal.SelectedValue);
            oParams.Add("SucursalID", (iSucursalID > 0 ? (int?)iSucursalID : null));
            // oParams.Add("SucursalID", "1,2,3");

            var oKardex = General.ExecuteProcedure<pauParteKardex_Result>("pauParteKardex", oParams);
            var oKardexOp = oKardex.GroupBy(c => c.Operacion).Select(c => new { Operacion = c.Key, Cantidad = c.Sum(s => s.Cantidad), Importe = c.Sum(s => s.Importe) });

            // Se llenan las operaciones
            this.dgvKardexMovs.Rows.Clear();
            foreach (var oReg in oKardexOp)
                this.dgvKardexMovs.Rows.Add(oReg.Operacion, oReg.Cantidad, oReg.Importe);
            // Se llena el kardex en sí
            this.dgvKardex.Rows.Clear();
            foreach (var oReg in oKardex)
            {
                this.dgvKardex.Rows.Add(oReg.ParteKardexID, oReg.Fecha, oReg.Folio, oReg.Tipo, oReg.Operacion, oReg.Entidad, oReg.Usuario, oReg.Origen, oReg.Destino
                    , oReg.Importe, oReg.Cantidad, oReg.ExistenciaNueva);
            }
            // Se llena el grid de existencias
            this.ctlExistencias.CargarDatos(this.oParte.ParteID);

            Cargando.Cerrar();
        }

        private void MostrarDiferenciasExistencia()
        {
            Cargando.Mostrar();

            var oDatos = General.GetListOf<PartesDiferenciasEnExistenciaView>();
            if (oDatos.Count <= 0)
            {
                Cargando.Cerrar();
                UtilLocal.MensajeInformacion("No se encontró ningún artículo con diferencia :D");
                return;
            }

            var oGrid = new DataGridView();
            oGrid.AgregarColumnaCadena("NumeroDeParte", "No. Parte", 100);
            oGrid.AgregarColumnaCadena("Descripcion", "Descripción", 100);
            oGrid.AgregarColumnaCadena("Sucursal", "Sucursal", 100);
            oGrid.AgregarColumnaDecimal("ExistenciaKardex", "Existencia Kárdex");
            oGrid.AgregarColumnaDecimal("Existencia", "Existencia");
            oGrid.AgregarColumnaDecimal("Diferencia", "Diferencia");
            foreach (var oReg in oDatos)
                oGrid.Rows.Add(oReg.NumeroDeParte, oReg.Descripcion, oReg.Sucursal, oReg.ExistenciaKardex, oReg.Existencia, oReg.Diferencia);

            UtilLocal.AbrirEnExcel(oGrid);

            Cargando.Cerrar();
        }

        private void LlenarObservacionKardex(int iKardexID)
        {
            var oKardex = General.GetEntity<ParteKardex>(c => c.ParteKardexID == iKardexID);
            switch (oKardex.RelacionTabla)
            {
                case Cat.Tablas.MovimientoInventario:
                    var oMov = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == oKardex.RelacionID && c.Estatus);
                    this.txtKardexObservacion.Text = oMov.Observacion;
                    break;
                case Cat.Tablas.VentaDevolucion:
                    var oDev = General.GetEntity<VentaDevolucion>(c => c.VentaDevolucionID == oKardex.RelacionID && c.Estatus);
                    this.txtKardexObservacion.Text = oDev.Observacion;
                    break;
            }
        }

        #endregion

        #region [ Públicos ]

        public void VerKardex(int iParteID)
        {
            this.txtBusqueda.Clear();
            this.dgvDatos.Rows.Clear();
            this.CargarParte(iParteID);
            this.CargarKardex(iParteID);
            this.tabPartes.SelectedTab = this.tbpKardex;
        }

        #endregion

        #endregion

        #region [ Avance ]

        #region [ Eventos ]

        private void dgvAvProveedores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvAvProveedores.CurrentCell == null) return;
            this.FiltrarPorProveedor(this.dgvAvProveedores.CurrentCell);
        }

        private void dgvAvMarcas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvAvMarcas.CurrentCell == null) return;
            this.FiltrarPorMarca(this.dgvAvMarcas.CurrentCell);
        }

        private void dgvAvLineas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvAvLineas.CurrentCell == null) return;
            this.FiltrarPorLinea(this.dgvAvLineas.CurrentCell);
        }

        private void btnAvActualizar_Click(object sender, EventArgs e)
        {
            this.LlenarAvances();
        }

        private void btnAvQuitarFiltro_Click(object sender, EventArgs e)
        {
            this.ActivarFiltroAvance(false);
        }

        #endregion

        #region [ Métodos ]

        private void LlenarDatosDeFotos(List<PartesAvancesView> oDatos)
        {
            // Se obtiene el número de imágenes que tiene cada parte
            var oCantImagenes = AdmonProc.ObtenerCantidadDeImagenesPorParte();
            // Se llenan los datos de las fotos
            foreach (var oReg in oDatos)
            {
                if (oCantImagenes.ContainsKey(oReg.ParteID))
                    oReg.Fotos = 1; // oCantImagenes[oReg.ParteID];
            }
        }

        private void LlenarAvances()
        {
            Cargando.Mostrar();

            // Se llenan los totales
            var oDatos = General.GetListOf<PartesAvancesView>();
            // Se llenan los datos de las fotos
            this.LlenarDatosDeFotos(oDatos);

            // Se empiezan a llenar los datos
            this.lblAvFotos.Text =
                (oDatos.Count(c => c.Fotos.HasValue && c.Fotos.Value > 0) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje)
                + " / " + (oDatos.Count(c => c.FotosVal == true) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje);
            this.lblAvEquivalentes.Text =
                (oDatos.Count(c => c.Equivalentes.HasValue && c.Equivalentes.Value > 0) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje)
                + " / " + (oDatos.Count(c => c.EquivalentesVal == true) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje);
            this.lblAvAplicaciones.Text =
                (oDatos.Count(c => c.Aplicaciones.HasValue && c.Aplicaciones.Value > 0) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje)
                + " / " + (oDatos.Count(c => c.AplicacionesVal == true) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje); ;
            this.lblAvAlternos.Text =
                (oDatos.Count(c => c.Alternos.HasValue && c.Alternos.Value > 0) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje)
                + " / " + (oDatos.Count(c => c.AlternosVal == true) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje);
            this.lblAvComplementarios.Text =
                (oDatos.Count(c => c.Complementarios.HasValue && c.Complementarios.Value > 0) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje)
                + " / " + (oDatos.Count(c => c.ComplementariosVal == true) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje);
            this.lblAvCaracteristicas.Text =
                (oDatos.Count(c => c.Caracteristicas.HasValue && c.Caracteristicas.Value > 0) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje)
                + " / " + (oDatos.Count(c => c.CaracteristicasVal == true) / (decimal)oDatos.Count).ToString(GlobalClass.FormatoPorcentaje);
            this.lblAvValidadosNum.Text = oDatos.Count(c => c.Validado == true).ToString(GlobalClass.FormatoEntero);

            // Por Proveedor
            var oPorProv = oDatos.GroupBy(c => new { c.ProveedorID, c.Proveedor }).Select(c => new
            {
                c.Key.ProveedorID,
                c.Key.Proveedor,
                Fotos = (c.Sum(s => s.Fotos) / (decimal)c.Count()) * 100,
                Equivalentes = (c.Sum(s => s.Equivalentes) / (decimal)c.Count()) * 100,
                Aplicaciones = (c.Sum(s => s.Aplicaciones) / (decimal)c.Count()) * 100,
                Alternos = (c.Sum(s => s.Alternos) / (decimal)c.Count()) * 100,
                Complementarios = (c.Sum(s => s.Complementarios) / (decimal)c.Count()) * 100,
                Caracteristicas = (c.Sum(s => s.Caracteristicas) / (decimal)c.Count()) * 100,
                FotosVal = (c.Count(s => s.FotosVal == true) / (decimal)c.Count()) * 100,
                EquivalentesVal = (c.Count(s => s.EquivalentesVal == true) / (decimal)c.Count()) * 100,
                AplicacionesVal = (c.Count(s => s.AplicacionesVal == true) / (decimal)c.Count()) * 100,
                AlternosVal = (c.Count(s => s.AlternosVal == true) / (decimal)c.Count()) * 100,
                ComplementariosVal = (c.Count(s => s.ComplementariosVal == true) / (decimal)c.Count()) * 100,
                CaracteristicasVal = (c.Count(s => s.CaracteristicasVal == true) / (decimal)c.Count()) * 100,
                ValidadosNum = c.Count(s => s.Validado == true)
            });
            this.dgvAvProveedores.Rows.Clear();
            foreach (var oReg in oPorProv)
                this.dgvAvProveedores.Rows.Add(oReg.ProveedorID, oReg.Proveedor, oReg.Fotos, oReg.FotosVal, oReg.Equivalentes, oReg.EquivalentesVal
                    , oReg.Aplicaciones, oReg.AplicacionesVal, oReg.Alternos, oReg.AlternosVal, oReg.Complementarios, oReg.ComplementariosVal
                    , oReg.Caracteristicas, oReg.CaracteristicasVal, oReg.ValidadosNum);

            // Por Marca
            var oPorMarca = oDatos.GroupBy(c => new { c.MarcaParteID, c.Marca }).Select(c => new
            {
                c.Key.MarcaParteID,
                c.Key.Marca,
                Fotos = (c.Sum(s => s.Fotos) / (decimal)c.Count()) * 100,
                Equivalentes = (c.Sum(s => s.Equivalentes) / (decimal)c.Count()) * 100,
                Aplicaciones = (c.Sum(s => s.Aplicaciones) / (decimal)c.Count()) * 100,
                Alternos = (c.Sum(s => s.Alternos) / (decimal)c.Count()) * 100,
                Complementarios = (c.Sum(s => s.Complementarios) / (decimal)c.Count()) * 100,
                Caracteristicas = (c.Sum(s => s.Caracteristicas) / (decimal)c.Count()) * 100,
                FotosVal = (c.Count(s => s.FotosVal == true) / (decimal)c.Count()) * 100,
                EquivalentesVal = (c.Count(s => s.EquivalentesVal == true) / (decimal)c.Count()) * 100,
                AplicacionesVal = (c.Count(s => s.AplicacionesVal == true) / (decimal)c.Count()) * 100,
                AlternosVal = (c.Count(s => s.AlternosVal == true) / (decimal)c.Count()) * 100,
                ComplementariosVal = (c.Count(s => s.ComplementariosVal == true) / (decimal)c.Count()) * 100,
                CaracteristicasVal = (c.Count(s => s.CaracteristicasVal == true) / (decimal)c.Count()) * 100,
                ValidadosNum = c.Count(s => s.Validado == true)
            });
            this.dgvAvMarcas.Rows.Clear();
            foreach (var oReg in oPorMarca)
                this.dgvAvMarcas.Rows.Add(oReg.MarcaParteID, oReg.Marca, oReg.Fotos, oReg.FotosVal, oReg.Equivalentes, oReg.EquivalentesVal
                    , oReg.Aplicaciones, oReg.AplicacionesVal, oReg.Alternos, oReg.AlternosVal, oReg.Complementarios, oReg.ComplementariosVal
                    , oReg.Caracteristicas, oReg.CaracteristicasVal, oReg.ValidadosNum);

            // Por Línea
            var oPorLinea = oDatos.GroupBy(c => new { c.LineaID, c.Linea }).Select(c => new
            {
                c.Key.LineaID,
                c.Key.Linea,
                Fotos = (c.Sum(s => s.Fotos) / (decimal)c.Count()) * 100,
                Equivalentes = (c.Sum(s => s.Equivalentes) / (decimal)c.Count()) * 100,
                Aplicaciones = (c.Sum(s => s.Aplicaciones) / (decimal)c.Count()) * 100,
                Alternos = (c.Sum(s => s.Alternos) / (decimal)c.Count()) * 100,
                Complementarios = (c.Sum(s => s.Complementarios) / (decimal)c.Count()) * 100,
                Caracteristicas = (c.Sum(s => s.Caracteristicas) / (decimal)c.Count()) * 100,
                FotosVal = (c.Count(s => s.FotosVal == true) / (decimal)c.Count()) * 100,
                EquivalentesVal = (c.Count(s => s.EquivalentesVal == true) / (decimal)c.Count()) * 100,
                AplicacionesVal = (c.Count(s => s.AplicacionesVal == true) / (decimal)c.Count()) * 100,
                AlternosVal = (c.Count(s => s.AlternosVal == true) / (decimal)c.Count()) * 100,
                ComplementariosVal = (c.Count(s => s.ComplementariosVal == true) / (decimal)c.Count()) * 100,
                CaracteristicasVal = (c.Count(s => s.CaracteristicasVal == true) / (decimal)c.Count()) * 100,
                ValidadosNum = c.Count(s => s.Validado == true)
            });
            this.dgvAvLineas.Rows.Clear();
            foreach (var oReg in oPorLinea)
                this.dgvAvLineas.Rows.Add(oReg.LineaID, oReg.Linea, oReg.Fotos, oReg.FotosVal, oReg.Equivalentes, oReg.EquivalentesVal
                    , oReg.Aplicaciones, oReg.AplicacionesVal, oReg.Alternos, oReg.AlternosVal, oReg.Complementarios, oReg.ComplementariosVal
                    , oReg.Caracteristicas, oReg.CaracteristicasVal, oReg.ValidadosNum);

            Cargando.Cerrar();
        }

        private void ActivarFiltroAvance(bool bActivar)
        {
            this.txtBusqueda.Enabled = !bActivar;
            this.btnAgregar.Enabled = !bActivar;
            this.btnModificar.Enabled = !bActivar;
            this.cboFiltro.Enabled = !bActivar;
            this.btnAvQuitarFiltro.Enabled = bActivar;

            // Se restaura filtro del grid
            if (!bActivar)
            {
                this.dgvDatos.Rows.Clear();
                this.lblEncontrados.Text = "";
            }
        }

        private void FiltrarPorProveedor(DataGridViewCell oCelda)
        {
            string sCol = oCelda.OwningColumn.Name;
            if (sCol == "proProveedor") return;

            // Se deshabilitan los campos de búsqueda
            this.ActivarFiltroAvance(true);

            Cargando.Mostrar();

            int iProveedorID = Helper.ConvertirEntero(oCelda.OwningRow.Cells["proProveedorID"].Value);
            List<PartesAvancesView> oDatos = new List<PartesAvancesView>();
            switch (sCol)
            {
                case "proFotos":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID);
                    // Se llenan los datos de las fotos
                    this.LlenarDatosDeFotos(oDatos);
                    oDatos = oDatos.Where(c => !c.Fotos.HasValue || c.Fotos.Value < 1).ToList();
                    // oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.Fotos.HasValue || c.Fotos.Value < 1));
                    break;
                case "proFotosVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.FotosVal.HasValue || !c.FotosVal.Value));
                    break;
                case "proEquivalentes":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.Equivalentes.HasValue || c.Equivalentes.Value < 1));
                    break;
                case "proEquivalentesVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.EquivalentesVal.HasValue || !c.EquivalentesVal.Value));
                    break;
                case "proAplicaciones":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.Aplicaciones.HasValue || c.Aplicaciones.Value < 1));
                    break;
                case "proAplicacionesVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.AplicacionesVal.HasValue || !c.AplicacionesVal.Value));
                    break;
                case "proAlternos":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.Alternos.HasValue || c.Alternos.Value < 1));
                    break;
                case "proAlternosVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.AlternosVal.HasValue || !c.AlternosVal.Value));
                    break;
                case "proComplementarios":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.Complementarios.HasValue || c.Complementarios.Value < 1));
                    break;
                case "proComplementariosVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.ComplementariosVal.HasValue || !c.ComplementariosVal.Value));
                    break;
                case "proCaracteristicas":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.Caracteristicas.HasValue || c.Caracteristicas.Value < 1));
                    break;
                case "proCaracteristicasVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.ProveedorID == iProveedorID && (!c.CaracteristicasVal.HasValue || !c.CaracteristicasVal.Value));
                    break;
            }
            
            // Se llena el grid de partes
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvDatos.Rows.Add(oReg.ParteID, oReg.NumeroDeParte, oReg.Descripcion, oReg.Proveedor, oReg.Marca, oReg.Linea, oReg.FechaRegistro);
            this.lblEncontrados.Text = string.Format("Encontrados: {0}", this.dgvDatos.Rows.Count.ToString(GlobalClass.FormatoEntero));

            Cargando.Cerrar();
        }

        private void FiltrarPorMarca(DataGridViewCell oCelda)
        {
            string sCol = oCelda.OwningColumn.Name;
            if (sCol == "marMarca") return;

            // Se deshabilitan los campos de búsqueda
            this.ActivarFiltroAvance(true);

            Cargando.Mostrar();

            int iMarcaID = Helper.ConvertirEntero(oCelda.OwningRow.Cells["marMarcaID"].Value);
            List<PartesAvancesView> oDatos = new List<PartesAvancesView>();
            switch (sCol)
            {
                case "marFotos":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID);
                    // Se llenan los datos de las fotos
                    this.LlenarDatosDeFotos(oDatos);
                    oDatos = oDatos.Where(c => !c.Fotos.HasValue || c.Fotos.Value < 1).ToList();
                    // oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.Fotos.HasValue || c.Fotos.Value < 1));
                    break;
                case "marFotosVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.FotosVal.HasValue || !c.FotosVal.Value));
                    break;
                case "marEquivalentes":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.Equivalentes.HasValue || c.Equivalentes.Value < 1));
                    break;
                case "marEquivalentesVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.EquivalentesVal.HasValue || !c.EquivalentesVal.Value));
                    break;
                case "marAplicaciones":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.Aplicaciones.HasValue || c.Aplicaciones.Value < 1));
                    break;
                case "marAplicacionesVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.AplicacionesVal.HasValue || !c.AplicacionesVal.Value));
                    break;
                case "marAlternos":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.Alternos.HasValue || c.Alternos.Value < 1));
                    break;
                case "marAlternosVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.AlternosVal.HasValue || !c.AlternosVal.Value));
                    break;
                case "marComplementarios":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.Complementarios.HasValue || c.Complementarios.Value < 1));
                    break;
                case "marComplementariosVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.ComplementariosVal.HasValue || !c.ComplementariosVal.Value));
                    break;
                case "marCaracteristicas":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.Caracteristicas.HasValue || c.Caracteristicas.Value < 1));
                    break;
                case "marCaracteristicasVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.MarcaParteID == iMarcaID && (!c.CaracteristicasVal.HasValue || !c.CaracteristicasVal.Value));
                    break;
            }

            // Se llena el grid de partes
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvDatos.Rows.Add(oReg.ParteID, oReg.NumeroDeParte, oReg.Descripcion, oReg.Proveedor, oReg.Marca, oReg.Linea, oReg.FechaRegistro);
            this.lblEncontrados.Text = string.Format("Encontrados: {0}", this.dgvDatos.Rows.Count.ToString(GlobalClass.FormatoEntero));

            Cargando.Cerrar();
        }

        private void FiltrarPorLinea(DataGridViewCell oCelda)
        {
            string sCol = oCelda.OwningColumn.Name;
            if (sCol == "linLinea") return;

            // Se deshabilitan los campos de búsqueda
            this.ActivarFiltroAvance(true);

            Cargando.Mostrar();

            int iLineaID = Helper.ConvertirEntero(oCelda.OwningRow.Cells["linLineaID"].Value);
            List<PartesAvancesView> oDatos = new List<PartesAvancesView>();
            switch (sCol)
            {
                case "linFotos":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID);
                    // Se llenan los datos de las fotos
                    this.LlenarDatosDeFotos(oDatos);
                    oDatos = oDatos.Where(c => !c.Fotos.HasValue || c.Fotos.Value < 1).ToList();
                    // oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.Fotos.HasValue || c.Fotos.Value < 1));
                    break;
                case "linFotosVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.FotosVal.HasValue || !c.FotosVal.Value));
                    break;
                case "linEquivalentes":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.Equivalentes.HasValue || c.Equivalentes.Value < 1));
                    break;
                case "linEquivalentesVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.EquivalentesVal.HasValue || !c.EquivalentesVal.Value));
                    break;
                case "linAplicaciones":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.Aplicaciones.HasValue || c.Aplicaciones.Value < 1));
                    break;
                case "linAplicacionesVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.AplicacionesVal.HasValue || !c.AplicacionesVal.Value));
                    break;
                case "linAlternos":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.Alternos.HasValue || c.Alternos.Value < 1));
                    break;
                case "linAlternosVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.AlternosVal.HasValue || !c.AlternosVal.Value));
                    break;
                case "linComplementarios":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.Complementarios.HasValue || c.Complementarios.Value < 1));
                    break;
                case "linComplementariosVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.ComplementariosVal.HasValue || !c.ComplementariosVal.Value));
                    break;
                case "linCaracteristicas":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.Caracteristicas.HasValue || c.Caracteristicas.Value < 1));
                    break;
                case "linCaracteristicasVal":
                    oDatos = General.GetListOf<PartesAvancesView>(c => c.LineaID == iLineaID && (!c.CaracteristicasVal.HasValue || !c.CaracteristicasVal.Value));
                    break;
            }

            // Se llena el grid de partes
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvDatos.Rows.Add(oReg.ParteID, oReg.NumeroDeParte, oReg.Descripcion, oReg.Proveedor, oReg.Marca, oReg.Linea, oReg.FechaRegistro);
            this.lblEncontrados.Text = string.Format("Encontrados: {0}", this.dgvDatos.Rows.Count.ToString(GlobalClass.FormatoEntero));

            Cargando.Cerrar();
        }

        private void GuardarValidacionParte(int iParteID, string sValidacion, bool bValidado)
        {
            var oVal = General.GetEntity<ParteCompletado>(c => c.ParteID == iParteID);
            if (oVal == null)
                oVal = new ParteCompletado() { ParteID = iParteID, Fotos = false, Equivalentes = false, Aplicaciones = false, Alternos = false
                    , Complementarios = false, Caracteristicas = false };
            switch (sValidacion)
            {
                case "Fotos": oVal.Fotos = bValidado; break;
                case "Equivalentes": oVal.Equivalentes = bValidado; break;
                case "Aplicaciones": oVal.Aplicaciones = bValidado; break;
                case "Alternos": oVal.Alternos = bValidado; break;
                case "Complementarios": oVal.Complementarios = bValidado; break;
                case "Caracteristicas": oVal.Caracteristicas = bValidado; break;
            }
            Guardar.Generico<ParteCompletado>(oVal);
        }

        #endregion
                                
        #endregion

        #region [ Errores ]

        #region [ Eventos ]

        private void chkErr_MostrarTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkErr_MostrarTodos.Focused)
                this.CargarErrores();
        }

        private void dgvErrores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int iParteErrorID = Helper.ConvertirEntero(this.dgvErrores.CurrentRow.Cells["err_ParteErrorID"].Value);
            this.MarcarResuelto(iParteErrorID);
        }

        #endregion

        #region [ Métodos ]

        private void CargarErrores()
        {
            Cargando.Mostrar();
            
            bool bTodos = this.chkErr_MostrarTodos.Checked;
            var oErrores = General.GetListOf<PartesErroresView>(c => bTodos || (c.Foto || c.Equivalente || c.Aplicacion || c.Alterno || c.Complemento || c.Otro));
            this.dgvErrores.Rows.Clear();
            foreach (var oReg in oErrores)
                this.dgvErrores.Rows.Add(oReg.ParteErrorID, oReg.NumeroDeParte, oReg.Descripcion, oReg.Marca, oReg.Linea
                    , oReg.Fecha, oReg.Foto, oReg.Equivalente, oReg.Aplicacion, oReg.Alterno, oReg.Complemento, oReg.Otro
                    , oReg.ComentarioError, oReg.ComentarioSolucion, oReg.UsuarioError, oReg.UsuarioSolucion);

            Cargando.Cerrar();
        }

        private void MarcarResuelto(int iParteErrorID)
        {
            // Se pide el comentario
            var oComentario = UtilLocal.ObtenerValor("Indica un comentario sobre la solución:", "", MensajeObtenerValor.Tipo.TextoLargo);
            if (oComentario == null)
                return;

            // Se pide la validación de usuario
            var oResU = UtilDatos.ValidarObtenerUsuario();
            if (oResU.Error)
                return;

            var oError = General.GetEntity<ParteError>(c => c.ParteErrorID == iParteErrorID);
            oError.Foto = false;
            oError.Equivalente = false;
            oError.Aplicacion = false;
            oError.Alterno = false;
            oError.Complemento = false;
            oError.Otro = false;
            oError.ComentarioSolucion = Helper.ConvertirCadena(oComentario);
            oError.SolucionUsuarioID = oResU.Respuesta.UsuarioID;
            Guardar.Generico<ParteError>(oError);
            this.CargarErrores();
        }

        #endregion

        private void flpCaracteristicas_DoubleClick(object sender, EventArgs e)
        {
            var x = "dos";
            var y = "tres";
        }

        #endregion

    }
}
