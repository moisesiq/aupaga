/* Script con modificaciones para el módulo de ventas. Archivo 71
 * Creado: 2014/12/02
 * Subido: 2014/12/03
 */


----------------------------------------------------------------------------------- Código de André

----------------------------------------------------------------------------------- Código de André


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE MovimientoInventario ADD TraspasoEntregado BIT NULL

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



/* *****************************************************************************
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

ALTER VIEW [dbo].[MovimientoInventarioDetalleView] AS
	SELECT
		MovimientoInventarioDetalle.MovimientoInventarioDetalleID
		,MovimientoInventarioDetalle.MovimientoInventarioID
		, mi.TipoOperacionID
		, mi.FechaRecepcion
		,MovimientoInventarioDetalle.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,MovimientoInventarioDetalle.Cantidad
		,MovimientoInventarioDetalle.PrecioUnitario
		,MovimientoInventarioDetalle.Importe
		,MovimientoInventarioDetalle.FueDevolucion
		,MovimientoInventarioDetalle.FechaRegistro
		,MovimientoInventarioDetalle.CantidadRecibida
		,MovimientoInventarioDetalle.PrecioUnitarioConDescuento
		,Parte.LineaID
	FROM 
		MovimientoInventarioDetalle
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = MovimientoInventarioDetalle.MovimientoInventarioID
			AND mi.Estatus = 1
		INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDetalle.ParteID
	WHERE
		MovimientoInventarioDetalle.Estatus = 1
GO

-- DROP VIEW PartesExistenciasMaxMinView
CREATE VIEW PartesExistenciasMaxMinView AS
	SELECT
		pe.ParteExistenciaID
		, pmm.ParteMaxMinID
		, pe.ParteID
		, pe.SucursalID
		, pe.Existencia
		, pmm.Maximo
		, pmm.Minimo
	FROM
		ParteExistencia pe
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = pe.ParteID AND pmm.SucursalID = pe.SucursalID
	WHERE pe.Estatus = 1
GO

/* 3 */
ALTER VIEW [dbo].[MovimientoInventarioTraspasosView] AS
	SELECT
		MovimientoInventario.MovimientoInventarioID
		,MovimientoInventario.TipoOperacionID
		,MovimientoInventario.SucursalOrigenID
		,Origen.NombreSucursal AS Origen
		,MovimientoInventario.SucursalDestinoID
		,Destino.NombreSucursal	AS Destino	
		,MovimientoInventario.FechaRegistro	
		,Usuario.NombreUsuario AS Registro
		,Solicito.NombreUsuario AS Solicito
		,MovimientoInventario.FechaRecepcion
		,Recibio.NombreUsuario AS Recibio
		,MovimientoInventario.Observacion
		,MovimientoInventario.TraspasoEntregado
	FROM 
		MovimientoInventario
		INNER JOIN Sucursal AS Origen ON Origen.SucursalID = MovimientoInventario.SucursalOrigenID
		INNER JOIN Sucursal AS Destino ON Destino.SucursalID = MovimientoInventario.SucursalDestinoID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID
		LEFT JOIN Usuario AS Solicito ON Solicito.UsuarioID = MovimientoInventario.UsuarioSolicitoTraspasoID
		LEFT JOIN Usuario AS Recibio ON Recibio.UsuarioID = MovimientoInventario.UsuarioRecibioTraspasoID
	WHERE
		MovimientoInventario.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE [dbo].[pauParteKardex] (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
) AS BEGIN
	SET NOCOUNT ON
	
	-- Definición de variables tipo constante
	

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(DAY, 1, @Hasta)
	
	SELECT
		pk.ParteKardexID
		, pk.Fecha
		, pk.Folio
		, pko.Tipo
		, pko.Operacion
		, pk.Entidad
		, u.NombreUsuario AS Usuario
		, pk.Origen
		, pk.Destino
		, pk.Importe
		, pk.Cantidad
		, pk.ExistenciaNueva
	FROM
		ParteKardex pk
		LEFT JOIN ParteKardexOperacion pko ON pko.ParteKardexOperacionID = pk.OperacionID
		LEFT JOIN Usuario u ON u.UsuarioID = pk.RealizoUsuarioID AND u.Estatus = 1
	WHERE
		pk.ParteID = @ParteID
		AND (pk.Fecha >= @Desde AND pk.Fecha < @Hasta)
		AND (@SucursalID IS NULL OR pk.SucursalID = @SucursalID)
	ORDER BY
		pk.Fecha
	
END
GO

ALTER PROCEDURE [dbo].[pauParteBusquedaEnTraspasos] (
	@MarcaParteID INT
	, @LineaID INT
	, @ProveedorID INT
	, @SucursalOrigenID INT
	, @SucursalDestinoID INT
	, @Abcs NVARCHAR(20)
) AS BEGIN

	--DECLARE @MarcaParteID INT = 0 --12
	--DECLARE @LineaID INT = 0 --180
	--DECLARE @ProveedorID INT = 0 --10
	--DECLARE @SucursalOrigenID INT = 1 
	--DECLARE @SucursalDestinoID INT = 3
	--DECLARE @Abcs NVARCHAR(20) = 'A'

	DECLARE @PedNoSurtido INT = 2

	SELECT
		Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,Parte.MarcaParteID	
		,Parte.LineaID	
		,Sistema.SistemaID
		,Parte.SubsistemaID
		, pv.NombreProveedor AS Proveedor
		,ExistenciaOrigen.SucursalID
		,ExistenciaOrigen.Existencia
		,MxMnOrigen.Maximo
		,MxMnOrigen.Minimo
		,ExistenciaDestino.SucursalID AS SucursalDestinoID
		,ExistenciaDestino.Existencia AS DestinoExistencia
		,MxMnDestino.Maximo AS DestinoMaximo
		,MxMnDestino.Minimo AS DestinoMinimo
		,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		,Parte.NumeroParte + Parte.NombreParte AS Busqueda
	FROM
		Parte	
		INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
		INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteAbc ON ParteAbc.ParteID = Parte.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = Parte.ProveedorID AND pv.Estatus = 1
		LEFT JOIN PedidoDetalle pd ON pd.ParteID = Parte.ParteID AND pd.PedidoEstatusID = @PedNoSurtido AND pd.Estatus = 1
	WHERE
		Parte.Estatus = 1
		AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
		AND (@MarcaParteID <= 0 OR Parte.MarcaParteID = @MarcaParteID)
		AND (@LineaID <= 0 OR Parte.LineaID = @LineaID)
		AND (@ProveedorID <= 0 OR Parte.ProveedorID = @ProveedorID)
		AND ParteAbc.AbcDeVentas IN (SELECT * FROM dbo.fnuDividirCadena(@Abcs, ','))
		AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
		AND ExistenciaOrigen.Existencia > 0
		AND ExistenciaDestino.Existencia <= MxMnDestino.Minimo
		-- AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)
		AND pd.PedidoDetalleID IS NULL

END
GO

ALTER PROCEDURE [dbo].[pauPedidosSugeridos] (
	@SucursalID NVARCHAR(10)
) 
AS BEGIN
SET NOCOUNT ON

	DECLARE @EstGenPendiente INT = 2
	DECLARE @EstPedidoNoSurtido INT = 2
	DECLARE @OpeTraspaso INT = 5

	;WITH _Pedidos AS (
		SELECT
			ParteID	
			,CAST(1 AS BIT) AS Sel
			,NumeroParte	
			,NombreParte	
			,UnidadEmpaque	
			,CriterioABC	
			,MaxMatriz	
			,MaxSuc02	
			,MaxSuc03	
			,ExistenciaMatriz	
			,ExistenciaSuc02	
			,ExistenciaSuc03	
			, CASE WHEN MinMatriz >= ExistenciaMatriz THEN MaxMatriz - ExistenciaMatriz - TraspasoMatriz ELSE 0.0 END AS NecesidadMatriz
			, CASE WHEN MinSuc02 >= ExistenciaSuc02 THEN MaxSuc02 - ExistenciaSuc02 - TraspasoSuc02 ELSE 0.0 END AS NecesidadSuc02
			, CASE WHEN MinSuc03 >= ExistenciaSuc03 THEN MaxSuc03 - ExistenciaSuc03 - TraspasoSuc03 ELSE 0.0 END AS NecesidadSuc03
			-- , MaxMatriz - ExistenciaMatriz AS NecesidadMatriz
			-- , MaxSuc02 - ExistenciaSuc02 AS NecesidadSuc02
			-- , MaxSuc03 - ExistenciaSuc03 AS NecesidadSuc03
			-- ,(MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) AS Total
			-- ,CEILING((MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) /
			-- (CASE WHEN Pre.UnidadEmpaque = 0 THEN 1 ELSE Pre.UnidadEmpaque END)) * Pre.UnidadEmpaque AS Pedido	
			,Costo	
			,ProveedorID	
			,NombreProveedor	
			,Beneficiario
			, '' AS Observacion
		FROM (
			SELECT 
				p.ParteID
				, p.NumeroParte
				, p.NombreParte
				, p.UnidadEmpaque
				,ParteAbc.AbcDeVentas AS CriterioABC	
				, SUM(CASE WHEN pe.SucursalID = 1 THEN pmm.Maximo ELSE 0.0 END) AS MaxMatriz
				, SUM(CASE WHEN pe.SucursalID = 2 THEN pmm.Maximo ELSE 0.0 END) AS MaxSuc02
				, SUM(CASE WHEN pe.SucursalID = 3 THEN pmm.Maximo ELSE 0.0 END) AS MaxSuc03	
				, SUM(CASE WHEN pmm.SucursalID = 1 THEN pmm.Minimo ELSE 0.0 END) AS MinMatriz
				, SUM(CASE WHEN pmm.SucursalID = 2 THEN pmm.Minimo ELSE 0.0 END) AS MinSuc02
				, SUM(CASE WHEN pmm.SucursalID = 3 THEN pmm.Minimo ELSE 0.0 END) AS MinSuc03
				, SUM(CASE WHEN pe.SucursalID = 1 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaMatriz
				, SUM(CASE WHEN pe.SucursalID = 2 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc02
				, SUM(CASE WHEN pe.SucursalID = 3 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc03
				, SUM(CASE WHEN tra.SucursalID = 1 THEN tra.Cantidad ELSE 0.0 END) AS TraspasoMatriz
				, SUM(CASE WHEN tra.SucursalID = 2 THEN tra.Cantidad ELSE 0.0 END) AS TraspasoSuc02
				, SUM(CASE WHEN tra.SucursalID = 3 THEN tra.Cantidad ELSE 0.0 END) AS TraspasoSuc03
				,PartePrecio.Costo
				, p.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			FROM 
				Parte p
				INNER JOIN ParteAbc ON  ParteAbc.ParteID = p.ParteID
				INNER JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID
				INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = pmm.SucursalID
				INNER JOIN PartePrecio ON PartePrecio.ParteID = p.ParteID
				INNER JOIN Proveedor ON Proveedor.ProveedorID = p.ProveedorID
				LEFT JOIN (
					SELECT
						mi.SucursalDestinoID AS SucursalID
						, mid.ParteID
						, mid.Cantidad
					FROM
						MovimientoInventario mi
						INNER JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioID = mi.MovimientoInventarioID AND mid.Estatus = 1
					WHERE
						mi.Estatus = 1
						AND mi.TipoOperacionID = @OpeTraspaso
						AND mi.FechaRecepcion IS NULL
				) tra ON tra.ParteID = p.ParteID AND tra.SucursalID = pmm.SucursalID
			WHERE
				p.Estatus = 1 
				AND pmm.Maximo > 0
				-- AND pe.Existencia <= pmm.Minimo
				AND p.ParteID NOT IN (SELECT PedidoDetalle.ParteID FROM PedidoDetalle WHERE PedidoDetalle.Estatus = 1 AND PedidoDetalle.PedidoEstatusID = 2)	
				AND pe.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
				/* Ya se incluyen aún las partes que estén en traspaso
				AND Parte.ParteID NOT IN 
					(SELECT d.ParteID 
					FROM MovimientoInventario m 
					INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID 
					WHERE m.TipoOperacionID = 5 
					AND m.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
					AND m.FechaRecepcion IS NULL 
					AND m.Estatus = 1)
				*/
			GROUP BY
				p.ParteID
				, p.NumeroParte
				, p.NombreParte
				, p.UnidadEmpaque
				,ParteAbc.AbcDeVentas
				,PartePrecio.Costo
				, p.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			) AS Pre
		
		WHERE
			(ExistenciaMatriz <= MinMatriz
			OR ExistenciaSuc02 <= MinSuc02
			OR ExistenciaSuc03 <= MinSuc03)
			-- Se sacan los que su necesidad se pueda cubrir con la existencia de matriz
			AND (ExistenciaMatriz - MinMatriz) <= ((MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03))
	)
	SELECT
		p.ParteID	
		,CAST(1 AS BIT) AS Sel
		, p.NumeroParte	
		, p.NombreParte	
		, p.UnidadEmpaque	
		, p.CriterioABC	
		,MaxMatriz	
		,MaxSuc02	
		,MaxSuc03	
		,ExistenciaMatriz	
		,ExistenciaSuc02	
		,ExistenciaSuc03	
		, NecesidadMatriz
		, NecesidadSuc02
		, NecesidadSuc03
		, p.NecesidadMatriz + p.NecesidadSuc02 + p.NecesidadSuc03 AS Total
		, CEILING((p.NecesidadMatriz + p.NecesidadSuc02 + p.NecesidadSuc03) /
			(CASE WHEN pt.UnidadEmpaque = 0 THEN 1 ELSE pt.UnidadEmpaque END)) * pt.UnidadEmpaque AS Pedido	
		,Costo	
		, p.ProveedorID
		,NombreProveedor	
		,Beneficiario
		, '' AS Observacion
		-- Para los que no se deben pedir por la existencia en sus equivalentes
		, CASE WHEN (
			(p.ExistenciaMatriz <= 0 AND pc.ExEquivMatriz > 0)
			OR (p.ExistenciaSuc02 <= 0 AND pc.ExEquivSuc02 > 0)
			OR (p.ExistenciaSuc03 <= 0 AND pc.ExEquivSuc03 > 0)
		) THEN 'NP' ELSE 'MxMn' END AS Caracteristica
	FROM
		_Pedidos p
		LEFT JOIN (
			SELECT
				pi.ParteID
				, SUM(CASE WHEN pe.SucursalID = 1 AND ISNULL(pmm.Calcular, 0) != 1 THEN pe.Existencia ELSE 0.0 END) AS ExEquivMatriz
				, SUM(CASE WHEN pe.SucursalID = 2 AND ISNULL(pmm.Calcular, 0) != 1 THEN pe.Existencia ELSE 0.0 END) AS ExEquivSuc02
				, SUM(CASE WHEN pe.SucursalID = 3 AND ISNULL(pmm.Calcular, 0) != 1 THEN pe.Existencia ELSE 0.0 END) AS ExEquivSuc03
			FROM
				_Pedidos pi
				LEFT JOIN ParteEquivalente pq ON pq.ParteID = pi.ParteID AND pq.Estatus = 1
				LEFT JOIN ParteEquivalente pee ON pee.GrupoID = pq.GrupoID AND pee.ParteID != pq.ParteID AND pee.Estatus = 1
				LEFT JOIN ParteExistencia pe ON pe.ParteID = pee.ParteID AND pe.Estatus = 1
				LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = pe.ParteID and pmm.SucursalID = pe.SucursalID
			GROUP BY pi.ParteID
		) pc ON pc.ParteID = p.ParteID
		LEFT JOIN Parte pt ON pt.ParteID = p.ParteID AND pt.Estatus = 1
	WHERE
		CEILING((p.NecesidadMatriz + p.NecesidadSuc02 + p.NecesidadSuc03) /
			(CASE WHEN pt.UnidadEmpaque = 0 THEN 1 ELSE pt.UnidadEmpaque END)) * pt.UnidadEmpaque
		> 0

	-- Se agregan los 9500
	UNION
	SELECT
		c9d.ParteID
		, CAST(1 AS BIT) AS Sel
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas AS CriterioAbc
		, NULL AS MaxMatriz
		, NULL AS MaxSuc02
		, NULL AS MaxSuc03
		-- , SUM(CASE WHEN pe.SucursalID = 1 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaMatriz
		-- , SUM(CASE WHEN pe.SucursalID = 2 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc02
		-- , SUM(CASE WHEN pe.SucursalID = 3 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc03
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, (SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaMatriz, 0.0)) AS NecesidadMatriz
		, (SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc02, 0.0)) AS NecesidadSuc02
		, (SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc03, 0.0)) AS NecesidadSuc03
		, SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			- ISNULL(pec.ExistenciaMatriz, 0.0) - ISNULL(pec.ExistenciaSuc02, 0.0) - ISNULL(pec.ExistenciaSuc03, 0.0)
		AS Total
		, -- Se calcula el Pedido
			(CEILING((
				SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			) / (CASE WHEN p.UnidadEmpaque = 0 THEN 1 ELSE p.UnidadEmpaque END)) * p.UnidadEmpaque
			) AS Pedido
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, '' AS Observacion
		, '9500' AS Caracteristica
	FROM
		Cotizacion9500Detalle c9d
		INNER JOIN Cotizacion9500 c9 ON c9.Cotizacion9500ID = c9d.Cotizacion9500ID AND c9.Estatus = 1
		LEFT JOIN PedidoDetalle pd ON pd.ParteID = c9d.ParteID AND pd.PedidoEstatusID = @EstPedidoNoSurtido AND pd.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = c9d.ParteID
		-- LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = c9d.ParteID
		-- LEFT JOIN ParteExistencia pe ON pe.ParteID = c9d.ParteID AND pe.Estatus = 1
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(CASE WHEN SucursalID = 1 THEN Existencia ELSE 0.0 END) AS ExistenciaMatriz
				, SUM(CASE WHEN SucursalID = 2 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc02
				, SUM(CASE WHEN SucursalID = 3 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc03
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = c9d.ParteID
		LEFT JOIN PartePrecio pp ON pp.ParteID = c9d.ParteID AND pp.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		c9d.Estatus = 1
		AND c9.EstatusGenericoID = @EstGenPendiente
		AND c9.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
		AND pd.PedidoDetalleID IS NULL
	GROUP BY
		c9d.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
	HAVING
		(SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaMatriz, 0.0)) > 0
		OR (SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc02, 0.0)) > 0
		OR (SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc03, 0.0)) > 0

	-- Se agregan los de Reporte de Faltante
	UNION
	SELECT
		rf.ParteID
		, CAST(1 AS BIT) AS Sel
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas AS CriterioABC
		, NULL AS MaxMatriz
		, NULL AS MaxSuc02
		, NULL AS MaxSuc03
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, SUM(CASE WHEN rf.SucursalID = 1 THEN rf.CantidadRequerida ELSE 0.0 END) AS NecesidadMatriz
		, SUM(CASE WHEN rf.SucursalID = 2 THEN rf.CantidadRequerida ELSE 0.0 END) AS NecesidadSuc02
		, SUM(CASE WHEN rf.SucursalID = 3 THEN rf.CantidadRequerida ELSE 0.0 END) AS NecesidadSuc03
		, SUM(CASE WHEN rf.SucursalID = 1 THEN rf.CantidadRequerida ELSE 0.0 END)
			+ SUM(CASE WHEN rf.SucursalID = 2 THEN rf.CantidadRequerida ELSE 0.0 END)
			+ SUM(CASE WHEN rf.SucursalID = 3 THEN rf.CantidadRequerida ELSE 0.0 END)
		AS Total
		, -- Se calcula el Pedido
			(CEILING((
				SUM(CASE WHEN rf.SucursalID = 1 THEN rf.CantidadRequerida ELSE 0.0 END)
				+ SUM(CASE WHEN rf.SucursalID = 2 THEN rf.CantidadRequerida ELSE 0.0 END)
				+ SUM(CASE WHEN rf.SucursalID = 3 THEN rf.CantidadRequerida ELSE 0.0 END)
			) / (CASE WHEN p.UnidadEmpaque = 0 THEN 1 ELSE p.UnidadEmpaque END)) * p.UnidadEmpaque
			) AS Pedido
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, ('(' + u.NombreUsuario + ') ' + rf.Comentario) AS Observacion
		, 'RF' AS Caracteristica
	FROM
		ReporteDeFaltante rf
		LEFT JOIN Parte p ON p.ParteID = rf.ParteID AND p.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = rf.ParteID
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(CASE WHEN SucursalID = 1 THEN Existencia ELSE 0.0 END) AS ExistenciaMatriz
				, SUM(CASE WHEN SucursalID = 2 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc02
				, SUM(CASE WHEN SucursalID = 3 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc03
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = rf.ParteID
		LEFT JOIN PartePrecio pp ON pp.ParteID = rf.ParteID AND pp.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = rf.RealizoUsuarioID AND u.Estatus = 1
	WHERE
		rf.Estatus = 1
		AND rf.Pedido = 0
		AND rf.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
	GROUP BY
		rf.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, rf.Comentario
		, u.NombreUsuario

END
GO