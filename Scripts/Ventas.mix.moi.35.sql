/* Script con modificaciones para el módulo de ventas. Archivo 35
 * Creado: 2014/04/01
 * Subido: 2014/04/24
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- MaxMin
	ALTER TABLE ParteMaxMinRegla ALTER COLUMN Regla NVARCHAR(32) NOT NULL
	ALTER TABLE ParteMaxMinRegla ALTER COLUMN Condicion NVARCHAR(1024) NOT NULL
	ALTER TABLE ParteMaxMinRegla ALTER COLUMN Maximo NVARCHAR(1024) NOT NULL
	ALTER TABLE ParteMaxMinRegla ALTER COLUMN Minimo NVARCHAR(1024) NOT NULL
	
	-- Contabilidad
	/*
	ALTER TABLE CajaEgreso DROP CONSTRAINT FK__CajaEgres__Conta__7BDB408F
	ALTER TABLE CajaEgreso DROP COLUMN ContaEgresoID
	DROP TABLE ContaEgresoDetalleDevengado
	DROP TABLE ContaEgresoDetalle
	DROP TABLE ContaConsumible
	DROP TABLE ContaEgresoDevengado
	DROP TABLE ContaEgreso
	DROP TABLE TipoDocumento
	DROP TABLE ContaCuentaAuxiliar
	DROP TABLE ContaCuentaDeMayor
	DROP TABLE ContaSubcuenta
	DROP TABLE ContaCuenta
	*/

	CREATE TABLE ContaCuenta (
		ContaCuentaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Cuenta NVARCHAR(32) NOT NULL
		, CuentaContpaq NVARCHAR(16) NOT NULL
	)
	
	CREATE TABLE ContaSubcuenta (
		ContaSubcuentaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ContaCuentaID INT NOT NULL FOREIGN KEY REFERENCES ContaCuenta(ContaCuentaID)
		, Subcuenta NVARCHAR(32) NOT NULL
		, CuentaContpaq NVARCHAR(16) NOT NULL
	)
	
	CREATE TABLE ContaCuentaDeMayor (
		ContaCuentaDeMayorID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ContaSubcuentaID INT NOT NULL FOREIGN KEY REFERENCES ContaSubcuenta(ContaSubcuentaID)
		, CuentaDeMayor NVARCHAR(32) NOT NULL
		, CuentaContpaq NVARCHAR(16) NOT NULL
	)
	
	CREATE TABLE ContaCuentaAuxiliar (
		ContaCuentaAuxiliarID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ContaCuentaDeMayorID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaDeMayor(ContaCuentaDeMayorID)
		, CuentaAuxiliar NVARCHAR(32) NOT NULL
		, CuentaContpaq NVARCHAR(16) NOT NULL
		, Devengable BIT NOT NULL
		, Detallable BIT NOT NULL
		, VisibleEnCaja BIT NOT NULL
	)
	
	CREATE TABLE TipoDocumento (
		TipoDocumentoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Documento NVARCHAR(32) NOT NULL
	)
	
	CREATE TABLE ContaEgreso (
		ContaEgresoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ContaCuentaAuxiliarID INT NOT NULL FOREIGN KEY REFERENCES ContacuentaAuxiliar(ContaCuentaAuxiliarID)
		, Fecha DATETIME NOT NULL
		, Importe DECIMAL(12, 2) NOT NULL
		, TipoFormaPagoID INT NOT NULL FOREIGN KEY REFERENCES TipoFormaPago(TipoFormaPagoID)
		, FolioDePago NVARCHAR(32) NULL
		, TipoDocumentoID INT NULL FOREIGN KEY REFERENCES TipoDocumento(TipoDocumentoID)
		, EsFiscal BIT NOT NULL
		, Observaciones NVARCHAR(512) NULL
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
		, Estatus BIT NOT NULL DEFAULT 1
	)
	CREATE INDEX Ix_ContaEgreso_Estatus ON ContaEgreso(Estatus)

	CREATE TABLE ContaEgresoDevengado (
		ContaEgresoDevengadoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ContaEgresoID INT NOT NULL FOREIGN KEY REFERENCES ContaEgreso(ContaEgresoID)
		, Fecha DATETIME NOT NULL
		, Importe DECIMAL(12, 2) NOT NULL
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	)

	CREATE TABLE ContaConsumible (
		ContaConsumibleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Consumible NVARCHAR(64) NOT NULL
	)

	CREATE TABLE ContaEgresoDetalle (
		ContaEgresoDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ContaEgresoID INT NOT NULL FOREIGN KEY REFERENCES ContaEgreso(ContaEgresoID)
		, ContaConsumibleID INT NOT NULL FOREIGN KEY REFERENCES ContaConsumible(ContaConsumibleID)
		, Cantidad DECIMAL(7, 2) NOT NULL
		, Importe DECIMAL(12, 2) NOT NULL

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
		, Estatus BIT NOT NULL DEFAULT 1
	)
	CREATE INDEX Ix_ContaEgresoDetalle_Estatus ON ContaEgresoDetalle(Estatus)
	
	CREATE TABLE ContaEgresoDetalleDevengado (
		ContaEgresoDetalleDevengadoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ContaEgresoDevengadoID INT NOT NULL FOREIGN KEY REFERENCES ContaEgresoDevengado(ContaEgresoDevengadoID)
		, ContaEgresoDetalleID INT NULL FOREIGN KEY REFERENCES ContaEgresoDetalle(ContaEgresoDetalleID)
		, Cantidad DECIMAL(7, 2) NOT NULL
	)

	INSERT INTO ContaCuenta (Cuenta, CuentaContpaq) VALUES
		('ACTIVO', '000-0100')
		, ('PASIVO', '000-0200')
		, ('CAPITAL CONTABLE', '000-0300')
		, ('INGRESOS', '000-0400')
		, ('EGRESOS', '000-0500')
	INSERT INTO ContaSubcuenta (ContaCuentaID, Subcuenta, CuentaContpaq) VALUES
		(1, 'CIRCULANTE', '000-0110')
		, (1, 'FIJO', '000-0120')
		, (1, 'DIFERIDO', '000-0140')
		, (2, 'CIRCULANTE', '000-0210')
		, (2, 'FIJO', '000-0220')
		, (2, 'DIFERIDO', '000-0230')
		, (3, 'PATRIMONIO', '000-0310')
		, (4, 'VENTAS', '400-0000')
		, (5, 'GASTOS DE VENTA', '501-0000')
		, (5, 'GASTOS FINANCIEROS', '502-0000')
		, (5, 'GASTOS ADMINISTRATIVOS', '503-0000')
	INSERT INTO ContaCuentaDeMayor (ContaSubcuentaID, CuentaDeMayor, CuentaContpaq) VALUES
		(1, 'FONDO DE CAJA', '100-0000')
		, (1, 'CAJA', '101-0000')
		, (1, 'BANCOS', '102-0000')
	INSERT INTO ContaCuentaAuxiliar (ContaCuentaDeMayorID, CuentaAuxiliar, CuentaContpaq, Devengable, Detallable, VisibleEnCaja) VALUES
		(1, 'FONDO DE CAJA', '', 0, 0, 0)
		, (2, 'CAJA', '101-0000', 0, 0, 1)
		, (2, 'RESGUARDO', '', 0, 0, 0)
		, (3, 'BANAMEX', '102-0003', 0, 0, 0)
		, (3, 'SCOTIABANK', '102-0001', 0, 0, 0)
		, (3, 'SCOTIALINE', '102-0002', 0, 0, 0)

	INSERT INTO TipoDocumento (Documento) VALUES ('FACTURA'), ('OTRO OFICIAL'), ('NOTA')

	ALTER TABLE CajaEgreso ADD ContaEgresoID INT NULL FOREIGN KEY REFERENCES ContaEgreso(ContaEgresoID)	
	INSERT INTO CajaTipoEgreso (NombreTipoEgreso, Seleccionable, UsuarioID) VALUES
		('CUENTA AUXILIAR', 0, 1)

	INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
		('Contabilidad.Acceso', 'No tienes permisos para acceder al módulo de Contabilidad.')

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

-- Migración de Equivalentes a la nueva forma de guardar los datos
ALTER TABLE ParteEquivalente ADD GrupoID INT NOT NULL CONSTRAINT _tmp DEFAULT 0
ALTER TABLE ParteEquivalente DROP CONSTRAINT _tmp
GO

DELETE FROM ParteEquivalente
DBCC CHECKIDENT(ParteEquivalente, RESEED, 0)
ALTER TABLE ParteEquivalente DROP COLUMN ParteIDEquivalente

/* Siempre no al migrador, no quedó bien por la forma en que están los datos actuales
DECLARE @Equivalentes TABLE (GrupoID INT, Parte1 INT, Parte2 INT)
INSERT INTO @Equivalentes
	SELECT DISTINCT
		DENSE_RANK() OVER (ORDER BY 
			(CASE WHEN ParteID > ParteIDequivalente THEN ParteIDequivalente ELSE ParteID END)) AS GrupoID
		, CASE WHEN ParteID > ParteIDequivalente THEN ParteIDequivalente ELSE ParteID END AS Parte1
		, CASE WHEN ParteID > ParteIDequivalente THEN ParteID ELSE ParteIDequivalente END AS Parte2
	FROM ParteEquivalente
	WHERE Estatus = 1

INSERT INTO ParteEquivalente (GrupoID, ParteID, UsuarioID, FechaRegistro)
	SELECT GrupoID, Parte2, 1, GETDATE() FROM (
		SELECT * FROM @Equivalentes
		UNION
		SELECT DISTINCT
			GrupoID
			, Parte1
			, Parte1 AS Parte2
		FROM @Equivalentes
	) c
*/

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO
-- DROP VIEW ContaCuentasSubcuentasView
CREATE VIEW ContaCuentasSubcuentasView AS
	SELECT
		cs.ContaSubcuentaID
		, cc.ContaCuentaID
		, (cc.Cuenta + ' - ' + cs.Subcuenta) AS CuentaSubcuenta
	FROM
		ContaSubcuenta cs
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
GO

-- DROP VIEW ContaEgresosDetalleView
CREATE VIEW ContaEgresosDetalleView AS
	SELECT
		ced.ContaEgresoDetalleID
		, ced.ContaEgresoID
		, ced.ContaConsumibleID
		, cc.Consumible
		, ced.Cantidad
		, ced.Importe
		-- , SUM(cedd.Cantidad) OVER (PARTITION BY cedd.ContaEgresoDetalleID) AS CantidadDev
		, SUM(cedd.Cantidad) AS CantidadDev
	FROM
		ContaEgresoDetalle ced
		LEFT JOIN ContaConsumible cc ON cc.ContaConsumibleID = ced.ContaConsumibleID
		LEFT JOIN ContaEgresoDetalleDevengado cedd ON cedd.ContaEgresoDetalleID = ced.ContaEgresoDetalleID
	WHERE ced.Estatus = 1
	GROUP BY
		ced.ContaEgresoDetalleID
		, ced.ContaEgresoID
		, ced.ContaConsumibleID
		, cc.Consumible
		, ced.Cantidad
		, ced.Importe
GO

-- DROP VIEW ContaEgresosView
CREATE VIEW ContaEgresosView AS
	SELECT
		ce.ContaEgresoID
		, ce.ContaCuentaAuxiliarID
		, ce.Fecha
		, ce.FolioDePago
		, ce.Importe
		, s.NombreSucursal AS Sucursal
		, tfp.NombreTipoFormaPago AS FormaDePago
		, u.NombreUsuario AS Usuario
		, ce.Observaciones
		, cedc.ImporteDev
	FROM
		ContaEgreso ce
		LEFT JOIN Sucursal s ON s.SucursalID = ce.SucursalID AND s.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = ce.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = ce.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS ImporteDev FROM ContaEgresoDevengado GROUP BY ContaEgresoID
		) cedc ON cedc.ContaEgresoID = ce.ContaEgresoID
	WHERE ce.Estatus = 1
GO

ALTER VIEW [dbo].[PartesEquivalentesView] AS
	SELECT
		pe.ParteEquivalenteID
		, per.ParteID
		, pe.ParteID AS ParteIDEquivalente
		, p.NumeroParte
		, p.NombreParte AS Descripcion
		, pi.NombreImagen
		, (SELECT SUM(Existencia) FROM ParteExistencia WHERE ParteID = pe.ParteID AND SucursalID = 1) AS Matriz
		, (SELECT SUM(Existencia) FROM ParteExistencia WHERE ParteID = pe.ParteID AND SucursalID = 2) AS Suc02
		, (SELECT SUM(Existencia) FROM ParteExistencia WHERE ParteID = pe.ParteID AND SucursalID = 3) AS Suc03
	FROM
		ParteEquivalente pe
		INNER JOIN ParteEquivalente per ON per.GrupoID = pe.GrupoID AND per.ParteID != pe.ParteID AND per.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = pe.ParteID AND p.Estatus = 1
		LEFT JOIN ParteImagen pi ON pi.ParteID = pe.ParteID AND pi.Orden = 1
	WHERE pe.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO
-- DROP PROCEDURE pauContaCuentasTotales
-- EXEC pauContaCuentasTotales '2014-04-01', '2014-04-30'
CREATE PROCEDURE pauContaCuentasTotales (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2014-04-01'
	DECLARE @Hasta DATE = '2014-04-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @SucMatrizID INT = 1
	DECLARE @Suc02ID INT = 2
	DECLARE @Suc03ID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		cc.ContaCuentaID
		, cs.ContaSubcuentaID
		, ccm.ContaCuentaDeMayorID
		, cca.ContaCuentaAuxiliarID
		, cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, SUM(CASE WHEN ce.EsFiscal = 1 THEN ce.Importe ELSE 0.00 END) AS Fiscal
		, SUM(ce.Importe) AS Total
		, SUM(CASE WHEN ce.SucursalID = @SucMatrizID THEN (ce.Importe 
			- ISNULL(ced2.Importe, 0.00) - ISNULL(ced3.Importe, 0.00)) ELSE ced1.Importe END) AS Matriz
		, SUM(CASE WHEN ce.SucursalID = @Suc02ID THEN (ce.Importe
			- ISNULL(ced1.Importe, 0.00) - ISNULL(ced3.Importe, 0.00)) ELSE ced2.Importe END) AS Suc02
		, SUM(CASE WHEN ce.SucursalID = @Suc03ID THEN (ce.Importe 
			- ISNULL(ced1.Importe, 0.00) - ISNULL(ced2.Importe, 0.00)) ELSE ced3.Importe END) AS Suc03
		, SUM(ISNULL(ced1.Importe, 0.00) + ISNULL(ced2.Importe, 0.00) + ISNULL(ced3.Importe, 0.00)) AS ImporteDev
	FROM
		ContaCuenta cc
		LEFT JOIN ContaSubcuenta cs ON cs.ContaCuentaID = cc.ContaCuentaID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaSubcuentaID = cs.ContaSubcuentaID
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaDeMayorID = ccm.ContaCuentaDeMayorID
		LEFT JOIN ContaEgreso ce ON ce.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
			AND (ce.Fecha >= @Desde AND ce.Fecha < @Hasta) AND ce.Estatus = 1
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengado
			WHERE SucursalID = @SucMatrizID GROUP BY ContaEgresoID, SucursalID
		) ced1 ON ced1.ContaEgresoID = ce.ContaEgresoID
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengado
			WHERE SucursalID = @Suc02ID GROUP BY ContaEgresoID, SucursalID
		) ced2 ON ced2.ContaEgresoID = ce.ContaEgresoID
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengado
			WHERE SucursalID = @Suc03ID GROUP BY ContaEgresoID, SucursalID
		) ced3 ON ced3.ContaEgresoID = ce.ContaEgresoID
	GROUP BY
		cc.ContaCuentaID
		, cs.ContaSubcuentaID
		, ccm.ContaCuentaDeMayorID
		, cca.ContaCuentaAuxiliarID
		, cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
	ORDER BY
		cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar

END
GO

-- DROP PROCEDURE pauPartesMaxMinEquivalentes
-- EXEC pauPartesMaxMinEquivalentes '2013-04-01', '2014-03-31'
CREATE PROCEDURE pauPartesMaxMinEquivalentes (
	@Desde DATE
	, @Hasta DATE
	, @ProveedorID INT = NULL
	, @MarcaID INT = NULL
	, @LineaID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2013-01-01'
	DECLARE @Hasta DATE = '2014-01-31'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @SucMatrizID INT = 1

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		pe.ParteEquivalenteID
		, pe.GrupoID
		, pe.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pv.NombreProveedor AS Proveedor
		, mr.NombreMarcaParte AS Marca
		, ISNULL(SUM(vd.Cantidad), 0.00) AS Cantidad
		, pp.CostoConDescuento
		, pmm.Calcular
		, pmm.FechaModificacion
	FROM
		ParteEquivalente pe
		INNER JOIN Parte p ON p.ParteID = pe.ParteID AND p.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = pe.ParteID AND pmm.SucursalID = @SucMatrizID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mr ON mr.MarcaParteID = p.MarcaParteID AND mr.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = pe.ParteID AND vd.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.Estatus = 1
	WHERE
		(@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		AND (@LineaID IS NULL OR p.LineaID = @LineaID)
	GROUP BY
		pe.ParteEquivalenteID
		, pe.GrupoID
		, pe.ParteID
		, p.NumeroParte
		, p.NombreParte
		, pv.NombreProveedor
		, mr.NombreMarcaParte
		, pp.CostoConDescuento
		, pmm.Calcular
		, pmm.FechaModificacion
END
GO

-- DROP PROCEDURE pauPartesMaxMinSinCalcular
-- EXEC pauPartesMaxMinSinCalcular '2013-04-01', '2014-03-31'
CREATE PROCEDURE pauPartesMaxMinSinCalcular (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2013-01-01'
	DECLARE @Hasta DATE = '2014-01-31'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	SELECT
		pmm.ParteID
		, s.NombreSucursal AS Sucursal
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pv.NombreProveedor AS Proveedor
		, mr.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, ISNULL(SUM(vd.Cantidad), 0.00) AS Cantidad
	FROM
		ParteMaxMin pmm
		LEFT JOIN Parte p ON p.ParteID = pmm.ParteID AND p.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = pmm.SucursalID AND s.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mr ON mr.MarcaParteID = p.MarcaParteID AND mr.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = pmm.ParteID AND vd.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.Estatus = 1
	WHERE pmm.Calcular = 0
	GROUP BY
		pmm.ParteID
		, s.NombreSucursal
		, p.NumeroParte
		, p.NombreParte
		, pv.NombreProveedor
		, mr.NombreMarcaParte
		, l.NombreLinea
	ORDER BY
		pv.NombreProveedor
		, mr.NombreMarcaParte
		, l.NombreLinea
		, p.NombreParte

END
GO

-- EXEC pauParteMaxMinDatosExtra 9267, '2013-04-01', '2014-03-31', 1
ALTER PROCEDURE [dbo].[pauParteMaxMinDatosExtra] (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @ParteID INT = 9267
	DECLARE @Desde DATE = '2013-04-01'
	DECLARE @Hasta DATE = '2014-03-31'
	DECLARE @SucursalID INT = 1
	*/
	
	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	-- Procedimiento

	;WITH _VentasParte AS (
		SELECT
			p.ParteID
			, CONVERT(DATE, v.Fecha) AS Dia
			, DATEPART(WK, v.Fecha) AS Semana
			, MONTH(v.Fecha) AS Mes
			, YEAR(v.Fecha) AS Anio
			, SUM(vd.Cantidad) AS Cantidad
			, 0 AS Negada
		FROM
			Parte p
			INNER JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		WHERE
			p.Estatus = 1
			AND p.ParteID = @ParteID
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		GROUP BY
			p.ParteID
			, CONVERT(DATE, v.Fecha)
			, DATEPART(WK, v.Fecha)
			, MONTH(v.Fecha)
			, YEAR(v.Fecha)

		-- Se agrega lo negado
		UNION ALL
		SELECT
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro) AS Dia
			, DATEPART(WK, rf.FechaRegistro) AS Semana
			, MONTH(rf.FechaRegistro) AS Mes
			, YEAR(rf.FechaRegistro) AS Anio
			, SUM(rf.CantidadRequerida) AS Cantidad
			, 1 AS Negada
		FROM
			ReporteDeFaltante rf
			-- Para validar que no se haya vendido en el día o en el siguiente
			LEFT JOIN (
				SELECT DISTINCT
					vd.ParteID
					, v.Fecha
				FROM
					VentaDetalle vd
					INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				WHERE
					vd.Estatus = 1
					AND v.VentaEstatusID = @EstPagadaID
					AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
			) vdc ON vdc.ParteID = rf.ParteID AND 
				(vdc.Fecha >= CONVERT(DATE, rf.FechaRegistro) AND vdc.Fecha < DATEADD(d, 2, CONVERT(DATE, rf.FechaRegistro)))
		WHERE
			rf.Estatus = 1
			AND rf.ParteID = @ParteID
			AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
			AND (@SucursalID IS NULL OR rf.SucursalID = @SucursalID)
			-- Parte de Venta
			AND vdc.ParteID IS NULL
		GROUP BY
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro)
			, DATEPART(WK, rf.FechaRegistro)
			, MONTH(rf.FechaRegistro)
			, YEAR(rf.FechaRegistro)
		-- Fin lo negado
	)

	-- Venta mayor de un día
	SELECT * FROM (
		SELECT TOP 1
			1 AS Grupo
			, 0 AS Anio
			, 0 AS Periodo
			-- , MAX(Cantidad) AS Cantidad
			, Cantidad
			, CASE WHEN Negada = 1 THEN Cantidad ELSE 0.00 END AS Negadas
		FROM _VentasParte
		ORDER BY Cantidad DESC
	) c
	
	-- Venta menor de un día
	UNION
	SELECT * FROM (
		SELECT TOP 1
			2 AS Grupo
			, 0 AS Anio
			, 0 AS Periodo
			-- , MIN(Cantidad) AS Cantidad
			, Cantidad
			, CASE WHEN Negada = 1 THEN Cantidad ELSE 0.00 END AS Negadas
		FROM _VentasParte
		ORDER BY Cantidad
	) c

	-- Ventas por semana
	UNION
	SELECT
		3 AS Grupo
		, Anio
		, Semana AS Periodo
		, SUM(Cantidad) AS Cantidad
		, SUM(CASE WHEN Negada = 1 THEN Cantidad ELSE 0.00 END) AS Negadas
	FROM _VentasParte
	GROUP BY
		Anio
		, Semana

	-- Ventas por mes
	UNION
	SELECT
		4 AS Grupo
		, Anio
		, Mes AS Periodo
		, SUM(Cantidad) AS Cantidad
		, SUM(CASE WHEN Negada = 1 THEN Cantidad ELSE 0.00 END) AS Negadas
	FROM _VentasParte
	GROUP BY
		Anio
		, Mes

	ORDER BY Grupo, Cantidad DESC

END
GO

-- EXEC pauPartesMaxMin 1, '2013-02-01', '2014-02-28'
ALTER PROCEDURE [dbo].[pauPartesMaxMin] (
	@SucursalID INT
	, @Desde DATE
	, @Hasta DATE
	, @ProveedorID INT = NULL
	, @MarcaID INT = NULL
	, @LineaID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @SucursalID INT = 1
	DECLARE @Desde DATE = '2013-02-01'
	DECLARE @Hasta DATE = '2014-03-31'
	DECLARE @ProveedorID INT = NULL
	DECLARE @MarcaID INT = NULL
	DECLARE @LineaID INT = NULL
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Procedimiento
	
	-- Ventas
	;WITH _Ventas AS (
		SELECT
			vd.ParteID
			, CONVERT(DATE, v.Fecha) AS Fecha
			, COUNT(v.VentaID) AS Ventas
			, SUM(vd.Cantidad) AS Cantidad
			, SUM((vd.PrecioUnitario - pp.Costo) * vd.Cantidad) AS Utilidad
		FROM
			VentaDetalle vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.SucursalID = @SucursalID
		GROUP BY
			vd.ParteID
			, CONVERT(DATE, v.Fecha)

		-- Se agrega lo negado
		UNION ALL
		SELECT
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro) AS Fecha
			, COUNT(rf.ReporteDeFaltanteID) AS Ventas
			, SUM(rf.CantidadRequerida) AS Cantidad
			, SUM((pp.PrecioUno - pp.Costo) * rf.CantidadRequerida) AS Utilidad
		FROM
			ReporteDeFaltante rf
			LEFT JOIN PartePrecio pp ON pp.ParteID = rf.ParteID AND pp.Estatus = 1
			-- Para validar que no se haya vendido en el día o en el siguiente
			LEFT JOIN (
				SELECT DISTINCT
					vd.ParteID
					, CONVERT(DATE, v.Fecha) AS Dia
				FROM
					VentaDetalle vd
					INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				WHERE
					vd.Estatus = 1
					AND v.VentaEstatusID = @EstPagadaID
					AND v.SucursalID = @SucursalID
			) vdc ON vdc.ParteID = rf.ParteID AND (rf.FechaRegistro >= vdc.Dia AND rf.FechaRegistro < DATEADD(d, 2, vdc.Dia))
		WHERE
			rf.Estatus = 1
			AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
			AND rf.SucursalID = @SucursalID
			-- Parte de Venta
			AND vdc.ParteID IS NULL
		GROUP BY
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro)
		-- Fin lo negado
	)
	-- Ventas por semana
	, _VentasPorSem AS (
		SELECT
			ParteID
			, DATEPART(WK, Fecha) AS Semana
			, SUM(Cantidad) AS Cantidad
		FROM _Ventas
		GROUP BY
			ParteID
			, DATEPART(WK, Fecha)
	)
	-- Ventas por mes
	, _VentasPorMes AS (
		SELECT
			ParteID
			, MONTH(Fecha) AS Mes
			, SUM(Cantidad) AS Cantidad
		FROM _Ventas
		GROUP BY
			ParteID
			, MONTH(Fecha)
	)

	-- Consulta final
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, p.UnidadEmpaque
		, p.EsPar
		, p.TiempoReposicion
		, pe.Existencia
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pa.AbcDeNegocio
		, pa.AbcDeProveedor
		, pa.AbcDeLinea
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo

		, ISNULL(SUM(vc.Ventas), 0) AS VentasTotal
		, ISNULL(SUM(vc.Cantidad), 0.00) AS CantidadTotal
		, ISNULL(MAX(vc.Cantidad), 0.00) AS CantidadMaxDia
		, ISNULL(MAX(vcs.Cantidad), 0.00) AS CantidadMaxSem
		, ISNULL(MAX(vcm.Cantidad), 0.00) AS CantidadMaxMes
		, ISNULL(SUM(vc.Utilidad), 0.00) AS UtilidadTotal
		, ISNULL(pmm.VentasGlobales, 0) AS VentasGlobales
	FROM
		Parte p
		INNER JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID AND pmm.SucursalID = @SucursalID
		LEFT JOIN _Ventas vc ON vc.ParteID = p.ParteID
		LEFT JOIN (
			SELECT ParteID, MAX(Cantidad) AS Cantidad FROM _VentasPorSem GROUP BY ParteID
		) vcs ON vcs.ParteID = p.ParteID
		LEFT JOIN (
			SELECT ParteID, MAX(Cantidad) AS Cantidad FROM _VentasPorMes GROUP BY ParteID
		) vcm ON vcm.ParteID = p.ParteID
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = p.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	WHERE
		p.Estatus = 1
		AND pmm.Calcular = 1
		AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		AND (@LineaID IS NULL OR p.LineaID = @LineaID)
	GROUP BY
		p.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.ProveedorID
		, pv.NombreProveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte
		, p.LineaID
		, l.NombreLinea
		, p.UnidadEmpaque
		, p.EsPar
		, p.TiempoReposicion
		, pe.Existencia
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pa.AbcDeNegocio
		, pa.AbcDeProveedor
		, pa.AbcDeLinea
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo
		, pmm.VentasGlobales
	ORDER BY
		VentasTotal DESC

END
GO

-- EXEC pauVentasPartesBusqueda @SucursalID = 1, @Descripcion1 = 'foco', @Descripcion2 = '890', @Equivalentes = 1
ALTER PROCEDURE pauVentasPartesBusqueda (
	@SucursalID INT
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
	, @SistemaID INT = NULL
	, @LineaID INT = NULL
	, @MarcaID INT = NULL
	, @TipoCilindroID INT = NULL
	, @Largo INT = NULL
	, @Alto INT = NULL
	, @Dientes INT = NULL
	, @Amperes INT = NULL
	, @Watts INT = NULL
	, @Diametro INT = NULL
	, @Astrias INT = NULL
	, @Terminales INT = NULL
	, @Voltios INT = NULL
	, @CodigoAlterno NVARCHAR(32) = NULL

	, @VehiculoModeloID INT = NULL -- Se debe incluir el ModeloID para que el filtro por vehículo tenga efecto
	, @VehiculoAnio INT = NULL
	, @VehiculoMotorID INT = NULL
	
	, @Equivalentes BIT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4

	IF @Codigo IS NULL BEGIN
		-- Si la búsqueda incluye Equivalentes
		IF @Equivalentes = 1 BEGIN
			;WITH _Partes AS (
				SELECT
					p.ParteID
					, pe.GrupoID
				FROM
					Parte p
					LEFT JOIN ParteEquivalente pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
				WHERE
					p.Estatus = 1
					AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
					AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
					AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
					AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
					AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
					AND (@SistemaID IS NULL OR p.SubsistemaID IN (
						SELECT SubsistemaID
						FROM Subsistema
						WHERE SistemaID = @SistemaID AND Estatus = 1
					))
					AND (@LineaID IS NULL OR p.LineaID = @LineaID)
					AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
					AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
					AND (@Largo IS NULL OR p.Largo = @Largo)
					AND (@Alto IS NULL OR p.Alto = @Alto)
					AND (@Dientes IS NULL OR p.Dientes = @Dientes)
					AND (@Amperes IS NULL OR p.Amperes = @Amperes)
					AND (@Watts IS NULL OR p.Watts = @Watts)
					AND (@Diametro IS NULL OR p.Diametro = @Diametro)
					AND (@Astrias IS NULL OR p.Astrias = @Astrias)
					AND (@Terminales IS NULL OR p.Terminales = @Terminales)
					AND (@Voltios IS NULL OR p.Voltios = @Voltios)
					AND (@CodigoAlterno IS NULL OR p.ParteID IN (
						SELECT DISTINCT ParteID
						FROM ParteCodigoAlterno
						WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
					))
					AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
						SELECT ParteID
						FROM ParteVehiculo
						WHERE
							TipoFuenteID <> @IdTipoFuenteMostrador
							AND ModeloID = @VehiculoModeloID
							AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
							AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
					))
			)
		
			, _Equivalentes AS (
				SELECT
					ISNULL(pe.ParteID, p.ParteID) AS ParteID
					, ROW_NUMBER() OVER(PARTITION BY p.ParteID ORDER BY 
						CASE WHEN pex.Existencia > 0 THEN 1 ELSE 2 END
						, pp.PrecioUno DESC
					) AS Fila
				FROM
					_Partes p
					LEFT JOIN ParteEquivalente pe ON pe.GrupoID = p.GrupoID AND pe.Estatus = 1
					LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
					LEFT JOIN ParteExistencia pex ON pex.ParteID = pe.ParteID 
						AND pex.SucursalID = @SucursalID AND pex.Estatus = 1
			)
			
			SELECT
				e.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
				, pe.Existencia
				, pic.NombreImagen AS Imagen
				, pic.CuentaImagenes
			FROM
				_Equivalentes e
				LEFT JOIN Parte p ON p.ParteID = e.ParteID AND p.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN (
					SELECT
						ParteID
						, MAX(CASE WHEN Orden = 1 THEN NombreImagen ELSE '' END) AS NombreImagen
						, COUNT(*) AS CuentaImagenes
					FROM ParteImagen
					WHERE Estatus = 1
					GROUP BY ParteID
				) pic ON pic.ParteID = p.ParteID
				LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
			WHERE e.Fila = 1
		
		-- Si es búsqueda normal
		END ELSE BEGIN
			SELECT
				p.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
				, pe.Existencia
				, pic.NombreImagen AS Imagen
				, pic.CuentaImagenes
			FROM
				Parte p
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				-- LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
				-- LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1 AND pi.Estatus = 1
				LEFT JOIN (
					SELECT
						ParteID
						, MAX(CASE WHEN Orden = 1 THEN NombreImagen ELSE '' END) AS NombreImagen
						, COUNT(*) AS CuentaImagenes
					FROM ParteImagen
					WHERE Estatus = 1
					GROUP BY ParteID
				) pic ON pic.ParteID = p.ParteID
				-- LEFT JOIN ParteCodigoAlterno pca ON pca.ParteID = p.ParteID AND pca.Estatus = 1
				LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
			WHERE
				p.Estatus = 1
				AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
				AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
				AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
				AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
				AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
				AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
				AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
				AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
				AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
				
				AND (@SistemaID IS NULL OR p.SubsistemaID IN (
					SELECT SubsistemaID
					FROM Subsistema
					WHERE SistemaID = @SistemaID AND Estatus = 1
				))
				AND (@LineaID IS NULL OR p.LineaID = @LineaID)
				AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
				AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
				AND (@Largo IS NULL OR p.Largo = @Largo)
				AND (@Alto IS NULL OR p.Alto = @Alto)
				AND (@Dientes IS NULL OR p.Dientes = @Dientes)
				AND (@Amperes IS NULL OR p.Amperes = @Amperes)
				AND (@Watts IS NULL OR p.Watts = @Watts)
				AND (@Diametro IS NULL OR p.Diametro = @Diametro)
				AND (@Astrias IS NULL OR p.Astrias = @Astrias)
				AND (@Terminales IS NULL OR p.Terminales = @Terminales)
				AND (@Voltios IS NULL OR p.Voltios = @Voltios)
				
				AND (@CodigoAlterno IS NULL OR p.ParteID IN (
					SELECT DISTINCT ParteID
					FROM ParteCodigoAlterno
					WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
				))
				
				AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
					SELECT ParteID
					FROM ParteVehiculo
					WHERE
						TipoFuenteID <> @IdTipoFuenteMostrador
						AND ModeloID = @VehiculoModeloID
						AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
						AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
				))
		END
	
	-- Si es búsqueda por código
	END ELSE BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte AS NumeroDeParte
			, p.NombreParte AS Descripcion
			, mp.NombreMarcaParte AS Marca
			, l.NombreLinea AS Linea
			, pe.Existencia
			, pi.NombreImagen AS Imagen
			, (SELECT COUNT(*) FROM ParteImagen WHERE ParteID = p.ParteID AND Estatus = 1) AS CuentaImagenes
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
			LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
		WHERE
			p.Estatus = 1
			AND (NumeroParte = @Codigo
			OR CodigoBarra = @Codigo)
	END

END
GO