/* Script con modificaciones a la base de datos de Theos. Archivo 065
 * Creado: 2016/03/29
 * Subido: 2016/04/11
 */

DECLARE @ScriptID INT = 65
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE CajaFacturaGlobal ADD PreFacturar DECIMAL(12, 2) NULL
GO
UPDATE CajaFacturaGlobal SET PreFacturar = Facturado
ALTER TABLE CajaFacturaGlobal ALTER COLUMN PreFacturar DECIMAL(12, 2) NOT NULL

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Reportes.Partes.Imanes.Salida', 'D', 'P', 'Salida donde se muestra el reporte de imánes de MaxMin. (D - Diseño, P - Pantalla, I - Impresora, N - Nada).')

-- DROP TABLE FacturaGlobalPendientePorDescontar
CREATE TABLE FacturaGlobalPendientePorDescontar (
	FacturaGlobalPendientePorDescontarID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
	, Fecha DATETIME NOT NULL
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, Importe DECIMAL(12, 2) NOT NULL
)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[VentasFacturasView] AS
	SELECT
		vf.VentaFacturaID
		, vf.Fecha
		, vf.Serie + vf.Folio AS SerieFolio
		, vf.ClienteID
		, COUNT(vfd.VentaID) AS Ventas
	FROM
		VentaFactura vf
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaFacturaID = vf.VentaFacturaID AND vfd.Estatus = 1
	WHERE vf.Estatus = 1
	GROUP BY
		vf.VentaFacturaID
		, vf.Fecha
		, vf.Serie
		, vf.Folio
		, vf.ClienteID
GO

ALTER VIEW [dbo].[CajaFacturasGlobalesView] AS
	SELECT
		fg.CajaFacturaGlobalID
		, fg.Dia
		, fg.SucursalID
		, s.NombreSucursal AS Sucursal
		, fg.Tickets
		, fg.FacturadoDeDiasAnt
		, fg.Negativos
		-- , fg.Devoluciones
		-- , fg.Cancelaciones
		, fg.DevolucionesDia
		, fg.DevolucionesDiasAnt
		, fg.GarantiasDia
		, fg.GarantiasDiasAnt
		, fg.Cobranza
		, (fg.Tickets - fg.FacturadoDeDiasAnt - fg.Negativos
			- fg.DevolucionesDia - fg.DevolucionesDiasAnt - fg.GarantiasDia - fg.GarantiasDiasAnt) AS Oficial
		, fg.Restar
		, (fg.Tickets - fg.FacturadoDeDiasAnt - fg.Negativos
			- fg.DevolucionesDia - fg.DevolucionesDiasAnt - fg.GarantiasDia - fg.GarantiasDiasAnt
			- fg.Restar) AS Supuesto
		, fg.CostoMinimo
		, fg.Restante
		, fg.SaldoRestante
		, fg.PreFacturar
		, fg.Facturado
	FROM
		CajaFacturaGlobal fg
		LEFT JOIN Sucursal s ON s.SucursalID = fg.SucursalID AND s.Estatus = 1
GO

ALTER VIEW [dbo].[PartesMaxMinView] AS
	SELECT
		p.ParteID
		, p.ParteEstatusID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pmm.SucursalID
		, pmm.Calcular
		, pmm.VentasGlobales
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
	FROM
		Parte p
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	WHERE
		p.Estatus = 1
	/* ORDER BY
		pv.NombreProveedor
		, mp.NombreMarcaParte
		, l.NombreLinea
		, p.NombreParte
		
	*/
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

-- DROP PROCEDURE pauPartesBusqueda
CREATE PROCEDURE pauPartesBusqueda (
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