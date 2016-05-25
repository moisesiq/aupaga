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
	, Entidad, Origen, Destino, Cantidad, Importe, ExistenciaNueva)

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
	, ISNULL(Unitario, 0.0) AS Importe
	, 0.0
FROM (

	/* Existencia Inicial */
	SELECT
		pe.FechaRegistro AS Fecha
		, NULL AS Folio
		, 'E' AS Tipo
		, 'ENTRADA INICIAL' AS Operacion
		, 'ENTRADA INICIAL' AS ClienteProveedor
		, NULL AS NombreUsuario
		, NULL AS Origen
		, s.NombreSucursal AS Destino
		, pp.Costo AS Unitario
		, pe.ExistenciaInicial AS Cantidad
		, 0.0 AS ExistenciaNueva
		, 0 AS Orden
		, pe.ParteID
		, @OpEntradaInventario AS OperacionID
		, pe.SucursalID
		, pe.UsuarioID
	FROM
		ParteExistencia pe
		LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = pe.SucursalID AND s.Estatus = 1
	WHERE pe.Estatus = 1

	/* ENTRADA COMPRA */
	UNION
	SELECT
		MovimientoInventario.FechaRegistro AS Fecha
		,CAST(MovimientoInventario.FolioFactura AS VARCHAR) AS Folio
		,'E' AS Tipo
		,'ENTRADA COMPRA' AS Operacion
		,Proveedor.NombreProveedor AS ClienteProveedor
		,Usuario.NombreUsuario
		,CAST(MovimientoInventario.ProveedorID AS VARCHAR) AS Origen
		,Sucursal.NombreSucursal AS Destino
		,MovimientoInventarioDetalle.PrecioUnitario AS Unitario
		,MovimientoInventarioDetalle.Cantidad
		,0.0 AS ExistenciaNueva
		,1 AS Orden
		, MovimientoInventarioDetalle.ParteID
		, @OpEntradaCompra AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		, Usuario.UsuarioID
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
		MovimientoInventario.FechaRegistro AS Fecha
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
		,'E' AS Tipo
		,'ENTRADA INVENTARIO' AS Operacion
		,'-----------------' AS ClienteProveedor
		,Usuario.NombreUsuario
		,'-----' AS Origen
		,Sucursal.NombreSucursal AS Destino
		,PartePrecio.Costo AS Unitario
		,MovimientoInventarioDetalle.Cantidad
		,0.0 AS ExistenciaNueva
		,2 AS Orden
		, MovimientoInventarioDetalle.ParteID
		, @OpEntradaInventario AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		, Usuario.UsuarioID
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
		MovimientoInventario.FechaRegistro AS Fecha
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
		,'S' AS Tipo
		,'SALIDA INVENTARIO' AS Operacion
		,'-----------------' AS ClienteProveedor
		,Usuario.NombreUsuario
		,'-----' AS Origen
		,Sucursal.NombreSucursal AS Destino
		,PartePrecio.Costo AS Unitario
		, (MovimientoInventarioDetalle.Cantidad * -1) AS Cantidad
		,0.0 AS ExistenciaNueva
		,6 AS Orden
		, MovimientoInventarioDetalle.ParteID
		, @OpSalidaInventario AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		, Usuario.UsuarioID
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
		mi.FechaRegistro AS Fecha
		, CAST(mc.MovFuenteID AS VARCHAR) AS Folio
		, (CASE WHEN mi.TipoOperacionID = @TipoOpEntradaInv THEN 'E' ELSE 'S' END) AS Tipo
		, ((CASE WHEN mi.TipoOperacionID = @TipoOpEntradaInv THEN 'ENTRADA' ELSE 'SALIDA' END) + ' INVENTARIO') AS Operacion
		, '-----------------' AS ClienteProveedor
		, u.NombreUsuario
		, '-----' AS Origen
		, s.NombreSucursal AS Destino
		, pp.Costo AS Unitario
		, (mid.Cantidad * (CASE WHEN mi.TipoOperacionID = @TipoOpEntradaInv THEN 1 ELSE -1 END)) AS Cantidad
		, 0.0 AS ExistenciaNueva
		, 6 AS Orden
		, mid.ParteID
		, (CASE WHEN mi.TipoOperacionID = @TipoOpEntradaInv THEN @OpEntradaInventario ELSE @OpSalidaInventario END) AS OperacionID
		, mif.SucursalOrigenID AS SucursalID
		, u.UsuarioID
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
		MovimientoInventario.FechaRegistro AS Fecha
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
		,'S' AS Tipo
		,'DEVOLUCION A PROVEEDOR' AS Operacion
		,Proveedor.NombreProveedor AS ClienteProveedor
		,Usuario.NombreUsuario
		,Sucursal.NombreSucursal AS Origen
		,CAST(MovimientoInventario.ProveedorID AS VARCHAR) AS Destino
		-- ,PartePrecio.Costo * @Iva AS Unitario
		, MovimientoInventarioDetalle.PrecioUnitario AS Unitario
		, (CASE WHEN MovimientoInventario.TipoConceptoOperacionID = @TipoConOpGarantia THEN 0 ELSE (MovimientoInventarioDetalle.Cantidad * -1) END) AS Cantidad
		,0.0 AS ExistenciaNueva
		,8 AS Orden
		, MovimientoInventarioDetalle.ParteID
		, @OpDevolucionAProveedor AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		, Usuario.UsuarioID
	FROM 
		MovimientoInventario 
		INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
		INNER JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
		INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
	WHERE 
		MovimientoInventario.TipoOperacionID = 4 
		AND MovimientoInventario.Estatus = 1
		-- AND MovimientoInventarioDetalle.ParteID = @ParteId
		AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

	/* TRASPASO SALIDA */
	UNION
	SELECT 
		MovimientoInventario.FechaRegistro AS Fecha
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio 
		,'S' AS Tipo
		,'TRASPASO SALIDA' AS Operacion
		,'-----------------' AS ClienteProveedor
		,Usuario.NombreUsuario
		,O.NombreSucursal AS Origen
		,D.NombreSucursal AS Destino
		,PartePrecio.Costo AS Unitario
		, (MovimientoInventarioDetalle.Cantidad * -1) AS Cantidad
		,0.0 AS ExistenciaNueva
		,7 AS Orden
		, MovimientoInventarioDetalle.ParteID
		, @OpTraspasoSalida AS OperacionID
		, MovimientoInventario.SucursalOrigenID AS SucursalID
		, Usuario.UsuarioID
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
		MovimientoInventario.FechaRecepcion AS Fecha
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio 
		,'E' AS Tipo
		,'TRASPASO ENTRADA' AS Operacion
		,'-----------------' AS ClienteProveedor
		,Usuario.NombreUsuario
		,O.NombreSucursal AS Origen
		,D.NombreSucursal AS Destino
		,PartePrecio.Costo AS Unitario
		, ISNULL(tg.CantidadRecibida, MovimientoInventarioDetalle.Cantidad) AS Cantidad
		,0.0 AS ExistenciaNueva
		,3 AS Orden
		, MovimientoInventarioDetalle.ParteID
		, @OpTraspasoEntrada AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		, Usuario.UsuarioID
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
		tg.FechaSoluciono AS Fecha
		, CAST(mi.MovimientoInventarioID AS VARCHAR) AS Folio 
		, 'E' AS Tipo
		, 'TRASPASO ENTRADA' AS Operacion
		, 'CONFLICTO RESUELTO SALIDA' AS ClienteProveedor
		, u.NombreUsuario
		, so.NombreSucursal AS Origen
		, sd.NombreSucursal AS Destino
		, pp.Costo AS Unitario
		, tg.CantidadEnviada AS Cantidad
		, 0.0 AS ExistenciaNueva
		, 6 AS Orden
		, tg.ParteID
		, @OpTraspasoEntrada AS OperacionID
		, mi.SucursalOrigenID AS SucursalID
		, u.UsuarioID
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
		Venta.Fecha AS Fecha		
		,Venta.Folio
		,'S' AS Tipo
		,'VENTAS' AS Operacion
		,Cliente.Nombre AS ClienteProveedor
		,Usuario.NombreUsuario
		,Sucursal.NombreSucursal AS Origen
		,CAST(Venta.ClienteID AS VARCHAR) AS Destino
		,(VentaDetalle.PrecioUnitario + VentaDetalle.Iva) AS Unitario
		, (VentaDetalle.Cantidad * -1) AS Cantidad
		,0.0 AS ExistenciaNueva
		,5 AS Orden
		, VentaDetalle.ParteID
		, 1 AS OperacionID
		, Venta.SucursalID AS SucursalID
		, Usuario.UsuarioID
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
			Venta.Fecha AS Fecha
			,CAST(Venta.Folio AS VARCHAR) AS Folio
			,'S' AS Tipo
			,'VENTAS' AS Operacion
			,Cliente.Nombre AS ClienteProveedor
			,Usuario.NombreUsuario
			,Sucursal.NombreSucursal AS Origen
			,CAST(Venta.ClienteID AS VARCHAR) AS Destino
			, (VentaDevolucionDetalle.PrecioUnitario + VentaDevolucionDetalle.Iva) AS Unitario
			, (VentaDevolucionDetalle.Cantidad * -1) AS Cantidad
			,0.0 AS ExistenciaNueva	
			,5 AS Orden	
			, VentaDevolucionDetalle.ParteID
			, @OpVenta AS OperacionID
			, VentaDevolucion.SucursalID AS SucursalID
			, Usuario.UsuarioID
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
			VentaDevolucion.Fecha AS Fecha
			,CAST(Venta.Folio AS VARCHAR) AS Folio
			,'E' AS Tipo
			,'VENTA CANCELADA' AS Operacion
			,Cliente.Nombre AS ClienteProveedor
			,Usuario.NombreUsuario
			,Sucursal.NombreSucursal AS Origen
			,CAST(Venta.ClienteID AS VARCHAR) AS Destino
			, (VentaDevolucionDetalle.PrecioUnitario + VentaDevolucionDetalle.Iva) AS Unitario
			,VentaDevolucionDetalle.Cantidad
			,0.0 AS ExistenciaNueva	
			,4 AS Orden		
			, VentaDevolucionDetalle.ParteID
			, @OpVentaDev AS OperacionID
			, VentaDevolucion.SucursalID AS SucursalID
			, Usuario.UsuarioID
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
			Venta.Fecha AS Fecha
			,CAST(Venta.Folio AS VARCHAR) AS Folio
			,'S' AS Tipo
			,'VENTAS' AS Operacion
			,Cliente.Nombre AS ClienteProveedor
			,Usuario.NombreUsuario
			,Sucursal.NombreSucursal AS Origen
			,CAST(Venta.ClienteID AS VARCHAR) AS Destino
			, (VentaDevolucionDetalle.PrecioUnitario + VentaDevolucionDetalle.Iva) AS Unitario
			, (VentaDevolucionDetalle.Cantidad * -1) AS Cantidad
			,0.0 AS ExistenciaNueva
			,5 AS Orden	
			, VentaDevolucionDetalle.ParteID
			, @OpVenta AS OperacionID
			, VentaDevolucion.SucursalID AS SucursalID
			, Usuario.UsuarioID
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
			VentaDevolucion.Fecha AS Fecha
			,CAST(Venta.Folio AS VARCHAR) AS Folio
			,'E' AS Tipo
			,'VENTA DEVUELTA' AS Operacion
			,Cliente.Nombre AS ClienteProveedor
			,Usuario.NombreUsuario
			,Sucursal.NombreSucursal AS Origen
			,CAST(Venta.ClienteID AS VARCHAR) AS Destino
			, (VentaDevolucionDetalle.PrecioUnitario + VentaDevolucionDetalle.Iva) AS Unitario
			,VentaDevolucionDetalle.Cantidad
			,0.0 AS ExistenciaNueva
			,9 AS Orden	
			, VentaDevolucionDetalle.ParteID
			, @OpVentaDev AS OperacionID
			, VentaDevolucion.SucursalID AS SucursalID
			, Usuario.UsuarioID
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
			v.Fecha AS Fecha
			, v.Folio
			, 'S' AS Tipo
			, 'VENTA' AS Operacion
			, c.Nombre AS ClienteProveedor
			, u.NombreUsuario
			, s.NombreSucursal AS Origen
			, '----' AS Destino
			, (vg.PrecioUnitario + vg.Iva) AS Unitario
			, -1 AS Cantidad
			, 0.0 AS ExistenciaNueva	
			, 10 AS Orden	
			, vg.ParteID
			, @OpVenta AS OperacionID
			, v.SucursalID AS SucursalID
			, u.UsuarioID
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
			vg.Fecha AS Fecha
			, CAST(v.Folio AS VARCHAR) AS Folio
			, 'E' AS Tipo
			, 'VENTA GARANTÍA' AS Operacion
			, c.Nombre AS ClienteProveedor
			, u.NombreUsuario
			, s.NombreSucursal AS Origen
			, CAST(vg.VentaGarantiaID AS VARCHAR) AS Destino
			, (vg.PrecioUnitario + vg.Iva) AS Unitario
			, 0 AS Cantidad
			, 0.0 AS ExistenciaNueva	
			, 11 AS Orden		
			, vg.ParteID
			, @OpVentaDev AS OperacionID
			, vg.SucursalID AS SucursalID
			, u.UsuarioID
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
		cr.Fecha
		, CONVERT(NVARCHAR(8), cr.CascoRegistroID) AS Folio
		,'E' AS Tipo
		,'CASCO RECIBIDO' AS Operacion
		, c.Nombre AS ClienteProveedor
		, NULL AS NombreUsuario
		, 'CONTROL DE CASCOS' AS Origen
		, s.NombreSucursal AS Destino
		, (pp.Costo * @Iva) AS Unitario
		, 1 AS Cantidad
		, 0.0 AS ExistenciaNueva
		, 12 AS Orden
		, cr.RecibidoCascoID
		, @OpEntradaInventario AS OperacionID
		, v.SucursalID
		, cr.RealizoUsuarioID AS UsuarioID
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
		ce.Fecha
		, CONVERT(NVARCHAR(8), cje.CajaEgresoID) AS Folio
		,'E' AS Tipo
		,'CASCO COMPRADO' AS Operacion
		, '----' AS ClienteProveedor
		, NULL AS NombreUsuario
		, 'GASTO POR CASCO' AS Origen
		, s.NombreSucursal AS Destino
		, ce.Importe AS Unitario
		, 1 AS Cantidad
		, 0.0 AS ExistenciaNueva
		, 13 AS Orden
		, CASE ce.ContaCuentaAuxiliarID
			WHEN @CaCascoChico THEN @PaDepCascoChico
			WHEN @CaCascoMediano THEN @PaDepCascoMediano
			WHEN @CaCascoGrande THEN @PaDepCascoGrande
			WHEN @CaCascoExtra THEN @PaDepCascoExtra
		END AS ParteID
		, @OpEntradaInventario AS OperacionID
		, ce.SucursalID
		, ce.RealizoUsuarioID AS UsuarioID
	FROM
		ContaEgreso ce
		LEFT JOIN CajaEgreso cje ON cje.ContaEgresoID = ce.ContaEgresoID AND cje.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = ce.SucursalID AND s.Estatus = 1
	WHERE
		ce.ContaCuentaAuxiliarID IN (@CaCascoChico, @CaCascoMediano, @CaCascoGrande, @CaCascoExtra)

	/* Devolución de parte que tiene casco */
	UNION
	SELECT
		vd.Fecha
		, CONVERT(NVARCHAR(8), cr.CascoRegistroID) AS Folio
		,'S' AS Tipo
		,'CASCO DEVUELTO' AS Operacion
		, c.Nombre AS ClienteProveedor
		, NULL AS NombreUsuario
		, 'CONTROL DE CASCOS' AS Origen
		, s.NombreSucursal AS Destino
		, pp.Costo AS Unitario
		, -1 AS Cantidad
		, 0.0 AS ExistenciaNueva
		, 14 AS Orden
		, cr.RecibidoCascoID
		, @OpSalidaInventario AS OperacionID
		, vd.SucursalID
		, vd.RealizoUsuarioID AS UsuarioID
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