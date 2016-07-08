using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace LibUtil
{
    public class CodigoDinamico
    {
        public string Nombre { get { return (this.EspacioDeNombres + "." + this.Clase); } }
        
        public List<string> Referencias { get; set; }
        public List<string> Usings { get; set; }
        public string EspacioDeNombres { get; set; }
        public string Clase { get; set; }
        public string Codigo { get; set; }
        public object Instancia { get; private set; }

        public CodigoDinamico()
        {
            this.Referencias = new List<string>();
            this.Usings = new List<string>();
            this.EspacioDeNombres = "UtilComun";
            this.Clase = "_CodigoDinamico";
        }

        public object GenerarInstancia(string sCodigo)
        {
            this.Codigo = sCodigo;
            return this.GenerarInstancia();
        }

        public object GenerarInstancia()
        {
            // Se crea el compilador
            CodeDomProvider oCompilador = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters oConfigParams = new CompilerParameters();

            // Se agregan agregan las librerías, si hubiera
            foreach (string sReferencia in this.Referencias)
                oConfigParams.ReferencedAssemblies.Add(sReferencia);
            
            // Must create a fully functional assembly as a string
            var oCodigo = new StringBuilder();
            // Se agregan los usings
            foreach (string sUsing in this.Usings)
                oCodigo.AppendLine("using " + sUsing + ";");
            // Se agrega el espacio de nombres
            oCodigo.AppendLine("namespace " + this.EspacioDeNombres + " {");
            // Se agrega la clase
            oCodigo.AppendLine("public class " + this.Clase + " {");
            // Se agrega el código
            oCodigo.AppendLine(this.Codigo);
            // Se cierra
            oCodigo.Append("}\n}");
                        
            string sCodigo = oCodigo.ToString();
            // Load the resulting assembly into memory
            //loParameters.GenerateInMemory = false;
            // Se compila el código
            CompilerResults oCompilado = oCompilador.CompileAssemblyFromSource(oConfigParams, sCodigo);
            if (oCompilado.Errors.HasErrors)
            {
                string sMensaje = ("Errores encontrados: " + oCompilado.Errors.Count.ToString() + "\n");
                var aLineasCodigo = sCodigo.Split('\n');
                for (int iCont = 0; iCont < oCompilado.Errors.Count; iCont++)
                {
                    int iLineaError = oCompilado.Errors[iCont].Line;
                    int iColError = (oCompilado.Errors[iCont].Column - 1);
                    string sLineaError = (iLineaError > 0 ? aLineasCodigo[iLineaError - 1] : "");
                    sMensaje += string.Format("\nLínea: {0} - {1} - ...{2}...",
                        oCompilado.Errors[iCont].Line,
                        oCompilado.Errors[iCont].ErrorText,
                        (iLineaError > 0
                            ? sLineaError.Substring(iColError, ((sLineaError.Length - iColError) > 32 ? 32 : (sLineaError.Length - iColError)))
                            : "")
                    );
                }
                Util.MensajeError(sMensaje, "Errores de compilación");
                return null;
            }

            // Se crea la instancia de la clase con el código a ejecutar
            Assembly oEnsamblado = oCompilado.CompiledAssembly;
            object oClase = oEnsamblado.CreateInstance(this.Nombre);
            if (oClase == null)
            {
                Util.MensajeError("No se pudo cargar la clase con el código especificado", "Error");
                return null;
            }

            this.Instancia = oClase;
            return oClase;
        }

        public object Ejecutar(string sMetodo, params object[] aParametros)
        {
            return CodigoDinamico.EjecutarMetodo(this.Instancia, sMetodo, aParametros);
        }

        #region [ Métodos estáticos ]

        static CodigoDinamico oCodigoDin;

        public static object EjecutarCodigo(string sCodigo, params object[] aParametros)
        {
            if (oCodigoDin == null)
                oCodigoDin = new CodigoDinamico();

            string sMetodo = "Ejecutar";
            sCodigo = string.Format("public object {0}(params object[] aParametros) {\n{1}\n}", sMetodo, sCodigo);
            var oInstancia = CodigoDinamico.oCodigoDin.GenerarInstancia(sCodigo);
            return CodigoDinamico.EjecutarMetodo(oInstancia, sMetodo, aParametros);
        }
        
        public static object EjecutarMetodo(object oObjeto, string sMetodo, params object[] aParametros)
        {
            try
            {
                var oMetodo = oObjeto.GetType().GetMethod(sMetodo);
                if (oMetodo == null)
                    throw new MissingMethodException();
                object oRes = oMetodo.Invoke(oObjeto, aParametros);
                // object oRes = oObjeto.GetType().InvokeMember(sMetodo, BindingFlags.InvokeMethod, null, oObjeto, aParametros);
                return oRes;
            }
            catch (Exception e)
            {
                Util.MensajeError(string.Format("Error en el método {0}\n\nError: {1}{2}", sMetodo, e.Message, 
                    (e.InnerException == null ? "" : ("\nExcepción interna: " + e.InnerException.Message))), "Error");
            }
            return null;
        }

        #endregion
    }
}
