/* ****************************************************************************
 * Motivo    : Procesos de la aplicación.                                     *
 * Autor     : Moisés Iquingari González Paredes                              *
 * Creado    : 2015/07/14                                                     *
 * Ult. Mod. : 2015/07/22                                                     *
 **************************************************************************** */

window.proc = {
	VerSesion: function() {
		/* var sUsuarioID = window.localStorage.getItem("UsuarioID");
		if (sUsuarioID) {
			var sSucursalID = window.localStorage.getItem("SucursalID");
			window.local.Ajax({
				url: window.local.RutaMetodo("Servicio", "UsuarioSucursal")
				, data: { eParam1: sUsuarioID, eParam2: sSucursalID }
				, success: function(oRes) {
					window.config.Usuario = {
						UsuarioID: oRes.Respuesta.UsuarioID
						, NombreUsuario: oRes.Respuesta.NombreUsuario
						, NombrePersona: oRes.Respuesta.NombrePersona
					};
					window.config.Sucursal = {
						SucursalID: oRes.Respuesta.SucursalID
						, NombreSucursal: oRes.Respuesta.NombreSucursal
					};
					window.proc.InicializarSesion();
				}
			});
		} else {
			window.location.href = "index.html";
		}
		*/
		
		// Se verifica si existe una sesión activa
		var sIdSesion = window.localStorage.getItem("IdSesion");
		window.config.IdSesion = sIdSesion;
		// 
		window.local.Ajax({
			url: window.local.RutaMetodo("Servicio", "DatosDeAcceso")
			, success: function(oRes) {
				if (oRes && oRes.Exito)
					window.proc.InicializarSesion(oRes.Respuesta);
				else
					window.location.href = "index.html";
			}
		});
	}

	/* , InicializarDatosDeAcceso: function(iUsuarioID, iSucursalID) {
		window.localStorage.setItem("UsuarioID", iUsuarioID);
		window.localStorage.setItem("SucursalID", iSucursalID);
		window.util.setCookie("UsuarioID", iUsuarioID, 1);
		window.util.setCookie("SucursalID", iSucursalID, 1);
	}
	*/

	, Inicializado: null
	, InicializarSesion: function(oAcceso) {
		window.config.Acceso = oAcceso;
		window.contenido.InfoAcceso();

		// Se ejecuta la función Inicializado, si existe
		if (this.Inicializado)
			this.Inicializado();
	}

	, CerrarSesion: function() {
		//
		window.localStorage.clear();
		/* window.util.clearCookie("UsuarioID");
		window.util.clearCookie("SucursalID");
		*/
		
		// Se manda borrar la sesión del servidor
		window.local.Ajax({
			url: window.local.RutaMetodo("Servicio", "CerrarSesion")
			, success: function(oRes) {
				window.location.href = "index.html";
			}
		});
	}

	, ReportarErrorReiniciar: function(oRes) {
		window.contenido.MensajeAdvertencia(oRes.Mensaje);
		if (oRes.Codigo == window.local.Codigos.SesionCaducada)
			window.setTimeout(function() { window.location.href = "index.html"; }, 2000);
	}
}
