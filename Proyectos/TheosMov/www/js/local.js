/* ****************************************************************************
 * Motivo    : Utilidades de Javascript locales a la aplicación               *
 * Autor     : Moisés Iquingari González Paredes                              *
 * Creado    : 2015/06/26                                                     *
 * Ult. Mod. : 2015/07/22                                                     *
 **************************************************************************** */

window.local = {
	Cargando: {
		Mostrar: function() {
			$("<img id='_imgCargando' class='centrado' src='img/64_Procesando.gif' />")
				.css("margin", "-32px 0px 0px -32px").appendTo("body");
		}
		, Cerrar: function() {
			$("#_imgCargando").remove();
		}
	}

	, RutaMetodo: function(sServicio, sMetodo) {
		return (window.config.Servidor + sServicio + "/" + sMetodo);
	}

	, Ajax: function(oParams) {
		var bCargando = true;
		if (oParams.Cargando != undefined && oParams.Cargando === false)
			bCargando = false;
		if (bCargando)
			window.local.Cargando.Mostrar();
		if (!oParams.complete)
			oParams.complete = function() { window.local.Cargando.Cerrar(); };

		// Se agregan los datos de la sesión, si aplica
		if (window.config.IdSesion)
			oParams.headers = { IdSesion: window.config.IdSesion };
			
		var oPred = {
			type: "post"
			, dataType: "json"
			, error: function(oAjax, sEstatus, sError) {
				console.log(oAjax);
				window.contenido.MensajeAdvertencia("Hubo un error al realizar la petición."
					+ "<br /><br />" + sError
					+ "<br /><br />" + oAjax.responseText);
				console.log("Error en la llamada Ajax.");
				console.log("Estatus: " + sEstatus);
				console.log("Error: " + sError);
			}
		}

		var oConfig = $.extend({}, oPred, oParams);
		$.ajax(oConfig);
	}

	, TablaArreglo: function(jTabla) {
		var aDatos = [];
		jTabla.find("tbody tr").each(function() {
			var oFila = {};
			$(this).find("input, select, textarea").each(function() {
				var sNombre = $(this).attr("name");
				oFila[sNombre] = $(this).val();
			});
			aDatos.push(oFila);
		});
		return aDatos;
	}
	
	, Ef: {
		Fecha: function(sFecha) {
			sFecha = sFecha.slice(6, -2);
			var dFecha = new Date();
			dFecha.setTime(sFecha);
			return dFecha.toLocaleString();
		}

		, Cadena: function(sDato) {
			return (sDato ? sDato : "");
		}
	}
}
