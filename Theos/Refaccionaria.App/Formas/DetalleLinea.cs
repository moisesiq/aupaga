using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class DetalleLinea : DetalleBase
    {
        Linea Linea;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        //Manuel: Controla el botón Agregar Imagen y Agregar Logo
        int IdPAraObjeto;
        CargaLogos Objeto = new CargaLogos();

        public static DetalleLinea Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly DetalleLinea instance = new DetalleLinea();
        }

        public DetalleLinea()
        {
            InitializeComponent();
            button1.Enabled = false;
            button2.Enabled = false;
        }
        
        public DetalleLinea(int Id)
        {
            InitializeComponent();
            Objeto.CargaLogo(Id.ToString(), picLogo, UtilLocal.RutaImagenesLineas());
            this.IdPAraObjeto = Id;

            //AbrirGuardarImg
            try
            {
                //Linea = General.GetEntityById<Linea>("Linea", "LineaID", Id);
                Linea = Datos.GetEntity<Linea>(c => c.LineaID == Id && c.Estatus);
                if (Linea == null)
                    throw new EntityNotFoundException(Id.ToString(), "Linea");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleLinea_Load(object sender, EventArgs e)
        {
            // Se configura el grid de las caractarísticas
            this.dgvCaracteristicas.DefaultCellStyle.ForeColor = Color.Black;
            this.dgvCaracteristicas.Inicializar();
            var oListaCaracteristicas = new List<LineasCaracteristicasView>();
            if (this.Linea != null)
                oListaCaracteristicas = Datos.GetListOf<LineasCaracteristicasView>(c => c.LineaID == this.Linea.LineaID);
            oListaCaracteristicas.Insert(0, new LineasCaracteristicasView() { CaracteristicaID = -1, Caracteristica = "(Nueva)" });
            this.colCaracteristicaID.CargarDatos("CaracteristicaID", "Caracteristica", oListaCaracteristicas);

            //
            this.CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                this.txtNombreLinea.Focus();
            }
            else
            {
                if (Linea.LineaID > 0)
                {
                    this.Text = "Modificar";
                    txtNombreLinea.Text = Linea.NombreLinea;
                    txtAbreviacion.Text = Linea.Abreviacion;
                    cboSubsistema.SelectedValue = Linea.SubsistemaID;
                    txtMachote.Text = Linea.Machote;

                    /* chkAlto.CheckState = Linea.Alto.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkDiametro.CheckState = Linea.Diametro.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkLargo.CheckState = Linea.Largo.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkDientes.CheckState = Linea.Dientes.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkAstrias.CheckState = Linea.Astrias.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkSistema.CheckState = Linea.Sistema.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkCapacidad.CheckState = Linea.Capacidad.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkAmperaje.CheckState = Linea.Amperaje.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkVoltaje.CheckState = Linea.Voltaje.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkWatts.CheckState = Linea.Watts.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkUbicacion.CheckState = Linea.Ubicacion.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkTerminales.CheckState = Linea.Terminales.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkCilindros.CheckState = Linea.Cilindros.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    chkGarantia.CheckState = Linea.Garantia.Equals(true) ? CheckState.Checked : CheckState.Unchecked;
                    */

                    // Se llenan las características
                    this.CargarCaracteristicas();
                }
            }
        }
                
        private void DetalleLinea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        private void dgvCaracteristicas_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dgvCaracteristicas.VerDirtyStateChanged("colCaracteristicaID");
        }

        private void dgvCaracteristicas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvCaracteristicas.Columns[e.ColumnIndex].Name == "colCaracteristicaID")
                this.VerCambioCaracteristica();
        }
                
        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            try
            {
                var subsistemaId = Util.Entero(this.cboSubsistema.SelectedValue);
                var subsistema = Datos.GetEntity<Subsistema>(s => s.SubsistemaID == subsistemaId);
                
                if (EsNuevo)
                {                    
                    var linea = new Linea()
                    {
                        NombreLinea = txtNombreLinea.Text,
                        Abreviacion = txtAbreviacion.Text,
                        SistemaID = subsistema.SistemaID,
                        SubsistemaID = Util.Entero(cboSubsistema.SelectedValue),
                        Machote = txtMachote.Text,
                        
                        /* Alto = chkAlto.CheckState.Equals(CheckState.Checked) ? true : false,
                        Diametro = chkDiametro.CheckState.Equals(CheckState.Checked) ? true : false,
                        Largo = chkLargo.CheckState.Equals(CheckState.Checked) ? true : false,
                        Dientes = chkDientes.CheckState.Equals(CheckState.Checked) ? true : false,
                        Astrias = chkAstrias.CheckState.Equals(CheckState.Checked) ? true : false,
                        Sistema = chkSistema.CheckState.Equals(CheckState.Checked) ? true : false,
                        Capacidad = chkCapacidad.CheckState.Equals(CheckState.Checked) ? true : false,
                        Amperaje = chkAmperaje.CheckState.Equals(CheckState.Checked) ? true : false,
                        Voltaje = chkVoltaje.CheckState.Equals(CheckState.Checked) ? true : false,
                        Watts = chkWatts.CheckState.Equals(CheckState.Checked) ? true : false,
                        Ubicacion = chkUbicacion.CheckState.Equals(CheckState.Checked) ? true : false,
                        Terminales = chkTerminales.CheckState.Equals(CheckState.Checked) ? true : false,
                        Cilindros = chkCilindros.CheckState.Equals(CheckState.Checked) ? true : false,
                        Garantia = chkGarantia.CheckState.Equals(CheckState.Checked) ? true : false,
                        */

                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<Linea>(linea);
                }
                else
                {
                    Linea.NombreLinea = txtNombreLinea.Text;
                    Linea.Abreviacion = txtAbreviacion.Text;
                    Linea.SistemaID = subsistema.SistemaID;
                    Linea.SubsistemaID = subsistemaId;
                    Linea.Machote = txtMachote.Text;

                    /* Linea.Alto = chkAlto.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Diametro = chkDiametro.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Largo = chkLargo.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Dientes = chkDientes.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Astrias = chkAstrias.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Sistema = chkSistema.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Capacidad = chkCapacidad.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Amperaje = chkAmperaje.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Voltaje = chkVoltaje.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Watts = chkWatts.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Ubicacion = chkUbicacion.CheckState.Equals(CheckState.Checked) ? true : false;
                    Linea.Terminales = chkTerminales.CheckState.Equals(CheckState.Checked) ? true : false;
                    */

                    Linea.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Linea.FechaModificacion = DateTime.Now;
                    Linea.Estatus = true;
                    Datos.SaveOrUpdate<Linea>(Linea);
                }

                // Se mandan guardar las características
                this.GuardarCaracteristicas();

                new Notificacion("Linea Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                lineas.Instance.CustomInvoke<lineas>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }
                
        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Metodos ]

        private void CargaInicial()
        {
            // Se validan los permisos
            //if (this.EsNuevo)
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Agregar", true))
            //    {
            //        this.Close();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Modificar", false))
            //        this.btnGuardar.Enabled = false;
            //}

            try
            {
                cboSubsistema.DataSource = Datos.GetListOf<Subsistema>(s => s.Estatus.Equals(true));
                cboSubsistema.DisplayMember = "NombreSubsistema";
                cboSubsistema.ValueMember = "SubsistemaID";
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool Validaciones()
        {
            try
            {
                var item = Datos.GetEntity<Linea>(m => m.NombreLinea.Equals(txtNombreLinea.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe una Linea con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.LineaID != Linea.LineaID)
                {
                    Util.MensajeError("Ya existe una Linea con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreLinea.Text == "")
                this.cntError.PonerError(this.txtNombreLinea, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        private void CargarCaracteristicas()
        {
            var oLineaCars = Datos.GetListOf<LineasCaracteristicasView>(c => c.LineaID == this.Linea.LineaID);
            this.dgvCaracteristicas.Rows.Clear();
            foreach (var oReg in oLineaCars)
                this.dgvCaracteristicas.AgregarFila(oReg.LineaCaracteristicaID, Cat.TiposDeAfectacion.SinCambios
                    , oReg.CaracteristicaID, oReg.CaracteristicaID, oReg.MultipleOpciones);
        }

        private void VerCambioCaracteristica()
        {
            int iCaracteristicaID = Util.Entero(this.dgvCaracteristicas.CurrentRow.Cells["colCaracteristicaID"].Value);
            if (iCaracteristicaID == -1)
            {
                // Se pregunta por la nueva característica
                var oCaracteristica = UtilLocal.ObtenerValor("Indica el nombre de la Característica:", "", MensajeObtenerValor.Tipo.Texto);
                if (oCaracteristica != null)
                {
                    // Se obtiene el valor correspondiente
                    var oListaCaracteristicas = (this.colCaracteristicaID.DataSource as List<LineasCaracteristicasView>);
                    int iValorSig = oListaCaracteristicas.Min(c => c.CaracteristicaID);
                    iValorSig--;
                    //
                    oListaCaracteristicas.Add(new LineasCaracteristicasView() { CaracteristicaID = iValorSig, Caracteristica = Util.Cadena(oCaracteristica) });
                    this.dgvCaracteristicas.CurrentRow.Cells["colCaracteristicaID"].Value = iValorSig;
                }
            }
            else
            {
                var oCaracteristica = Datos.GetEntity<Caracteristica>(c => c.CaracteristicaID == iCaracteristicaID);
                if (oCaracteristica != null)
                    this.dgvCaracteristicas.CurrentRow.Cells["colMultipleOpciones"].Value = oCaracteristica.MultipleOpciones;
            }
        }

        private void GuardarCaracteristicas()
        {
            Cargando.Mostrar();

            LineaCaracteristica oReg;
            foreach (DataGridViewRow oFila in this.dgvCaracteristicas.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iId = this.dgvCaracteristicas.ObtenerId(oFila);
                int iCambio = this.dgvCaracteristicas.ObtenerIdCambio(oFila);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new LineaCaracteristica();
                        else
                            oReg = Datos.GetEntity<LineaCaracteristica>(c => c.LineaCaracteristicaID == iId);

                        // Se llenan los datos
                        oReg.LineaID = this.Linea.LineaID;
                        oReg.CaracteristicaID = Util.Entero(oFila.Cells["colCaracteristicaID"].Value);
                        // Se valida que no exista ya
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                        {
                            if (Datos.Exists<LineaCaracteristica>(c => c.LineaID == this.Linea.LineaID && c.CaracteristicaID == oReg.CaracteristicaID))
                                continue;
                        }
                        // Se verifican los cambios a la característica en sí
                        var oCaracteristica = Datos.GetEntity<Caracteristica>(c => c.CaracteristicaID == oReg.CaracteristicaID);
                        if (oCaracteristica == null)
                            oCaracteristica = new Caracteristica()
                            {
                                Caracteristica1 = Util.Cadena((oFila.Cells["colCaracteristicaID"] as DataGridViewComboBoxCell).FormattedValue)
                            };
                        // Se valida si cambiaron las opciones
                        bool bOpcionEnUso = false;
                        string sOpciones = Util.Cadena(oFila.Cells["colMultipleOpciones"].Value);
                        if (oCaracteristica.MultipleOpciones != null && sOpciones != oCaracteristica.MultipleOpciones)
                        {
                            var oOpcionesAnt = oCaracteristica.MultipleOpciones.Split(',');
                            var oOpciones = sOpciones.Split(',');
                            var oDif = oOpcionesAnt.Except(oOpciones);
                            foreach (var sOpcion in oDif)
                            {
                                if (Datos.Exists<ParteCaracteristica>(c => c.CaracteristicaID == oReg.CaracteristicaID && c.Valor == sOpcion))
                                {
                                    UtilLocal.MensajeError("Alguna de las características removidas está siendo usada. No se puede guardar.");
                                    bOpcionEnUso = true;
                                    break;
                                }
                            }
                        }
                        if (!bOpcionEnUso)
                        {
                            //
                            oCaracteristica.MultipleOpciones = sOpciones;
                            oCaracteristica.Multiple = (oCaracteristica.MultipleOpciones != "");
                            Datos.Guardar<Caracteristica>(oCaracteristica);
                            // Se guarda
                            oReg.CaracteristicaID = oCaracteristica.CaracteristicaID;
                            Datos.Guardar<LineaCaracteristica>(oReg);
                        }
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = Datos.GetEntity<LineaCaracteristica>(c => c.LineaCaracteristicaID == iId);
                        Datos.Eliminar<LineaCaracteristica>(oReg);
                        break;
                }
            }

            Cargando.Cerrar();
            // this.CargarCaracteristicas();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Objeto.AbrirGuardarImg(this.IdPAraObjeto.ToString(), picLogo, UtilLocal.RutaImagenesLineas());
            Objeto.CargaLogo(this.IdPAraObjeto.ToString(), picLogo, UtilLocal.RutaImagenesLineas());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ContenedorControl frmContenedor = new ContenedorControl("Agregar archivo...", new CargaImgMarcaLinea(this.IdPAraObjeto, false));
            frmContenedor.ShowDialog();
            frmContenedor.Dispose();

        }

                        
    }
}
