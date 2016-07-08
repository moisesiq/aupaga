using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class DetalleMotor : DetalleBase
    {
        Motor Motor;
        MotoresView MotorView;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        private enum NombreOperaciones
        {
            None = 0,
            Add = 1,
            Change = 2,
            Delete = 3
        }

        public static DetalleMotor Instance
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

            internal static readonly DetalleMotor instance = new DetalleMotor();
        }

        public DetalleMotor()
        {
            InitializeComponent();
        }

        public DetalleMotor(int Id)
        {
            InitializeComponent();
            try
            {
                //Motor = General.GetEntityById<Motor>("Motor", "MotorID", Id);
                Motor = Datos.GetEntity<Motor>(c => c.MotorID == Id && c.Estatus);
                if (Motor == null)
                    throw new EntityNotFoundException(Id.ToString(), "Motor");
                MotorView = Datos.GetEntity<MotoresView>(m => m.MotorID == Id);
                if (Motor == null)
                    throw new EntityNotFoundException(Id.ToString(), "MotoresView");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleMotor_Load(object sender, EventArgs e)
        {
            this.CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                cboMarca.Focus();
            }
            else
            {
                if (Motor.MotorID > 0)
                {
                    this.Text = "Modificar";
                    try
                    {
                        var marca = Datos.GetEntity<Modelo>(m => m.ModeloID.Equals(Motor.ModeloID));
                        this.cboMarca.SelectedValue = marca.MarcaID;
                        this.cboModelo.SelectedValue = Motor.ModeloID;
                        this.cboMotor.Text = Motor.NombreMotor;

                        this.cboMarca.Enabled = false;
                        this.cboModelo.Enabled = false;
                        this.cboMotor.Enabled = false;

                        var anios = Datos.GetListOf<MotorAnio>(m => m.MotorID == Motor.MotorID).ToList();
                        if (anios != null)
                        {
                            anios.Sort((x, y) => x.Anio.CompareTo(y.Anio));
                            anios.Reverse();
                            foreach (var anio in anios)
                            {
                                for (int i = 0; i < clbAnios.Items.Count; i++)
                                {
                                    var anioMotor = Util.Entero(this.clbAnios.Items[i]);
                                    if (anioMotor == anio.Anio)
                                        this.clbAnios.SetItemChecked(i, true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        private void DetalleMotor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            var lista = new List<int>();
            if (!Validaciones())
                return;

            try
            {
                if (EsNuevo)
                {
                    //Valida, que no exista un motor con un modelo y un nombreMotor igual
                    var modeloId = Util.Entero(cboModelo.SelectedValue);
                    var motorVal = Datos.GetEntity<Motor>(m => m.ModeloID == modeloId && m.NombreMotor.Equals(this.cboMotor.Text));
                    if (motorVal == null)
                    {
                        var motor = new Motor()
                        {
                            ModeloID = modeloId,
                            NombreMotor = this.cboMotor.Text,
                            UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                            FechaRegistro = DateTime.Now,
                            Estatus = true,
                            Actualizar = true
                        };
                        Datos.SaveOrUpdate<Motor>(motor);

                        foreach (object itemChecked in clbAnios.CheckedItems)
                        {
                            lista.Add(Util.Entero(itemChecked));
                        }
                        UpdateAniosMotores(motor.MotorID, lista);
                    }
                }
                else
                {
                    Motor.ModeloID = Util.Entero(cboModelo.SelectedValue);
                    Motor.NombreMotor = this.cboMotor.Text;
                    Motor.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Motor.FechaModificacion = DateTime.Now;
                    Motor.Estatus = true;
                    Datos.SaveOrUpdate<Motor>(Motor);

                    foreach (object itemChecked in clbAnios.CheckedItems)
                    {
                        lista.Add(Util.Entero(itemChecked));
                    }
                    UpdateAniosMotores(Motor.MotorID, lista);
                }
                new Notificacion("Relación Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                motores.Instance.CustomInvoke<motores>(m => m.ActualizarListado());
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

        private void cboMarca_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (int.TryParse(cboMarca.SelectedValue.ToString(), out id))
                {
                    var modelos = Datos.GetListOf<Modelo>(m => m.MarcaID.Equals(id));
                    modelos.Sort((x, y) => x.NombreModelo.CompareTo(y.NombreModelo));
                    cboModelo.DataSource = modelos;
                    cboModelo.DisplayMember = "NombreModelo";
                    cboModelo.ValueMember = "ModeloID";
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void cboModelo_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (int.TryParse(cboModelo.SelectedValue.ToString(), out id))
                {
                    cboMotor.DataSource = Datos.GetListOf<MotorExistente>(m => m.Estatus);
                    cboMotor.DisplayMember = "NombreMotorExistente";
                    cboMotor.ValueMember = "MotorExistenteID";
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtAnioInicial_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAnioFinal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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
                cboMarca.DataSource = Datos.GetListOf<MarcasView>(m => m.MarcaID > 0);
                cboMarca.DisplayMember = "NombreMarca";
                cboMarca.ValueMember = "MarcaID";

                if (clbAnios.Items.Count > 0)
                {
                    ((ListBox)clbAnios).DataSource = null;
                    this.clbAnios.Items.Clear();
                }
                                
                for (var x = DateTime.Now.AddYears(1).Year; x >= 1970; x--)
                {
                    this.clbAnios.Items.Add(x, false);
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private static void UpdateAniosMotores(int motorId, List<int> values)
        {
            var anioActuales = Datos.GetListOf<MotorAnio>(m => m.MotorID.Equals(motorId));
            var selectedValues = new Dictionary<int, string>();

            foreach (var item in values)
            {
                if (!selectedValues.ContainsKey(item))
                    selectedValues.Add(item, NombreOperaciones.Add.ToString());
            }

            foreach (var item in anioActuales)
            {
                if (selectedValues.ContainsKey(item.Anio))
                {
                    selectedValues[item.Anio] = NombreOperaciones.None.ToString();
                }
                else
                {
                    selectedValues[item.Anio] = NombreOperaciones.Delete.ToString();
                }
            }

            foreach (var item in selectedValues)
            {
                if (item.Value == NombreOperaciones.Add.ToString()) //add new
                {
                    var motorAnio = new MotorAnio
                    {
                        MotorID = motorId,
                        Anio = item.Key,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<MotorAnio>(motorAnio);
                }
                else if (item.Value == NombreOperaciones.Delete.ToString()) //search and delete
                {
                    var anio = Datos.GetEntity<MotorAnio>(n => n.MotorID.Equals(motorId) && n.Anio.Equals(item.Key));
                    if (anio != null)
                        Datos.Delete<MotorAnio>(anio);
                }
            }
        }

        private bool Validaciones()
        {
            //var item = General.GetEntity<Motor>(m => m.NombreMedida.Equals(txtNombreMedida.Text));
            //if (EsNuevo.Equals(true) && item != null)
            //{
            //    Util.MensajeError("Ya existe una Medida con ese nombre, intente con otro.", GlobalClass.NombreApp);
            //    return false;
            //}
            //else if ((EsNuevo.Equals(false) && item != null) && item.MedidaID != Medida.MedidaID)
            //{
            //    Util.MensajeError("Ya existe una Medida con ese nombre, intente con otro.", GlobalClass.NombreApp);
            //    return false;
            //}
            this.cntError.LimpiarErrores();
            if (this.cboMarca.Text == "")
                this.cntError.PonerError(this.cboMarca, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.cboModelo.Text == "")
                this.cntError.PonerError(this.cboModelo, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.cboMotor.Text == "")
                this.cntError.PonerError(this.cboMotor, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.clbAnios.CheckedItems.Count < 1)
                this.cntError.PonerError(this.clbAnios, "Es necesario seleccionar por lo menos un año.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
