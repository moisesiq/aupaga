using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

using Refaccionaria.Negocio.Controles;

namespace Refaccionaria.App
{
    public partial class DetalleMarcaParte : DetalleBase
    {
        MarcaParte MarcaParte;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;
        
        //Manuel: Controla el botón Agregar Imagen y Agregar Logo
            CargaLogos Objeto = new CargaLogos();  
            int IdPAraObjeto;

        private enum marcaParteOperaciones
        {
            None = 0,
            Add = 1,
            Change = 2,
            Delete = 3
        }

        public static DetalleMarcaParte Instance
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

            internal static readonly DetalleMarcaParte instance = new DetalleMarcaParte();
        }

        public DetalleMarcaParte()
        {
            InitializeComponent();
            btnAddFile.Enabled = false;
            btnAddLogo.Enabled = false;
        }

        public DetalleMarcaParte(int Id)
        {
            InitializeComponent();
            Objeto.CargaLogo(Id.ToString(), picLogo, UtilLocal.RutaImagenesMarcas());
            this.IdPAraObjeto = Id;
            try
            {
                MarcaParte = Negocio.General.GetEntityById<MarcaParte>("MarcaParte", "MarcaParteID", Id);
                if (MarcaParte == null)
                    throw new EntityNotFoundException(Id.ToString(), "MarcaParte");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleMarcaParte_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Negocio.Helper.ClearTextBoxes(this);
                txtNombreMarca.Focus();
            }
            else
            {
                if (MarcaParte.MarcaParteID > 0)
                {
                    this.Text = "Modificar";
                    var lineas = Negocio.General.GetListOf<LineaMarcaParte>(l => l.MarcaParteID.Equals(MarcaParte.MarcaParteID));
                    foreach (var linea in lineas)
                    {
                        for (int i = 0; i < clbLineas.Items.Count; i++)
                        {
                            var x = (Linea)clbLineas.Items[i];
                            if (x.LineaID == linea.LineaID)
                            {
                                clbLineas.SetItemChecked(i, true);
                            }
                        }
                    }
                    txtNombreMarca.Text = MarcaParte.NombreMarcaParte;
                    txtAbreviacion.Text = MarcaParte.Abreviacion;
                }
            }
        }

        private void DetalleMarcaParte_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            try
            {
                var lista = new List<int>();
                foreach (object itemChecked in clbLineas.CheckedItems)
                {
                    Linea castedItem = itemChecked as Linea;
                    lista.Add(castedItem.LineaID);
                }

                if (EsNuevo)
                {
                    var marcaParte = new MarcaParte()
                    {
                        NombreMarcaParte = txtNombreMarca.Text,
                        Abreviacion = txtAbreviacion.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Negocio.General.SaveOrUpdate<MarcaParte>(marcaParte, marcaParte);
                    UpdateLineaMarcaParte(marcaParte.MarcaParteID, lista);
                }
                else
                {
                    MarcaParte.NombreMarcaParte = txtNombreMarca.Text;
                    MarcaParte.Abreviacion = txtAbreviacion.Text;
                    MarcaParte.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    MarcaParte.FechaModificacion = DateTime.Now;
                    MarcaParte.Estatus = true;
                    Negocio.General.SaveOrUpdate<MarcaParte>(MarcaParte, MarcaParte);
                    UpdateLineaMarcaParte(MarcaParte.MarcaParteID, lista);
                }
                new Notificacion("Marca Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                marcaPartes.Instance.CustomInvoke<marcaPartes>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                var lineas = General.GetListOf<Linea>(l => l.Estatus.Equals(true));
                lineas.Sort((x, y) => x.NombreLinea.CompareTo(y.NombreLinea));
                ((ListBox)clbLineas).DataSource = lineas;
                ((ListBox)clbLineas).DisplayMember = "NombreLinea";
                ((ListBox)clbLineas).ValueMember = "LineaID";
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private static void UpdateLineaMarcaParte(int marcaParteId, IEnumerable<int> values)
        {
            var lineasActuales = Negocio.General.GetListOf<LineaMarcaParte>(l => l.MarcaParteID.Equals(marcaParteId));
            var selectedValues = new Dictionary<int, int>();

            foreach (var item in values)
            {
                selectedValues.Add(item, (int)marcaParteOperaciones.Add);
            }

            foreach (var item in lineasActuales)
            {
                if (selectedValues.ContainsKey(item.LineaID))
                {
                    selectedValues[item.LineaID] = (int)marcaParteOperaciones.None;
                }
                else
                {
                    selectedValues[item.LineaID] = (int)marcaParteOperaciones.Delete;
                }
            }

            foreach (var item in selectedValues)
            {
                if (item.Value == (int)marcaParteOperaciones.Add) //add new
                {
                    var lineaMarcaParte = new LineaMarcaParte
                    {
                        MarcaParteID = marcaParteId,
                        LineaID = Helper.ConvertirEntero(item.Key),
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    General.SaveOrUpdate<LineaMarcaParte>(lineaMarcaParte, lineaMarcaParte);

                    //Aqui se trae la lista de proveedores que tengan una relacion con esta marca
                    var provs = General.GetListOf<ProveedorMarcaParte>(m => m.MarcaParteID == marcaParteId && m.Estatus).GroupBy(c => new { c.ProveedorID, c.MarcaParteID }).ToList();
                    if (provs != null)
                    {
                        //Y se inserta en la tabla proveedorGanancia la marca y la linea con valores en 0
                        foreach (var pro in provs)
                        {
                            var proveedorid = Helper.ConvertirEntero(pro.Key.ProveedorID);
                            var lineaId = Helper.ConvertirEntero(item.Key);

                            var pg = General.GetEntity<ProveedorGanancia>(p => p.ProveedorID == proveedorid
                                && p.MarcaParteID == marcaParteId && p.LineaID == lineaId);
                            if (pg == null)
                            {
                                var ganancia = new ProveedorGanancia()
                                {
                                    ProveedorID = pro.Key.ProveedorID,
                                    MarcaParteID = marcaParteId,
                                    LineaID = lineaId,
                                    PorcentajeUno = 0,
                                    PorcentajeDos = 0,
                                    PorcentajeTres = 0,
                                    PorcentajeCuatro = 0,
                                    PorcentajeCinco = 0
                                };
                                Guardar.Generico<ProveedorGanancia>(ganancia);
                            }
                        }
                    }
                }
                else if (item.Value == (int)marcaParteOperaciones.Delete) //search and delete
                {
                    var lineaMarcaParte = General.GetEntity<LineaMarcaParte>(l => l.MarcaParteID.Equals(marcaParteId) && l.LineaID.Equals(item.Key));
                    if (lineaMarcaParte != null)
                    {
                        General.Delete<LineaMarcaParte>(lineaMarcaParte);

                        //Aqui se trae la lista de proveedores que tengan una relacion con esta marca
                        var provs = General.GetListOf<ProveedorMarcaParte>(m => m.MarcaParteID == marcaParteId && m.Estatus).GroupBy(c => new { c.ProveedorID, c.MarcaParteID }).ToList();
                        if (provs != null)
                        {
                            //Y se elimina en la tabla proveedorGanancia el registro 
                            foreach (var pro in provs)
                            {
                                var proveedorid = Helper.ConvertirEntero(pro.Key.ProveedorID);
                                var lineaId = Helper.ConvertirEntero(item.Key);
                                var pg = General.GetEntity<ProveedorGanancia>(p => p.ProveedorID == proveedorid
                                    && p.MarcaParteID == marcaParteId && p.LineaID == lineaId);
                                if (pg != null)
                                {
                                    General.Delete<ProveedorGanancia>(pg);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool Validaciones()
        {
            try
            {
                var item = Negocio.General.GetEntity<MarcaParte>(m => m.NombreMarcaParte.Equals(txtNombreMarca.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Negocio.Helper.MensajeError("Ya existe una Marca con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.MarcaParteID != MarcaParte.MarcaParteID)
                {
                    Negocio.Helper.MensajeError("Ya existe una Marca con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreMarca.Text == "")
                this.cntError.PonerError(this.txtNombreMarca, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            if (this.clbLineas.CheckedItems.Count < 1)
                this.cntError.PonerError(this.clbLineas, "Es necesario seleccionar por lo menos una linea.", ErrorIconAlignment.MiddleRight);

            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

        private void btnAddLogo_Click(object sender, EventArgs e)
        {
            Objeto.AbrirGuardarImg(this.IdPAraObjeto.ToString(), picLogo, UtilLocal.RutaImagenesMarcas());
            Objeto.CargaLogo(this.IdPAraObjeto.ToString(), picLogo, UtilLocal.RutaImagenesMarcas());
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            ContenedorControl frmContenedor = new ContenedorControl("Agregar archivo...", new CargaImgMarcaLinea(this.IdPAraObjeto, true));
            frmContenedor.ShowDialog();
            frmContenedor.Dispose();
        }

    }
}
