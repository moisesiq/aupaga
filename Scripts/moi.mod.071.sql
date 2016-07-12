/* Script con modificaciones a la base de datos de Theos. Archivo 071
 * Creado: 2016/07/08
 * Subido: 2016/07/12
 */

DECLARE @ScriptID INT = 71
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */



/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[PartesBusquedaEnMovimientosView] AS
	SELECT
		Parte.ParteID
		, Parte.ParteEstatusID
		,MarcaParte.MarcaParteID
		,Linea.LineaID
		,Parte.NumeroParte
		,Parte.NombreParte	
		,Linea.NombreLinea AS Linea		
		,MarcaParte.NombreMarcaParte AS Marca
		,PartePrecio.PartePrecioID
		,PartePrecio.Costo
		, PartePrecio.CostoConDescuento
		,PartePrecio.PorcentajeUtilidadUno
		,PartePrecio.PorcentajeUtilidadDos
		,PartePrecio.PorcentajeUtilidadTres
		,PartePrecio.PorcentajeUtilidadCuatro
		,PartePrecio.PorcentajeUtilidadCinco
		,PartePrecio.PrecioUno
		,PartePrecio.PrecioDos
		,PartePrecio.PrecioTres
		,PartePrecio.PrecioCuatro
		,PartePrecio.PrecioCinco
		,Parte.Etiqueta
		,Parte.SoloUnaEtiqueta
		,Parte.NumeroParte
			+ Parte.NombreParte	
			+ Linea.NombreLinea 	
			+ MarcaParte.NombreMarcaParte AS Busqueda
	FROM
		Parte
		INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
		INNER JOIN PartePrecio ON PartePrecio.ParteID = Parte.ParteID
	WHERE
		Parte.Estatus = 1
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
		-- , MAX(mi.FechaRecepcion) AS UltimaCompra
		, mov.FechaRecepcion AS UltimaCompra
		, mov.FolioFactura
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
		-- LEFT JOIN MovimientoInventarioDetalle mid ON mid.ParteID = p.ParteID AND mid.Estatus = 1
		-- LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.TipoOperacionID = 1 AND mi.Estatus = 1
		LEFT JOIN (
			SELECT
				mid.ParteID
				, ROW_NUMBER() over (partition by mid.ParteID order by mi.FechaRecepcion DESC) as Registro
				, mi.FechaRecepcion
				, mi.FolioFactura
			FROM
				MovimientoInventario mi
				LEFT JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioID = mi.MovimientoInventarioID
					AND mid.Estatus = 1
			WHERE
				mi.Estatus = 1
				AND mi.TipoOperacionID = 1
			GROUP BY
				mid.ParteID
				, mi.FechaRecepcion
				, mi.FolioFactura
		) mov ON mov.ParteID = p.ParteID and mov.Registro = 1
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
		, mov.FechaRecepcion
		, mov.FolioFactura
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

