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

namespace Refaccionaria.App
{
    public partial class DetalleNivel : DetalleBase
    {
        Sucursal Sucursal;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        private enum Operaciones
        {
            None = 0,
            Add = 1,
            Change = 2,
            Delete = 3
        }

        public static DetalleNivel Instance
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

            internal static readonly DetalleNivel instance = new DetalleNivel();
        }

        public DetalleNivel()
        {
            InitializeComponent();
        }

        public DetalleNivel(int Id)
        {
            InitializeComponent();
            try
            {
                Sucursal = General.GetEntityById<Sucursal>("Sucursal", "SucursalID", Id);
                if (Sucursal == null)
                    throw new EntityNotFoundException(Id.ToString(), "CriterioABC");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleNivel_Load(object sender, EventArgs e)
        {
            try
            {
                this.CargaInicial();
                if (EsNuevo)
                {
                    this.Text = "Nuevo";
                }
                else
                {
                    if (Sucursal.SucursalID > 0)
                    {
                        this.Text = "Modificar";
                        var criterios = General.GetListOf<SucursalCriterioABC>(s => s.SucursalID == Sucursal.SucursalID && s.Estatus);
                        foreach (var criterio in criterios)
                        {
                            for (int i = 0; i < this.clbCriterioAbc.Items.Count; i++)
                            {
                                var x = (CriterioABC)clbCriterioAbc.Items[i];
                                if (x.CriterioAbcID == criterio.CriterioAbcID)
                                {
                                    this.clbCriterioAbc.SetItemChecked(i, true);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
        
        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var lista = new List<int>();
                foreach (object itemChecked in this.clbCriterioAbc.CheckedItems)
                {
                    CriterioABC castedItem = itemChecked as CriterioABC;
                    lista.Add(castedItem.CriterioAbcID);
                }

                if (EsNuevo)
                {
                    
                }
                else
                {
                    this.UpdateSucursalCriterioAbc(Sucursal.SucursalID, lista);
                }
                //new Notificacion("Información Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                catalogosPedidos.Instance.CustomInvoke<catalogosPedidos>(m => m.CargaInicial());
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void clbCriterioAbc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                var valor = (CheckedListBox)sender;
                if (valor.GetItemCheckState(valor.SelectedIndex).Equals(CheckState.Unchecked))
                    valor.SetItemChecked(valor.SelectedIndex, true);
                else
                    valor.SetItemChecked(valor.SelectedIndex, false);
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
                if (this.clbCriterioAbc.Items.Count > 0)
                {
                    ((ListBox)clbCriterioAbc).DataSource = null;
                    this.clbCriterioAbc.Items.Clear();
                }

                var criterios = General.GetListOf<CriterioABC>(c => c.Estatus);
                criterios.Sort((x, y) => x.Clasificacion.CompareTo(y.Clasificacion));
                ((ListBox)clbCriterioAbc).DataSource = criterios;
                ((ListBox)clbCriterioAbc).DisplayMember = "Clasificacion";
                ((ListBox)clbCriterioAbc).ValueMember = "CriterioAbcID";

            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void UpdateSucursalCriterioAbc(int sucursalId, IEnumerable<int> values)
        {
            var criteriosActuales = General.GetListOf<SucursalCriterioABC>(s => s.SucursalID == sucursalId);
            var selectedValues = new Dictionary<int, int>();

            foreach (var item in values)
            {
                selectedValues.Add(item, (int)Operaciones.Add);
            }

            foreach (var item in criteriosActuales)
            {
                if (selectedValues.ContainsKey(Helper.ConvertirEntero(item.CriterioAbcID)))
                {
                    selectedValues[Helper.ConvertirEntero(item.CriterioAbcID)] = (int)Operaciones.None;
                }
                else
                {
                    selectedValues[Helper.ConvertirEntero(item.CriterioAbcID)] = (int)Operaciones.Delete;
                }
            }

            foreach (var item in selectedValues)
            {
                if (item.Value == (int)Operaciones.Add) //add new
                {
                    var criterio = new SucursalCriterioABC
                    {
                        SucursalID = sucursalId,
                        CriterioAbcID = Helper.ConvertirEntero(item.Key),
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    General.SaveOrUpdate<SucursalCriterioABC>(criterio, criterio);                    
                }
                else if (item.Value == (int)Operaciones.Delete) //search and delete
                {
                    var criterio = General.GetEntity<SucursalCriterioABC>(s => s.SucursalID == sucursalId && s.CriterioAbcID == item.Key);
                    if (criterio != null)
                    {
                        General.Delete<SucursalCriterioABC>(criterio);                        
                    }
                }
            }
        }

        #endregion

    }
}
