/* Script con modificaciones para el módulo de ventas. Archivo 27
 * Creado: 2014/06/10
 * Subido: 2015/08/17
 */

-- Se retoma el proyecto de invnetario (27/07/2015), después de un año :s

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- Para lo del inventario

-- DROP TABLE InventarioUsuario
-- DROP TABLE InventarioLineaPeriodicidad
-- DROP TABLE InventarioConteo
-- DROP TABLE InventarioLinea

CREATE TABLE InventarioUsuario (
	InventarioUsuarioID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, InvUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, ArticulosDiarios INT NOT NULL
)

CREATE TABLE InventarioLineaPeriodicidad (
	InventarioLineaPeriodicidadID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, LineaID INT NOT NULL FOREIGN KEY REFERENCES Linea(LineaID)
	, Periodicidad INT NOT NULL
)

CREATE TABLE InventarioLinea (
	InventarioLineaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, LineaID INT NOT NULL FOREIGN KEY REFERENCES Linea(LineaID)
	, EstatusGenericoID INT NULL FOREIGN KEY REFERENCES EstatusGenerico(EstatusGenericoID)
	, FechaIniciado DATETIME NULL
	, FechaCompletado DATETIME NULL
	, AvVuelta INT NULL
	, AvPeriodicidad INT NULL
	, AvManual INT NULL
)
-- INSERT INTO EstatusGenerico (Descripcion) VALUES ('EN CURSO'), ('EN REVISIÓN')

CREATE TABLE InventarioConteo (
	InventarioConteoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, InventarioLineaID INT NOT NULL FOREIGN KEY REFERENCES InventarioLinea(InventarioLineaID)
	, Dia DATE NOT NULL
	, ConteoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, Diferencia DECIMAL(12, 2) NULL
	, RealizoUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, Revision INT NULL
	, Valido BIT NULL
)

ALTER TABLE Sucursal ADD GerenteID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
GO
UPDATE Sucursal SET GerenteID = 1
ALTER TABLE Sucursal ALTER COLUMN GerenteID INT NOT NULL

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion, SucursalID) VALUES
	('Inventario.Asignacion.Habilitar', 'V', 'V', 'Indica si se deben asignar automáticamente partes para inventariar.', 1)
	, ('Inventario.Asignacion.Habilitar', 'V', 'V', 'Indica si se deben asignar automáticamente partes para inventariar.', 2)
	, ('Inventario.Asignacion.Habilitar', 'V', 'V', 'Indica si se deben asignar automáticamente partes para inventariar.', 3)
	, ('Inventario.Conteo.RevisarEnCorte', 'V', 'V', 'Indica si se debe validar que todos los conteos estén realizados antes de hacer el Corte de Caja.', 1)
	, ('Inventario.Conteo.RevisarEnCorte', 'V', 'V', 'Indica si se debe validar que todos los conteos estén realizados antes de hacer el Corte de Caja.', 2)
	, ('Inventario.Conteo.RevisarEnCorte', 'V', 'V', 'Indica si se debe validar que todos los conteos estén realizados antes de hacer el Corte de Caja.', 3)

INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
	('Ventas.Cobranza.Ver', 'No tienes permisos para acceder a la opción de Cobranza.')
	, ('Administracion.Master.Ver', 'No tienes permisos para acceder a la opción de Master.')
	, ('Administracion.Inventario.Ver', 'No tienes permisos para acceder a la opción de Inventario.')
	, ('Administracion.CriteriosAbc.Ver', 'No tienes permisos para acceder a la opción de Criterios Abc.')
	, ('Administracion.Aplicaciones.Ver', 'No tienes permisos para acceder a la opción de Aplicaciones.')
	, ('Administracion.CapitalHumano.Ver', 'No tienes permisos para acceder a la opción de Capital Humano.')

-- Para la búsqueda en Ventas
CREATE NONCLUSTERED INDEX Ix_SegunPlan_BusquedaVenta ON Parte (ParteEstatusID, Estatus) 
	INCLUDE (ParteID, LineaID, MarcaParteID, SubsistemaID, NumeroParte, NombreParte)
CREATE NONCLUSTERED INDEX Ix_SegunPlan_BusquedaVenta ON ParteExistencia (SucursalID, Estatus) INCLUDE (ParteID, Existencia)
CREATE NONCLUSTERED INDEX Ix_SegunPlan_BusquedaVenta ON PartePrecio (Estatus) INCLUDE (ParteID, PrecioUno)

-- Para pagos a proveedores
ALTER TABLE ProveedorPolizaDetalle ADD CajaEgresoID INT NULL FOREIGN KEY REFERENCES CajaEgreso(CajaEgresoID)

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

-- DROP VIEW InventarioConteosView
CREATE VIEW InventarioConteosView AS
	SELECT
		ic.InventarioConteoID
		, ic.InventarioLineaID
		, ic.Dia
		, ic.ConteoUsuarioID
		, ic.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pe.Existencia
		, CONVERT(DECIMAL(12, 2), NULL) AS Conteo
		, ic.Diferencia
		, ic.RealizoUsuarioID
	FROM
		InventarioConteo ic
		LEFT JOIN InventarioUsuario iu ON iu.InvUsuarioID = ic.ConteoUsuarioID
		LEFT JOIN Parte p ON p.ParteID = ic.ParteID AND p.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = iu.SucursalID AND pe.Estatus = 1
GO

-- DROP VIEW InventarioLineasPartesView
CREATE VIEW InventarioLineasPartesView AS
	SELECT
		ISNULL(ROW_NUMBER() OVER (ORDER BY l.LineaID, s.SucursalID), 0) AS Id
		, l.LineaID
		, s.SucursalID
		, COUNT(p.ParteID) AS Partes
	FROM
		Linea l
		LEFT JOIN Sucursal s ON s.Estatus = 1
		LEFT JOIN Parte p ON p.LineaID = l.LineaID AND p.Estatus = 1
		INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = s.SucursalID AND pe.Estatus = 1
	WHERE pe.Existencia > 0
	GROUP BY
		l.LineaID
		, s.SucursalID
GO

-- DROP VIEW InventarioUsuariosView
CREATE VIEW InventarioUsuariosView AS
	SELECT
		iu.InventarioUsuarioID
		, iu.InvUsuarioID
		, u.NombreUsuario AS Usuario
		, iu.SucursalID
		, s.NombreSucursal AS Sucursal
		, iu.ArticulosDiarios
	FROM
		InventarioUsuario iu
		LEFT JOIN Usuario u ON u.UsuarioID = iu.InvUsuarioID AND u.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = iu.SucursalID AND s.Estatus = 1
GO

-- DROP VIEW InventarioLineasAgrupadoView
CREATE VIEW InventarioLineasAgrupadoView AS
	SELECT
		ISNULL(ROW_NUMBER() OVER (ORDER BY il.LineaID), 0) AS Id
		, il.LineaID
		, l.NombreLinea AS Linea
		, il.AvVuelta
		, il.AvPeriodicidad
		, il.AvManual
		, CASE WHEN MIN(il.EstatusGenericoID) = MAX(il.EstatusGenericoID) THEN MIN(il.EstatusGenericoID) ELSE NULL END
			AS EstatusGenericoID
		, MIN(il.FechaIniciado) AS FechaIniciado
		, CASE WHEN MIN(ISNULL(il.FechaCompletado, '19700101')) = '19700101' THEN NULL ELSE MAX(il.FechaCompletado) END
			AS FechaCompletado
	FROM
		InventarioLinea il
		LEFT JOIN Linea l ON l.LineaID = il.LineaID AND l.Estatus = 1
	GROUP BY
		il.LineaID
		, l.NombreLinea
		, il.AvVuelta
		, il.AvPeriodicidad
		, il.AvManual
GO

-- DROP VIEW InventarioResultadosView
CREATE VIEW InventarioResultadosView AS
	SELECT
		ISNULL(ROW_NUMBER() OVER (ORDER BY il.LineaID), 0) AS Id
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, il.LineaID
		, l.NombreLinea AS Linea
		, mp.NombreMarcaParte AS Marca
		, pp.Costo
		, SUM(CASE WHEN il.SucursalID = 1 THEN pe.Existencia ELSE 0 END) AS ExistenciaMatriz
		, SUM(CASE WHEN il.SucursalID = 1 THEN ic.Diferencia ELSE 0 END) AS DiferenciaMatriz
		, SUM(CASE WHEN il.SucursalID = 2 THEN pe.Existencia ELSE 0 END) AS ExistenciaSuc2
		, SUM(CASE WHEN il.SucursalID = 2 THEN ic.Diferencia ELSE 0 END) AS DiferenciaSuc2
		, SUM(CASE WHEN il.SucursalID = 3 THEN pe.Existencia ELSE 0 END) AS ExistenciaSuc3
		, SUM(CASE WHEN il.SucursalID = 3 THEN ic.Diferencia ELSE 0 END) AS DiferenciaSuc3
		, SUM(ic.Diferencia) AS DiferenciaTotal
	FROM
		InventarioConteo ic
		LEFT JOIN Parte p ON p.ParteID = ic.ParteID AND p.Estatus = 1
		LEFT JOIN InventarioLinea il ON il.InventarioLineaID = ic.InventarioLineaID
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = il.SucursalID AND pe.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = il.LineaID AND l.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
	GROUP BY
		il.AvVuelta
		, il.AvPeriodicidad
		, il.AvManual
		, p.NumeroParte
		, p.NombreParte
		, il.LineaID
		, l.NombreLinea
		, mp.NombreMarcaParte
		, pp.Costo
GO

-- DROP VIEW InventarioLineasConteoView
CREATE VIEW InventarioLineasConteosView AS
	SELECT
		il.InventarioLineaID
		, il.SucursalID
		, il.LineaID
		, SUM(CASE WHEN pe.Existencia > 0 THEN 1 ELSE 0 END) AS PartesLinea
		, icc.Conteo
	FROM
		InventarioLinea il
		LEFT JOIN Parte p ON p.LineaID = il.LineaID AND p.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = il.SucursalID AND pe.Estatus = 1
		-- LEFT JOIN InventarioConteo ic ON ic.InventarioLineaID = il.InventarioLineaID
		LEFT JOIN (
			SELECT
				InventarioLineaID
				, SUM(CASE WHEN Valido = 1 THEN 1 ELSE 0 END) AS Conteo
			FROM InventarioConteo
			GROUP BY InventarioLineaID
		) icc ON icc.InventarioLineaID = il.InventarioLineaID
	GROUP BY
		il.InventarioLineaID
		, il.SucursalID
		, il.LineaID
		, icc.Conteo
GO

-- DROP VIEW CajaEgresosProveedoresView
CREATE VIEW CajaEgresosProveedoresView AS
	SELECT
		ce.CajaEgresoID
		, ce.Fecha
		, ce.Concepto
		, ce.Subtotal
		, ce.Iva
		, ce.Importe AS Total
		, ISNULL(SUM(ppd.Importe), 0.0) AS Usado
		, (ce.Importe - ISNULL(SUM(ppd.Importe), 0.0)) AS Restante
		, cne.ContaCuentaAuxiliarID
		, ce.Facturado
		, ce.AfectadoEnProveedores
	FROM
		CajaEgreso ce
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.CajaEgresoID = ce.CajaEgresoID AND ppd.Estatus = 1
		LEFT JOIN ContaEgreso cne ON cne.ContaEgresoID = ce.ContaEgresoID
	GROUP BY
		ce.CajaEgresoID
		, ce.Fecha
		, ce.Concepto
		, ce.Subtotal
		, ce.Iva
		, ce.Importe
		, cne.ContaCuentaAuxiliarID
		, ce.Facturado
		, ce.AfectadoEnProveedores
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

-- DROP PROCEDURE pauInventarioProgreso
CREATE PROCEDURE pauInventarioProgreso (
	@Opcion INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauInventarioProgreso 1, '2014-06-20', '2014-12-31'
	DECLARE @Opcion INT = 1
	DECLARE @Desde DATE = '2014-01-01'
	DECLARE @Hasta DATE = '2014-12-31'
	*/

	-- Definición de variables tipo constante
	DECLARE @OpRealizados INT = 1
	DECLARE @OpFuturos INT = 2
	
	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Se obtienen las línea que ya fueron contadas
	IF @Opcion = @OpRealizados BEGIN
		SELECT
			AvVuelta
			, LineaID
			, Linea
			, MAX(Partes) AS Partes
			, MAX(Costo) AS Costo
			, MAX(Dias) AS Dias
			, SUM(CASE WHEN SucursalID = 1 THEN Porcentaje ELSE 0 END) AS PorMatriz
			, SUM(CASE WHEN SucursalID = 2 THEN Porcentaje ELSE 0 END) AS PorSuc2
			, SUM(CASE WHEN SucursalID = 3 THEN Porcentaje ELSE 0 END) AS PorSuc3
			, MIN(FechaIniciado) AS FechaIniciado
			, MAX(FechaCompletado) AS FechaCompletado
		FROM (
			SELECT
				il.AvVuelta
				, il.SucursalID
				, il.LineaID
				, l.NombreLinea AS Linea
				, COUNT(p.ParteID) AS Partes
				, iuc.Capacidad
				, SUM(pe.Existencia * pp.Costo) AS Costo
				, CEILING(COUNT(p.ParteID) / CONVERT(DECIMAL, iuc.Capacidad)) AS Dias
				, ((CONVERT(DECIMAL, icc.Conteo) / COUNT(p.ParteID)) * 100) AS Porcentaje
				, il.FechaIniciado
				, il.FechaCompletado
			FROM
				InventarioLinea il
				LEFT JOIN Linea l ON l.LineaID = il.LineaID AND l.Estatus = 1
				LEFT JOIN Parte p ON p.LineaID = il.LineaID AND p.Estatus = 1
				INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = il.SucursalID AND pe.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
				LEFT JOIN (
					SELECT SucursalID, SUM(ArticulosDiarios) AS Capacidad
					FROM InventarioUsuario
					GROUP BY SucursalID
				) iuc ON iuc.SucursalID = il.SucursalID
				LEFT JOIN (
					SELECT InventarioLineaId, COUNT(*) AS Conteo
					FROM InventarioConteo
					WHERE Valido = 1
					GROUP BY InventarioLineaId
				) icc ON icc.InventarioLineaId = il.InventarioLineaID
			WHERE
				(il.FechaIniciado >= @Desde AND il.FechaIniciado < @Hasta)
				-- AND il.AvManual > 0
				AND pe.Existencia > 0
			GROUP BY
				il.AvVuelta
				, il.SucursalID
				, il.LineaID
				, l.NombreLinea
				, il.FechaIniciado
				, il.FechaCompletado
				, iuc.Capacidad
				, icc.Conteo
		) c
		GROUP BY
			AvVuelta
			, LineaID
			, Linea
		ORDER BY FechaIniciado
	END

	-- Se obtienen las líneas que no han sido contadas (Programa)
	IF @Opcion = @OpFuturos BEGIN
		SELECT
			0 AS AvVuelta
			, lc.LineaID
			, lc.Linea
			, MAX(lc.Partes) AS Partes
			, MAX(lc.Costo) AS Costo
			, MAX(lc.Dias) AS Dias
			, NULL AS PorMatriz
			, NULL AS PorSuc2
			, NULL AS PorSuc3
			, NULL AS FechaIniciado
			, NULL AS FechaCompletado
		FROM (
			SELECT
				s.SucursalID
				, l.LineaID
				, l.NombreLinea AS Linea
				, COUNT(p.ParteID) AS Partes
				, SUM(pe.Existencia * pp.Costo) AS Costo
				, CEILING(COUNT(p.ParteID) / CONVERT(DECIMAL, iuc.Capacidad)) AS Dias
			FROM
				Linea l
				LEFT JOIN Sucursal s ON s.Estatus = 1
				LEFT JOIN Parte p ON p.LineaID = l.LineaID AND p.Estatus = 1
				INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = s.SucursalID AND pe.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
				LEFT JOIN (
					SELECT SucursalID, SUM(ArticulosDiarios) AS Capacidad
					FROM InventarioUsuario
					GROUP BY SucursalID
				) iuc ON iuc.SucursalID = s.SucursalID
			WHERE
				l.Estatus = 1
				AND pe.Existencia > 0
			GROUP BY
				s.SucursalID
				, l.LineaID
				, l.NombreLinea
				, iuc.Capacidad
		) lc
		GROUP BY
			lc.LineaID
			, lc.Linea
		ORDER BY Costo DESC
	END

END
GO

-- DROP PROCEDURE pauProRespaldo
CREATE PROCEDURE pauProRespaldo AS BEGIN
	SET NOCOUNT ON
	
	DECLARE @Nombre NVARCHAR(32) = ('Theos_' + CONVERT(NVARCHAR(10), GETDATE(), 112) + '.bak')
	DECLARE @Archivo NVARCHAR(512)
	
	-- Ruta 1. Disco duro externo
	SET @Archivo = (N'E:\BackUp\' + @Nombre)
	BACKUP DATABASE [ControlRefaccionaria] TO DISK = @Archivo
	
	-- Ruta 2. Disco duro interno C:
	SET @Archivo = (N'C:\BackUp\' + @Nombre)
	BACKUP DATABASE [ControlRefaccionaria] TO DISK = @Archivo
END
GO

-- DROP PROCEDURE pauProAsignarConteoInventario
CREATE PROCEDURE pauProAsignarConteoInventario
AS BEGIN
	SET NOCOUNT ON

	DECLARE @EstGenCompletado INT = 3
	DECLARE @EstGenEnCurso INT = 6
	-- DECLARE @EstGenContado INT = 7
	DECLARE @ConfigHabilitar NVARCHAR(32) = 'Inventario.Asignacion.Habilitar'

	-- Variables calculadas para el proceso
	DECLARE @Hoy DATETIME = GETDATE()

	-- Se obtiene la lista de las líneas ordenadas
	DECLARE @Lineas TABLE (LineaID INT, Costo DECIMAL(12, 2), Procesada BIT)
	INSERT INTO @Lineas
		SELECT
			l.LineaID
			-- , SUM(pe.Existencia) AS Existencia
			, SUM(pe.Existencia * pp.Costo) AS Costo
			, 0
		FROM
			Linea l
			LEFT JOIN Parte p ON p.LineaID = l.LineaID AND p.Estatus = 1
			LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
			LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1
			-- LEFT JOIN InventarioLinea il ON il.LineaID = l.LineaID AND il.EstatusGenericoID = @EstGenCompletado
			--	AND il.InventarioVueltaID = @Vuelta
		WHERE
			l.Estatus = 1
			-- AND il.Id IS NULL
		GROUP BY l.LineaID
		ORDER BY Costo DESC

	-- Se obtiene la lista de periodicidad que aplica para el día
	DECLARE @Periodicidad TABLE (SucursalID INT, LineaID INT, Procesada BIT)
	INSERT INTO @Periodicidad
		SELECT s.SucursalID, ilp.LineaID, 0
		FROM
			InventarioLineaPeriodicidad ilp
			LEFT JOIN Sucursal s ON s.Estatus = 1
			LEFT JOIN InventarioLinea il ON il.LineaID = ilp.LineaID AND il.EstatusGenericoID = @EstGenCompletado
			-- LEFT JOIN InventarioLinea il2 ON il2.LineaID = ilp.LineaID AND il2.SucursalID = s.sucursalID
			-- 	AND ((il2.EstatusGenericoID = @EstGenEnCurso AND il2.AvPeriodicidad IS NULL) OR il2.EstatusGenericoID = @EstGenContado)
			-- Para sacar las líneas sucursal que están en curso o en esta vuelta ya fueron procesadas
			LEFT JOIN InventarioLinea il2 ON il2.LineaID = ilp.LineaID AND il2.SucursalID = s.SucursalID
				AND (il2.EstatusGenericoID = @EstGenEnCurso OR (il2.EstatusGenericoID = @EstGenCompletado AND
				il2.AvPeriodicidad >= (SELECT MAX(AvPeriodicidad) FROM InventarioLinea)))
		WHERE il2.InventarioLineaID IS NULL
		GROUP BY ilp.LineaID, ilp.Periodicidad, s.SucursalID
		HAVING (DATEDIFF(D, MAX(il.FechaCompletado), @Hoy) - ilp.Periodicidad) >= 0
		ORDER BY (DATEDIFF(D, MAX(il.FechaCompletado), @Hoy) - ilp.Periodicidad) DESC

	-- Se comienza el proceso de asignación, primero por sucursal
	DECLARE @SucursalID INT = 0
	DECLARE @UsuarioID INT = 0
	DECLARE @CantidadUsuario INT
	DECLARE @LineaID INT
	DECLARE @InvLineaId INT
	DECLARE @Insertados INT
	DECLARE @ComConteo NVARCHAR(1024)
	DECLARE @EsPeriod BIT
	DECLARE @Vuelta INT

	WHILE 1 = 1 BEGIN
		-- Se obtiene la sucursal
		SET @SucursalID = (SELECT TOP 1 SucursalID FROM Sucursal WHERE SucursalID > @SucursalID AND Estatus = 1 ORDER BY SucursalID)
		IF @SucursalID IS NULL BEGIN BREAK END
		-- Se verifica si está habilitada
		IF EXISTS(SELECT 1 FROM Configuracion WHERE Nombre = @ConfigHabilitar AND SucursalID = @SucursalID AND Valor != 'V') BEGIN
			CONTINUE
		END
		-- Se obtiene la vuelta correspondiente a la sucursal
		SET @Vuelta = (SELECT ISNULL(MAX(AvVuelta), 1) FROM InventarioLinea WHERE SucursalID = @SucursalID)
		--
		UPDATE @Lineas SET Procesada = 0
		-- Ciclo de Usuario
		WHILE 1 = 1 BEGIN
			SELECT TOP 1
				@UsuarioID = InvUsuarioID
				, @CantidadUsuario = ArticulosDiarios
			FROM InventarioUsuario
			WHERE SucursalID = @SucursalID AND InvUsuarioID > @UsuarioID
			ORDER BY InvUsuarioID
		
			IF @@ROWCOUNT = 0 BEGIN BREAK END
		
			WHILE @CantidadUsuario > 0 BEGIN

				-- Se obtiene la línea actual
				SET @LineaID = (SELECT TOP 1 LineaID FROM @Lineas WHERE Procesada = 0)
				-- Si la línea es NULL, significa que ya se acabó la vuelta
				IF @LineaID IS NULL BEGIN
					-- SET @Vuelta = (SELECT TOP 1 Id FROM InventarioVuelta WHERE Id > @Vuelta ORDER BY Id)
					-- IF @Vuelta IS NULL BEGIN
					-- 	INSERT INTO InventarioVuelta (FechaInicio) VALUES (@Hoy)
					--	SET @Vuelta = @@IDENTITY
					-- END
					SET @Vuelta = @Vuelta + 1
					-- Para asignar la línea
					UPDATE @Lineas SET Procesada = 0
					SET @LineaID = (SELECT TOP 1 LineaID FROM @Lineas WHERE Procesada = 0)
				END

				SET @EsPeriod = 0

				-- Se obtiene el InvetarioLineaId correspondiente a este conteo, si existe, si no, se crea
				SET @InvLineaId = (SELECT InventarioLineaID FROM InventarioLinea
					WHERE SucursalID = @SucursalID AND AvVuelta = @Vuelta AND LineaID = @LineaID)
				IF @InvLineaId IS NULL BEGIN
				
					-- Para lo de la periodicidad
					IF EXISTS(SELECT TOP 1 1 FROM @Periodicidad WHERE SucursalID = @SucursalID AND Procesada = 0) BEGIN
						SET @LineaID = (SELECT TOP 1 LineaID FROM @Periodicidad WHERE SucursalID = @SucursalID AND Procesada = 0)
						SET @InvLineaId = (SELECT InventarioLineaID FROM InventarioLinea
							WHERE SucursalID = @SucursalID AND LineaID = @LineaID AND AvPeriodicidad > 0 AND EstatusGenericoID != @EstGenCompletado)
						IF @InvLineaId IS NULL BEGIN
							DECLARE @AvPerSig INT = (SELECT (ISNULL(MAX(AvPeriodicidad), 0) + 1) FROM InventarioLinea
								WHERE SucursalID = @SucursalID)
							INSERT INTO InventarioLinea (SucursalID, LineaID, EstatusGenericoID, FechaIniciado, AvPeriodicidad)
								VALUES (@SucursalID, @LineaID, @EstGenEnCurso, @Hoy, @AvPerSig)
							SET @InvLineaId = @@IDENTITY
						END
						SET @EsPeriod = 1
					END
					--

					ELSE BEGIN
						INSERT INTO InventarioLinea (SucursalID, LineaID, EstatusGenericoID, AvVuelta, FechaIniciado)
							VALUES (@SucursalID, @LineaID, @EstGenEnCurso, @Vuelta, @Hoy)
						SET @InvLineaId = @@IDENTITY
					END
				END ELSE BEGIN
					-- Se valida si ya se procesó esa línea, para no volverlo a ejecutar
					IF EXISTS(SELECT 1 FROM InventarioLinea WHERE InventarioLineaID = @InvLineaId AND EstatusGenericoID != @EstGenEnCurso) BEGIN
						UPDATE @Lineas SET Procesada = 1 WHERE LineaID = @LineaID
						CONTINUE
					END
				END

				-- Se insertan las partes para el conteo
				SET @ComConteo = N'
					SELECT TOP ' + CONVERT(NVARCHAR(4), @CantidadUsuario) + '
						@InvLineaId
						, @Hoy
						, @UsuarioID
						, p.ParteID
						-- , 0
					FROM
						Parte p
						INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
						LEFT JOIN (
							SELECT DISTINCT ic.ParteID
							FROM
								InventarioConteo ic
								INNER JOIN InventarioUsuario iu ON iu.InvUsuarioID = ic.ConteoUsuarioID AND iu.SucursalID = @SucursalID
							WHERE ic.InventarioLineaId = @InvLineaId
						) icc ON icc.ParteID = p.ParteID
						-- LEFT JOIN InventarioUsuario iu ON iu.SucursalID = @SucursalID
						-- LEFT JOIN InventarioConteo ic ON ic.ParteID = p.ParteID AND ic.ConteoUsuarioID = iu.InvUsuarioID
						-- 	AND ic.InventarioLineaId = @InvLineaId
					WHERE
						p.Estatus = 1
						AND p.LineaID = @LineaID
						AND pe.Existencia > 0
						AND icc.ParteId IS NULL
					'
				INSERT INTO InventarioConteo (InventarioLineaId, Dia, ConteoUsuarioID, ParteID)
					EXEC sp_executesql @ComConteo
					, N'@CantidadUsuario INT, @InvLineaId INT, @Hoy DATETIME, @UsuarioID INT, @SucursalID INT, @LineaID INT'
					, @CantidadUsuario = @CantidadUsuario
					, @InvLineaID = @InvLineaID
					, @Hoy = @Hoy
					, @UsuarioID = @UsuarioID
					, @SucursalID = @SucursalID
					, @LineaID = @LineaID
				SET @Insertados = @@ROWCOUNT

				SET @CantidadUsuario = (@CantidadUsuario - @Insertados)

				-- Si @@RowCount es cero, significa que ya se procesaron todas las partes de la línea
				IF @Insertados = 0 BEGIN
					IF @EsPeriod = 1 BEGIN
						UPDATE @Periodicidad SET Procesada = 1 WHERE SucursalID = @SucursalID AND LineaID = @LineaID
					END ELSE BEGIN
					
						UPDATE @Lineas SET Procesada = 1 WHERE LineaID = @LineaID
					
					END
				END
			END
		END
	END

	-- Se verifica si ya se completaron los conteos de todas las sucursales, para cambiar estatus
	/* Revisar si realmente esto será necesario aquí en el procedimiento
	SET @InvLineaId = 0
	WHILE 1 = 1 BEGIN
		SELECT TOP 1
			@InvLineaId = InventarioLineaID
			, @SucursalID = SucursalID
			, @LineaID = LineaID
		FROM InventarioLinea
		WHERE EstatusGenericoID = @EstGenEnCurso AND InventarioLineaID > @InvLineaId
		ORDER BY InventarioLineaID
		IF @@ROWCOUNT = 0 BEGIN BREAK END

		IF EXISTS(
			SELECT 1 FROM (
				SELECT
					-- , COUNT(DISTINCT p.ParteID) AS Partes
					-- , COUNT(ic.ParteID) AS Contadas
					(COUNT(DISTINCT p.ParteID) - COUNT(ic.ParteID)) AS Diferencia
				FROM
					Parte p
					INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
					LEFT JOIN InventarioUsuario iu ON iu.SucursalID = @SucursalID
					LEFT JOIN InventarioConteo ic ON ic.ParteID = p.ParteID AND ic.ConteoUsuarioID = iu.InvUsuarioID
						AND ic.InventarioLineaId = @InvLineaId AND ic.Diferencia IS NOT NULL AND ic.AplicaRevision = 0
				WHERE
					p.Estatus = 1
					AND p.LineaID = @LineaID
					AND pe.Existencia > 0
				-- GROUP BY s.SucursalID
			) c
			HAVING SUM(Diferencia) <= 0
		) BEGIN
			UPDATE InventarioLinea SET EstatusGenericoID = @EstGenContado WHERE InventarioLineaID = @InvLineaId
		END
	END
	*/

END
GO

ALTER PROCEDURE [dbo].[pauVentasPartesBusqueda] (
	@SucursalID INT
	, @Codigo NVARCHAR(32) = NULL
	, @Descripcion1 NVARCHAR(32) = NULL
	, @Descripcion2 NVARCHAR(32) = NULL
	, @Descripcion3 NVARCHAR(32) = NULL
	, @Descripcion4 NVARCHAR(32) = NULL
	, @Descripcion5 NVARCHAR(32) = NULL
	-- , @Descripcion6 NVARCHAR(32) = NULL
	-- , @Descripcion7 NVARCHAR(32) = NULL
	-- , @Descripcion8 NVARCHAR(32) = NULL
	-- , @Descripcion9 NVARCHAR(32) = NULL
	, @SistemaID INT = NULL
	, @LineaID INT = NULL
	, @MarcaID INT = NULL
	
	/* , @TipoCilindroID INT = NULL
	, @Largo INT = NULL
	, @Alto INT = NULL
	, @Dientes INT = NULL
	, @Amperes INT = NULL
	, @Watts INT = NULL
	, @Diametro INT = NULL
	, @Astrias INT = NULL
	, @Terminales INT = NULL
	, @Voltios INT = NULL
	*/
	, @Caracteristicas BIT = NULL
	, @Car01 NVARCHAR(64) = NULL
	, @Car02 NVARCHAR(64) = NULL
	, @Car03 NVARCHAR(64) = NULL
	, @Car04 NVARCHAR(64) = NULL
	, @Car05 NVARCHAR(64) = NULL
	, @Car06 NVARCHAR(64) = NULL
	, @Car07 NVARCHAR(64) = NULL
	, @Car08 NVARCHAR(64) = NULL
	, @Car09 NVARCHAR(64) = NULL
	, @Car10 NVARCHAR(64) = NULL
	, @Car11 NVARCHAR(64) = NULL
	, @Car12 NVARCHAR(64) = NULL
	, @Car13 NVARCHAR(64) = NULL
	, @Car14 NVARCHAR(64) = NULL
	, @Car15 NVARCHAR(64) = NULL
	, @Car16 NVARCHAR(64) = NULL
	, @Car17 NVARCHAR(64) = NULL

	, @CodigoAlterno NVARCHAR(32) = NULL

	, @VehiculoModeloID INT = NULL -- Se debe incluir el ModeloID para que el filtro por vehículo tenga efecto
	, @VehiculoAnio INT = NULL
	, @VehiculoMotorID INT = NULL
	
	, @Equivalentes BIT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4
	DECLARE @ParteEstActivo INT = 1

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
					LEFT JOIN (
						SELECT
							pcc.ParteID
							, MAX(CASE WHEN pcc.Registro = 1 THEN pcc.Valor ELSE NULL END) AS Car01
							, MAX(CASE WHEN pcc.Registro = 2 THEN pcc.Valor ELSE NULL END) AS Car02
							, MAX(CASE WHEN pcc.Registro = 3 THEN pcc.Valor ELSE NULL END) AS Car03
							, MAX(CASE WHEN pcc.Registro = 4 THEN pcc.Valor ELSE NULL END) AS Car04
							, MAX(CASE WHEN pcc.Registro = 5 THEN pcc.Valor ELSE NULL END) AS Car05
							, MAX(CASE WHEN pcc.Registro = 6 THEN pcc.Valor ELSE NULL END) AS Car06
							, MAX(CASE WHEN pcc.Registro = 7 THEN pcc.Valor ELSE NULL END) AS Car07
							, MAX(CASE WHEN pcc.Registro = 8 THEN pcc.Valor ELSE NULL END) AS Car08
							, MAX(CASE WHEN pcc.Registro = 9 THEN pcc.Valor ELSE NULL END) AS Car09
							, MAX(CASE WHEN pcc.Registro = 10 THEN pcc.Valor ELSE NULL END) AS Car10
							, MAX(CASE WHEN pcc.Registro = 11 THEN pcc.Valor ELSE NULL END) AS Car11
							, MAX(CASE WHEN pcc.Registro = 12 THEN pcc.Valor ELSE NULL END) AS Car12
							, MAX(CASE WHEN pcc.Registro = 13 THEN pcc.Valor ELSE NULL END) AS Car13
							, MAX(CASE WHEN pcc.Registro = 14 THEN pcc.Valor ELSE NULL END) AS Car14
							, MAX(CASE WHEN pcc.Registro = 15 THEN pcc.Valor ELSE NULL END) AS Car15
							, MAX(CASE WHEN pcc.Registro = 16 THEN pcc.Valor ELSE NULL END) AS Car16
							, MAX(CASE WHEN pcc.Registro = 17 THEN pcc.Valor ELSE NULL END) AS Car17
						FROM
							(SELECT
								ROW_NUMBER() OVER (PARTITION BY pcd.ParteID ORDER BY lc.CaracteristicaID) AS Registro
								, pcd.ParteID
								, lc.CaracteristicaID
								, pc.Valor
							FROM
								(SELECT DISTINCT ParteID FROM ParteCaracteristica) pcd
								LEFT JOIN Parte p on p.ParteID = pcd.ParteID AND p.Estatus = 1
								LEFT JOIN LineaCaracteristica lc on lc.LineaID = p.LineaID
								LEFT JOIN ParteCaracteristica pc on pc.ParteID = pcd.ParteID 
									AND pc.CaracteristicaID = lc.CaracteristicaID
							) pcc
						GROUP BY pcc.ParteID
				) pcc ON pcc.ParteID = p.ParteID
					AND (@Car01 IS NULL OR pcc.Car01 LIKE '%' + @Car01 + '%')
					AND (@Car02 IS NULL OR pcc.Car02 LIKE '%' + @Car02 + '%')
					AND (@Car03 IS NULL OR pcc.Car03 LIKE '%' + @Car03 + '%')
					AND (@Car04 IS NULL OR pcc.Car04 LIKE '%' + @Car04 + '%')
					AND (@Car05 IS NULL OR pcc.Car05 LIKE '%' + @Car05 + '%')
					AND (@Car06 IS NULL OR pcc.Car06 LIKE '%' + @Car06 + '%')
					AND (@Car07 IS NULL OR pcc.Car07 LIKE '%' + @Car07 + '%')
					AND (@Car08 IS NULL OR pcc.Car08 LIKE '%' + @Car08 + '%')
					AND (@Car09 IS NULL OR pcc.Car09 LIKE '%' + @Car09 + '%')
					AND (@Car10 IS NULL OR pcc.Car10 LIKE '%' + @Car10 + '%')
					AND (@Car11 IS NULL OR pcc.Car11 LIKE '%' + @Car11 + '%')
					AND (@Car12 IS NULL OR pcc.Car12 LIKE '%' + @Car12 + '%')
					AND (@Car13 IS NULL OR pcc.Car13 LIKE '%' + @Car13 + '%')
					AND (@Car14 IS NULL OR pcc.Car14 LIKE '%' + @Car14 + '%')
					AND (@Car15 IS NULL OR pcc.Car15 LIKE '%' + @Car15 + '%')
					AND (@Car16 IS NULL OR pcc.Car16 LIKE '%' + @Car16 + '%')
					AND (@Car17 IS NULL OR pcc.Car17 LIKE '%' + @Car17 + '%')
				WHERE
					p.Estatus = 1
					AND ParteEstatusID = @ParteEstActivo
					AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
					-- AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
					-- AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
					-- AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
					-- AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
					AND (@SistemaID IS NULL OR p.SubsistemaID IN (
						SELECT SubsistemaID
						FROM Subsistema
						WHERE SistemaID = @SistemaID AND Estatus = 1
					))
					AND (@LineaID IS NULL OR p.LineaID = @LineaID)
					AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
					
					/* AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
					AND (@Largo IS NULL OR p.Largo = @Largo)
					AND (@Alto IS NULL OR p.Alto = @Alto)
					AND (@Dientes IS NULL OR p.Dientes = @Dientes)
					AND (@Amperes IS NULL OR p.Amperes = @Amperes)
					AND (@Watts IS NULL OR p.Watts = @Watts)
					AND (@Diametro IS NULL OR p.Diametro = @Diametro)
					AND (@Astrias IS NULL OR p.Astrias = @Astrias)
					AND (@Terminales IS NULL OR p.Terminales = @Terminales)
					AND (@Voltios IS NULL OR p.Voltios = @Voltios)
					*/
					AND (@Caracteristicas IS NULL OR pcc.ParteID IS NOT NULL)
					
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

			SELECT DISTINCT
				e.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
				, SUM(pe.Existencia) AS Existencia
				, SUM(CASE WHEN pe.SucursalID = @SucursalID THEN pe.Existencia ELSE 0.00 END) AS ExistenciaLocal
				-- , pic.NombreImagen AS Imagen
				-- , pic.CuentaImagenes
			FROM
				_Equivalentes e
				LEFT JOIN Parte p ON p.ParteID = e.ParteID AND p.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				/* LEFT JOIN (
					SELECT
						ParteID
						, MAX(CASE WHEN Orden = 1 THEN NombreImagen ELSE '' END) AS NombreImagen
						, COUNT(*) AS CuentaImagenes
					FROM ParteImagen
					WHERE Estatus = 1
					GROUP BY ParteID
				) pic ON pic.ParteID = p.ParteID
				*/
				-- LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
				LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
			WHERE e.Fila = 1
			GROUP BY
				e.ParteID
				, p.NumeroParte
				, p.NombreParte
				, mp.NombreMarcaParte
				, l.NombreLinea
				-- , pic.NombreImagen
				-- , pic.CuentaImagenes
		-- Si es búsqueda normal
		END ELSE BEGIN
			SELECT
				p.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
				-- , pe.Existencia
				, SUM(pe.Existencia) AS Existencia
				, SUM(CASE WHEN pe.SucursalID = @SucursalID THEN pe.Existencia ELSE 0.00 END) AS ExistenciaLocal
				-- , pic.NombreImagen AS Imagen
				-- , pic.CuentaImagenes
			FROM
				Parte p
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				-- LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
				-- LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1 AND pi.Estatus = 1
				/* LEFT JOIN (
					SELECT
						ParteID
						, MAX(CASE WHEN Orden = 1 THEN NombreImagen ELSE '' END) AS NombreImagen
						, COUNT(*) AS CuentaImagenes
					FROM ParteImagen
					WHERE Estatus = 1
					GROUP BY ParteID
				) pic ON pic.ParteID = p.ParteID
				*/
				-- LEFT JOIN ParteCodigoAlterno pca ON pca.ParteID = p.ParteID AND pca.Estatus = 1
				-- LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
				LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
				LEFT JOIN (
					SELECT
						pcc.ParteID
						, MAX(CASE WHEN pcc.Registro = 1 THEN pcc.Valor ELSE NULL END) AS Car01
						, MAX(CASE WHEN pcc.Registro = 2 THEN pcc.Valor ELSE NULL END) AS Car02
						, MAX(CASE WHEN pcc.Registro = 3 THEN pcc.Valor ELSE NULL END) AS Car03
						, MAX(CASE WHEN pcc.Registro = 4 THEN pcc.Valor ELSE NULL END) AS Car04
						, MAX(CASE WHEN pcc.Registro = 5 THEN pcc.Valor ELSE NULL END) AS Car05
						, MAX(CASE WHEN pcc.Registro = 6 THEN pcc.Valor ELSE NULL END) AS Car06
						, MAX(CASE WHEN pcc.Registro = 7 THEN pcc.Valor ELSE NULL END) AS Car07
						, MAX(CASE WHEN pcc.Registro = 8 THEN pcc.Valor ELSE NULL END) AS Car08
						, MAX(CASE WHEN pcc.Registro = 9 THEN pcc.Valor ELSE NULL END) AS Car09
						, MAX(CASE WHEN pcc.Registro = 10 THEN pcc.Valor ELSE NULL END) AS Car10
						, MAX(CASE WHEN pcc.Registro = 11 THEN pcc.Valor ELSE NULL END) AS Car11
						, MAX(CASE WHEN pcc.Registro = 12 THEN pcc.Valor ELSE NULL END) AS Car12
						, MAX(CASE WHEN pcc.Registro = 13 THEN pcc.Valor ELSE NULL END) AS Car13
						, MAX(CASE WHEN pcc.Registro = 14 THEN pcc.Valor ELSE NULL END) AS Car14
						, MAX(CASE WHEN pcc.Registro = 15 THEN pcc.Valor ELSE NULL END) AS Car15
						, MAX(CASE WHEN pcc.Registro = 16 THEN pcc.Valor ELSE NULL END) AS Car16
						, MAX(CASE WHEN pcc.Registro = 17 THEN pcc.Valor ELSE NULL END) AS Car17
					FROM
						(SELECT
							ROW_NUMBER() OVER (PARTITION BY pcd.ParteID ORDER BY lc.CaracteristicaID) AS Registro
							, pcd.ParteID
							, lc.CaracteristicaID
							, pc.Valor
						FROM
							(SELECT DISTINCT ParteID FROM ParteCaracteristica) pcd
							LEFT JOIN Parte p on p.ParteID = pcd.ParteID AND p.Estatus = 1
							LEFT JOIN LineaCaracteristica lc on lc.LineaID = p.LineaID
							LEFT JOIN ParteCaracteristica pc on pc.ParteID = pcd.ParteID 
								AND pc.CaracteristicaID = lc.CaracteristicaID
						) pcc
					GROUP BY pcc.ParteID
				) pcc ON pcc.ParteID = p.ParteID
					AND (@Car01 IS NULL OR pcc.Car01 LIKE '%' + @Car01 + '%')
					AND (@Car02 IS NULL OR pcc.Car02 LIKE '%' + @Car02 + '%')
					AND (@Car03 IS NULL OR pcc.Car03 LIKE '%' + @Car03 + '%')
					AND (@Car04 IS NULL OR pcc.Car04 LIKE '%' + @Car04 + '%')
					AND (@Car05 IS NULL OR pcc.Car05 LIKE '%' + @Car05 + '%')
					AND (@Car06 IS NULL OR pcc.Car06 LIKE '%' + @Car06 + '%')
					AND (@Car07 IS NULL OR pcc.Car07 LIKE '%' + @Car07 + '%')
					AND (@Car08 IS NULL OR pcc.Car08 LIKE '%' + @Car08 + '%')
					AND (@Car09 IS NULL OR pcc.Car09 LIKE '%' + @Car09 + '%')
					AND (@Car10 IS NULL OR pcc.Car10 LIKE '%' + @Car10 + '%')
					AND (@Car11 IS NULL OR pcc.Car11 LIKE '%' + @Car11 + '%')
					AND (@Car12 IS NULL OR pcc.Car12 LIKE '%' + @Car12 + '%')
					AND (@Car13 IS NULL OR pcc.Car13 LIKE '%' + @Car13 + '%')
					AND (@Car14 IS NULL OR pcc.Car14 LIKE '%' + @Car14 + '%')
					AND (@Car15 IS NULL OR pcc.Car15 LIKE '%' + @Car15 + '%')
					AND (@Car16 IS NULL OR pcc.Car16 LIKE '%' + @Car16 + '%')
					AND (@Car17 IS NULL OR pcc.Car17 LIKE '%' + @Car17 + '%')
			WHERE
				p.Estatus = 1
				AND ParteEstatusID = @ParteEstActivo
				AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
				AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
				AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
				AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
				AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
				-- AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
				-- AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
				-- AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
				-- AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
				
				AND (@SistemaID IS NULL OR p.SubsistemaID IN (
					SELECT SubsistemaID
					FROM Subsistema
					WHERE SistemaID = @SistemaID AND Estatus = 1
				))
				AND (@LineaID IS NULL OR p.LineaID = @LineaID)
				AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
				
				/* AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
				AND (@Largo IS NULL OR p.Largo = @Largo)
				AND (@Alto IS NULL OR p.Alto = @Alto)
				AND (@Dientes IS NULL OR p.Dientes = @Dientes)
				AND (@Amperes IS NULL OR p.Amperes = @Amperes)
				AND (@Watts IS NULL OR p.Watts = @Watts)
				AND (@Diametro IS NULL OR p.Diametro = @Diametro)
				AND (@Astrias IS NULL OR p.Astrias = @Astrias)
				AND (@Terminales IS NULL OR p.Terminales = @Terminales)
				AND (@Voltios IS NULL OR p.Voltios = @Voltios)
				*/
				AND (@Caracteristicas IS NULL OR pcc.ParteID IS NOT NULL)
				
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
			GROUP BY
				p.ParteID
				, p.NumeroParte
				, p.NombreParte
				, mp.NombreMarcaParte
				, l.NombreLinea
				-- , pe.Existencia
				-- , pic.NombreImagen
				-- , pic.CuentaImagenes
		END
	
	-- Si es búsqueda por código
	END ELSE BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte AS NumeroDeParte
			, p.NombreParte AS Descripcion
			, mp.NombreMarcaParte AS Marca
			, l.NombreLinea AS Linea
			-- , pe.Existencia
			, SUM(pe.Existencia) AS Existencia
				, SUM(CASE WHEN pe.SucursalID = @SucursalID THEN pe.Existencia ELSE 0.00 END) AS ExistenciaLocal
			-- , pi.NombreImagen AS Imagen
			-- , (SELECT COUNT(*) FROM ParteImagen WHERE ParteID = p.ParteID AND Estatus = 1) AS CuentaImagenes
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			-- LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
			-- LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
			LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
		WHERE
			p.Estatus = 1
			AND ParteEstatusID = @ParteEstActivo
			AND (NumeroParte = @Codigo OR CodigoBarra = @Codigo)
		GROUP BY
			p.ParteID
			, p.NumeroParte
			, p.NombreParte
			, mp.NombreMarcaParte
			, l.NombreLinea
			-- , pe.Existencia
			-- , pi.NombreImagen
	END

END
GO
