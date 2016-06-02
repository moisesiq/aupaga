/* Script con modificaciones a la base de datos de Theos. Archivo 067
 * Creado: 2016/05/07
 * Subido: 2016/05/26
 */

DECLARE @ScriptID INT = 67
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE ParteMaxMin ADD FechaIman DATETIME NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[PartesExistenciasView] AS
	SELECT
		ISNULL(ROW_NUMBER() OVER (ORDER BY p.ParteID, pe.SucursalID), 0) AS Registro
		, pe.ParteExistenciaID
		, p.ParteID
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
		, MAX(vd.FechaRegistro) AS UltimaVenta
		, MAX(mi.FechaRecepcion) AS UltimaCompra
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
		LEFT JOIN MovimientoInventarioDetalle mid ON mid.ParteID = p.ParteID AND mid.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.TipoOperacionID = 1 AND mi.Estatus = 1
	WHERE
		p.Estatus = 1
	GROUP BY
		pe.ParteExistenciaID
		, p.ParteID
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
		, pe.SucursalID
		, pe.Existencia
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauPartesBusqueda] (
	@SucursalID INT
	, @Proveedores tpuTablaEnteros READONLY
	, @Marcas tpuTablaEnteros READONLY
	, @Lineas tpuTablaEnteros READONLY
	, @NumeroDeParte NVARCHAR(64) = NULL
	, @Descripcion NVARCHAR(512) = NULL
) AS BEGIN
	SET NOCOUNT ON

	/*
	
	*/

	-- Definición de variables tipo constante
	

	-- Variables calculadas para el proceso
	

	-- Procedimiento
	
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.CodigoBarra AS CodigoDeBarras
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, m.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, pmm.Maximo
		, pmm.Minimo
		, pmm.FechaCalculo AS FechaMaxMin
		, pmm.FechaIman
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte m ON m.MarcaParteID = p.MarcaParteID AND m.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		-- LEFT JOIN Medida md ON md.MedidaID = p.MedidaID AND md.Estatus = 1
		-- LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
		-- LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID AND pmm.SucursalID = @SucursalID
	WHERE
		p.Estatus = 1
		AND (@NumeroDeParte IS NULL OR p.NumeroParte = @NumeroDeParte)
		AND (@Descripcion IS NULL OR p.NombreParte LIKE ('%' + @Descripcion + '%'))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Proveedores) OR p.ProveedorID IN (SELECT Entero FROM @Proveedores))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Marcas) OR p.MarcaParteID IN (SELECT Entero FROM @Marcas))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Lineas) OR p.LineaID IN (SELECT Entero FROM @Lineas))

END
GO