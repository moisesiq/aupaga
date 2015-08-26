using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Data.SqlClient;

using System.Data.Entity;
using System.Data.EntityClient;

namespace Refaccionaria.Negocio
{
    public static class General
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static List<T> GetListOf<T>(Expression<Func<T, bool>> expression) where T : class
        {
            using (var context = ModelHelper.CreateDataContext())
            {
                return context.CreateObjectSet<T>().Where(expression).ToList();
            }
        }

		public static List<T> GetListOf<T>() where T : class
        {
            // return General.GetListOf<T>(q => q is object);
            using (var context = ModelHelper.CreateDataContext())
            {
                return context.CreateObjectSet<T>().ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T GetEntity<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var context = ModelHelper.CreateDataContext();
            var oEntity = context.CreateObjectSet<T>().Where(expression).FirstOrDefault();
            ModelHelper.ReleaseDataContext(ref context);
            return oEntity;
            /* using (var context = ModelHelper.CreateDataContext())
            {
                return context.CreateObjectSet<T>().Where(expression).FirstOrDefault();
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitySetName"></param>
        /// <param name="nameId"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static T GetEntityById<T>(string nameId, int Id) where T : class
        {
            object val = null;
            string entitySetName = typeof(T).Name;
            using (var context = ModelHelper.CreateDataContext())
            {   // Create the EntityKey                
                var keyValues = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(nameId, Id) };
                var key = new EntityKey(string.Format("{0}.{1}", context.DefaultContainerName, entitySetName), keyValues);
                context.TryGetObjectByKey(key, out val); // Try to get the Object using the Key                
            }
            return val as T; // cast the object as T
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitySetName"></param>
        /// <param name="nameId"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static T GetEntityById<T>(string entitySetName, string nameId, int Id) where T : class
        {
            object val = null;
            using (var context = ModelHelper.CreateDataContext())
            {   // Create the EntityKey                
                var keyValues = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(nameId, Id) };
                var key = new EntityKey(string.Format("{0}.{1}", context.DefaultContainerName, entitySetName), keyValues);
                context.TryGetObjectByKey(key, out val); // Try to get the Object using the Key                
            }
            return val as T; // cast the object as T
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="entiti"></param>
        public static void SaveOrUpdate<TEntity>(EntityObject entity, TEntity entiti) where TEntity : class
        {
            if (entity == null) throw new ArgumentNullException("entity");

            var context = ModelHelper.CreateDataContext();
            //using (var context = ModelHelper.CreateDataContext())
            //{
                if (entity.EntityState.Equals(EntityState.Detached))
                {
                    if (entity.EntityKey == null)
                    {
                        ObjectStateEntry stateEntry = null;
                        context.ObjectStateManager.TryGetObjectStateEntry(entity, out stateEntry);
                        var objectSet = context.CreateObjectSet<TEntity>();
                        if (stateEntry == null || stateEntry.EntityKey.IsTemporary)
                            objectSet.AddObject(entiti);
                    }
                    context.AddObject(entity.EntityKey.EntitySetName, entity);
                }
                if (entity.EntityState.Equals(EntityState.Modified))
                    context.AttachUpdated(entity);

                context.SaveChanges();
            //}
            ModelHelper.ReleaseDataContext(ref context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public static void Delete<TEntity>(EntityObject entity) where TEntity : class
        {
            if (entity == null) throw new ArgumentNullException("entity");

            using (var context = ModelHelper.CreateDataContext())
            {
                context.Attach(entity);
                context.DeleteObject(entity);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sName"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
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

            var context = ModelHelper.CreateDataContext();
            var oRes = context.ExecuteStoreQuery<T>(sName + sParamsNames, SqlParams.ToArray<object>()).ToList();
            ModelHelper.ReleaseDataContext(ref context);
            return oRes;
            /* using (var context = ModelHelper.CreateDataContext())
            {
                return context.ExecuteStoreQuery<T>(sName + sParamsNames, SqlParams.ToArray<object>()).ToList();
                // return context.ExecuteFunction<T>(sName, Pars).ToList();
            } */
        }

        public static bool Exists<T>(Expression<Func<T, bool>> oExpression) where T : class
        {
            var context = ModelHelper.CreateDataContext();
            bool bExists = context.CreateObjectSet<T>().Any(oExpression);
            ModelHelper.ReleaseDataContext(ref context);
            return bExists;
            /* using (var context = ModelHelper.CreateDataContext())
            {
                return context.CreateObjectSet<T>().Any(oExpression);
            }
            */
        }
        
        public static List<T> ExecuteQuery<T>(string sCommand, params object[] oParams)
        {
            var context = ModelHelper.CreateDataContext();
            var oDatos = context.ExecuteStoreQuery<T>(sCommand, oParams).ToList();
            ModelHelper.ReleaseDataContext(ref context);
            return oDatos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityModified"></param>
        public static void AttachUpdated(this ObjectContext context, EntityObject entityModified)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetEntitySetFullName(this ObjectContext context, EntityObject entity)
        {
            // If the EntityKey exists, simply get the Entity Set name from the key
            if (entity.EntityKey != null)
            {
                return entity.EntityKey.EntitySetName;
            }
            else
            {
                string entityTypeName = entity.GetType().Name;
                var container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);
                string entitySetName = (from meta in container.BaseEntitySets
                                        where meta.ElementType.Name == entityTypeName
                                        select meta.Name).First();

                return container.Name + "." + entitySetName;
            }
        }

        /// <summary>
        /// </summary>
        /// <example>
        /// ListViewControl.CustomInvoke(lv => lv.Items.Add(lv))
        /// this.InvokeEx(f => f.listView1.Items.Clear());        
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlToInvoke"></param>
        /// <param name="actionToPerform"></param>
        public static void CustomInvoke<T>(this T controlToInvoke, Action<T> actionToPerform) where T : ISynchronizeInvoke
        {            
            if (controlToInvoke.InvokeRequired)
                controlToInvoke.Invoke(actionToPerform, new object[] { controlToInvoke });
            else
                actionToPerform(controlToInvoke);
        }

        /// <summary>
        /// </summary>
        /// <example>Refaccionaria.Negocio.InvokeControlAction<Label>(lblTime, lbl => lbl.Text = String.Format("The current time is: {0}", DateTime.Now.ToString("h:mm:ss tt")));</example>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlToInvoke"></param>
        /// <param name="action"></param>
        public static void InvokeControlAction<T>(T controlToInvoke, Action<T> actionToPerform) where T : Control
        {
            if (controlToInvoke.InvokeRequired)
                controlToInvoke.Invoke(new Action<T, Action<T>>(InvokeControlAction), new object[] { controlToInvoke, actionToPerform });
            else
                actionToPerform(controlToInvoke);
        }
    }
}
