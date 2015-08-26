using System;
using System.Collections.Generic;
using System.Collections;

namespace Refaccionaria.Negocio
{
    public class ListaEstatus<T> : IList<ObjetoEstatus<T>>
    {
        public List<ObjetoEstatus<T>> Lista { get; set; }

        public ListaEstatus()
        {
            this.Lista = new List<ObjetoEstatus<T>>();
        }
        
        public ObjetoEstatus<T> this[string sLLave]
        {
            get
            {
                foreach (ObjetoEstatus<T> Objeto in this.Lista)
                {
                    if (Objeto.Llave == sLLave)
                        return Objeto;
                }
                return null;
            }
        }

        public List<T> ObtenerLista()
        {
            var Lista = new List<T>();
            foreach (var Objeto in this.Lista)
                Lista.Add(Objeto.Objeto);
            return Lista;
        }

        /* private bool VerLlaveRepetida(string sLLave)
        {

        } */

        #region [ IList ]

        public int IndexOf(ObjetoEstatus<T> item)
        {
            return this.Lista.IndexOf(item);
        }

        public void Insert(int index, ObjetoEstatus<T> item)
        {
            this.Lista.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.Lista.RemoveAt(index);
        }

        public ObjetoEstatus<T> this[int index]
        {
            get
            {
                return this.Lista[index];
            }
            set
            {
                this.Lista[index] = value;
            }
        }

        public void Add(ObjetoEstatus<T> item)
        {
            this.Lista.Add(item);
        }

        public void Clear()
        {
            this.Lista.Clear();
        }

        public bool Contains(ObjetoEstatus<T> item)
        {
            return this.Lista.Contains(item);
        }

        public void CopyTo(ObjetoEstatus<T>[] array, int arrayIndex)
        {
            this.Lista.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.Lista.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ObjetoEstatus<T> item)
        {
            return this.Lista.Remove(item);
        }

        IEnumerator<ObjetoEstatus<T>> IEnumerable<ObjetoEstatus<T>>.GetEnumerator()
        {
            return this.Lista.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this.Lista.GetEnumerator());
        }

        #endregion
    }


    public class ObjetoEstatus<T>
    {
        public string Llave { get; set; }
        public int IdEstatus { get; set; }
        public T Objeto { get; set; }

        public ObjetoEstatus(string sLlave, int iIdEstatus, T Objeto)
        {
            this.Llave = sLlave;
            this.IdEstatus = iIdEstatus;
            this.Objeto = Objeto;
        }
    }
}
