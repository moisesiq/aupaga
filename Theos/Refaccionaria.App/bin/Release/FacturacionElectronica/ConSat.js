/* ***************************************************************************
 * Función : Funciones de Javascript para realizar consultas de facturas en 
 *           el Sat.
 * Creado  : 2016-01-31 Moi
 * Mod.    : 2016-02-02 Moi
 * ************************************************************************* */

if (!window.j)
    window.j = $;

function IniciarSesion(sRfc, sClaveCiec) {
    document.getElementsByName('Ecom_User_ID')[0].value = sRfc;
    document.getElementsByName('Ecom_Password')[0].value = sClaveCiec;
    j('#submit').click();
}

function CerrarSesion() {
    j("#ctl100_LnkBtnCierraSesion").click();
    /* window.setTimeout(function () {
        window.location.href = "about:blank";
    }, 1000);
    */
}

function BuscarEmitidasPorFecha(sFechaIni, sFechaFin) {
    j('#ctl00_MainContent_RdoFechas').click();
    // j('[name=ctl00$MainContent$TxtUUID').val('');
    // j('[name=ctl00$MainContent$hfInicial').val(2016);
    j('#ctl00_MainContent_CldFechaInicial2_Calendario_text').val(sFechaIni);
    j('#ctl00_MainContent_CldFechaInicial2_DdlHora').val('0');
    j('#ctl00_MainContent_CldFechaInicial2_DdlMinuto').val('0');
    j('#ctl00_MainContent_CldFechaInicial2_DdlSegundo').val('0');
    // j('[name=ctl00$MainContent$hfFinal').val(2016);
    j('#ctl00_MainContent_CldFechaFinal2_Calendario_text').val(sFechaFin);
    j('#ctl00_MainContent_CldFechaFinal2_DdlHora').val('23');
    j('#ctl00_MainContent_CldFechaFinal2_DdlMinuto').val('59');
    j('#ctl00_MainContent_CldFechaFinal2_DdlSegundo').val('59');
    // j('[name=ctl00$MainContent$TxtRfcReceptor').val('');
    // j('[name=ctl00$MainContent$DdlEstadoComprobante').val('-1');
    // j('[name=ctl00$MainContent$ddlComplementos').val('-1');

    // Se manda hacer la petición de búsqueda
    window.setTimeout(function () {
        j('#ctl00_MainContent_BtnBusqueda').click();

        // Se manda notificar la búsqueda
        window.setTimeout(function () {
            document.title = "b";
            window.open(("about:blank?paso=" + document.title), "_blank", "width=64,height=64");
        }, 2000);
    }, 400);

}

function BuscarRecibidasPorFecha(iAnio, iMes, iDia) {
    j('#ctl00_MainContent_RdoFechas').click();
    // j('#ctl00_MainContent_TxtUUID').val('');
        
    window.setTimeout(function () {
        j('#DdlAnio').val(iAnio);
        j('#ctl00_MainContent_CldFecha_DdlMes').val(iMes);
        j('#ctl00_MainContent_CldFecha_DdlDia').val(iDia ? ('00' + iDia).slice(-2) : '');
        j('#ctl00_MainContent_CldFecha_DdlHora').val('0');
        j('#ctl00_MainContent_CldFecha_DdlMinuto').val('0');
        j('#ctl00_MainContent_CldFecha_DdlSegundo').val('0');
        j('#ctl00_MainContent_CldFecha_DdlHoraFin').val('23');
        j('#ctl00_MainContent_CldFecha_DdlMinutoFin').val('59');
        j('#ctl00_MainContent_CldFecha_DdlSegundoFin').val('59');
        // j('#ctl00_MainContent_TxtRfcReceptor').val(''); ""
        // j('#ctl00_MainContent_DdlEstadoComprobante').val('-1');
        // j('#ddlComplementos').val('-1');

        // j("body").append("<pre>" + JSON.stringify(j("#aspnetForm").serializeArray(), null, 4) + "</pre>");
        // window.alert(j('#ctl00_MainContent_CldFecha_DdlDia').val());

        // Se manda hacer la petición de búsqueda
        window.setTimeout(function () {
            j('#ctl00_MainContent_BtnBusqueda').click();

            // Se manda notificar la búsqueda
            window.setTimeout(function () {
                document.title = "b";
                window.open(("about:blank?paso=" + document.title), "_blank", "width=64,height=64");
            }, 2000);
        }, 1000);

    }, 1000);
}

function ObtenerXmls() {
    var aXmls = [];

    // Se verifica si ya se cargó el elemento
    if (j("#DivPaginas").children().length <= 0)
        return aXmls;

    //
    if (!window.location.origin)
        window.location.origin = (window.location.protocol + "//" + window.location.hostname
            + (window.location.port ? (":" + window.location.port) : ""));
    var sPagina = (window.location.origin + "/");
    //
    j("#DivPaginas").find("[id=BtnDescarga]").each(function(i, e) {
        var sParte = $(e).attr("onclick").split("'");
        sParte = sParte[1];
        aXmls.push(sPagina + sParte);
    });

    // Se ponen los xmls en un div, para fácil acceso
    var jRes = j("<pre id='preXmls' style='display: none;'></pre>").prependTo("body");
    for (var i = 0; i < aXmls.length; i++) {
        jRes.text(jRes.text() + aXmls[i] + "\n");
    }

    // Se manda notificar la búsqueda
    window.setTimeout(function () {
        document.title = "x";
        window.open(("about:blank?paso=" + document.title), "_blank", "width=64,height=64");
    }, 400);
}