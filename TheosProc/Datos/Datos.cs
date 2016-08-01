using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.SqlClient;
using System.Reflection;

using LibUtil;

namespace TheosProc
{
    public static class Datos
    {
        private const string DefaultContainerName = "TheosEntityContainer";
        private static ObjectContext PersistentContext;

        #region [ Propiedades ]

        public static bool PersistentContextEnabled { get; set; }
        public static string CadenaDeConexion { get; set; }

        #endregion

        #region [ Contexto ]
        
        public static void StartPersistentContext()
        {
            Datos.PersistentContext = new ObjectContext(Datos.CadenaDeConexion) { DefaultContainerName = Datos.DefaultContainerName };
            Datos.PersistentContextEnabled = true;
        }

        public static void EndPersistentContext()
        {
            Datos.PersistentContextEnabled = false;
            Datos.ReleaseDataContext(ref Datos.PersistentContext);
        }

        #endregion

        #region [ Métodos de acceso a la base de datos ]

        public static List<T> GetListOf<T>() where T : class
        {
            var context = Datos.GetDataContext();
            var oList = context.CreateObjectSet<T>().ToList();
            Datos.ReleaseDataContext(ref context);
            return oList;

            // return General.GetListOf<T>(q => q is object);
            /* using (var context = Datos.GetDataContext())
            {
                return context.CreateObjectSet<T>().ToList();
            }
            */
        }

        public static List<T> GetListOf<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var context = Datos.GetDataContext();
            var oList = context.CreateObjectSet<T>().Where(expression).ToList();
            Datos.ReleaseDataContext(ref context);
            return oList;
            /* using (var context = Datos.GetDataContext())
            {
                return context.CreateObjectSet<T>().Where(expression).ToList();
            }
            */
        }       

        public static T GetEntity<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var context = Datos.GetDataContext();
            var oEntity = context.CreateObjectSet<T>().Where(expression).FirstOrDefault();
            Datos.ReleaseDataContext(ref context);
            return oEntity;
            /* using (var context = ModelHelper.CreateDataContext())
            {
                return context.CreateObjectSet<T>().Where(expression).FirstOrDefault();
            }
            */
        }

        public static void SaveOrUpdate<T>(T oObject) where T : class
        {
            var oEntity = (oObject as EntityObject);
            if (oEntity == null) throw new ArgumentNullException("entity");

            var context = Datos.GetDataContext();
            //using (var context = ModelHelper.CreateDataContext())
            //{
            if (oEntity.EntityState.Equals(EntityState.Detached))
            {
                if (oEntity.EntityKey == null)
                {
                    ObjectStateEntry stateEntry = null;
                    context.ObjectStateManager.TryGetObjectStateEntry(oEntity, out stateEntry);
                    var objectSet = context.CreateObjectSet<T>();
                    if (stateEntry == null || stateEntry.EntityKey.IsTemporary)
                        objectSet.AddObject(oObject);
                }
                context.AddObject(oEntity.EntityKey.EntitySetName, oEntity);
            }
            if (oEntity.EntityState.Equals(EntityState.Modified))
                context.AttachUpdated(oEntity);

            context.SaveChanges();
            //}
            Datos.ReleaseDataContext(ref context);
        }

        public static void Delete<TEntity>(EntityObject entity) where TEntity : class
        {
            if (entity == null) throw new ArgumentNullException("entity");

            using (var context = Datos.GetDataContext())
            {
                context.Attach(entity);
                context.DeleteObject(entity);
                context.SaveChanges();
            }
        }

        public static List<T> ExecuteProcedure<T>(string sName, Dictionary<string, object> Parameters)
        {
            var SqlParams = new List<SqlParameter>();
            string sParamsNames = "";
            foreach (var Parameter in Parameters)
            {
                if (Parameter.Value == null)
                {
                    sParamsNames += (" @" + Parameter.Key + " = NULL,");
                }
                else
                {
                    string sParamName, sSpecialType = "";
                    sParamName = Parameter.Key;
                    if (Parameter.Key.Contains("/"))
                    {
                        sParamName = Parameter.Key.Split('/')[0];
                        sSpecialType = Parameter.Key.Split('/')[1];
                    }

                    var oSqlParam = new SqlParameter("_" + sParamName, Parameter.Value);
                    if (sSpecialType != "")
                        oSqlParam.TypeName = sSpecialType;

                    SqlParams.Add(oSqlParam);
                    sParamsNames += (" @" + sParamName + " = @_" + sParamName + ",");
                    // SqlParams.Add(new SqlParameter("_" + Parameter.Key, Parameter.Value));
                }
            }
            sParamsNames = (sParamsNames == "" ? "" : sParamsNames.Izquierda(sParamsNames.Length - 1));

            /* var Pars = new ObjectParameter[Parameters.Count]; 
            int iCount = 0;
            foreach (var Parameter in Parameters)
            {
                Pars[iCount++] = new ObjectParameter(Parameter.Key, Parameter.Value);
            } */

            var context = Datos.GetDataContext();
            var oRes = context.ExecuteStoreQuery<T>(sName + sParamsNames, SqlParams.ToArray<object>()).ToList();
            Datos.ReleaseDataContext(ref context);
            return oRes;
            /* using (var context = ModelHelper.CreateDataContext())
            {
                return context.ExecuteStoreQuery<T>(sName + sParamsNames, SqlParams.ToArray<object>()).ToList();
                // return context.ExecuteFunction<T>(sName, Pars).ToList();
            } */
        }

        public static bool Exists<T>(Expression<Func<T, bool>> oExpression) where T : class
        {
            var context = Datos.GetDataContext();
            bool bExists = context.CreateObjectSet<T>().Any(oExpression);
            Datos.ReleaseDataContext(ref context);
            return bExists;
            /* using (var context = ModelHelper.CreateDataContext())
            {
                return context.CreateObjectSet<T>().Any(oExpression);
            }
            */
        }

        public static List<T> ExecuteQuery<T>(string sCommand, params object[] oParams)
        {
            var context = Datos.GetDataContext();
            var oDatos = context.ExecuteStoreQuery<T>(sCommand, oParams).ToList();
            Datos.ReleaseDataContext(ref context);
            return oDatos;
        }

        #endregion

        #region [ Métodos auxiliares para el manejo específico de la base de datos ControlRefaccionaria ]

        public static ResAcc Guardar<T>(T Objeto, bool bLlenarDatosPredefinidos) where T : EntityObject
        {
            if (bLlenarDatosPredefinidos)
                Datos.LlenarDatosPredefinidos(Objeto);

            Datos.SaveOrUpdate<T>(Objeto);
            return new ResAcc(true);
        }

        public static ResAcc Guardar<T>(T Objeto) where T : EntityObject
        {
            return Datos.Guardar<T>(Objeto, true); ;
        }

        public static ResAcc Eliminar<T>(T oObjeto, bool bLogico) where T : EntityObject
        {
            if (bLogico)
            {
                var oTipo = oObjeto.GetType();
                var oProp = oObjeto.GetType().GetProperty("Estatus");
                if (oProp != null)
                    oProp.SetValue(oObjeto, false, null);
                return Datos.Guardar<T>(oObjeto);
            }
            else
            {
                Datos.Delete<T>(oObjeto);
                return new ResAcc(true);
            }
        }

        public static ResAcc Eliminar<T>(T oObjeto) where T : EntityObject
        {
            return Datos.Eliminar<T>(oObjeto, false);
        }

        #endregion

        #region [ Uso interno ]

        private static ObjectContext GetDataContext() {
            if (Datos.PersistentContextEnabled)
                return Datos.PersistentContext;
            else
                return new ObjectContext(Datos.CadenaDeConexion) { DefaultContainerName = Datos.DefaultContainerName };
        }

        private static void ReleaseDataContext(ref ObjectContext oContext)
        {
            if (!Datos.PersistentContextEnabled)
            {
                oContext.Dispose();
                oContext = null;
            }
        }

        private static void AttachUpdated(this ObjectContext context, EntityObject entityModified)
        {
            if (entityModified.EntityState == EntityState.Modified)
            {
                object original = null;
                if (context.TryGetObjectByKey(entityModified.EntityKey, out original))
                    context.ApplyCurrentValues(entityModified.EntityKey.EntitySetName, entityModified);
                else
                    throw new ObjectNotFoundException();
            }
        }

        private static void LlenarDatosPredefinidos(EntityObject Objeto)
        {
            Type TipoObjeto = Objeto.GetType();
            PropertyInfo Propiedad;
            DateTime dAhora = DateTime.Now;

            Propiedad = TipoObjeto.GetProperty("UsuarioID");
            if (Propiedad != null)
                Propiedad.SetValue(Objeto, Theos.UsuarioID, null);
            Propiedad = TipoObjeto.GetProperty("FechaModificacion");
            if (Propiedad != null)
                Propiedad.SetValue(Objeto, dAhora, null);

            // Valores que tiene Default en la bd
            if (Objeto.EntityState == EntityState.Detached)
            {
                Propiedad = TipoObjeto.GetProperty("Estatus");
                if (Propiedad != null)
                    Propiedad.SetValue(Objeto, true, null);
                Propiedad = TipoObjeto.GetProperty("Actualizar");
                if (Propiedad != null)
                    Propiedad.SetValue(Objeto, true, null);
                if (Objeto.EntityState == EntityState.Detached)
                {
                    Propiedad = TipoObjeto.GetProperty("FechaRegistro");
                    if (Propiedad != null)
                        Propiedad.SetValue(Objeto, dAhora, null);
                }
            }
        }

        #endregion
    }
}
