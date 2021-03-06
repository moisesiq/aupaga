/*
*/
ALTER TABLE [dbo].[ParteExistencia]
    ADD [ExistenciaInicial] DECIMAL (18, 2) NULL;    
GO

UPDATE ParteExistencia SET ExistenciaInicial = 0
GO

EXECUTE sp_refreshview N'dbo.PartesVentasView';
GO

EXECUTE sp_refreshview N'dbo.PartesEquivalentesView';
GO

EXECUTE sp_refreshview N'dbo.ExistenciasView';
GO

EXECUTE sp_refreshview N'dbo.MxMnView';
GO

EXECUTE sp_refreshview N'dbo.PedidosSugeridosView';
GO

/**/
CREATE PROCEDURE [dbo].[pauParteBusquedaAvanzadaEnKardex] (
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
			,Parte.LineaID	
			,Parte.ProveedorID
		FROM
			Parte			
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
			,Parte.LineaID	
			,Parte.ProveedorID
		FROM
			Parte
		WHERE
			Parte.Estatus = 1
			AND Parte.NumeroParte = @Codigo
	END

END
GO

/**/
CREATE PROCEDURE [dbo].[pauParteBusquedaEnKardex] (
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
			,Parte.LineaID	
			,Parte.ProveedorID		
		FROM
			Parte
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
			,Parte.LineaID	
			,Parte.ProveedorID		
		FROM
			Parte			
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
			,Parte.LineaID	
			,Parte.ProveedorID		
		FROM
			Parte			
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
			,Parte.LineaID	
			,Parte.ProveedorID		
		FROM
			Parte			
		WHERE
			Parte.Estatus = 1		
			AND Parte.MarcaParteID = @MarcaParteID
			AND Parte.LineaID = @LineaID
			AND Parte.ProveedorID = @ProveedorID
	END
END
GO

/**/
CREATE PROCEDURE [dbo].[pauKardex] (
	@ParteID INT	
	, @SucursalID NVARCHAR
	, @FechaInicial DATE
	, @FechaFinal DATE
) 
AS BEGIN
SET NOCOUNT ON
	
	/* ENTRADA COMPRA */
	SELECT 
		CONVERT(VARCHAR, MovimientoInventario.FechaRegistro, 103) AS Fecha
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
		AND MovimientoInventario.FechaRegistro BETWEEN @FechaInicial AND @FechaFinal
		AND MovimientoInventario.SucursalDestinoID IN (REPLACE(@SucursalID, '''', ''))

	/* ENTRADA INVENTARIO */
	UNION
	SELECT 
		CONVERT(VARCHAR, MovimientoInventario.FechaRegistro, 103) AS Fecha
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
		AND MovimientoInventario.FechaRegistro BETWEEN @FechaInicial AND @FechaFinal
		AND MovimientoInventario.SucursalDestinoID IN (REPLACE(@SucursalID, '''', ''))
		
	/* SALIDA INVENTARIO */
	UNION
	SELECT 
		CONVERT(VARCHAR, MovimientoInventario.FechaRegistro, 103) AS Fecha
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
		,'S' AS Tipo
		,'SALIDA INVENTARIO' AS Operacion
		,'-----------------' AS ClienteProveedor
		,Usuario.NombreUsuario
		,'-----' AS Origen
		,Sucursal.NombreSucursal AS Destino
		,PartePrecio.Costo AS Unitario
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
		AND MovimientoInventario.FechaRegistro BETWEEN @FechaInicial AND @FechaFinal
		AND MovimientoInventario.SucursalDestinoID IN (REPLACE(@SucursalID, '''', ''))

	/* DEVOLUCION A PROVEEDOR */
	UNION
	SELECT 
		CONVERT(VARCHAR, MovimientoInventario.FechaRegistro, 103) AS Fecha
		,CAST(MovimientoInventario.FolioFactura AS VARCHAR) AS Folio
		,'S' AS Tipo
		,'DEVOLUCION A PROVEEDOR' AS Operacion
		,Proveedor.NombreProveedor AS ClienteProveedor
		,Usuario.NombreUsuario
		,Sucursal.NombreSucursal AS Origen
		,CAST(MovimientoInventario.ProveedorID AS VARCHAR) AS Destino
		,PartePrecio.Costo AS Unitario
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
		AND MovimientoInventario.FechaRegistro BETWEEN @FechaInicial AND @FechaFinal
		AND MovimientoInventario.SucursalDestinoID IN (REPLACE(@SucursalID, '''', ''))

	/* TRASPASO SALIDA */
	UNION
	SELECT 
		CONVERT(VARCHAR, MovimientoInventario.FechaRegistro, 103) AS Fecha
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio 
		,'S' AS Tipo
		,'TRASPASO SALIDA' AS Operacion
		,'-----------------' AS ClienteProveedor
		,Usuario.NombreUsuario
		,O.NombreSucursal AS Origen
		,D.NombreSucursal AS Destino
		,PartePrecio.Costo AS Unitario
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
		AND MovimientoInventario.MovimientoInventarioID = @ParteId
		AND MovimientoInventario.FechaRegistro BETWEEN @FechaInicial AND @FechaFinal
		AND MovimientoInventario.SucursalDestinoID IN (REPLACE(@SucursalID, '''', ''))
		
	/* TRASPASO SALIDA */
	UNION
	SELECT 
		CONVERT(VARCHAR, MovimientoInventario.FechaRegistro, 103) AS Fecha
		,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio 
		,'E' AS Tipo
		,'TRASPASO ENTRADA' AS Operacion
		,'-----------------' AS ClienteProveedor
		,Usuario.NombreUsuario
		,O.NombreSucursal AS Origen
		,D.NombreSucursal AS Destino
		,PartePrecio.Costo AS Unitario
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
		AND MovimientoInventario.MovimientoInventarioID = @ParteId
		AND MovimientoInventario.FechaRecepcion IS NOT NULL
		AND MovimientoInventario.UsuarioRecibioTraspasoID IS NOT NULL
		AND MovimientoInventario.FechaRegistro BETWEEN @FechaInicial AND @FechaFinal
		AND MovimientoInventario.SucursalDestinoID IN (REPLACE(@SucursalID, '''', ''))

	/* VENTA (PAGADA Y COBRADA) */
	UNION
	SELECT
		CONVERT(VARCHAR, Venta.Fecha, 103) AS Fecha
		,Venta.Folio 
		,'S' AS Tipo
		,'VENTAS' AS Operacion
		,Cliente.Nombre AS ClienteProveedor
		,Usuario.NombreUsuario
		,CAST(Venta.ClienteID AS VARCHAR) AS Origen
		,Sucursal.NombreSucursal AS Destino
		,VentaDetalle.PrecioUnitario AS Unitario
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
		AND	Venta.Fecha BETWEEN @FechaInicial AND @FechaFinal
		AND Venta.VentaEstatusID IN (2, 3)
		AND VentaDetalle.ParteID = @ParteId
		AND VentaDetalle.Estatus = 1 
		AND Venta.SucursalID IN (REPLACE(@SucursalID, '''', ''))

	/* VENTA (CANCELADA) */
	UNION
	SELECT
		CONVERT(VARCHAR, Venta.Fecha, 103) AS Fecha
		,Venta.Folio
		,'E' AS Tipo
		,'VENTA CANCELADA' AS Operacion
		,Cliente.Nombre AS ClienteProveedor
		,Usuario.NombreUsuario
		,CAST(Venta.ClienteID AS VARCHAR) AS Origen
		,Sucursal.NombreSucursal AS Destino
		,VentaDevolucionDetalle.PrecioUnitario AS Unitario
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
		AND	Venta.Fecha BETWEEN @FechaInicial AND @FechaFinal
		AND Venta.VentaEstatusID IN (4, 5)
		AND VentaDevolucionDetalle.ParteID = @ParteId
		AND VentaDevolucion.EsCancelacion = 1
		AND VentaDevolucionDetalle.Estatus = 1
		AND Venta.SucursalID IN (REPLACE(@SucursalID, '''', ''))

	/* VENTA (DEVUELTA) */
	UNION
	SELECT
		CONVERT(VARCHAR, Venta.Fecha, 103) AS Fecha
		,Venta.Folio
		,'E' AS Tipo
		,'VENTA DEVUELTA' AS Operacion
		,Cliente.Nombre AS ClienteProveedor
		,Usuario.NombreUsuario
		,CAST(Venta.ClienteID AS VARCHAR) AS Origen
		,Sucursal.NombreSucursal AS Destino
		,VentaDevolucionDetalle.PrecioUnitario AS Unitario
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
		AND	Venta.Fecha BETWEEN @FechaInicial AND @FechaFinal
		AND Venta.VentaEstatusID IN (4, 5)
		AND VentaDevolucionDetalle.ParteID = @ParteId
		AND VentaDevolucion.EsCancelacion = 0
		AND VentaDevolucionDetalle.Estatus = 1
		AND Venta.SucursalID IN (REPLACE(@SucursalID, '''', ''))

END
GO

/**/
ALTER VIEW [dbo].[PartesClasificacionAbcView]
AS
SELECT TOP 1000000000000
	Parte.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,ISNULL(CAST(Ventas.Cantidad AS DECIMAL(9,2)), CAST(0.0 AS DECIMAL(9,2))) AS Cantidad
	,Parte.CriterioABC AS VigenteAbc
	,c.Clasificacion AS NuevaAbc
FROM
	Parte
	LEFT JOIN (
		SELECT 	
			VentaDetalle.ParteID
			,SUM(VentaDetalle.Cantidad) AS Cantidad					
		FROM
			Venta
			INNER JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID
		WHERE
			CONVERT(DATE,Venta.Fecha) BETWEEN CONVERT(DATE,DATEADD(YEAR,-1,GETDATE())) AND CONVERT(DATE,GETDATE())
			AND Venta.Estatus = 1
			AND Venta.VentaEstatusID IN (3)				
		GROUP BY
			VentaDetalle.ParteID) AS Ventas ON Ventas.ParteID = Parte.ParteID	
	LEFT JOIN CriterioABC c ON ISNULL(Ventas.Cantidad, 0) BETWEEN c.RangoInicial AND c.RangoFinal
WHERE
	Parte.Estatus = 1
ORDER BY
	Parte.ParteID
GO