/* ****************************************************************************
 * Motivo    : Utilidades generales de Javascript relacionadas con la vista   *
 * Autor     : Moisés Iquingari González Paredes                              *
 * Creado    : 2015/12/22                                                     *
 * Ult. Mod. : 2015/12/22                                                     *
 **************************************************************************** */

window.vista = {
	OrdenarTabla: function(oTabla, iColumna, bAscendente) {
		if (bAscendente === undefined)
			bAscendente = true;
		var sClaseOrden = "_Orden";
		var sClaseDir = (bAscendente ? "_Asc" : "_Desc");
		// Se limpian las clases de todas las columnas
		$(oTabla).find("th").removeClass("_Orden").removeClass("_Asc").removeClass("_Desc");

		var jTablaC = $(oTabla).children("tbody");
		// Se obtienen las filas directas de la tabla a ordenar
		var jFilas = jTablaC.find("> tr");
		// Se compara la primera fila con el resto de las filas
		for (var i = 0; i < jFilas.length; i++) {
			var jPrimera = jFilas.eq(i).children("td").eq(iColumna);
			for (var ii = 1; ii < jFilas.length; ii++) {
				var jComp = jFilas.eq(ii).children("td").eq(iColumna);
				if (bAscendente) {
					if (jComp.text() < jPrimera.text())
						jPrimera = jComp;
				} else {
					if (jComp.text() > jPrimera.text())
						jPrimera = jComp;
				}
			}
			var jFila = jPrimera.parent();
			jFilas.splice(jFilas.index(jFila), 1);
			i--;
			jFila = jFila.detach();
			jTablaC.append(jFila);
		}

		// Se marca la fila ordenada, con la clase correspondiente
		$(oTabla).find("th").eq(iColumna).addClass(sClaseOrden).addClass(sClaseDir);
	}
}
