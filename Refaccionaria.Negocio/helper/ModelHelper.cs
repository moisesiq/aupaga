using Refaccionaria.Modelo;

namespace Refaccionaria.Negocio
{
    public static class ModelHelper
    {
        private static bool PersistentContextEnabled = false;
        private static DataContext PersistentContext;

        public static string CadenaDeConexion { get; set; }

        public static DataContext CreateDataContext()
        {
            return (ModelHelper.PersistentContextEnabled ? ModelHelper.PersistentContext : new DataContext(ModelHelper.CadenaDeConexion));

            /*
            //var stringConnection = System.Configuration.ConfigurationManager.ConnectionStrings["DataContext"].ToString();
            var stringConnection = ModelHelper.CadenaDeConexion;
            var context = new Refaccionaria.Modelo.DataContext(stringConnection);
            //context.Usuario.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            return context;
            */
        }

        public static void ReleaseDataContext(ref DataContext oContext)
        {
            if (!ModelHelper.PersistentContextEnabled)
            {
                oContext.Dispose();
                oContext = null;
            }
        }

        public static void StartPersistentContext()
        {
            ModelHelper.PersistentContext = new DataContext(ModelHelper.CadenaDeConexion);
            ModelHelper.PersistentContextEnabled = true;
        }

        public static void EndPersistentContext()
        {
            ModelHelper.PersistentContextEnabled = false;
            ModelHelper.ReleaseDataContext(ref ModelHelper.PersistentContext);
        }
    }
}
