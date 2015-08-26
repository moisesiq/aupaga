/* ****************************************************************************
 * Motivo    : Funciones de ayuda para crear tags html                        *
 * Autor     : Moisés Iquingari González Paredes                              *
 * Creado    : 2015/07/08                                                     *
 * Ult. Mod. : 2015/07/17                                                     *
 **************************************************************************** */

window.html = {
	Atributos: function(oAtributos) {
		if (!oAtributos) return "";

		var sAtributos = "";
		for (var s in oAtributos) {
			sAtributos += (s + "=\"" + oAtributos[s] + "\" ")
		}
		return sAtributos;
	}

	, Tag: function(sTag, sTexto, oAtributos) {
		var sHtml = ("<" + sTag);
		if (oAtributos)
			sHtml += (" " + this.Atributos(oAtributos));
		sHtml += (">" + sTexto + "</" + sTag + ">");
		return sHtml;
	}

	, Input: function(sTipo, sNombre, sValor, oAtributos) {
		var sHtml = ('<input type="' + sTipo + '" name="' + sNombre + '" value="' + sValor + '"');
		if (oAtributos)
			sHtml += (" " + this.Atributos(oAtributos));
		sHtml += " />";
		return sHtml;
	}

	, span: function(sTexto, oAtributos) {
		return this.Tag("span", sTexto, oAtributos);
	}

	, tr: function(sTexto, oAtributos) {
		return this.Tag("tr", sTexto, oAtributos);
	}

	, td: function(sTexto, oAtributos) {
		return this.Tag("td", sTexto, oAtributos);
	}

	, a: function(sTexto, sEnlace, oAtributos) {
		var sHtml = ('<a href="' + sEnlace + '"');
		if (oAtributos)
			sHtml += (" " + this.Atributos(oAtributos));
		sHtml += (">" + sTexto + "</a>");
		return sHtml;
	}

	, li_a: function(sTexto, sEnlace, oAtributos) {
		return ("<li>" + this.a(sTexto, sEnlace, oAtributos) + "</li>");
	}

	, img: function(sImagen, sAlt, oAtributos) {
		var sHtml = ('<img src="' + sImagen + '" alt="' + sAlt + '"');
		if (oAtributos)
			sHtml += (" " + this.Atributos(oAtributos));
		sHtml += " />";
		return sHtml;
	}

	, text: function(sNombre, sValor, oAtributos) {
		return this.Input("text", sNombre, sValor, oAtributos);
	}

	, number: function(sNombre, sValor, oAtributos) {
		return this.Input("number", sNombre, sValor, oAtributos);
	}

	, hidden: function(sNombre, sValor, oAtributos) {
		return this.Input("hidden", sNombre, sValor, oAtributos);
	}

	, Fila: function(aDatos) {
		var sHtml = "<tr>";
		for (var i = 0; i < aDatos.length; i++) {
			sHtml += ("<td>" + aDatos[i] + "</td>");
		}
		sHtml += "</tr>";
		return sHtml;
	}
}
