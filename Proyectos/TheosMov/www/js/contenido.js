/* ****************************************************************************
 * Motivo    : Código para inicializar componetes comunes en la apliación.    *
 * Autor     : Moisés Iquingari González Paredes                              *
 * Creado    : 2015/07/07                                                     *
 * Ult. Mod. : 2015/07/22                                                     *
 **************************************************************************** */

// Se configuran algunas opciones de phonegap
$(function() {
	document.addEventListener("deviceready", function() {
		document.addEventListener("backbutton", function(e) {
			e.preventDefault();
			if (window.confirm("¿Estás seguro que deseas cerrar la aplicación?"))
				navigator.app.exitApp();
		}, true);
	}, true);
});

//
window.contenido = {
	Inicializar: function(sId) {
		this.Encabezados();
		this.Panel(sId);
	}

	, Encabezados: function() {
		var jEncabezados = $("div.Pagina div.Encabezado");

		//
		jEncabezados.append(window.html.img("img/32_Garibaldi.png", "AG", { "data-id": "imgIcono" }));
		jEncabezados.append(window.html.span(window.config.Aplicacion + " ", { "data-id": "spanAplicacion" }));
		jEncabezados.append(window.html.span(window.config.Version + " ", { "data-id": "spanVersion" }));
		jEncabezados.append(window.html.span("", { "data-id": "spanSucursal" }));
		jEncabezados.append(window.html.span("", { "data-id": "spanUsuario" }));
		jEncabezados.append(window.html.a("", "#divPanel", { "data-id": "aMenu"
			, class: "ui-btn ui-btn-right ui-icon-bars ui-corner-all ui-btn-icon-notext"
			, style: "display: none;" }));
		
		jEncabezados.find("[data-id=imgIcono]").click(function() {
			window.contenido.MensajeInfo(
				'<div style="white-space: pre; font-family: monospace;">'
				+ JSON.stringify(window.config, null, 4)
				+ '</div>'
			);
		});
	}

	, Panel: function(sId) {
		var jPanel = $("#divPanel");

		// Se agrega el panel
		var sMenu = ' \
			<ul> \
				<li><a id="aVentas" href="#" class="ui-btn ui-btn-icon-left ui-icon-carat-d">Ventas</a></li> \
				<li> \
					<a id="aAdministracion" href="#" class="ui-btn ui-btn-icon-left ui-icon-carat-d">Administración</a> \
					<ul> \
						<li><a id="aTraspasos" href="traspasos.html" data-ajax="false" class="ui-btn">Traspasos</a></li> \
						<li><a id="aInventario" href="inventario.html" data-ajax="false" class="ui-btn">Inventario</a></li> \
					</ul> \
				</li> \
				<li><a id="aConfiguracion" href="#" class="ui-btn ui-btn-icon-left ui-icon-carat-d">Configuración</a></li> \
				<li><a id="aCerrarSesion" href="#" class="ui-btn">Cerrar sesión</a></li> \
			</ul> \
		';
		jPanel.panel();
		jPanel.append(sMenu);

		// Se ocultan los submenús
		jPanel.find("ul li ul").hide();

		// Se configura el swipe
		$(window).on("swipeleft", function(e) {
			var bVerMenu = ($("a[data-id=aMenu]:visible").length > 0);
			var mDistSwipe = $(window).width() * 0.85;
			if (e.swipestart.coords[0] > mDistSwipe && bVerMenu) {
				jPanel.panel("open");
			}
		});
		
		// Se definen los eventos --------------------------------------------------

		$("#aAdministracion").click(function(e) {
			e.preventDefault();
			var jSub = $(this).next("ul");
			if (jSub.filter(":visible").length > 0) {
				jSub.hide();
				$(this).removeClass("ui-icon-carat-u").addClass("ui-icon-carat-d");
			} else {
				jSub.show();
				$(this).removeClass("ui-icon-carat-d").addClass("ui-icon-carat-u");
			}
		});

		$("#aCerrarSesion").click(function(e) {
			e.preventDefault();
			window.proc.CerrarSesion();
		});
	}

	, InfoAcceso: function() {
		$("[data-id=spanSucursal]").text(" - " + window.config.Acceso.Sucursal);
		$("[data-id=spanUsuario]").text(" - " + window.config.Acceso.Usuario);
		$("[data-id=aMenu]").fadeIn();
	}

	, MostrarAcceso: function(fAcceso, sPermiso) {
		var jAcceso = $("#divAccesoPopup");
		var bCancel = false;
		if (jAcceso.length > 0) {
			jAcceso.find("input").val("");
		} else {
			var sHtml = '\
				<div id="divAccesoPopup" data-role="popup" data-theme="a" data-overlay-theme="b"> \
					<form action="#"> \
						<h4>Validación de usuario</h4> \
						<div class="ui-input-text ui-body-a ui-corner-all ui-shadow-inset"> \
							<input type="password" name="Contrasenia" placeholder="Contraseña" /> \
						</div> \
						<span class="Error"></span> \
						<!-- <input type="button" class="Cancelar ui-btn ui-btn-a ui-corner-all ui-btn-icon-left ui-icon-delete" value="Cancelar" /> --> \
						<!-- <input type="button" class="Aceptar ui-btn ui-btn-b ui-corner-all ui-btn-icon-right ui-icon-check" value="Aceptar" /> --> \
					</form> \
				</div> \
			';
			jAcceso = $(sHtml).appendTo("body");
			jAcceso.popup();
			//
			jAcceso.find("input.Cancelar").click(function() {
				jAcceso.popup("close");
			});
			jAcceso.find("input.Aceptar").click(function() {
				jAcceso.find("form").submit();
			});
		}

		jAcceso.find("form").submit(function() {
			var sContrasenia = jAcceso.find("input[name=Contrasenia]").val();
			window.local.Ajax({
				url: window.local.RutaMetodo("Servicio", "ValidarUsuario")
				, data: { eParam1: sContrasenia, eParam2: sPermiso }
				, success: function(oRes) {
					if (oRes && oRes.Exito) {
						jAcceso.popup("close");
						fAcceso(oRes.Respuesta);
					} else {
						jAcceso.find("span.Error").text("* " + oRes.Mensaje);
					}
				}
			});
			return false;
		});

		jAcceso.popup("open");
		jAcceso.find("input[name=Contrasenia]").focus();
	}

	, MensajePregunta: function(sPregunta, fSi, fNo) {
		var sHtml = '\
			<div class="popPregunta" data-role="popup" data-theme="a" data-overlay-theme="b" data-dismissible="false"> \
				<h5></h5> \
				<a data-id="aSi" href="#" class="ui-btn ui-corner-all ui-btn-inline ui-btn-b">Sí</a> \
				<a data-id="aNo" href="#" class="ui-btn ui-corner-all ui-btn-inline ui-btn-b">No</a> \
			</div> \
		';
		var jMensaje = $(sHtml).appendTo("body");
		jMensaje.find("h5").text(sPregunta);
		jMensaje.find("a[data-id=aSi]").click(function(e) {
			e.preventDefault();
			jMensaje.popup("close");
			jMensaje.remove();
			if (fSi) fSi();
		});
		jMensaje.find("a[data-id=aNo]").click(function(e) {
			e.preventDefault();
			jMensaje.popup("close");
			jMensaje.remove();
			if (fNo) fNo();
		});
		jMensaje.popup();
		jMensaje.popup("open");
	}

	, Mensaje: function(sMensaje, sClase) {
		var sHtml = '\
			<div class="' + sClase + '" data-role="popup" data-theme="a" data-overlay-theme="b"> \
				<h5></h5> \
				<a href="#" class="ui-btn ui-corner-all ui-btn-a">Aceptar</a> \
			</div> \
		';
		var jMensaje = $(sHtml).appendTo("body");
		jMensaje.find("h5").html(sMensaje);
		jMensaje.find("a").click(function(e) {
			e.preventDefault();
			jMensaje.popup("close");
			jMensaje.remove();
		});
		jMensaje.popup();
		jMensaje.popup("open");
	}
	, MensajeAdvertencia: function(sMensaje) {
		window.contenido.Mensaje(sMensaje, "popAdvertencia");
	}
	, MensajeInfo: function(sMensaje) {
		window.contenido.Mensaje(sMensaje, "popInfo");
	}

	, Notificacion: function(sMensaje, iDuracion) {
		if (!iDuracion) iDuracion = 2000;
		var sHtml = '\
			<div class="popNotificacion" data-role="popup" data-theme="a" data-overlay-theme="b"> \
				<h5></h5> \
			</div> \
		';
		var jMensaje = $(sHtml).appendTo("body");
		jMensaje.find("h5").text(sMensaje);
		jMensaje.popup();
		jMensaje.popup("open");
		window.setTimeout(function() { jMensaje.popup("close"); }, iDuracion)
	}

	, TecladoNumerico: function(oContenedor) {
		var sNumerico = '\
			<div class="TecladoNumerico"> \
				<div> \
					<a class="borrar" data-val="b" href="#">Borrar</a> \
					<!-- <a class="esconder" data-val="e" href="#">v</a> --> \
				</div> \
				<div> \
					<a class="siete" data-val="7" href="#">7</a> \
					<a class="ocho" data-val="8" href="#">8</a> \
					<a class="nueve" data-val="9" href="#">9</a> \
				</div> \
				<div> \
					<a class="cuatro" data-val="4" href="#">4</a> \
					<a class="cinco" data-val="5" href="#">5</a> \
					<a class="seis" data-val="6" href="#">6</a> \
				</div> \
				<div> \
					<a class="uno" data-val="1" href="#">1</a> \
					<a class="dos" data-val="2" href="#">2</a> \
					<a class="tres" data-val="3" href="#">3</a> \
				</div> \
				<div> \
					<a class="cero" data-val="0" href="#">0</a> \
					<a class="punto" data-val="." href="#">.</a> \
					<a class="siguiente" data-val="s" href="#">&gt;</a> \
				</div> \
			</div> \
		';
		jNumerico = $(sNumerico).appendTo($(oContenedor));
		jNumerico.find("a").click(function(e) {
		var jNumerico = $(this).parent().parent();
			e.preventDefault();
			if (!jNumerico[0].oInput)
				return;

			var oInput = jNumerico[0].oInput;
			var jInput = $(oInput);
			var sCaracter = $(this).attr("data-val");
			var sValor = oInput.value;
			switch (sCaracter) {
				case "b":
					oInput.value = sValor.substr(0, (sValor.length - 1));
					jInput.focus();
					break;
				case "s":
					var jForm = $(oInput).parentsUntil("form").parent();
					var jInputs = jForm.find("input").filter(":visible:enabled");
					var oSig = jInputs.get(jInputs.index(oInput) + 1);
					if (oSig) {
						var iAltoTn = 0; // oInput.jNumerico.height();
						// window.contenido.EsconderTecladoNumerico(oInput);
						jInput.focusout();
						jInput = $(oSig);
						// Se verifica si el control es visible
						var iVisibleA = (window.innerHeight - iAltoTn);
						var oRectIn = oSig.getBoundingClientRect();
						if (oRectIn.bottom >= iVisibleA) {
							window.scrollBy(0, oRectIn.bottom - iVisibleA + 8);
							// $.mobile.silentScroll(oRectIn.bottom + 40);
						}
					}
					jInput.focus();
					break;
				case "e":
					window.contenido.EsconderTecladoNumerico(oInput);
					break;
				default:
					oInput.value += sCaracter;
					jInput.focus();
					break;
			}
		});
	}

	// Al final ya no se usó
	, TecladoNumericoAnt: function(oContenedor, oInput) {
		/* if (oInput.jNumerico !== undefined) {
			if (oInput.jNumerico === false)
				oInput.jNumerico = undefined;
			return;
		}
		*/

		var jInput = $(oInput);
		jInput.attr("readonly", "readonly");
		jInput.focusout(function() {
			/* var jNum = $("div.TecladoNumerico");
			window.setTimeout(function() {
				if (document.activeElement.parentElement.parentElement
				&& document.activeElement.parentElement.parentElement.className === "TecladoNumerico"
				|| document.activeElement === oInput)
					return;
				window.contenido.EsconderTecladoNumerico(oInput);
			}, 0);
			*/
			$(this).removeAttr("readonly");
		});
		// jInput.blur();
		
		// Se verifica si ya existe el teclado numérico
		var jNumerico = $("div.TecladoNumerico");
		if (jNumerico.length <= 0) {
			var sNumerico = '\
				<div class="TecladoNumerico"> \
					<div> \
						<a class="borrar" data-val="b" href="#">Borrar</a> \
						<!-- <a class="esconder" data-val="e" href="#">v</a> --> \
					</div> \
					<div> \
						<a class="siete" data-val="7" href="#">7</a> \
						<a class="ocho" data-val="8" href="#">8</a> \
						<a class="nueve" data-val="9" href="#">9</a> \
					</div> \
					<div> \
						<a class="cuatro" data-val="4" href="#">4</a> \
						<a class="cinco" data-val="5" href="#">5</a> \
						<a class="seis" data-val="6" href="#">6</a> \
					</div> \
					<div> \
						<a class="uno" data-val="1" href="#">1</a> \
						<a class="dos" data-val="2" href="#">2</a> \
						<a class="tres" data-val="3" href="#">3</a> \
					</div> \
					<div> \
						<a class="cero" data-val="0" href="#">0</a> \
						<a class="punto" data-val="." href="#">.</a> \
						<a class="siguiente" data-val="s" href="#">&gt;</a> \
					</div> \
				</div> \
			';
			jNumerico = $(sNumerico).appendTo($(oContenedor));
			jNumerico.find("a").click(function(e) {
				e.preventDefault();
				var oInput = jNumerico.oInput;
				var jInput = $(oInput);
				var sCaracter = $(this).attr("data-val");
				var sValor = oInput.value;
				switch (sCaracter) {
					case "b":
						oInput.value = sValor.substr(0, (sValor.length - 1));
						jInput.focus();
						break;
					case "s":
						var jForm = $(oInput).parentsUntil("form").parent();
						var jInputs = jForm.find("input").filter(":visible:enabled");
						var oSig = jInputs.get(jInputs.index(oInput) + 1);
						if (oSig) {
							var iAltoTn = 0; // oInput.jNumerico.height();
							window.contenido.EsconderTecladoNumerico(oInput);
							jInput = $(oSig);
							// Se verifica si el control es visible
							var iVisibleA = (window.innerHeight - iAltoTn);
							var oRectIn = oSig.getBoundingClientRect();
							if (oRectIn.bottom >= iVisibleA) {
								window.scrollBy(0, oRectIn.bottom - iVisibleA + 8);
								// $.mobile.silentScroll(oRectIn.bottom + 40);
							}
						}
						jInput.focus();
						break;
					case "e":
						window.contenido.EsconderTecladoNumerico(oInput);
						break;
					default:
						oInput.value += sCaracter;
						jInput.focus();
						break;
				}
			});
		}

		// oInput.jNumerico = jNumerico;

		jNumerico.oInput = $(oInput);
	}
	, EsconderTecladoNumerico: function(oInput) {
		$(oInput).removeAttr("readonly");
		if (oInput.jNumerico) {
			oInput.jNumerico.remove();
			oInput.jNumerico = undefined;
		}
	}
}
