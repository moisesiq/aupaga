/* Script con modificaciones para el módulo de ventas. Archivo 40
 * Creado: 2014/05/01
 * Subido: 2014/05/16
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Para metas
	
	CREATE TABLE MetaUtilidadSucursal (
		MetaUtilidadSucursalID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, Fecha DATETIME NOT NULL
		, Utilidad DECIMAL(12, 2) NOT NULL
	)
	
	CREATE TABLE MetaComisionVendedor (
		MetaComisionVendedorID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VendedorID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, Fecha DATETIME NOT NULL
		, Comision DECIMAL(12, 2) NOT NULL
	)
	
	CREATE TABLE MetaGeneral (
		MetaGeneralID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, Importe DECIMAL(12, 2) NOT NULL
	)

	CREATE TABLE Meta (
		MetaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, VendedorID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, NombreMeta NVARCHAR(32) NOT NULL
		, MarcaParteID INT NULL FOREIGN KEY REFERENCES MarcaParte(MarcaParteID)
		, LineaID INT NULL FOREIGN KEY REFERENCES Linea(LineaID)
		, ParteID INT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, Cantidad DECIMAL(12, 2) NULL
		, RutaImagen NVARCHAR(256) NULL
	)
	
	CREATE TABLE MetaVendedor (
		MetaVendedorID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VendedorID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, SueldoFijo DECIMAL(12, 2) NOT NULL
		, Meta DECIMAL(12, 2) NOT NULL
	)

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

GO

-- DROP VIEW MetasView
CREATE VIEW MetasView AS
	SELECT
		m.MetaID
		, m.SucursalID
		, m.VendedorID
		, m.MarcaParteID AS MarcaID
		, m.LineaID
		, m.ParteID
		, p.NumeroParte AS NumeroDeParte
		, m.NombreMeta
		, m.Cantidad
		, m.RutaImagen
	FROM
		Meta m
		LEFT JOIN Parte p ON p.ParteID = m.ParteID AND p.Estatus = 1
GO

CREATE VIEW MetasUtilidadSucursalesView AS
	SELECT
		ROW_NUMBER() OVER (ORDER BY SucursalID) AS Registro
		, SucursalID
		, DATEPART(YEAR, Fecha) AS Anio
		, DATEPART(MONTH, Fecha) AS Mes
		, DATEPART(WEEK, Fecha) AS Semana
		, SUM(Utilidad) AS Utilidad
	FROM MetaUtilidadSucursal
	GROUP BY
		SucursalID
		, DATEPART(YEAR, Fecha)
		, DATEPART(MONTH, Fecha)
		, DATEPART(WEEK, Fecha)
GO

CREATE VIEW MetasComisionVendedoresView AS
	SELECT
		ROW_NUMBER() OVER (ORDER BY VendedorID) AS Registro
		, VendedorID
		, DATEPART(YEAR, Fecha) AS Anio
		, DATEPART(MONTH, Fecha) AS Mes
		, DATEPART(WEEK, Fecha) AS Semana
		, SUM(Comision) AS Comision
	FROM MetaComisionVendedor
	GROUP BY
		VendedorID
		, DATEPART(YEAR, Fecha)
		, DATEPART(MONTH, Fecha)
		, DATEPART(WEEK, Fecha)
GO

CREATE VIEW VentasPartesView AS
	SELECT
		vd.VentaDetalleID
		, vd.VentaID
		, v.Fecha
		, v.VentaEstatusID
		, v.SucursalID
		, v.RealizoUsuarioID AS VendedorID
		, vd.ParteID
		, vd.Cantidad
		, l.LineaID
		, mp.MarcaParteID AS MarcaID
	FROM
		VentaDetalle vd
		INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	WHERE
		vd.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

-- EXEC pauContaCuentasMovimientosTotales 1, '2014-04-01', '2014-04-30'
-- DROP PROCEDURE pauContaCuentasMovimientosTotales
CREATE PROCEDURE pauContaCuentasMovimientosTotales (
	@SucursalID INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauContaCuentasMovimientosTotales 1, '2014-04-01', '2014-04-30'
	DECLARE @Desde DATE = '2014-04-01'
	DECLARE @Hasta DATE = '2014-04-30'
	*/

	-- Definición de variables tipo constante
	

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Gastos fijos - SucursalGastoFijo
	SELECT
		gf.SucursalGastoFijoID AS TablaRegistroID
		, CONVERT(BIT, 1) AS GastoFijo
		, gf.SucursalID
		, gf.Importe
		, cca.ContaCuentaAuxiliarID
		, ccm.ContaCuentaDeMayorID
		, cs.ContaSubcuentaID
		, cc.ContaCuentaID
		, NULL AS Egreso
		, cca.CuentaAuxiliar
		, ccm.CuentaDeMayor
		, cs.Subcuenta
		, cc.Cuenta
	FROM
		SucursalGastoFijo gf
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = gf.ContaCuentaAuxiliarID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE gf.SucursalID = @SucursalID

	UNION

	-- Gastos variables - ContaEgreso
	SELECT
		ce.ContaEgresoID AS TablaRegistroID
		, CONVERT(BIT, 0) AS GastoFijo
		, cedc.SucursalID
		, cedc.ImporteDev AS Importe
		, cca.ContaCuentaAuxiliarID
		, ccm.ContaCuentaDeMayorID
		, cs.ContaSubcuentaID
		, cc.ContaCuentaID
		, ce.Observaciones AS Egreso
		, cca.CuentaAuxiliar
		, ccm.CuentaDeMayor
		, cs.Subcuenta
		, cc.Cuenta
	FROM
		ContaEgreso ce
		INNER JOIN (
			SELECT
				ContaEgresoID
				, SucursalID
				, SUM(Importe) AS ImporteDev
			FROM ContaEgresoDevengado
			WHERE (Fecha >= @Desde AND Fecha < @Hasta)
			GROUP BY ContaEgresoID, SucursalID
		) cedc ON cedc.ContaEgresoID = ce.ContaEgresoID
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE
		ce.Estatus = 1
		AND cedc.SucursalID = @SucursalID
		-- Se quitan los egresos que ya se tienen como gastos fijos
		AND ce.ContaCuentaAuxiliarID NOT IN (
			SELECT ContaCuentaAuxiliarID
			FROM SucursalGastoFijo
			WHERE SucursalID = @SucursalID
		)
	
	-- Se asigna el orden
	ORDER BY
		cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, Egreso

END
GO

CREATE PROCEDURE pauComisionesAgrupado (
	@Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* 
	DECLARE @Desde DATE = '2014-04-01'
	DECLARE @Hasta DATE = '2014-05-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstGenCompletado INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- De momento no se toma en cuenta la sucursal
	DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
	DECLARE @PorComision9500 DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje9500')
	DECLARE @IvaMul DECIMAL(5, 2) = (SELECT (1 + (CONVERT(DECIMAL(5, 2), Valor) / 100)) FROM Configuracion WHERE Nombre = 'IVA')

	-- Procedimiento
	SELECT
		-- vp.VentaID
		-- , MAX(vp.Fecha) AS Fecha
		v.SucursalID
		, v.RealizoUsuarioID
		-- , SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Importe
		, SUM((vd.PrecioUnitario - pp.Costo) * vd.Cantidad) AS Utilidad
		, (
			CASE WHEN v.ComisionistaClienteID > 0 THEN
				SUM(((
					CASE com.ListaDePrecios
						WHEN 1 THEN (pp.PrecioUno / @IvaMul)
						WHEN 2 THEN (pp.PrecioDos / @IvaMul)
						WHEN 3 THEN (pp.PrecioTres / @IvaMul)
						WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
						WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
					END
					- pp.Costo) * vd.Cantidad)
					* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
			ELSE
				SUM(((vd.PrecioUnitario - pp.Costo) * vd.Cantidad)
				* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
			END
		) AS Comision
	FROM
		VentaPago vp
		INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
		INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
		LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
			AND c9.Estatus = 1
	WHERE
		vp.Estatus = 1
		AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		AND v.VentaEstatusID = @EstPagadaID
	GROUP BY
		vp.VentaID
		, v.SucursalID
		, v.RealizoUsuarioID
		, v.ComisionistaClienteID
		, c9.VentaID

END
GO