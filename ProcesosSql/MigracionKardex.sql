/*
DELETE FROM ParteKardex
DBCC CHECKIDENT('ParteKardex', RESEED, 0)
*/

--DECLARE @ParteID AS INT = 2301
DECLARE @SucursalID AS NVARCHAR(10) =  N'1,2,3'
--DECLARE @FechaInicial AS DATE = '2013-11-17'
--DECLARE @FechaFinal AS DATE = '2014-02-17'
/*
*****************************
*****************************
DEBEMOS DE MOFIFICAR ESTE MIGRADOR
PARA QUE DONDE SÍ TENEMOS EL DATO PRECISO NO LO CALCULE
(ES EL CASO DEL COSTO DE COMPRAS, QUE LO TOMA DE LA TABLA PARTEPRECIO EN
VEZ DEL MOVIMIENTO COMPRAS)
*****************************
*****************************
*/
DECLARE @OpVenta INT = 1
DECLARE @OpVentaDev INT = 2
DECLARE @OpEntradaCompra INT = 3
DECLARE @OpDevolucionAProveedor INT = 4
DECLARE @OpEntradaInventario INT = 5
DECLARE @OpSalidaInventario INT = 6
DECLARE @OpTraspasoEntrada INT = 7
DECLARE @OpTraspasoSalida INT = 8

DECLARE @Iva AS DECIMAL(18,2) = (((SELECT CAST(Valor AS DECIMAL(18,2))FROM Configuracion WHERE Configuracion.ConfiguracionID = 1)/100) + 1)

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
	, ISNULL(Unitario, 0.0) AS Unitario
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
		, pp.Costo * @Iva AS Unitario
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
		,PartePrecio.Costo AS Unitario
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
		INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
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
		,PartePrecio.Costo * @Iva AS Unitario
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
		,PartePrecio.Costo * @Iva AS Unitario
		,MovimientoInventarioDetalle.Cantidad
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

	/* DEVOLUCION A PROVEEDOR */
	UNION
	SELECT 
		MovimientoInventario.FechaRegistro AS Fecha
		,CAST(MovimientoInventario.FolioFactura AS VARCHAR) AS Folio
		,'S' AS Tipo
		,'DEVOLUCION A PROVEEDOR' AS Operacion
		,Proveedor.NombreProveedor AS ClienteProveedor
		,Usuario.NombreUsuario
		,Sucursal.NombreSucursal AS Origen
		,CAST(MovimientoInventario.ProveedorID AS VARCHAR) AS Destino
		,PartePrecio.Costo * @Iva AS Unitario
		,MovimientoInventarioDetalle.Cantidad
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
		INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
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
		,PartePrecio.Costo * @Iva AS Unitario
		,MovimientoInventarioDetalle.Cantidad
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
		,PartePrecio.Costo * @Iva AS Unitario
		,MovimientoInventarioDetalle.Cantidad
		,0.0 AS ExistenciaNueva
		,3 AS Orden
		, MovimientoInventarioDetalle.ParteID
		, @OpTraspasoEntrada AS OperacionID
		, MovimientoInventario.SucursalDestinoID AS SucursalID
		, Usuario.UsuarioID
	FROM 
		MovimientoInventario 
		INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
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
		,VentaDetalle.PrecioUnitario * @Iva AS Unitario
		,VentaDetalle.Cantidad
		,0.0 AS ExistenciaNueva
		,5 AS Orden
		, VentaDetalle.ParteID
		, @OpVenta AS OperacionID
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
		AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

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
			,VentaDevolucionDetalle.PrecioUnitario * @Iva AS Unitario
			,VentaDevolucionDetalle.Cantidad
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
			,VentaDevolucionDetalle.PrecioUnitario * @Iva AS Unitario
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
			,VentaDevolucionDetalle.PrecioUnitario * @Iva AS Unitario
			,VentaDevolucionDetalle.Cantidad
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
			,VentaDevolucionDetalle.PrecioUnitario * @Iva AS Unitario
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
			AND Venta.VentaEstatusID IN (4, 5)
			-- AND VentaDevolucionDetalle.ParteID = @ParteId
			AND VentaDevolucion.EsCancelacion = 0
			AND VentaDevolucionDetalle.Estatus = 1
			AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
	) AS Devoluciones

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

	SET @Existencia = @Existencia + (@Cantidad *
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
	)

	UPDATE ParteKardex SET ExistenciaNueva = @Existencia WHERE ParteKardexID = @ParteKardexID
	
	FETCH NEXT FROM cKardex INTO @ParteKardexID, @OperacionID, @ParteID, @SucursalID, @Cantidad
END

CLOSE cKardex
DEALLOCATE cKardex