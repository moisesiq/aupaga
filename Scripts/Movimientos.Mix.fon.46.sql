CREATE VIEW ClienteKardexDetalleView
AS
SELECT
	Venta.VentaID
	,Venta.Fecha
	,Venta.Folio
	,Venta.ClienteID		
	,Sucursal.NombreSucursal AS Sucursal
	,VentaDetalle.ParteID	
	,VentaDetalle.PrecioUnitario
	,VentaDetalle.Iva
	,VentaDetalle.PrecioUnitario + VentaDetalle.Iva AS Importe
	,VentaEstatus.Descripcion AS EstatusActual
FROM
	Venta
	INNER JOIN VentaDetalle ON Venta.VentaID = VentaDetalle.VentaID
	INNER JOIN VentaEstatus ON VentaEstatus.VentaEstatusID = Venta.VentaEstatusID
	INNER JOIN Parte ON Parte.ParteID = VentaDetalle.ParteID
	INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
	INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
	INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID
	INNER JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
WHERE
	Venta.Estatus = 1
GO

/**/
ALTER PROCEDURE [dbo].[pauClienteKardex] (
	@ClienteID AS INT
	, @SucursalID AS INT
	, @FechaInicial AS DATE
	, @FechaFinal AS DATE
) 
AS BEGIN
SET NOCOUNT ON

--DECLARE @ClienteID AS INT = 3
--DECLARE @SucursalID AS INT = 1
--DECLARE @FechaInicial AS DATE = '2012-12-12'
--DECLARE @FechaFinal AS DATE = GETDATE()

	SELECT
		VentaDetalle.ParteID	
		,Parte.NumeroParte
		,Parte.NombreParte AS Descripcion
		,MarcaParte.NombreMarcaParte AS Marca
		,Linea.NombreLinea AS Linea
		,Proveedor.NombreProveedor AS Proveedor
		,SUM(VentaDetalle.Cantidad) AS Cantidad
		,Parte.NumeroParte + ' ' + Parte.NombreParte AS Busqueda
	FROM
		Venta
		INNER JOIN VentaDetalle ON Venta.VentaID = VentaDetalle.VentaID
		INNER JOIN VentaEstatus ON VentaEstatus.VentaEstatusID = Venta.VentaEstatusID
		INNER JOIN Parte ON Parte.ParteID = VentaDetalle.ParteID
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
		INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
		INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID
		INNER JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
	WHERE
		Venta.Estatus = 1
		AND Venta.ClienteID = @ClienteID		
		AND Venta.Fecha BETWEEN @FechaInicial AND @FechaFinal
		AND (@SucursalID <= 0 OR Venta.SucursalID = @SucursalID)
	GROUP BY
		VentaDetalle.ParteID	
		,Parte.NumeroParte
		,Parte.NombreParte
		,MarcaParte.NombreMarcaParte
		,Linea.NombreLinea
		,Proveedor.NombreProveedor
	ORDER BY
		Linea.NombreLinea

END
GO