using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public class tjPresenta : FlowLayoutPanel
    {
        // Contador Interno
        private int count;

        public tjPresenta()
        {
            count = 0;
        }

        /// <summary>
        /// Agrega objetos tjPresentaItem al panel [declarar: tjPresentaItem Item = AddTPresenta()]
        /// </summary>
        /// <returns>Objeto tjPresentaItem</returns>
        public tjPresentaItem AddTPresenta()
        {
            count += 1;
            tjPresentaItem nuevo = new tjPresentaItem(count);
            nuevo.WasClicked += nuevo_WasClicked;
            this.Controls.Add(nuevo);
            return nuevo;
        }

        // evento que maneja el cambio de selección entre objetos
        private void nuevo_WasClicked(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                ((tjPresentaItem)c).UnSelect();
            }
            ((tjPresentaItem)sender).WasSelect();

            wasCliked(sender);
        }

        /// <summary>
        /// Implementar para obtener un resultado del evento ChangeSelection
        /// </summary>
        public virtual void wasCliked(object sender) { return; }

        /// <summary>
        /// Limpia el objeto y el contador interno
        /// </summary>
        public void ClearTPresenta()
        {
            this.count = 0;
            this.Controls.Clear();
        }

        /// <summary>
        /// Devuelve el Indice del Item seleccionado
        /// </summary>
        /// <returns>Indice</returns>
        public int ItemTPSelected()
        {
            int ReturnVal = -1;
            foreach (Control c in this.Controls)
            {
                if (((tjPresentaItem)c).Selected) ReturnVal = ((tjPresentaItem)c).Item;
            }

            return ReturnVal;
        }

        /// <summary>
        /// Forza la selección de un control, según su Indice
        /// </summary>
        /// <param name="index">Item a seleccionar, por defecto es el primero</param>
        public void IndexTPSelect(int index = 1)
        {
            if (this.Controls.Count > 0)
            {
                foreach (Control c in this.Controls)
                {
                    ((tjPresentaItem)c).UnSelect();
                    if (((tjPresentaItem)c).Item == index) ((tjPresentaItem)c).WasSelect();
                }

            }
        }

        /// <summary>
        /// Devuelve el Id correspondiente al registro del item seleccionado
        /// </summary>
        /// <returns>IdMaestro</returns>
        public int IdMaestro(int Index)
        {
            return ((tjPresentaItem)this.Controls[Index]).IdProvee;
        }

        /// <summary>
        /// Devuelve el Nombre correspondiente al registro del item seleccionado
        /// </summary>
        /// <returns>IdMaestro</returns>
        public string NombreProveedor(int Index)
        {
            return ((tjPresentaItem)this.Controls[Index]).Proveedor;
        }

    }
}
