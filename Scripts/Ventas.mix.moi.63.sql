/* Script con modificaciones para el módulo de ventas. Archivo 63
 * Creado: 2014/09/29
 * Subido: 2014/10/17
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE Parte ALTER COLUMN NombreParte NVARCHAR(512) NOT NULL

	DROP FUNCTION fnuClienteCreditoDeuda
	DROP FUNCTION fnuClientePromedioPago
	DROP FUNCTION fnuClienteAcumulado
	DROP FUNCTION fnuLlenarIzquierda
	DROP FUNCTION fnuTextoEnCadena

	DELETE FROM ParteComplementaria
	ALTER TABLE ParteComplementaria ADD ParteIDComplementaria INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)

	INSERT INTO Permiso (NombrePermiso, MensajeDeError, FechaRegistro) VALUES
		('Administracion.Clientes.ExportarListado', 'No tienes permisos para exportar listado de Clientes.', GETDATE())

	COMMIT TRAN
END TRY
BEGIN CATCH
	PRINT 'Hubo un error al ejecutar el script:'
	PRINT ERROR_MESSAGE()
	ROLLBACK TRAN
	RETURN
END CATCH
GO

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */

CREATE TABLE [dbo].[UsuarioAsistencia](
	[UsuarioAsistenciaID] [int] IDENTITY(1,1) NOT NULL,
	[AccesoUsuarioID] [int] NOT NULL,
	[SucursalID] [int] NOT NULL,
	[FechaHora] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UsuarioAsistenciaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UsuarioAsistencia]  WITH CHECK ADD FOREIGN KEY([AccesoUsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO

ALTER TABLE [dbo].[UsuarioAsistencia]  WITH CHECK ADD FOREIGN KEY([SucursalID])
REFERENCES [dbo].[Sucursal] ([SucursalID])
GO

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

ALTER VIEW [dbo].[PartesCodigosAlternosView] AS
	SELECT
		ParteCodigoAlterno.ParteCodigoAlternoID,
		ParteCodigoAlterno.ParteID,
		ParteCodigoAlterno.MarcaParteID,		
		MarcaParte.NombreMarcaParte,
		MarcaParte.Abreviacion AS MarcaAbreviacion,
		ParteCodigoAlterno.CodigoAlterno
	FROM
		ParteCodigoAlterno
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = ParteCodigoAlterno.MarcaParteID
	WHERE
		ParteCodigoAlterno.Estatus = 1
GO

ALTER VIEW [dbo].[PartesExistenciasView] AS
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, pp.Costo
		, pp.CostoConDescuento
		, pe.SucursalID
		, pe.Existencia
		-- , SUM(pe.Existencia) AS Existencia
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1
	WHERE
		p.Estatus = 1
	/* GROUP BY
		p.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.ProveedorID
		, pv.NombreProveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte
		, p.LineaID
		, l.NombreLinea
		, pp.Costo
		, pp.CostoConDescuento
	*/
GO

ALTER VIEW [dbo].[ClientesCreditoView] AS
	WITH _Tiempos AS (
		SELECT
			v.VentaID
			, v.Fecha
			, v.ClienteID
			, DATEDIFF(DAY, v.Fecha, MAX(vp.Fecha)) AS DiasPago
		FROM
			Venta v
			LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		WHERE
			v.Estatus = 1
			AND v.ACredito = 1
			AND v.VentaEstatusID = 3
			AND v.Fecha >= DATEADD(YEAR, -1, GETDATE())
		GROUP BY
			v.VentaID
			, v.Fecha
			, v.ClienteID
	)
	SELECT
		c.ClienteID
		, c.Nombre
		, c.TieneCredito
		, c.LimiteCredito AS LimiteDeCredito
		, c.DiasDeCredito
		, c.Tolerancia
		, SUM(ca.Adeudo) AS Adeudo
		, MIN(ca.Fecha) AS FechaPrimerAdeudo
		-- , ISNULL(CONVERT(BIT, CASE WHEN (ca.Adeudo >= c.LimiteCredito OR ca.FechaPrimerAdeudo >= (GETDATE() + c.DiasDeCredito))
		-- 	THEN 1 ELSE 0 END), 0) AS CreditoVencido
		, SUM(CASE WHEN ca.Dias > c.DiasDeCredito THEN ca.Adeudo ELSE 0.0 END) AS AdeudoVencido
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID) AS PromedioDePagoAnual
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID
			AND Fecha > DATEADD(MONTH, -3, GETDATE())) AS PromedioDePago3Meses
	FROM
		Cliente c
		LEFT JOIN (
			SELECT
				VentaID
				, ClienteID
				, Fecha
				, SUM(Total - Pagado) AS Adeudo
				, DATEDIFF(DAY, Fecha, GETDATE()) AS Dias
			FROM VentasView
			WHERE ACredito = 1 AND (Total - Pagado) > 0 AND VentaEstatusID = 2
			GROUP BY
				VentaID
				, ClienteID
				, Fecha
		) ca ON ca.ClienteID = c.ClienteID
	GROUP BY
		c.ClienteID
		, c.Nombre
		, c.TieneCredito
		, c.LimiteCredito
		, c.DiasDeCredito
		, c.Tolerancia
GO

ALTER VIEW [dbo].[ClientesView]
AS
	SELECT
		Cliente.ClienteID
		,ClienteFacturacion.Rfc
		,Cliente.Nombre
		,Cliente.Calle
		,Cliente.NumeroInterior
		,Cliente.NumeroExterior
		,Cliente.Colonia
		,Cliente.CodigoPostal
		,Cliente.EstadoID
		,Estado.NombreEstado
		,Cliente.MunicipioID
		,Municipio.NombreMunicipio
		,Cliente.CiudadID
		,Ciudad.NombreCiudad
		,Cliente.Telefono
		,Cliente.Celular
		,Cliente.Particular
		,Cliente.Nextel
		,Cliente.ListaDePrecios
		,ISNULL(Cliente.TieneCredito, 0) AS TieneCredito
		-- ,Cliente.Credito
		,Cliente.DiasDeCredito
		,Cliente.TipoFormaPagoID
		,TipoFormaPago.NombreTipoFormaPago
		,Cliente.BancoID
		,Banco.NombreBanco
		,Cliente.TipoClienteID
		,TipoCliente.NombreTipoCliente
		,Cliente.CuentaBancaria
		,ISNULL(Cliente.Tolerancia, 0) AS Tolerancia
		,Cliente.LimiteCredito
		,ISNULL(Cliente.EsClienteComisionista, 0) AS EsClienteComisionista
		,ISNULL(Cliente.EsTallerElectrico, 0) AS EsTallerElectrico
		,ISNULL(Cliente.EsTallerMecanico, 0) AS EsTallerMecanico
		,ISNULL(Cliente.EsTallerDiesel, 0) AS EsTallerDiesel
		,ISNULL(Cliente.Vip, 0) AS Vip
		,Cliente.ClienteComisionistaID
		,Com.Nombre AS NombreClienteComisionista
		,Cliente.FechaRegistro
		,Cliente.Alias
		-- ,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 1) AS DeudaActual
		-- ,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 2) AS DeudaVencido
		-- ,dbo.fnuClientePromedioPago(Cliente.ClienteID, 1) AS PromedioPagoTotal
		-- ,dbo.fnuClientePromedioPago(Cliente.ClienteID, 2) AS PromedioPagoTresMeses
		,Cliente.Acumulado AS AcumuladoH
		-- ,dbo.fnuClienteAcumulado(Cliente.ClienteID) AS Acumulado
	FROM
		Cliente
		LEFT JOIN ClienteFacturacion ON ClienteFacturacion.ClienteID = Cliente.ClienteID
		INNER JOIN Estado ON Estado.EstadoID = Cliente.EstadoID
		INNER JOIN Municipio ON Municipio.MunicipioID = Cliente.MunicipioID
		INNER JOIN Ciudad ON Ciudad.CiudadID = Cliente.CiudadID
		LEFT JOIN TipoFormaPago ON TipoFormaPago.TipoFormaPagoID = Cliente.TipoFormaPagoID
		LEFT JOIN Banco ON Banco.BancoID = Cliente.BancoID
		LEFT JOIN TipoCliente ON TipoCliente.TipoClienteID = Cliente.TipoClienteID
		LEFT JOIN Cliente Com ON Com.ClienteComisionistaID = Cliente.ClienteID
	WHERE
		Cliente.Estatus = 1
GO

ALTER VIEW [dbo].[PartesComplementariasView] AS
	SELECT
		pc.ParteComplementariaID
		-- , pcr.ParteID
		-- , pc.ParteID AS ParteIDComplementaria
		, pc.ParteID
		, pc.ParteIDComplementaria
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pi.NombreImagen
	FROM
		ParteComplementaria pc
		-- INNER JOIN ParteComplementaria pcr ON pcr.GrupoID = pc.GrupoID AND pcr.ParteID != pc.ParteID
		LEFT JOIN Parte p ON p.ParteID = pc.ParteIDComplementaria AND p.Estatus = 1
		LEFT JOIN ParteImagen pi ON pi.ParteID = pc.ParteIDComplementaria AND pi.Orden = 1
GO

CREATE VIEW [dbo].[UsuariosAsistenciasView] as	
	SELECT
		ua.AccesoUsuarioID
		, ua.UsuarioAsistenciaID
		, u.NombrePersona
		, u.NombreUsuario
		, ua.FechaHora
		, ua.SucursalID
		, s.NombreSucursal
	FROM
		UsuarioAsistencia ua
		LEFT JOIN Usuario u on ua.AccesoUsuarioID = u.UsuarioID AND u.Estatus = 1
		LEFT JOIN Sucursal s on s.SucursalID = ua.SucursalID AND u.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE [dbo].[pauContaCuentasMovimientosTotales] (
    @SucursalID INT
    , @Desde DATE
    , @Hasta DATE
) AS BEGIN
    SET NOCOUNT ON

    /* EXEC pauContaCuentasMovimientosTotales 1, '2014-08-23', '2014-08-28'
    DECLARE @Desde DATE = '2014-04-01'
    DECLARE @Hasta DATE = '2014-04-30'
    */

    -- Definición de variables tipo constante
    DECLARE @CdmRepartoDeUtilidades INT = 18

    -- Variables calculadas para el proceso
    SET @Hasta = DATEADD(d, 1, @Hasta)

    -- Gastos fijos - SucursalGastoFijo
    SELECT
        gf.SucursalGastoFijoID AS TablaRegistroID
        , CONVERT(BIT, 1) AS GastoFijo
        , gf.SucursalID
        , gf.Importe
        , cca.ContaCuentaAuxiliarID
        , ccm.ContaCuentaDeMayorID
        , cs.ContaSubcuentaID
        , cc.ContaCuentaID
        , NULL AS Egreso
        , cca.CuentaAuxiliar
        , ccm.CuentaDeMayor
        , cs.Subcuenta
        , cc.Cuenta
        , cca.AfectaMetas
        , cca.SumaGastosFijos
        , cca.CalculoSemanal
    FROM
        SucursalGastoFijo gf
        LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = gf.ContaCuentaAuxiliarID
        LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
        LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
        LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
    WHERE gf.SucursalID = @SucursalID

    UNION

    -- Gastos variables - ContaEgreso
    SELECT
        ce.ContaEgresoID
        , CONVERT(BIT, 0) AS GastoFijo
        , cedc.SucursalID
        , cedc.ImporteDev AS Importe
        , cca.ContaCuentaAuxiliarID
        , ccm.ContaCuentaDeMayorID
        , cs.ContaSubcuentaID
        , cc.ContaCuentaID
        , ce.Observaciones AS Egreso
        , cca.CuentaAuxiliar
        , ccm.CuentaDeMayor
        , cs.Subcuenta
        , cc.Cuenta
        , cca.AfectaMetas
        , cca.SumaGastosFijos
        , cca.CalculoSemanal
    FROM
        ContaEgreso ce
        INNER JOIN (
            SELECT
                ContaEgresoID
                , SucursalID
                , SUM(Importe) AS ImporteDev
            FROM ContaEgresoDevengado
            WHERE (Fecha >= @Desde AND Fecha < @Hasta)
            GROUP BY ContaEgresoID, SucursalID
        ) cedc ON cedc.ContaEgresoID = ce.ContaEgresoID
        INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
        LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
        LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
        LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
    WHERE
        cedc.SucursalID = @SucursalID
        -- Se quitan los egresos que ya se tienen como gastos fijos
        AND ce.ContaCuentaAuxiliarID NOT IN (
            SELECT ContaCuentaAuxiliarID
            FROM SucursalGastoFijo
            WHERE SucursalID = @SucursalID
        )
        -- Se quitan los egresos de la Cuenta de Mayor "Reparto de Utilidades"
        AND cca.ContaCuentaDeMayorID != @CdmRepartoDeUtilidades
    
    -- Se asigna el orden
    ORDER BY
        cc.Cuenta
        , cs.Subcuenta
        , ccm.CuentaDeMayor
        , cca.CuentaAuxiliar
        , Egreso

END
GO

ALTER PROCEDURE [dbo].[pauClienteKardex] (
	@ClienteID AS INT
	, @FechaInicial AS DATE
	, @FechaFinal AS DATE
	, @SucursalID AS INT = NULL
	, @VehiculoID INT = NULL
) 
AS BEGIN
SET NOCOUNT ON

/* DECLARE @ClienteID AS INT = 1
DECLARE @FechaInicial AS DATE = '2014-06-19'
DECLARE @FechaFinal AS DATE = GETDATE()
DECLARE @SucursalID AS INT = null
DECLARE @VehiculoID INT = null
*/

	DECLARE @EstCobrada INT = 2
	DECLARE @EstPagada INT = 3

	SELECT
		-- v.VentaID
		vd.ParteID
		, p.NumeroParte
		, p.NombreParte AS Descripcion
		, m.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, pv.NombreProveedor AS Proveedor
		, SUM(vd.Cantidad) AS Cantidad
		, p.NumeroParte + ' ' + p.NombreParte AS Busqueda
	FROM
		Venta v
		LEFT JOIN VentaDetalle vd ON v.VentaID = vd.VentaID AND vd.Estatus = 1
		-- LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN MarcaParte m ON m.MarcaParteID = p.MarcaParteID AND m.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		-- LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
	WHERE
		v.Estatus = 1
		AND v.ClienteID = @ClienteID		
		AND v.Fecha BETWEEN @FechaInicial AND @FechaFinal
		AND v.VentaEstatusID IN (@EstCobrada, @EstPagada)
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (@VehiculoID IS NULL OR v.ClienteVehiculoID = @VehiculoID)
	GROUP BY
		-- v.VentaID
		vd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, m.NombreMarcaParte
		, l.NombreLinea
		, pv.NombreProveedor
	ORDER BY
		l.NombreLinea

END
GO