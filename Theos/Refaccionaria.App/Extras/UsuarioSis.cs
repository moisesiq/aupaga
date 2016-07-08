using System;
using System.Collections.Generic;

using TheosProc;

namespace Refaccionaria.App
{
    public class UsuarioSis
    {
        public int UsuarioID { get; set; }
        public string NombrePersona { get; set; }
        public string NombreUsuario { get; set; }
        public List<UsuarioPerfilesView> Perfiles { get; set; }
        public List<PerfilPermisosView> Permisos { get; set; }

        public bool VerPerfil(string sPerfil)
        {
            return this.Perfiles.Exists(q => q.NombrePerfil.Equals(sPerfil));
        }

        public bool VerPermiso(string sPermiso)
        {
            return this.Permisos.Exists(q => q.NombrePermiso.Equals(sPermiso));
        }
    }
}
