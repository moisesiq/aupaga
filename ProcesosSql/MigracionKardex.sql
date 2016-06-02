/*
DELETE FROM ParteKardex
DBCC CHECKIDENT('ParteKardex', RESEED, 0)
*/

--DECLARE @ParteID AS INT = 2301
DECLARE @SucursalID AS NVARCHAR(10) =  N'1,2,3'
--DECLARE @FechaInicial AS DATE = '2013-11-17'
--DECLARE @FechaFinal AS DATE = '2014-02-17'

DECLARE @OpVenta INT = 1
DECLARE @OpVentaDev INT = 2
DECLARE @OpEntradaCompra INT = 3
DECLARE @OpDevolucionAProveedor INT = 4
DECLARE @OpEntradaInventario INT = 5
DECLARE @OpSalidaInventario INT = 6
DECLARE @OpTraspasoEntrada INT = 7
DECLARE @OpTraspasoSalida INT = 8

DECLARE @TipoOpEntradaInv INT = 2
DECLARE @TipoOpSalidaInv INT = 3
DECLARE @TipoOpTraspaso INT = 5
DECLARE @TipoConOpGarantia INT = 12

DECLARE @CaCascoChico INT = 28
DECLARE @CaCascoMediano INT = 29
DECLARE @CaCascoGrande INT = 30
DECLARE @CaCascoExtra INT = 64
DECLARE @PaDepCascoChico INT = 3397
DECLARE @PaDepCascoMediano INT = 3406
DECLARE @PaDepCascoGrande INT = 3403
DECLARE @PaDepCascoExtra INT = 3400

DECLARE @Iva AS DECIMAL(18,2) = (((SELECT CAST(Valor AS DECIMAL(18,2))FROM Configuracion WHERE Configuracion.ConfiguracionID = 1)/100) + 1)

/* PARA DIFERENCIA MOVIMIENTOS ENTRADA Y SALIDA DE INVENTARIO PROVENIENTES DE CONFLICTO */
DECLARE @MovsDeConflicto TABLE (OperacionID INT, MovID INT, MovFuenteID INT)
INSERT INTO @MovsDeConflicto
SELECT
	TipoOperacionID
	, MovimientoInventarioID
	, CONVERT(INT, REPLACE(Observacion, 'Traspaso por resolucion de un Conflicto. Movimiento: : ', ''))
FROM MovimientoInventario
WHERE
	Estatus = 1
	AND Observacion LIKE 'Traspaso por resolucion de un Conflicto. Movimiento: : %'
	AND SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

/* INICIO DE LA MIGRACIÓN */

INSERT INTO ParteKardex (ParteID, OperacionID, SucursalID, Folio, Fecha, RealizoUsuarioID
	, Entidad, Origen, Destino, Cantidad, Importe, ExistenciaNueva, RelacionTabla, RelacionID)

SELECT
	ParteID
	, OperacionID
	, SucursalID
	, Folio
	, Fecha
	, UsuarioID
	, ClienteProveedor
	, Origen
	, Destino
	, ISNULL(Cantidad, 0.0) AS Cantidad
	, ISNULL(Importe, 0.0) AS Importe
	, 0.0 AS ExistenciaNueva
	, RelacionTabla
	, RelacionID
FROM (

	/* Existencia Inicial */
	SELECT
		pe.ParteID
		, @OpEntradaInventario AS OperacionID
		, pe.SucursalID
		, NULL AS Folio
		, pe.FechaRegistro AS Fecha
		, pe.UsuarioID
		, 'ENTRADA INICIAL' AS ClienteProveedor
		, NULL AS Origen
		, s.NombreSucursal AS Destino
		, pe.ExistenciaInicial AS Cantidad
		, pp.Costo AS Importe
		, NULL AS RelacionTabla
		, NULL AS RelacionID
		-- , 'E' AS Tipo
		-- , 'ENTRADA INICIAL' AS Operacion
		-- , NULL AS NombreUsuario
		-- , 0.0 AS ExistenciaNueva
		, 0 AS Orden
	FROM
		ParteExistencia pe
		LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = pe.SucursalID AND s.Estatus = 1
	WHERE pe.Estatus = 1

	/* ENTRADA COMPRA */
	UNION
	SELECT
		MovimientoInventarioDetalle.ParteID
		, @OpEntradaCompra AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		,CAST(MovimientoInventario.FolioFactura AS VARCHAR) AS Folio
		, MovimientoInventario.FechaRegistro AS Fecha
		, Usuario.UsuarioID
		,Proveedor.NombreProveedor AS ClienteProveedor
		,CAST(MovimientoInventario.ProveedorID AS VARCHAR) AS Origen
		,Sucursal.NombreSucursal AS Destino
		,MovimientoInventarioDetalle.Cantidad
		,MovimientoInventarioDetalle.PrecioUnitario AS Importe
		, 'MovimientoInventario' AS RelacionTabla
		, MovimientoInventario.MovimientoInventarioID AS RelacionID
		-- ,'E' AS Tipo
		-- ,'ENTRADA COMPRA' AS Operacion
		-- ,Usuario.NombreUsuario
		-- ,0.0 AS ExistenciaNueva
		,1 AS Orden
	FROM
		MovimientoInventario 
		INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
		INNER JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
		INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
	WHERE 
		MovimientoInventario.TipoOperacionID = 1 
		AND MovimientoInventario.Estatus = 1
		-- AND MovimientoInventarioDetalle.ParteID = @ParteId			
		AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

	/* ENTRADA INVENTARIO */
	UNION
	SELECT 
		MovimientoInventarioDetalle.ParteID
		, @OpEntradaInventario AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
		, MovimientoInventario.FechaRegistro AS Fecha
		, Usuario.UsuarioID
		,'-----------------' AS ClienteProveedor
		,'-----' AS Origen
		,Sucursal.NombreSucursal AS Destino
		,MovimientoInventarioDetalle.Cantidad
		,PartePrecio.Costo AS Importe
		, 'MovimientoInventario' AS RelacionTabla
		, MovimientoInventario.MovimientoInventarioID AS RelacionID
		-- ,'E' AS Tipo
		-- ,'ENTRADA INVENTARIO' AS Operacion
		-- ,Usuario.NombreUsuario
		-- ,0.0 AS ExistenciaNueva
		,2 AS Orden
	FROM 
		MovimientoInventario 
		INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
		INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
		INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
	WHERE 
		MovimientoInventario.TipoOperacionID = 2
		AND MovimientoInventario.Estatus = 1
		-- AND MovimientoInventarioDetalle.ParteID = @ParteId			
		AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		-- Se excluyen los movimientos provenientes de conflicto
		AND MovimientoInventario.MovimientoInventarioID NOT IN (SELECT MovID FROM @MovsDeConflicto)
		
	/* SALIDA INVENTARIO */
	UNION
	SELECT 
		MovimientoInventarioDetalle.ParteID
		, @OpSalidaInventario AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
		,MovimientoInventario.FechaRegistro AS Fecha
		, Usuario.UsuarioID
		,'-----------------' AS ClienteProveedor
		,'-----' AS Origen
		,Sucursal.NombreSucursal AS Destino
		, (MovimientoInventarioDetalle.Cantidad * -1) AS Cantidad
		,PartePrecio.Costo AS Importe
		, 'MovimientoInventario' AS RelacionTabla
		, MovimientoInventario.MovimientoInventarioID AS RelacionID
		-- ,'S' AS Tipo
		-- ,'SALIDA INVENTARIO' AS Operacion
		-- ,Usuario.NombreUsuario
		-- ,0.0 AS ExistenciaNueva
		,6 AS Orden
	FROM 
		MovimientoInventario 
		INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
		INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
		INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
	WHERE 
		MovimientoInventario.TipoOperacionID = 3
		AND MovimientoInventario.Estatus = 1
		-- AND MovimientoInventarioDetalle.ParteID = @ParteId
		AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		-- Se excluyen los movimientos provenientes de conflicto
		AND MovimientoInventario.MovimientoInventarioID NOT IN (SELECT MovID FROM @MovsDeConflicto)

	/* SE AGREGAN LOS MOVIMIENTOS PROVENIENTES DE CONFLICTOS */
	UNION
	SELECT
		mid.ParteID
		, (CASE WHEN mi.TipoOperacionID = @TipoOpEntradaInv THEN @OpEntradaInventario ELSE @OpSalidaInventario END) AS OperacionID
		, mif.SucursalOrigenID AS SucursalID
		, CAST(mc.MovFuenteID AS VARCHAR) AS Folio
		, mi.FechaRegistro AS Fecha
		, u.UsuarioID
		, '-----------------' AS ClienteProveedor
		, '-----' AS Origen
		, s.NombreSucursal AS Destino
		, (mid.Cantidad * (CASE WHEN mi.TipoOperacionID = @TipoOpEntradaInv THEN 1 ELSE -1 END)) AS Cantidad
		, pp.Costo AS Importe
		, 'MovimientoInventario' AS RelacionTabla
		, mc.MovID AS RelacionID
		-- , (CASE WHEN mi.TipoOperacionID = @TipoOpEntradaInv THEN 'E' ELSE 'S' END) AS Tipo
		-- , ((CASE WHEN mi.TipoOperacionID = @TipoOpEntradaInv THEN 'ENTRADA' ELSE 'SALIDA' END) + ' INVENTARIO') AS Operacion
		-- , u.NombreUsuario
		-- , 0.0 AS ExistenciaNueva
		, 6 AS Orden
	FROM 
		@MovsDeConflicto mc
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mc.MovID AND mi.Estatus = 1
		LEFT JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioID = mi.MovimientoInventarioID AND mid.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = mi.SucursalDestinoID AND s.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = mid.ParteID AND pp.Estatus = 1
		LEFT JOIN MovimientoInventario mif ON mif.MovimientoInventarioID = mc.MovFuenteID AND mif.Estatus = 1
	WHERE
		mc.OperacionID IN (@TipoOpEntradaInv, @TipoOpSalidaInv)

	/* DEVOLUCION A PROVEEDOR */
	UNION
	SELECT
		MovimientoInventarioDetalle.ParteID
		, @OpDevolucionAProveedor AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
		, MovimientoInventario.FechaRegistro AS Fecha
		, Usuario.UsuarioID
		,Proveedor.NombreProveedor AS ClienteProveedor
		,Sucursal.NombreSucursal AS Origen
		, tco.NombreConceptoOperacion AS Destino
		, (CASE WHEN MovimientoInventario.TipoConceptoOperacionID = @TipoConOpGarantia THEN 0
			ELSE (MovimientoInventarioDetalle.Cantidad * -1) END) AS Cantidad
		, MovimientoInventarioDetalle.PrecioUnitario AS Importe
		, 'MovimientoInventario' AS RelacionTabla
		, MovimientoInventario.MovimientoInventarioID AS RelacionID
		-- ,'S' AS Tipo
		-- ,'DEVOLUCION A PROVEEDOR' AS Operacion
		-- ,Usuario.NombreUsuario
		-- ,PartePrecio.Costo * @Iva AS Unitario
		-- ,0.0 AS ExistenciaNueva
		,8 AS Orden
	FROM 
		MovimientoInventario 
		INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
		INNER JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
		INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
		LEFT JOIN TipoConceptoOperacion tco ON tco.TipoConceptoOperacionID = MovimientoInventario.TipoConceptoOperacionID
			AND tco.Estatus = 1
	WHERE 
		MovimientoInventario.TipoOperacionID = 4 
		AND MovimientoInventario.Estatus = 1
		-- AND MovimientoInventarioDetalle.ParteID = @ParteId
		AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

	/* TRASPASO SALIDA */
	UNION
	SELECT
		MovimientoInventarioDetalle.ParteID
		, @OpTraspasoSalida AS OperacionID
		, MovimientoInventario.SucursalOrigenID AS SucursalID
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
		, MovimientoInventario.FechaRegistro AS Fecha
		, Usuario.UsuarioID
		,'-----------------' AS ClienteProveedor
		,O.NombreSucursal AS Origen
		,D.NombreSucursal AS Destino
		, (MovimientoInventarioDetalle.Cantidad * -1) AS Cantidad
		,PartePrecio.Costo AS Importe
		, 'MovimientoInventario' AS RelacionTabla
		, MovimientoInventario.MovimientoInventarioID AS RelacionID
		-- ,'S' AS Tipo
		-- ,'TRASPASO SALIDA' AS Operacion
		-- ,Usuario.NombreUsuario
		-- ,0.0 AS ExistenciaNueva
		,7 AS Orden
	FROM 
		MovimientoInventario 
		INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
		INNER JOIN Sucursal O ON O.SucursalID = MovimientoInventario.SucursalOrigenID 
		INNER JOIN Sucursal D ON D.SucursalID = MovimientoInventario.SucursalDestinoID 
		INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
	WHERE 
		MovimientoInventario.TipoOperacionID = 5
		AND MovimientoInventario.Estatus = 1
		-- AND MovimientoInventarioDetalle.ParteID = @ParteId		
		AND MovimientoInventario.SucursalOrigenID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		-- Se excluyen los movimientos provenientes de conflicto
		AND MovimientoInventario.MovimientoInventarioID NOT IN (SELECT MovID FROM @MovsDeConflicto)
		
	/* TRASPASO ENTRADA */
	UNION
	SELECT
		MovimientoInventarioDetalle.ParteID
		, @OpTraspasoEntrada AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio 
		, MovimientoInventario.FechaRecepcion AS Fecha
		, Usuario.UsuarioID
		,'-----------------' AS ClienteProveedor
		,O.NombreSucursal AS Origen
		,D.NombreSucursal AS Destino
		, ISNULL(tg.CantidadRecibida, MovimientoInventarioDetalle.Cantidad) AS Cantidad
		,PartePrecio.Costo AS Importe
		, 'MovimientoInventario' AS RelacionTabla
		, MovimientoInventario.MovimientoInventarioID AS RelacionID
		-- ,'E' AS Tipo
		-- ,'TRASPASO ENTRADA' AS Operacion
		-- ,Usuario.NombreUsuario
		-- ,0.0 AS ExistenciaNueva
		,3 AS Orden
	FROM 
		MovimientoInventario 
		INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
		LEFT JOIN MovimientoInventarioTraspasoContingencia tg ON tg.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
			AND tg.ParteID = MovimientoInventarioDetalle.ParteID AND tg.Estatus = 1
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioRecibioTraspasoID 
		INNER JOIN Sucursal O ON O.SucursalID = MovimientoInventario.SucursalOrigenID 
		INNER JOIN Sucursal D ON D.SucursalID = MovimientoInventario.SucursalDestinoID 
		INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
	WHERE 
		MovimientoInventario.TipoOperacionID = 5
		AND MovimientoInventario.Estatus = 1
		-- AND MovimientoInventarioDetalle.ParteID = @ParteId		
		AND MovimientoInventario.FechaRecepcion IS NOT NULL
		AND MovimientoInventario.UsuarioRecibioTraspasoID IS NOT NULL
		AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

	/* CONFLICTO TRASPASO RESUELTO CON SALIDA DE INVENTARIO - SE AGREGA REGISTRO DE ENTRADA */
	UNION
	SELECT
		tg.ParteID
		, @OpTraspasoEntrada AS OperacionID
		, mi.SucursalOrigenID AS SucursalID
		, CAST(mi.MovimientoInventarioID AS VARCHAR) AS Folio 
		, tg.FechaSoluciono AS Fecha
		, u.UsuarioID
		, 'CONFLICTO RESUELTO SALIDA' AS ClienteProveedor
		, so.NombreSucursal AS Origen
		, sd.NombreSucursal AS Destino
		, tg.CantidadEnviada AS Cantidad
		, pp.Costo AS Importe
		, 'MovimientoInventario' AS RelacionTabla
		, mid.MovimientoInventarioID AS RelacionID
		-- , 'E' AS Tipo
		-- , 'TRASPASO ENTRADA' AS Operacion
		-- , u.NombreUsuario
		-- , 0.0 AS ExistenciaNueva
		, 6 AS Orden
	FROM
		MovimientoInventarioTraspasoContingencia tg
		LEFT JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioDetalleID = tg.MovimientoInventarioDetalleID AND mid.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = tg.UsuarioSolucionoID AND u.Estatus = 1
		LEFT JOIN Sucursal so ON so.SucursalID = mi.SucursalOrigenID AND so.Estatus = 1
		LEFT JOIN Sucursal sd ON sd.SucursalID = mi.SucursalDestinoID AND sd.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = tg.ParteID AND pp.Estatus = 1
	WHERE
		tg.MovimientoInventarioEstatusContingenciaID = 1
		AND tg.TipoOperacionID = 3

	/* VENTA (PAGADA Y COBRADA) */
	UNION
	SELECT
		VentaDetalle.ParteID
		, @OpVenta AS OperacionID
		, Venta.SucursalID AS SucursalID
		,Venta.Folio
		, Venta.Fecha AS Fecha
		, Usuario.UsuarioID
		,Cliente.Nombre AS ClienteProveedor
		,Sucursal.NombreSucursal AS Origen
		,CAST(Venta.ClienteID AS VARCHAR) AS Destino
		, (VentaDetalle.Cantidad * -1) AS Cantidad
		,(VentaDetalle.PrecioUnitario + VentaDetalle.Iva) AS Importe
		, 'Venta' AS RelacionTabla
		, Venta.VentaID AS RelacionID
		-- ,'S' AS Tipo
		-- ,'VENTAS' AS Operacion
		-- ,Usuario.NombreUsuario
		-- ,0.0 AS ExistenciaNueva
		,5 AS Orden
	FROM
		Venta
		LEFT JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID
		LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
		LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.RealizoUsuarioID
		LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
	WHERE
		Venta.Estatus = 1
		AND	Venta.Fecha > '2013-12-12'
		AND Venta.VentaEstatusID IN (2, 3)
		-- AND VentaDetalle.ParteID = @ParteId
		AND VentaDetalle.Estatus = 1 
		AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena('1,2,3', ','))

	/* VENTA (CANCELADA) */
	UNION		
	SELECT * FROM (
		SELECT
			VentaDevolucionDetalle.ParteID
			, @OpVenta AS OperacionID
			, VentaDevolucion.SucursalID AS SucursalID
			,CAST(Venta.Folio AS VARCHAR) AS Folio
			, Venta.Fecha AS Fecha
			, Usuario.UsuarioID
			,Cliente.Nombre AS ClienteProveedor
			,Sucursal.NombreSucursal AS Origen
			,CAST(Venta.ClienteID AS VARCHAR) AS Destino
			, (VentaDevolucionDetalle.Cantidad * -1) AS Cantidad
			, (VentaDevolucionDetalle.PrecioUnitario + VentaDevolucionDetalle.Iva) AS Importe
			, 'Venta' AS RelacionTabla
			, Venta.VentaID AS RelacionID
			-- ,'S' AS Tipo
			-- ,'VENTAS' AS Operacion
			-- ,Usuario.NombreUsuario
			-- ,0.0 AS ExistenciaNueva	
			,5 AS Orden
		FROM
			Venta
			LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
			LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
			LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
			LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.RealizoUsuarioID
			LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
		WHERE
			Venta.Estatus = 1
			AND	Venta.Fecha > '2013-12-12'
			AND Venta.VentaEstatusID IN (4, 5)
			-- AND VentaDevolucionDetalle.ParteID = @ParteId
			AND VentaDevolucion.EsCancelacion = 1
			AND VentaDevolucionDetalle.Estatus = 1
			AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
	
		UNION			
		SELECT
			VentaDevolucionDetalle.ParteID
			, @OpVentaDev AS OperacionID
			, VentaDevolucion.SucursalID AS SucursalID
			,CAST(Venta.Folio AS VARCHAR) AS Folio
			, VentaDevolucion.Fecha AS Fecha
			, Usuario.UsuarioID
			,Cliente.Nombre AS ClienteProveedor
			,Sucursal.NombreSucursal AS Origen
			,CAST(Venta.ClienteID AS VARCHAR) AS Destino
			,VentaDevolucionDetalle.Cantidad
			, (VentaDevolucionDetalle.PrecioUnitario + VentaDevolucionDetalle.Iva) AS Importe
			, 'VentaDevolucion' AS RelacionTabla
			, VentaDevolucion.VentaDevolucionID AS RelacionID
			-- ,'E' AS Tipo
			-- ,'VENTA CANCELADA' AS Operacion
			-- ,Usuario.NombreUsuario
			-- ,0.0 AS ExistenciaNueva	
			,4 AS Orden
		FROM
			Venta
			LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
			LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
			LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
			LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.RealizoUsuarioID
			LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
		WHERE
			Venta.Estatus = 1
			AND	Venta.Fecha > '2013-12-12'
			AND Venta.VentaEstatusID IN (4, 5)
			-- AND VentaDevolucionDetalle.ParteID = @ParteId
			AND VentaDevolucion.EsCancelacion = 1
			AND VentaDevolucionDetalle.Estatus = 1
			AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		) AS Canceldas

	/* VENTA (DEVUELTA) */
	UNION
		SELECT * FROM (
		SELECT
			VentaDevolucionDetalle.ParteID
			, @OpVenta AS OperacionID
			, VentaDevolucion.SucursalID AS SucursalID
			,CAST(Venta.Folio AS VARCHAR) AS Folio
			, Venta.Fecha AS Fecha
			, Usuario.UsuarioID
			,Cliente.Nombre AS ClienteProveedor
			,Sucursal.NombreSucursal AS Origen
			,CAST(Venta.ClienteID AS VARCHAR) AS Destino
			, (VentaDevolucionDetalle.Cantidad * -1) AS Cantidad
			, (VentaDevolucionDetalle.PrecioUnitario + VentaDevolucionDetalle.Iva) AS Importe
			, 'Venta' AS RelacionTabla
			, Venta.VentaID AS RelacionID
			-- ,'S' AS Tipo
			-- ,'VENTAS' AS Operacion
			-- ,Usuario.NombreUsuario
			-- ,0.0 AS ExistenciaNueva
			,5 AS Orden
		FROM
			Venta
			LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
			LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
			LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
			LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.RealizoUsuarioID
			LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
		WHERE
			Venta.Estatus = 1
			AND	Venta.Fecha > '2013-12-12'
			-- AND Venta.VentaEstatusID IN (4, 5)
			-- AND VentaDevolucionDetalle.ParteID = @ParteId
			AND VentaDevolucion.EsCancelacion = 0
			AND VentaDevolucionDetalle.Estatus = 1
			AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		
		UNION
		SELECT
			VentaDevolucionDetalle.ParteID
			, @OpVentaDev AS OperacionID
			, VentaDevolucion.SucursalID AS SucursalID
			,CAST(Venta.Folio AS VARCHAR) AS Folio
			, VentaDevolucion.Fecha AS Fecha
			, Usuario.UsuarioID
			,Cliente.Nombre AS ClienteProveedor
			,Sucursal.NombreSucursal AS Origen
			,CAST(Venta.ClienteID AS VARCHAR) AS Destino
			,VentaDevolucionDetalle.Cantidad
			, (VentaDevolucionDetalle.PrecioUnitario + VentaDevolucionDetalle.Iva) AS Importe
			, 'VentaDevolucion' AS RelacionTabla
			, VentaDevolucion.VentaDevolucionID AS RelacionID
			-- ,'E' AS Tipo
			-- ,'VENTA DEVUELTA' AS Operacion
			-- ,Usuario.NombreUsuario
			-- ,0.0 AS ExistenciaNueva
			,9 AS Orden	
		FROM
			Venta
			LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
			LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
			LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
			LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.RealizoUsuarioID
			LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
		WHERE
			Venta.Estatus = 1
			AND	Venta.Fecha > '2013-12-12'
			-- AND Venta.VentaEstatusID IN (4, 5)
			-- AND VentaDevolucionDetalle.ParteID = @ParteId
			AND VentaDevolucion.EsCancelacion = 0
			AND VentaDevolucionDetalle.Estatus = 1
			AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
	) AS Devoluciones
	
	/* Ventas Garantías */
	UNION		
	SELECT * FROM (
		SELECT
			vg.ParteID
			, @OpVenta AS OperacionID
			, v.SucursalID AS SucursalID
			, v.Folio
			, v.Fecha AS Fecha
			, u.UsuarioID
			, c.Nombre AS ClienteProveedor
			, s.NombreSucursal AS Origen
			, '----' AS Destino
			, -1 AS Importe
			, (vg.PrecioUnitario + vg.Iva) AS Unitario
			, 'Venta' AS RelacionTabla
			, v.VentaID AS RelacionID
			-- , 'S' AS Tipo
			-- , 'VENTA' AS Operacion
			-- , u.NombreUsuario
			-- , 0.0 AS ExistenciaNueva
			, 10 AS Orden
		FROM
			Venta v
			LEFT JOIN VentaGarantia vg ON vg.VentaID = v.VentaID AND vg.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
			LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		WHERE
			v.Estatus = 1
			AND	v.Fecha > '2013-12-12'
			AND v.VentaEstatusID IN (6)
			AND v.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

		UNION
		SELECT
			vg.ParteID
			, @OpVentaDev AS OperacionID
			, vg.SucursalID AS SucursalID
			, CAST(v.Folio AS VARCHAR) AS Folio
			, vg.Fecha AS Fecha
			, u.UsuarioID
			, c.Nombre AS ClienteProveedor
			, s.NombreSucursal AS Origen
			, CAST(vg.VentaGarantiaID AS VARCHAR) AS Destino
			, 0 AS Cantidad
			, (vg.PrecioUnitario + vg.Iva) AS Importe
			, 'VentaGarantia' AS RelacionTabla
			, vg.VentaGarantiaID AS RelacionID
			-- , 'E' AS Tipo
			-- , 'VENTA GARANTÍA' AS Operacion
			-- , u.NombreUsuario
			-- , 0.0 AS ExistenciaNueva
			, 11 AS Orden
		FROM
			Venta v
			LEFT JOIN VentaGarantia vg ON vg.VentaID = v.VentaID AND vg.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			LEFT JOIN Usuario u ON u.UsuarioID = vg.RealizoUsuarioID AND u.Estatus = 1
			LEFT JOIN Sucursal s ON s.SucursalID = vg.SucursalID AND s.Estatus = 1
		WHERE
			v.Estatus = 1
			AND	v.Fecha > '2013-12-12'
			AND v.VentaEstatusID IN (6)
			AND vg.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
	) AS Garantias
	
	/* Casco recibido por venta de acumulador */
	UNION
	SELECT
		cr.RecibidoCascoID
		, @OpEntradaInventario AS OperacionID
		, v.SucursalID
		, CONVERT(NVARCHAR(8), cr.CascoRegistroID) AS Folio
		, cr.Fecha
		, cr.RealizoUsuarioID AS UsuarioID
		, c.Nombre AS ClienteProveedor
		, 'CONTROL DE CASCOS' AS Origen
		, s.NombreSucursal AS Destino
		, 1 AS Cantidad
		, (pp.Costo * @Iva) AS Importe
		, 'CascoRegistro' AS RelacionTabla
		, cr.CascoRegistroID AS RelacionID
		-- ,'E' AS Tipo
		-- ,'CASCO RECIBIDO' AS Operacion
		-- , NULL AS NombreUsuario
		-- , 0.0 AS ExistenciaNueva
		, 12 AS Orden
	FROM
		CascoRegistro cr
		INNER JOIN Venta v ON v.VentaID = cr.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		-- LEFT JOIN Usuario u ON u.UsuarioID = cr.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = cr.ParteID AND pp.Estatus = 1
	WHERE
		cr.RecibidoCascoID IS NOT NULL

	/* Gasto por casco, casco comprado */
	UNION
	SELECT
		CASE ce.ContaCuentaAuxiliarID
			WHEN @CaCascoChico THEN @PaDepCascoChico
			WHEN @CaCascoMediano THEN @PaDepCascoMediano
			WHEN @CaCascoGrande THEN @PaDepCascoGrande
			WHEN @CaCascoExtra THEN @PaDepCascoExtra
		END AS ParteID
		, @OpEntradaInventario AS OperacionID
		, ce.SucursalID
		, CONVERT(NVARCHAR(8), cje.CajaEgresoID) AS Folio
		, ce.Fecha
		, ce.RealizoUsuarioID AS UsuarioID
		, '----' AS ClienteProveedor
		, 'GASTO POR CASCO' AS Origen
		, s.NombreSucursal AS Destino
		, 1 AS Cantidad
		, ce.Importe AS Importe
		, 'CascoRegistro' AS RelacionTabla
		, NULL AS RelacionID
		-- ,'E' AS Tipo
		-- ,'CASCO COMPRADO' AS Operacion
		-- , NULL AS NombreUsuario
		-- , 0.0 AS ExistenciaNueva
		, 13 AS Orden
	FROM
		ContaEgreso ce
		LEFT JOIN CajaEgreso cje ON cje.ContaEgresoID = ce.ContaEgresoID AND cje.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = ce.SucursalID AND s.Estatus = 1
	WHERE
		ce.ContaCuentaAuxiliarID IN (@CaCascoChico, @CaCascoMediano, @CaCascoGrande, @CaCascoExtra)

	/* Devolución de parte que tiene casco */
	UNION
	SELECT
		cr.RecibidoCascoID
		, @OpSalidaInventario AS OperacionID
		, vd.SucursalID
		, CONVERT(NVARCHAR(8), cr.CascoRegistroID) AS Folio
		, vd.Fecha
		, vd.RealizoUsuarioID AS UsuarioID
		, c.Nombre AS ClienteProveedor
		, 'CONTROL DE CASCOS' AS Origen
		, s.NombreSucursal AS Destino
		, -1 AS Cantidad
		, pp.Costo AS Importe
		, 'CascoRegistro' AS RelacionTabla
		, cr.CascoRegistroID AS RelacionID
		-- ,'S' AS Tipo
		-- ,'CASCO DEVUELTO' AS Operacion
		-- , NULL AS NombreUsuario
		-- , 0.0 AS ExistenciaNueva
		, 14 AS Orden
	FROM
		VentaDevolucionDetalle vdd
		INNER JOIN VentaDevolucion vd ON vd.VentaDevolucionID = vdd.VentaDevolucionID AND vd.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
		INNER JOIN CascoRegistro cr ON cr.VentaID = vd.VentaID
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = vd.SucursalID AND s.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = vdd.ParteID AND pp.Estatus = 1
	WHERE
		vdd.Estatus = 1
		AND p.RequiereCascoDe IS NOT NULL
		AND cr.RecibidoCascoID IS NOT NULL

) AS Kardex

-- where ParteID = 7

ORDER BY
	Kardex.Fecha ASC
	, Kardex.Orden
	, Kardex.SucursalID

GO


-- Proceso de inserción para existencia nueva
DECLARE @OpVenta INT = 1
DECLARE @OpVentaDev INT = 2
DECLARE @OpEntradaCompra INT = 3
DECLARE @OpDevolucionAProveedor INT = 4
DECLARE @OpEntradaInventario INT = 5
DECLARE @OpSalidaInventario INT = 6
DECLARE @OpTraspasoEntrada INT = 7
DECLARE @OpTraspasoSalida INT = 8

DECLARE @Existencia DECIMAL(12, 2), @UltParteID INT, @UltSucursalID INT
DECLARE @ParteKardexID INT, @ParteID INT, @OperacionID INT, @SucursalID INT, @Cantidad DECIMAL(12, 2)

DECLARE cKardex CURSOR FOR
SELECT ParteKardexID, OperacionID, ParteID, SucursalID, Cantidad
FROM ParteKardex
-- where parteid = 6296
ORDER BY ParteID, SucursalID, Fecha, ParteKardexID

OPEN cKardex
FETCH NEXT FROM cKardex INTO @ParteKardexID, @OperacionID, @ParteID, @SucursalID, @Cantidad
SET @UltParteID = 0
SET @UltSucursalID = 0

WHILE @@FETCH_STATUS = 0 BEGIN
	IF @ParteID != @UltParteID OR @SucursalID != @UltSucursalID BEGIN
		SET @UltParteID = @ParteID
		SET @UltSucursalID = @SucursalID
		SET @Existencia = 0
	END

	SET @Existencia = (@Existencia + @Cantidad)
		/*
		CASE @OperacionID
			WHEN @OpVenta THEN -1
			WHEN @OpVentaDev THEN 1
			WHEN @OpEntradaCompra THEN 1
			WHEN @OpDevolucionAProveedor THEN -1
			WHEN @OpEntradaInventario THEN 1
			WHEN @OpSalidaInventario THEN -1
			WHEN @OpTraspasoEntrada THEN 1
			WHEN @OpTraspasoSalida THEN -1
		END
		*/
	-- )

	UPDATE ParteKardex SET ExistenciaNueva = @Existencia WHERE ParteKardexID = @ParteKardexID
	
	FETCH NEXT FROM cKardex INTO @ParteKardexID, @OperacionID, @ParteID, @SucursalID, @Cantidad
END

CLOSE cKardex
DEALLOCATE cKardex