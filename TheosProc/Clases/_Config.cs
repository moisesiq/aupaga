using System;
using System.Collections.Generic;
using System.Linq;

namespace TheosProc
{
    public static class Config
    {
        public static string Valor(string sParametro)
        {
            // Se busca el parámetro con la sucursal actual
            string sValor = Config.Valor(Theos.SucursalID, sParametro);
            // Si no se encontró, se busca en los parámetros globales
            if (sValor == null)
                return Config.Valor(0, sParametro);
            else
                return sValor;
        }

        public static Dictionary<string, string> ValoresVarios(params string[] Parametros)
        {
            // Se buscan los parámetros con la sucursal actual
            var Valores = Config.ValoresVarios(Theos.SucursalID, Parametros);
            // Se buscan los parámetros globales
            var ValoresGlobal = Config.ValoresVarios(0, Parametros);
            // Se juntan los valores
            foreach (var ValorG in ValoresGlobal)
            {
                if (!Valores.ContainsKey(ValorG.Key))
                    Valores.Add(ValorG.Key, ValorG.Value);
            }

            return Valores;
        }

        public static Dictionary<string, string> ValoresVarios(string sLike)
        {
            // Se buscan los parámetros con la sucursal actual
            var Valores = Config.ValoresVarios(Theos.SucursalID, sLike);
            // Se buscan los parámetros globales
            var ValoresGlobal = Config.ValoresVarios(0, sLike);
            // Se juntan los valores
            foreach (var ValorG in ValoresGlobal)
            {
                if (!Valores.ContainsKey(ValorG.Key))
                    Valores.Add(ValorG.Key, ValorG.Value);
            }

            return Valores;
        }

        public static string Valor(int iSucursalID, string sParametro)
        {
            var oConfig = Datos.GetEntity<Configuracion>(q => q.Nombre == sParametro && q.SucursalID == iSucursalID);
            return (oConfig == null ? null : oConfig.Valor);
        }

        public static Dictionary<string, string> ValoresVarios(int iSucursalID, params string[] Parametros)
        {
            var Res = new Dictionary<string, string>();
            var oConfigs = Datos.GetListOf<Configuracion>(q => Parametros.Contains(q.Nombre) && q.SucursalID == iSucursalID);
            foreach (var oConfig in oConfigs)
                Res.Add(oConfig.Nombre, oConfig.Valor);
            return Res;
        }

        public static Dictionary<string, string> ValoresVarios(int iSucursalID, string sLike)
        {
            var Res = new Dictionary<string, string>();
            var oConfigs = Datos.GetListOf<Configuracion>(q => q.Nombre.Contains(sLike) && q.SucursalID == iSucursalID);
            foreach (var oConfig in oConfigs)
                Res.Add(oConfig.Nombre, oConfig.Valor);
            return Res;
        }

        public static bool EstablecerValor(string sParametro, string sValor)
        {
            // Se busca el parámetro con la sucursal actual
            bool bGuardado = Config.EstablecerValor(Theos.SucursalID, sParametro, sValor);
            // Si no se encontró, se busca en los parámetros globales
            if (bGuardado)
                return true;
            else
                return Config.EstablecerValor(0, sParametro, sValor);
        }

        public static bool EstablecerValor(int iSucursalID, string sParametro, string sValor)
        {
            var oConfig = Datos.GetEntity<Configuracion>(q => q.Nombre == sParametro && q.SucursalID == iSucursalID);
            if (oConfig == null)
            {
                return false;
            }
            else
            {
                oConfig.Valor = sValor;
                Datos.Guardar<Configuracion>(oConfig);
                return true;
            }
        }

        //public static string Local
    }
}
