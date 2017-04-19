/* ***************************************************************************
 * Función : Funciones de $avascript para realizar consultas de facturas en 
 *           el Sat.
 * Creado  : 2016-01-31 Moi
 * Mod.    : 2016-02-02 Moi
 * ************************************************************************* */

//if (!window.$)
//    window.$ = $;

function IniciarSesion(sRfc, sClaveCiec) {
    document.getElementsByName('Ecom_User_ID')[0].value = sRfc;
    document.getElementsByName('Ecom_Password')[0].value = sClaveCiec;
    $('#submit').click();
}

function CerrarSesion() {
    $("#ctl100_LnkBtnCierraSesion").click();
    /* window.setTimeout(function () {
        window.location.href = "about:blank";
    }, 1000);
    */
}

function BuscarEmitidasPorFecha(sFechaIni, sFechaFin) {
    $('#ctl00_MainContent_RdoFechas').click();
    // $('[name=ctl00$MainContent$TxtUUID').val('');
    // $('[name=ctl00$MainContent$hfInicial').val(2016);
    $('#ctl00_MainContent_CldFechaInicial2_Calendario_text').val(sFechaIni);
    $('#ctl00_MainContent_CldFechaInicial2_DdlHora').val('0');
    $('#ctl00_MainContent_CldFechaInicial2_DdlMinuto').val('0');
    $('#ctl00_MainContent_CldFechaInicial2_DdlSegundo').val('0');
    // $('[name=ctl00$MainContent$hfFinal').val(2016);
    $('#ctl00_MainContent_CldFechaFinal2_Calendario_text').val(sFechaFin);
    $('#ctl00_MainContent_CldFechaFinal2_DdlHora').val('23');
    $('#ctl00_MainContent_CldFechaFinal2_DdlMinuto').val('59');
    $('#ctl00_MainContent_CldFechaFinal2_DdlSegundo').val('59');
    // $('[name=ctl00$MainContent$TxtRfcReceptor').val('');
    // $('[name=ctl00$MainContent$DdlEstadoComprobante').val('-1');
    // $('[name=ctl00$MainContent$ddlComplementos').val('-1');

    // Se manda hacer la petición de búsqueda
    window.setTimeout(function () {
        $('#ctl00_MainContent_BtnBusqueda').click();

        // Se manda notificar la búsqueda
        window.setTimeout(function () {
            document.title = "b";
            window.open(("about:blank?paso=" + document.title), "_blank", "width=64,height=64");
        }, 2000);
    }, 400);

}

function BuscarRecibidasPorFecha(iAnio, iMes, iDia) {
    $('#ctl00_MainContent_RdoFechas').click();
    // $('#ctl00_MainContent_TxtUUID').val('');
        
    window.setTimeout(function () {
        $('#DdlAnio').val(iAnio);
        $('#ctl00_MainContent_CldFecha_DdlMes').val(iMes);
        $('#ctl00_MainContent_CldFecha_DdlDia').val(iDia ? ('00' + iDia).slice(-2) : '');
        $('#ctl00_MainContent_CldFecha_DdlHora').val('0');
        $('#ctl00_MainContent_CldFecha_DdlMinuto').val('0');
        $('#ctl00_MainContent_CldFecha_DdlSegundo').val('0');
        $('#ctl00_MainContent_CldFecha_DdlHoraFin').val('23');
        $('#ctl00_MainContent_CldFecha_DdlMinutoFin').val('59');
        $('#ctl00_MainContent_CldFecha_DdlSegundoFin').val('59');
        // $('#ctl00_MainContent_TxtRfcReceptor').val(''); ""
        // $('#ctl00_MainContent_DdlEstadoComprobante').val('-1');
        // $('#ddlComplementos').val('-1');

        // $("body").append("<pre>" + $SON.stringify($("#aspnetForm").serializeArray(), null, 4) + "</pre>");
        // window.alert($('#ctl00_MainContent_CldFecha_DdlDia').val());

        // Se manda hacer la petición de búsqueda
        window.setTimeout(function () {
            $('#ctl00_MainContent_BtnBusqueda').click();

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

    //document.getElementById("#ctl00_MainContent_LblEstado").innerHTML = 5 + 6;

    // Se verifica si ya se cargó el elemento
    //if ($("#DivPaginas").children().length <= 0)
    if (("#DivPaginas").children().length <= 0)
        return aXmls;

    //
    if (!window.location.origin)
        window.location.origin = (window.location.protocol + "//" + window.location.hostname
            + (window.location.port ? (":" + window.location.port) : ""));
    var sPagina = (window.location.origin + "/");
    //
    //$("#DivPaginas").find("[id=BtnDescarga]").each(function(i, e) {
    ("#DivPaginas").find("[id=BtnDescarga]").each(function (i, e) {
        var sParte = $(e).attr("onclick").split("'");
        sParte = sParte[1];
        aXmls.push(sPagina + sParte);
        alert(sParte);
    }
   );

    // Se ponen los xmls en un div, para fácil acceso
    //var $Res = $("<pre id='preXmls' style='display: none;'></pre>").prependTo("body");
    var $Res = ("<pre id='preXmls' style='display: none;'></pre>").prependTo("body");
    for (var i = 0; i < aXmls.length; i++) {
        $Res.text($Res.text() + aXmls[i] + "\n");
    }

    // Se manda notificar la búsqueda
    window.setTimeout(function () {
        document.title = "x";
        window.open(("about:blank?paso=" + document.title), "_blank", "width=64,height=64");
    }, 400);
}