
using LibUtil;

namespace TheosProc
{
    public static class Consultas
    {
        public static Usuario ObtenerUsuarioDeContrasenia(string sContrasenia)
        {
            string sContraseniaEnc = UtilTheos.Encriptar(sContrasenia);
            var oUsuario = Datos.GetEntity<Usuario>(q => q.Contrasenia == sContraseniaEnc && q.Estatus);
            return oUsuario;
        }

        public static ResAcc ValidarPermiso(int iUsuarioID, string sPermiso)
        {
            var oUsuarioPerV = Datos.GetEntity<UsuariosPermisosView>(c => c.UsuarioID == iUsuarioID && c.Permiso == sPermiso);

            bool bValido = Datos.Exists<UsuariosPermisosView>(c => c.UsuarioID == iUsuarioID && c.Permiso == sPermiso);
            if (bValido)
            {
                return new ResAcc(true);
            }
            else
            {
                var oRes = new ResAcc(false);
                var oPermiso = Datos.GetEntity<Permiso>(c => c.NombrePermiso == sPermiso && c.Estatus);
                if (oPermiso == null)
                    oRes.Mensaje = "El Permiso especificado ni siquiera existe. ¡Échame la mano!";
                else
                    oRes.Mensaje = oPermiso.MensajeDeError;
                return oRes;
            }
        }
    }
}
