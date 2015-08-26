ALTER PROCEDURE [dbo].[pauKardex] (
	@ParteID INT	
	, @SucursalID NVARCHAR(10)
	, @FechaInicial DATE
	, @FechaFinal DATE
) 
AS BEGIN
SET NOCOUNT ON
	
	--DECLARE @ParteID AS INT = 2301
	--DECLARE @SucursalID AS NVARCHAR(10) =  N'1'
	--DECLARE @FechaInicial AS DATE = '2013-11-17'
	--DECLARE @FechaFinal AS DATE = '2014-02-17'
			
	DECLARE @Iva AS DECIMAL(18,2) = (((SELECT CAST(Valor AS DECIMAL(18,2))FROM Configuracion WHERE Configuracion.ConfiguracionID = 1)/100) + 1)
		
	SELECT 
		Fecha
		,Folio	
		,Tipo	
		,Operacion	
		,ClienteProveedor	
		,NombreUsuario	
		,Origen	
		,Destino	
		,CAST(Unitario AS DECIMAL(18,2)) AS Unitario
		,Cantidad	
		,ExistenciaNueva
	FROM (				
		/* ENTRADA COMPRA */
		SELECT 
			MovimientoInventario.FechaRegistro AS Fecha
			,CAST(MovimientoInventario.FolioFactura AS VARCHAR) AS Folio
			,'E' AS Tipo
			,'ENTRADA COMPRA' AS Operacion
			,Proveedor.NombreProveedor AS ClienteProveedor
			,Usuario.NombreUsuario
			,CAST(MovimientoInventario.ProveedorID AS VARCHAR) AS Origen
			,Sucursal.NombreSucursal AS Destino
			,PartePrecio.Costo * @Iva AS Unitario
			,MovimientoInventarioDetalle.Cantidad
			,0.0 AS ExistenciaNueva
			,1 AS Orden
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
			AND MovimientoInventarioDetalle.ParteID = @ParteId			
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
		FROM 
			MovimientoInventario 
			INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
			INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
			INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
		WHERE 
			MovimientoInventario.TipoOperacionID = 2
			AND MovimientoInventario.Estatus = 1
			AND MovimientoInventarioDetalle.ParteID = @ParteId			
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
		FROM 
			MovimientoInventario 
			INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
			INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
			INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
		WHERE 
			MovimientoInventario.TipoOperacionID = 3
			AND MovimientoInventario.Estatus = 1
			AND MovimientoInventarioDetalle.ParteID = @ParteId
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
			AND MovimientoInventarioDetalle.ParteID = @ParteId
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
			AND MovimientoInventarioDetalle.ParteID = @ParteId		
			AND MovimientoInventario.SucursalOrigenID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
			
		/* TRASPASO ENTRADA */
		UNION
		SELECT 
			MovimientoInventario.FechaRegistro AS Fecha
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
			AND MovimientoInventarioDetalle.ParteID = @ParteId		
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
		FROM
			Venta
			LEFT JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID
			LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
			LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
			LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
		WHERE
			Venta.Estatus = 1
			AND	Venta.Fecha > '2013-12-12'
			AND Venta.VentaEstatusID IN (2, 3)
			AND VentaDetalle.ParteID = @ParteId
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
			FROM
				Venta
				LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
				LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
				LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
				LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
				LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
			WHERE
				Venta.Estatus = 1
				AND	Venta.Fecha > '2013-12-12'
				AND Venta.VentaEstatusID IN (4, 5)
				AND VentaDevolucionDetalle.ParteID = @ParteId
				AND VentaDevolucion.EsCancelacion = 1
				AND VentaDevolucionDetalle.Estatus = 1
				AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		
			UNION			
			SELECT
				Venta.Fecha AS Fecha
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
			FROM
				Venta
				LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
				LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
				LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
				LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
				LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
			WHERE
				Venta.Estatus = 1
				AND	Venta.Fecha > '2013-12-12'
				AND Venta.VentaEstatusID IN (4, 5)
				AND VentaDevolucionDetalle.ParteID = @ParteId
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
			FROM
				Venta
				LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
				LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
				LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
				LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
				LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
			WHERE
				Venta.Estatus = 1
				AND	Venta.Fecha > '2013-12-12'
				AND Venta.VentaEstatusID IN (4, 5)
				AND VentaDevolucionDetalle.ParteID = @ParteId
				AND VentaDevolucion.EsCancelacion = 0
				AND VentaDevolucionDetalle.Estatus = 1
				AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
			
			UNION
			
			SELECT
				Venta.Fecha AS Fecha
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
			FROM
				Venta
				LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
				LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
				LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
				LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
				LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
			WHERE
				Venta.Estatus = 1
				AND	Venta.Fecha > '2013-12-12'
				AND Venta.VentaEstatusID IN (4, 5)
				AND VentaDevolucionDetalle.ParteID = @ParteId
				AND VentaDevolucion.EsCancelacion = 0
				AND VentaDevolucionDetalle.Estatus = 1
				AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		) AS Devoluciones
	) AS Kardex
	ORDER BY
		Kardex.Fecha ASC			
		,Kardex.Orden
END
GO


