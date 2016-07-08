using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class DetalleProveedorContacto : DetalleBase
    {
        public int ProveedorId = 0;
        ProveedorContacto ProveedorContacto;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleProveedorContacto Instance
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

            internal static readonly DetalleProveedorContacto instance = new DetalleProveedorContacto();
        }

        public DetalleProveedorContacto()
        {
            InitializeComponent();
        }

        public DetalleProveedorContacto(int Id)
        {
            InitializeComponent();
            try
            {
                //ProveedorContacto = General.GetEntityById<ProveedorContacto>("ProveedorContacto", "ProveedorContactoID", Id);
                ProveedorContacto = Datos.GetEntity<ProveedorContacto>(c => c.ProveedorContactoID == Id && c.Estatus);
                if (ProveedorContacto == null)
                    throw new EntityNotFoundException(Id.ToString(), "ProveedorContacto");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleProveedorContacto_Load(object sender, EventArgs e)
        {
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
            }
            else
            {
                if (ProveedorContacto.ProveedorContactoID > 0)
                {
                    this.Text = "Modificar";
                    var pc = Datos.GetEntity<ProveedorContacto>(p => p.ProveedorContactoID.Equals(ProveedorContacto.ProveedorContactoID));
                    if (pc != null)
                    {
                        this.txtNombreContacto.Text = pc.NombreContacto;
                        this.txtTelUno.Text = pc.TelUno;
                        this.txtTelDos.Text = pc.TelDos;
                        this.txtTelTres.Text = pc.TelTres;
                        this.txtTelCuatro.Text = pc.TelCuatro;
                        this.txtDepartamento.Text = pc.Departamento;
                        this.txtCorreoE.Text = pc.CorreoElectronico;
                        this.cbPrincipal.Checked = (pc.Principal == true) ? true : false;
                    }
                }
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            try
            {
                int Id = 0;
                if (EsNuevo)
                {
                    var proveedorContacto = new ProveedorContacto()
                    {
                        ProveedorID = ProveedorId,
                        NombreContacto = this.txtNombreContacto.Text,
                        TelUno = this.txtTelUno.Text,
                        TelDos = this.txtTelDos.Text,
                        TelTres = this.txtTelTres.Text,
                        TelCuatro = this.txtTelCuatro.Text,
                        Departamento = this.txtDepartamento.Text,
                        CorreoElectronico = this.txtCorreoE.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true

                    };

                    // Inicio - Detremina proveedor principal [Manuel 101014]
                    var principal = Datos.GetEntity<ProveedorContacto>(pl => pl.ProveedorID == ProveedorId && pl.Principal == true) ;
                    if (principal == null)
                    {
                        proveedorContacto.Principal = true;
                        this.cbPrincipal.Checked = true;
                    }
                    else if (this.cbPrincipal.Checked == true)
                    {
                        //quitar lo principal al contacto existente otro
                        principal.Principal = false;
                        Datos.SaveOrUpdate<ProveedorContacto>(principal);
                        //poner lo prinipal al contacto actual
                        proveedorContacto.Principal = true;
                    }
                    // Fin - Detremina proveedor principal [Manuel 101014]

                    Datos.SaveOrUpdate<ProveedorContacto>(proveedorContacto);
                    Id = proveedorContacto.ProveedorID;
                }
                else
                {
                    ProveedorContacto.NombreContacto = this.txtNombreContacto.Text;
                    ProveedorContacto.TelUno = this.txtTelUno.Text;
                    ProveedorContacto.TelDos = this.txtTelDos.Text;
                    ProveedorContacto.TelTres = this.txtTelTres.Text;
                    ProveedorContacto.TelCuatro = this.txtTelCuatro.Text;
                    ProveedorContacto.Departamento = this.txtDepartamento.Text;
                    ProveedorContacto.CorreoElectronico = this.txtCorreoE.Text;
                    ProveedorContacto.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    ProveedorContacto.FechaModificacion = DateTime.Now;
                    ProveedorContacto.Estatus = true;

                    // Inicio - Detremina proveedor principal [Manuel 101014]
                    if (!(ProveedorContacto.Principal == true && this.cbPrincipal.Checked == true))
                    {
                        var principal = Datos.GetEntity<ProveedorContacto>(pl => pl.ProveedorID == ProveedorId && pl.Principal == true);

                        if (principal == null)
                        {
                            ProveedorContacto.Principal = true;
                            this.cbPrincipal.Checked = true;
                        }
                        else
                        {
                            if (principal.ProveedorContactoID == ProveedorContacto.ProveedorContactoID)
                            {
                                ProveedorContacto.Principal = true;
                                this.cbPrincipal.Checked = true;
                            }
                            else if (this.cbPrincipal.Checked == true)
                            {
                                //quitar lo principal al contacto existente otro
                                principal.Principal = false;
                                Datos.SaveOrUpdate<ProveedorContacto>(principal);
                                //poner lo prinipal al contacto actual
                                ProveedorContacto.Principal = true;
                            }
                        }
                    }
                    // Fin - Detremina proveedor principal [Manuel 101014]

                    Datos.SaveOrUpdate<ProveedorContacto>(ProveedorContacto);
                    Id = ProveedorContacto.ProveedorID;
                }
                new Notificacion("Contacto Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                proveedorContactos.Instance.CustomInvoke<proveedorContactos>(m => m.ActualizarListado(Id));
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleProveedorContacto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Metodos ]

        private bool Validaciones()
        {
            try
            {
                var item = Datos.GetEntity<ProveedorContacto>(pc => pc.NombreContacto.Equals(txtNombreContacto.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe un Contacto con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.ProveedorContactoID != ProveedorContacto.ProveedorContactoID)
                {
                    Util.MensajeError("Ya existe un Contacto con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreContacto.Text == "")
                this.cntError.PonerError(this.txtNombreContacto, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtDepartamento.Text == "")
                this.cntError.PonerError(this.txtDepartamento, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
