using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheosProc;

namespace Refaccionaria.App
{
    public static class CuadroControlPermisos
    {

        public static string GetTabPage;

        //se validan los permisos otorgados para llenar el combo de calculo en las pestañas de cuadro de control
        public static ArrayList ValidarPermisosCalculo()
        {
            List<UsuariosPermisosView> list = Datos.GetListOf<UsuariosPermisosView>(i => i.UsuarioID == Theos.UsuarioID);
            ArrayList per = new ArrayList();

            foreach (var x in list)
            {
                switch (x.Permiso)
                {
                    case "CuadroDeControl.Pestania.Fecha.Ver.Utilidad":
                        per.Add("Utilidad");
                        break;
                    case "CuadroDeControl.Pestania.Fecha.Ver.UtilidadDesc":
                        per.Add("Utilidad Desc.");
                        break;
                    case "CuadroDeControl.Pestania.Fecha.Ver.Precio":
                        per.Add("Precio");
                        break;
                    case "CuadroDeControl.Pestania.Fecha.Ver.Costo":
                        per.Add("Costo");
                        break;
                    case "CuadroDeControl.Pestania.Fecha.Ver.CostoDesc":
                        per.Add("Costo Desc.");
                        break;
                    case "CuadroDeControl.Pestania.Fecha.Ver.Ventas":
                        per.Add("Ventas");
                        break;
                    case "CuadroDeControl.Pestania.Fecha.Ver.Productos":
                        per.Add("Productos");
                        break;
                }
            }
            return per;
        }


        //se validan los permisos para mostrar el combo de tienda en cuadro de control
        public static List<Sucursal> ValidarPermisosTienda()
        {
            
            List<Sucursal> oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            List<UsuariosPermisosView> listaPermisos = Datos.GetListOf<UsuariosPermisosView>(i => i.UsuarioID == Theos.UsuarioID);
            List<Sucursal> listaSucursales = new List<Sucursal>();

            foreach (var x in listaPermisos)
            {
                switch (x.Permiso)
                {
                    case "CuadroDeControl.Pestania.Fecha.Ver.Sucursal1" :
                        listaSucursales.Add(oSucursales.ElementAt(0));
                        break;
                    case "CuadroDeControl.Pestania.Fecha.Ver.Sucursal2":
                        listaSucursales.Add(oSucursales.ElementAt(1));
                        break;
                    case "CuadroDeControl.Pestania.Fecha.Ver.Sucursal3":
                        listaSucursales.Add(oSucursales.ElementAt(2));
                        break;
                }
            }
            return listaSucursales;
        }


        public static ArrayList ValidarPermisosCalculoCuadroMultiple(string pagina)
        {
            List<UsuariosPermisosView> list = Datos.GetListOf<UsuariosPermisosView>(i => i.UsuarioID == Theos.UsuarioID);
            ArrayList per = new ArrayList();

            foreach (var x in list)
            {
                if ("CuadroDeControl." + pagina + ".Ver.Utilidad" == x.Permiso)
                {
                    per.Add("Utilidad");
                }
                else if ("CuadroDeControl." + pagina + ".Ver.UtilidadDesc" == x.Permiso)
                {
                    per.Add("Utilidad Desc.");
                }
                else if ("CuadroDeControl." + pagina + ".Ver.Precio" == x.Permiso)
                {
                    per.Add("Precio");
                }
                else if ("CuadroDeControl." + pagina + ".Ver.Costo" == x.Permiso)
                {
                    per.Add("Costo");
                }
                else if ("CuadroDeControl." + pagina + ".Ver.CostoDesc" == x.Permiso)
                {
                    per.Add("CostoDesc");
                }
                else if ("CuadroDeControl." + pagina + ".Ver.Ventas" == x.Permiso)
                {
                    per.Add("Ventas");
                }
                else if ("CuadroDeControl." + pagina + ".Ver.Productos" == x.Permiso)
                {
                    per.Add("Productos");
                }
            }
            return per;
        }

        public static List<Sucursal> ValidarPermisosTiendaCuadroMultiple(string pagina)
        {
            List<Sucursal> oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            List<UsuariosPermisosView> listaPermisos = Datos.GetListOf<UsuariosPermisosView>(i => i.UsuarioID == Theos.UsuarioID);
            List<Sucursal> listaSucursales = new List<Sucursal>();

            foreach (var x in listaPermisos)
            {
                if( x.Permiso == "CuadroDeControl." + pagina + ".Ver.Sucursal1")
                {
                    listaSucursales.Add(oSucursales.ElementAt(0));
                }
                else if( x.Permiso == "CuadroDeControl." + pagina + ".Ver.Sucursal2")
                {
                    listaSucursales.Add(oSucursales.ElementAt(1));
                }
                else if( x.Permiso == "CuadroDeControl." + pagina + ".Ver.Sucursal3")
                {
                    listaSucursales.Add(oSucursales.ElementAt(2));
                }
                
            }
            return listaSucursales;
        }


        public static bool ValidarTodosPermisos(string pagina)
        {
            if (ValidarPermisosCalculoCuadroMultiple(pagina).Count <= 0 || ValidarPermisosTiendaCuadroMultiple(pagina).ToArray().Length <= 0)
            {
                UtilLocal.MensajeAdvertencia("No existen permisos para esta página");
                return false;
            }
            return true;
        }

    }
}
