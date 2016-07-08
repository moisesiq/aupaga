using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class Inventario : UserControl
    {
        ControlError ctlError = new ControlError();

        public Inventario()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void Inventario_Load(object sender, EventArgs e)
        {
            // Progreso
            this.dtpDesde.Value = DateTime.Now;
            this.dtpHasta.Value = new DateTime(DateTime.Now.Year, 12, 31);
            // Resultados
            this.cmbResultadosLineas.CargarDatos("LineaID", "Linea", Datos.GetListOf<InventarioLineasAgrupadoView>(c => c.EstatusGenericoID == Cat.EstatusGenericos.Completada));
            this.cmbResultadosCasos.Items.AddRange(new object[] { "TRASPASOS", "FALTANTES Y SOB.", "SIN DIFERENCIA" });
            // Asignación manual
            this.cmbLinea.CargarDatos("LineaID", "NombreLinea", Datos.GetListOf<Linea>(c => c.Estatus).OrderBy(o => o.NombreLinea).ToList());

            //
            this.LlenarProgreso();
        }

        private void dtpDesde_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpDesde.Focused)
                this.LlenarProgreso();
        }

        private void dtpHasta_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpHasta.Focused)
                this.LlenarProgreso();
        }

        private void cmbResultadosLineas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbResultadosLineas.Focused)
                this.LlenarResultados();
        }
                
        private void cmbResultadosCasos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbResultadosCasos.Focused)
                this.LlenarResultados();
        }

        private void dgvResultados_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            this.CambioResultados(this.dgvResultados.Columns[e.ColumnIndex].Name, this.dgvResultados.Rows[e.RowIndex]);
        }
                
        private void cmbLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbLinea.Focused)
                this.CargarAsignacion();
        }

        private void btnAsignar_Click(object sender, EventArgs e)
        {
            this.GuardarAsignacion();
        }

        #endregion

        #region [ Métodos ]

        private void LlenarProgreso()
        {
            Cargando.Mostrar();

            var oParams = new Dictionary<string, object>();
            oParams.Add("Opcion", 1);
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);
            
            // Se obtienen los datos para el programa a futuro
            var oDatos = Datos.ExecuteProcedure<pauInventarioProgreso_Result>("pauInventarioProgreso", oParams);
            var oPeriodicidad = Datos.GetListOf<InventarioLineaPeriodicidad>();
            var oFechasPer = new Dictionary<int, DateTime>();
            this.dgvProgreso.Rows.Clear();

            // Se llenan los datos de las líneas ya procesadas
            int iFila = 0;
            DateTime dAvance = this.dtpDesde.Value;
            int iPrimeraVuelta = int.MaxValue;
            var oVueltas = new List<string>();
            foreach (var oReg in oDatos)
            {
                iFila = this.dgvProgreso.Rows.Add(oReg.LineaID, oReg.Linea, oReg.Partes, oReg.Costo, oReg.Dias, oReg.PorMatriz
                    , oReg.PorSuc2, oReg.PorSuc3, oReg.FechaIniciado, oReg.FechaCompletado);
                this.dgvProgreso["Progreso_FechaIniciado", iFila].Style.ForeColor = Color.Black;
                if (oReg.FechaCompletado.HasValue)
                    this.dgvProgreso["Progreso_FechaCompletado", iFila].Style.ForeColor = Color.Black;
                else
                    this.dgvProgreso["Progreso_FechaCompletado", iFila].Value = oReg.FechaIniciado.Valor().AddDays((double)oReg.Dias);

                // Para determinar la fecha de avance
                if (Util.FechaHora(this.dgvProgreso["Progreso_FechaCompletado", iFila].Value) > dAvance)
                    dAvance = Util.FechaHora(this.dgvProgreso["Progreso_FechaCompletado", iFila].Value);
                // Para saber cuáles líneas ya fueron procesadas
                if (oReg.AvVuelta.HasValue && iPrimeraVuelta > oReg.AvVuelta)
                    iPrimeraVuelta = oReg.AvVuelta.Valor();
                oVueltas.Add(string.Format("{0}-{1}", oReg.LineaID, oReg.AvVuelta));
            }

            // Se meten los datos del programa a futuro
            oParams["Opcion"] = 2;
            oDatos = Datos.ExecuteProcedure<pauInventarioProgreso_Result>("pauInventarioProgreso", oParams);
            int iExistentes = this.dgvProgreso.Rows.Count;
            int iReg = 0;
            while (dAvance <= this.dtpHasta.Value)
            {
                if (iReg >= oDatos.Count)
                {
                    iReg = 0;
                    iPrimeraVuelta++;

                    // Se verifica el avance de fechas si ya se dió una vuelta, por si no hay avance, detener el proceso y no se cicle
                    if (dAvance == this.dtpDesde.Value)
                        break;
                }
                var oReg = oDatos[iReg++];

                // Se verifica si la línea ya fue contada
                /* if (iExistentes >= iReg)
                {
                    // No recuerdo para qué eran estas líneas
                    if (oReg.LineaID == Util.ConvertirEntero(this.dgvProgreso["Progreso_LineaID", iReg - 1].Value)
                        && (Util.ConvertirFechaHora(this.dgvProgreso["Progreso_FechaIniciado", iReg - 1].Value) - dAvance).Days < 2)
                        continue;
                    dAvance = Util.ConvertirFechaHora(this.dgvProgreso["Progreso_FechaCompletado", iFila].Value);
                }
                */
                if (oVueltas.Count > 0)
                {
                    string sVuelta = string.Format("{0}-{1}", oReg.LineaID, iPrimeraVuelta);
                    if (oVueltas.Contains(sVuelta)) {
                        oVueltas.Remove(sVuelta);
                        continue;
                    }
                }

                // Se agrega el registro
                iFila = this.dgvProgreso.Rows.Add(oReg.LineaID, oReg.Linea, oReg.Partes, oReg.Costo, oReg.Dias);

                // Se determinan
                this.dgvProgreso["Progreso_FechaIniciado", iFila].Value = dAvance;
                dAvance = dAvance.AddDays((double)oReg.Dias.Valor());
                this.dgvProgreso["Progreso_FechaCompletado", iFila].Value = dAvance;

                // Se revisa lo de la periodicidad
                foreach (var oPer in oPeriodicidad)
                {
                    if (!oFechasPer.ContainsKey(oPer.LineaID))
                        oFechasPer.Add(oPer.LineaID, this.dtpDesde.Value);
                    if (oPer.LineaID == oReg.LineaID)
                        oFechasPer[oPer.LineaID] = dAvance;
                    if ((dAvance - oFechasPer[oPer.LineaID]).TotalDays >= oPer.Periodicidad)
                    {
                        var oLinea = oDatos.FirstOrDefault(c => c.LineaID == oPer.LineaID);
                        if (oLinea != null)
                        {
                            iFila = this.dgvProgreso.Rows.Add(oLinea.LineaID, oLinea.Linea, oLinea.Partes, oLinea.Costo, oLinea.Dias, 0, 0, 0
                                , dAvance, dAvance = dAvance.AddDays((double)oLinea.Dias));
                        }
                        oFechasPer[oPer.LineaID] = dAvance;
                    }
                }
            }
                        
            Cargando.Cerrar();
        }

        private void LlenarResultados()
        {
            Cargando.Mostrar();

            int iLineaID = Util.Entero(this.cmbResultadosLineas.SelectedValue);
            int iOpcion = this.cmbResultadosCasos.SelectedIndex;

            var oDatos = Datos.GetListOf<InventarioResultadosView>(c => c.LineaID == iLineaID
                && ((iOpcion == 0 && ((c.DiferenciaMatriz != 0 && (c.DiferenciaSuc2 != 0 || c.DiferenciaSuc3 != 0)) || (c.DiferenciaSuc2 != 0 && c.DiferenciaSuc3 != 0)))
                || (iOpcion == 1 && (c.DiferenciaMatriz == c.DiferenciaTotal || c.DiferenciaSuc2 == c.DiferenciaTotal || c.DiferenciaSuc3 == c.DiferenciaTotal)
                    && c.DiferenciaTotal != 0)
                || (iOpcion == 2 && c.DiferenciaMatriz == 0 && c.DiferenciaSuc2 == 0 && c.DiferenciaSuc3 == 0))
            );

            // Se llenan los datos
            this.dgvResultados.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                this.dgvResultados.Rows.Add(oReg.Costo, oReg.NumeroDeParte, oReg.Descripcion, oReg.Linea, oReg.Marca
                    , oReg.ExistenciaMatriz, oReg.DiferenciaMatriz, (oReg.DiferenciaMatriz * oReg.Costo)
                    , oReg.ExistenciaSuc2, oReg.DiferenciaSuc2, (oReg.DiferenciaSuc2 * oReg.Costo)
                    , oReg.ExistenciaSuc3, oReg.DiferenciaSuc3, (oReg.DiferenciaSuc3 * oReg.Costo));
            }

            Cargando.Cerrar();
        }

        private void CambioResultados(string sColumna, DataGridViewRow oFila)
        {
            switch (sColumna)
            {
                case "Resultados_DecisionMatriz":
                    oFila.Cells["Resultados_ImporteMatriz"].Value =
                        ((Util.Decimal(oFila.Cells["Resultados_DiferenciaMatriz"].Value) + Util.Decimal(oFila.Cells["Resultados_DecisionMatriz"].Value))
                        * Util.Decimal(oFila.Cells["Resultados_Costo"].Value));
                    break;
                case "Resultados_DecisionSuc2":
                    oFila.Cells["Resultados_ImporteSuc2"].Value =
                        ((Util.Decimal(oFila.Cells["Resultados_DiferenciaSuc2"].Value) + Util.Decimal(oFila.Cells["Resultados_DecisionSuc2"].Value))
                        * Util.Decimal(oFila.Cells["Resultados_Costo"].Value));
                    break;
                case "Resultados_DecisionSuc3":
                    oFila.Cells["Resultados_ImporteSuc3"].Value =
                        ((Util.Decimal(oFila.Cells["Resultados_DiferenciaSuc3"].Value) + Util.Decimal(oFila.Cells["Resultados_DecisionSuc3"].Value))
                        * Util.Decimal(oFila.Cells["Resultados_Costo"].Value));
                    break;
            }
        }

        private void CargarAsignacion()
        {
            Cargando.Mostrar();

            int iLineaID = Util.Entero(this.cmbLinea.SelectedValue);
            var oCantidad = Datos.GetListOf<InventarioLineasPartesView>(c => c.LineaID == iLineaID);
            var oCapacidad = Datos.GetListOf<InventarioUsuariosView>().OrderBy(o => o.SucursalID).ToList();

            // Se determina cuántos le corresponden a cada usuario
            var oUsuariosCant = new Dictionary<int, int>();
            bool bUsuariosRep = false;
            foreach (var oReg in oCantidad)
            {
                var oUsuarios = oCapacidad.Where(c => c.SucursalID == oReg.SucursalID).ToList();
                if (oUsuarios.Count <= 0) continue;

                int iCantPorUsuario = (oReg.Partes.Valor() / oUsuarios.Count);
                foreach (var oUsuario in oUsuarios)
                {
                    if (oUsuariosCant.ContainsKey(oUsuario.InvUsuarioID))
                        bUsuariosRep = true;
                    else
                        oUsuariosCant.Add(oUsuario.InvUsuarioID, iCantPorUsuario);
                }
                int iDiferencia = (oReg.Partes.Valor() - (iCantPorUsuario * oUsuarios.Count));
                if (iDiferencia > 0)
                    oUsuariosCant[oUsuarios[0].InvUsuarioID] += iDiferencia;
            }
            if (bUsuariosRep)
                UtilLocal.MensajeAdvertencia("Existe uno o más usuarios repetidos en las diferentes sucursales. Revisar.");

            // Se llena el grid de asignación
            this.dgvAsignacionManual.Rows.Clear();
            foreach (var oReg in oCapacidad)
            {
                int iPartes = oCantidad.SingleOrDefault(c => c.SucursalID == oReg.SucursalID).Partes.Valor();
                this.dgvAsignacionManual.Rows.Add(oReg.SucursalID, oReg.InvUsuarioID, oReg.Sucursal, iPartes, oReg.Usuario, oUsuariosCant[oReg.InvUsuarioID]);
            }

            Cargando.Cerrar();
            this.btnAsignar.Enabled = true;
        }

        private void GuardarAsignacion()
        {
            // Se valida que la línea quede completa
            var oSucPartes = new Dictionary<string, int>();
            var oSucCant = new Dictionary<string, int>();
            foreach (DataGridViewRow oFila in this.dgvAsignacionManual.Rows)
            {
                string sSucursal = Util.Cadena(oFila.Cells["Asignacion_Sucursal"].Value);
                oSucPartes[sSucursal] = Util.Entero(oFila.Cells["Asignacion_Partes"].Value);
                if (!oSucCant.ContainsKey(sSucursal))
                    oSucCant.Add(sSucursal, 0);
                oSucCant[sSucursal] += Util.Entero(oFila.Cells["Asignacion_Cantidad"].Value);
            }
            this.ctlError.LimpiarErrores();
            string sError = "";
            foreach (var oSucursal in oSucPartes)
            {
                if (oSucCant[oSucursal.Key] != oSucursal.Value)
                    sError += string.Format("La cantidad asignada a la sucursal {0}, es diferente a las partes existentes para el conteo.\n", oSucursal.Key);
            }
            if (sError != "")
            {
                this.ctlError.PonerError(this.btnAsignar, sError, ErrorIconAlignment.MiddleLeft);
                return;
            }

            this.btnAsignar.Enabled = false;
            Cargando.Mostrar();

            // Se procede a guardar los datos
            int iLineaID = Util.Entero(this.cmbLinea.SelectedValue);
            var oAvManual = Datos.GetListOf<InventarioLinea>(c => c.AvManual > 0);
            int iAvManual = (oAvManual.Count > 0 ? (oAvManual.Max(c => c.AvManual).Valor() + 1) : 1);
            var oPartes = Datos.GetListOf<Parte>(c => c.LineaID == iLineaID && c.Estatus);
            int iParte = 0, iSucursalID = 0, iCantidad, iUsuarioID;
            var oInvLineas = new Dictionary<int, int>();
            DateTime dAhora = DateTime.Now;
            foreach (DataGridViewRow oFila in this.dgvAsignacionManual.Rows)
            {
                if (Util.Entero(oFila.Cells["Asignacion_SucursalID"].Value) != iSucursalID)
                {
                    iSucursalID = Util.Entero(oFila.Cells["Asignacion_SucursalID"].Value);
                    iParte = 0;
                }
                iCantidad = Util.Entero(oFila.Cells["Asignacion_Cantidad"].Value);
                iUsuarioID = Util.Entero(oFila.Cells["Asignacion_UsuarioID"].Value);
                while (iCantidad > 0 && iParte < oPartes.Count)
                {
                    int iParteID = oPartes[iParte++].ParteID;
                    
                    // Se valida la existencia
                    var oParteEx = Datos.GetEntity<ParteExistencia>(c => c.ParteID == iParteID && c.SucursalID == iSucursalID && c.Estatus);
                    if (oParteEx.Existencia <= 0) continue;

                    // Se agrega el InventarioLinea, si no se ha agregado
                    if (!oInvLineas.ContainsKey(iSucursalID))
                    {
                        var oInvLinea = new InventarioLinea()
                        {
                            SucursalID = iSucursalID
                            , LineaID = iLineaID
                            , EstatusGenericoID = Cat.EstatusGenericos.EnCurso
                            , FechaIniciado = dAhora
                            , AvManual = iAvManual
                        };
                        Datos.Guardar<InventarioLinea>(oInvLinea);
                        oInvLineas.Add(iSucursalID, oInvLinea.InventarioLineaID);
                    }
                    // Se agrega al conteo
                    var oConteo = new InventarioConteo()
                    {
                        InventarioLineaID = oInvLineas[iSucursalID]
                        , Dia = dAhora
                        , ConteoUsuarioID = iUsuarioID
                        , ParteID = iParteID
                    };
                    Datos.Guardar<InventarioConteo>(oConteo);

                    //
                    iCantidad--;
                }
            }

            Cargando.Cerrar();
        }

        #endregion
                                                                                                
    }
}
