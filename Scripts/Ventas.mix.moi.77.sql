/* Script con modificaciones para el módulo de ventas. Archivo 77
 * Creado: 2014/12/26
 * Subido: 2015/01/03
 */


----------------------------------------------------------------------------------- Código de André

----------------------------------------------------------------------------------- Código de André


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Garantías
	CREATE TABLE VentaGarantiaRespuesta (
		VentaGarantiaRespuestaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Respuesta NVARCHAR(32)
	)
	INSERT INTO VentaGarantiaRespuesta (Respuesta) VALUES
		('ARTÍCULO NUEVO')
		, ('NOTA DE CRÉDITO')
		, ('NO PROCEDIÓ')
	
	DROP TABLE VentaGarantiaDetalle
	UPDATE VentaFacturaDevolucionDetalle SET VentaGarantiaID = NULL
	DELETE FROM VentaGarantia
	DBCC CHECKIDENT (VentaGarantia, RESEED, 0)
	
	ALTER TABLE VentaGarantia DROP COLUMN AccionPosterior, AccionFechaCompletado, AccionObservacion
	ALTER TABLE VentaGarantia ADD
		RespuestaID INT NULL FOREIGN KEY REFERENCES VentaGarantiaRespuesta(VentaGarantiaRespuestaID)
		, FechaCompletado DATETIME NULL
		, ObservacionCompletado NVARCHAR(512) NULL
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, Costo DECIMAL(12, 2) NOT NULL
		, CostoConDescuento DECIMAL(12, 2) NOT NULL
		, PrecioUnitario DECIMAL(12, 2) NOT NULL
		, Iva DECIMAL(12, 2) NOT NULL

	ALTER TABLE MovimientoInventarioDetalle ADD CantidadDevuelta DECIMAL(12, 2) NOT NULL DEFAULT 0

	INSERT INTO EstatusGenerico (Descripcion) VALUES
		('EN REVISIÓN')
		, ('RECIBIDO')
		, ('RESUELTO')

	INSERT INTO TipoConceptoOperacion (TipoOperacionID, NombreConceptoOperacion, Abreviacion, UsuarioID, FechaRegistro) VALUES
		(4, 'GARANTÍA', 'GAR', 1, GETDATE())
	UPDATE TipoConceptoOperacion SET NombreConceptoOperacion = 'GARANTÍA', Abreviacion = 'GAR'
		WHERE TipoConceptoOperacionID = 10

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

DROP VIEW VentasGarantiasDetalleView
GO

ALTER VIEW [dbo].[VentasGarantiasView] AS
	SELECT
		vg.VentaGarantiaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, vg.VentaID
		, v.Folio AS FolioDeVenta
		, v.Fecha AS FechaDeVenta
		, vg.SucursalID
		, s.NombreSucursal AS Sucursal
		, ISNULL(CONVERT(BIT, v.ACredito), 0) AS VentaACredito
		, vg.Fecha
		, vg.MotivoID
		, vgm.Motivo
		, vg.MotivoObservacion
		, vg.AccionID
		, vga.Accion
		, vg.EstatusGenericoID
		, eg.Descripcion AS Estatus
		, vg.RealizoUsuarioID
		, u.NombrePersona AS Realizo
		, ISNULL(CONVERT(BIT, CASE WHEN ac.AutorizoUsuarioID IS NULL THEN 0 ELSE 1 END), 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'PENDIENTE') AS AutorizoUsuario
		-- , CONVERT(DECIMAL(12, 2), vgd.Subtotal) AS Subtotal
		-- , CONVERT(DECIMAL(12, 2), vgd.Iva) AS Iva
		-- , CONVERT(DECIMAL(12, 2), vgd.Total) AS Total
		, (vg.PrecioUnitario + vg.Iva) AS Total
		, vg.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS NombreDeParte
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, vg.Costo
		, vg.PrecioUnitario
		, vg.Iva
		, m.NombreMedida AS Medida
		, p.ProveedorID
		, vg.RespuestaID
		, vg.FechaCompletado
		, vg.ObservacionCompletado
		, vg.NotaDeCreditoID
	FROM
		VentaGarantia vg
		LEFT JOIN VentaGarantiaMotivo vgm ON vgm.VentaGarantiaMotivoID = vg.MotivoID
		LEFT JOIN VentaGarantiaAccion vga ON vga.VentaGarantiaAccionID = vg.AccionID
		LEFT JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = vg.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = vg.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN (
			SELECT
				ROW_NUMBER() OVER (PARTITION BY TablaRegistroID ORDER BY FechaAutorizo) AS Registro
				, TablaRegistroID
				, AutorizoUsuarioID
			FROM Autorizacion
			WHERE Tabla = 'VentaGarantia' AND Estatus = 1
		) ac ON ac.TablaRegistroID = vg.VentaGarantiaID AND ac.Registro = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = ac.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
		LEFT JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
		LEFT JOIN Medida m ON m.MedidaID = p.MedidaID AND m.Estatus = 1
		LEFT JOIN EstatusGenerico eg ON eg.EstatusGenericoID = vg.EstatusGenericoID
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	WHERE vg.Estatus = 1
GO

-- DROP VIEW MovimientosInventarioDetalleAvanzadoView
CREATE VIEW MovimientosInventarioDetalleAvanzadoView AS
	SELECT
		mid.MovimientoInventarioDetalleID
		, mid.MovimientoInventarioID
		, mi.TipoOperacionID
		, mi.FolioFactura
		, mi.FechaFactura
		, mi.FechaRecepcion
		, mi.ImporteTotal
		, u.NombreUsuario AS Usuario
		, mid.ParteID
		, mid.Cantidad
		, mid.CantidadDevuelta
	FROM
		MovimientoInventarioDetalle mid
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
	WHERE mid.Estatus = 1
GO

ALTER VIEW [dbo].[MovimientosInventarioDevolucionesView] AS
	SELECT
		MovimientoInventario.MovimientoInventarioID AS DevolucionID
		,MovimientoInventario.ProveedorID
		,MovimientoInventario.FolioFactura AS Factura
		,MovimientoInventario.FechaRegistro AS Fecha
		,MovimientoInventario.ImporteTotal AS Importe
		, MovimientoInventario.Observacion
	FROM 
		MovimientoInventario
	WHERE
		MovimientoInventario.Estatus = 1
		AND MovimientoInventario.TipoOperacionID = 4	
		AND MovimientoInventario.AplicaEnMovimientoInventarioID IS NULL
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

DROP PROCEDURE pauParteBusquedaAvanzadaDeMovimientos
GO

-- DROP PROCEDURE pauPartesMovimientosDevoluciones
CREATE PROCEDURE pauPartesMovimientosDevoluciones (
	@ProveedorID INT
	, @Desde DATE
	, @Hasta DATE
	, @Garantias BIT = 0
	, @Codigo NVARCHAR(32) = NULL
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

	DECLARE @OpCompra INT = 1
	DECLARE @EstGenPendiente INT = 2

	SET @Hasta = DATEADD(DAY, 1, @Hasta)

	IF @Garantias = 1 BEGIN
	
		-- Búsqueda por descripción
		IF @Codigo IS NULL BEGIN
			SELECT
				vg.VentaGarantiaID AS Id
				, v.Folio
				, vg.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				VentaGarantia vg
				INNER JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = vg.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
			WHERE
				vg.Estatus = 1
				AND vg.EstatusGenericoID = @EstGenPendiente
				AND p.ProveedorID = @ProveedorID
				AND (			
					(@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
					AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
					AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
					AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
					AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
				)
		
		-- Búsqueda por código			
		END ELSE BEGIN
			SELECT DISTINCT
				vg.VentaGarantiaID AS Id
				, v.Folio
				, vg.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				VentaGarantia vg
				INNER JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = vg.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
			WHERE
				vg.Estatus = 1
				AND vg.EstatusGenericoID = @EstGenPendiente
				AND p.ProveedorID = @ProveedorID
				AND p.NumeroParte = @Codigo
		END
	
	END ELSE BEGIN

		-- Búsqueda por descripción
		IF @Codigo IS NULL BEGIN
			SELECT DISTINCT
				0 AS Id
				, '' AS Folio
				, mid.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				MovimientoInventarioDetalle mid
				INNER JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
				INNER JOIN Parte p ON p.ParteID = mid.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = mid.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			WHERE
				mid.Estatus = 1
				AND mi.TipoOperacionID = @OpCompra
				AND mi.ProveedorID = @ProveedorID
				AND (mi.FechaFactura >= @Desde AND mi.FechaFactura < @Hasta)
				AND (mid.Cantidad - mid.CantidadDevuelta) > 0
				AND (			
					(@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
					AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
					AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
					AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
					AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
				)
		
		-- Búsqueda por código			
		END ELSE BEGIN
			SELECT DISTINCT
				0 AS Id
				, '' AS Folio
				, mid.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				MovimientoInventarioDetalle mid
				INNER JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
				INNER JOIN Parte p ON p.ParteID = mid.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = mid.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			WHERE
				mid.Estatus = 1
				AND mi.TipoOperacionID = @OpCompra
				AND mi.ProveedorID = @ProveedorID
				AND (mi.FechaFactura >= @Desde AND mi.FechaFactura < @Hasta)
				AND (mid.Cantidad - mid.CantidadDevuelta) > 0
				AND p.NumeroParte = @Codigo
		END
	END
	
END
GO