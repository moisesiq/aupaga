ALTER PROCEDURE [dbo].[pauParteBusquedaAvanzadaEnKardex] (
	@Codigo NVARCHAR(32) = NULL
	, @Descripcion1 NVARCHAR(32) = NULL
	, @Descripcion2 NVARCHAR(32) = NULL
	, @Descripcion3 NVARCHAR(32) = NULL
	, @Descripcion4 NVARCHAR(32) = NULL
	, @Descripcion5 NVARCHAR(32) = NULL
	, @Descripcion6 NVARCHAR(32) = NULL
	, @Descripcion7 NVARCHAR(32) = NULL
	, @Descripcion8 NVARCHAR(32) = NULL
	, @Descripcion9 NVARCHAR(32) = NULL
) AS BEGIN
	SET NOCOUNT ON

	IF @Codigo IS NULL BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,MarcaParte.NombreMarcaParte 
			,Parte.LineaID	
			,Linea.NombreLinea 
			,Parte.ProveedorID
			,Parte.NumeroParte + Parte.NombreParte AS Busqueda
		FROM
			Parte			
			INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
			INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
		WHERE
			Parte.Estatus = 1			
			AND (@Descripcion1 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion9 + '%')
	END ELSE BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,MarcaParte.NombreMarcaParte 
			,Parte.LineaID	
			,Linea.NombreLinea 
			,Parte.ProveedorID
			,Parte.NumeroParte + Parte.NombreParte AS Busqueda
		FROM
			Parte
			INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
			INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
		WHERE
			Parte.Estatus = 1
			AND Parte.NumeroParte = @Codigo
	END

END

GO


/**/
ALTER PROCEDURE [dbo].[pauKardex] (
	@ParteID INT	
	, @SucursalID NVARCHAR(10)
	, @FechaInicial DATE
	, @FechaFinal DATE
) 
AS BEGIN
SET NOCOUNT ON
	
	--DECLARE @ParteID AS INT = 2301
	--DECLARE @SucursalID AS NVARCHAR(10) =  N'1,2,3'
	--DECLARE @FechaInicial AS DATE = '2013-11-17'
	--DECLARE @FechaFinal AS DATE = '2014-02-17'
			
	DECLARE @Iva AS DECIMAL(18,2) = ((SELECT CAST(Valor AS DECIMAL(18,2))FROM Configuracion WHERE Configuracion.ConfiguracionID = 1)/100)
		
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
			AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
			
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
			CAST(Venta.Fecha AS DATE) AS Fecha		
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
		FROM
			Venta
			INNER JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID
			INNER JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
			INNER JOIN Usuario ON Usuario.UsuarioID = Venta.ClienteID
			INNER JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
		WHERE
			Venta.Estatus = 1
			AND	Venta.Fecha > '2013-12-12'
			AND Venta.VentaEstatusID IN (2, 3)
			AND VentaDetalle.ParteID = @ParteId
			AND VentaDetalle.Estatus = 1 
			AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

		/* VENTA (CANCELADA) */
		UNION
		SELECT
			CAST(Venta.Fecha AS DATE) AS Fecha
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
		FROM
			Venta
			INNER JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
			INNER JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
			INNER JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
			INNER JOIN Usuario ON Usuario.UsuarioID = Venta.ClienteID
			INNER JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
		WHERE
			Venta.Estatus = 1
			AND	Venta.Fecha > '2013-12-12'
			AND Venta.VentaEstatusID IN (4, 5)
			AND VentaDevolucionDetalle.ParteID = @ParteId
			AND VentaDevolucion.EsCancelacion = 1
			AND VentaDevolucionDetalle.Estatus = 1
			AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

		/* VENTA (DEVUELTA) */
		UNION
		SELECT
			CAST(Venta.Fecha AS DATE) AS Fecha
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
		FROM
			Venta
			INNER JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
			INNER JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
			INNER JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
			INNER JOIN Usuario ON Usuario.UsuarioID = Venta.ClienteID
			INNER JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
		WHERE
			Venta.Estatus = 1
			AND	Venta.Fecha > '2013-12-12'
			AND Venta.VentaEstatusID IN (4, 5)
			AND VentaDevolucionDetalle.ParteID = @ParteId
			AND VentaDevolucion.EsCancelacion = 0
			AND VentaDevolucionDetalle.Estatus = 1
			AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
	) AS Kardex
	ORDER BY
		Kardex.Fecha ASC	
END

GO

/**/
USE [ControlRefaccionaria]
GO

/****** Object:  StoredProcedure [dbo].[pauParteBusquedaEnKardex]    Script Date: 02/18/2014 00:22:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/**/
ALTER PROCEDURE [dbo].[pauParteBusquedaEnKardex] (
	@MarcaParteID INT
	, @LineaID INT
	, @ProveedorID INT
) AS BEGIN
	SET NOCOUNT ON
	IF @MarcaParteID = 0 AND @ProveedorID = 0 
	BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,MarcaParte.NombreMarcaParte 
			,Parte.LineaID	
			,Linea.NombreLinea 
			,Parte.ProveedorID
			,Parte.NumeroParte + Parte.NombreParte AS Busqueda		
		FROM
			Parte
			INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
			INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
		WHERE
			Parte.Estatus = 1
	END 
	ELSE IF @MarcaParteID = 0 AND @ProveedorID > 0
	BEGIN 
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,MarcaParte.NombreMarcaParte 
			,Parte.LineaID	
			,Linea.NombreLinea 
			,Parte.ProveedorID
			,Parte.NumeroParte + Parte.NombreParte AS Busqueda	
		FROM
			Parte	
			INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
			INNER JOIN Linea ON Linea.LineaID = Parte.LineaID		
		WHERE
			Parte.Estatus = 1		
			AND Parte.ProveedorID = @ProveedorID			
	END
	ELSE IF @MarcaParteID > 0 AND @ProveedorID = 0
	BEGIN 
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,MarcaParte.NombreMarcaParte 
			,Parte.LineaID	
			,Linea.NombreLinea 
			,Parte.ProveedorID
			,Parte.NumeroParte + Parte.NombreParte AS Busqueda
		FROM
			Parte
			INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
			INNER JOIN Linea ON Linea.LineaID = Parte.LineaID			
		WHERE
			Parte.Estatus = 1		
			AND Parte.MarcaParteID = @MarcaParteID
			AND Parte.LineaID = @LineaID
	END
	ELSE IF @MarcaParteID > 0 AND @ProveedorID > 0
	BEGIN 
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,MarcaParte.NombreMarcaParte 
			,Parte.LineaID	
			,Linea.NombreLinea 
			,Parte.ProveedorID
			,Parte.NumeroParte + Parte.NombreParte AS Busqueda	
		FROM
			Parte			
			INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
			INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
		WHERE
			Parte.Estatus = 1		
			AND Parte.MarcaParteID = @MarcaParteID
			AND Parte.LineaID = @LineaID
			AND Parte.ProveedorID = @ProveedorID
	END
END

GO




