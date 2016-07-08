/* Script con modificaciones a la base de datos de Theos. Archivo 059
 * Creado: 2016/01/04
 * Subido: 2016/01/05
 */

DECLARE @ScriptID INT = 59
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- DROP TABLE ParteCompletado
CREATE TABLE ParteCompletado (
	ParteCompletadoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, Fotos BIT NOT NULL
	, Equivalentes BIT NOT NULL
	, Aplicaciones BIT NOT NULL
	, Alternos BIT NOT NULL
	, Complementarios BIT NOT NULL
	, Caracteristicas BIT NOT NULL
)

INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
	('Administracion.CatalogosPartes.Validar', 'No tienes permisos para validar Partes.')

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[PartesAvancesView] AS
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
		, p.FechaRegistro
		, (SELECT 0) AS Fotos
		-- , (SELECT TOP 1 1 FROM ParteImagen WHERE ParteID = p.ParteID AND Estatus = 1) AS Fotos
		, (SELECT TOP 1 1 FROM ParteEquivalente WHERE GrupoID = (SELECT GrupoID FROM ParteEquivalente
			WHERE ParteID = p.ParteID)) AS Equivalentes
		, (SELECT TOP 1 1 FROM ParteVehiculo WHERE ParteID = p.ParteID) AS Aplicaciones
		, (SELECT TOP 1 1 FROM ParteCodigoAlterno WHERE ParteID = p.ParteID) AS Alternos
		, (SELECT TOP 1 1 FROM ParteComplementaria WHERE ParteID = p.ParteID) AS Complementarios
		, (SELECT TOP 1 1 FROM ParteCaracteristica WHERE ParteID = p.ParteID) AS Caracteristicas
		, pcm.Fotos AS FotosVal
		, pcm.Equivalentes AS EquivalentesVal
		, pcm.Aplicaciones AS AplicacionesVal
		, pcm.Alternos AS AlternosVal
		, pcm.Complementarios AS ComplementariosVal
		, pcm.Caracteristicas AS CaracteristicasVal
		, CONVERT(BIT, (CASE WHEN (pcm.Fotos = 1 AND pcm.Equivalentes = 1 AND pcm.Aplicaciones = 1 AND pcm.Alternos = 1
			AND pcm.Complementarios = 1 AND pcm.Caracteristicas = 1) THEN 1 ELSE 0 END)) AS Validado
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteCompletado pcm ON pcm.ParteID = p.ParteID
	WHERE p.Estatus = 1

	/* SELECT
		pc.*
		, pv.NombreProveedor AS Proveedor
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
	FROM
		(
			SELECT
				p.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, p.ProveedorID
				, p.MarcaParteID
				, p.LineaID
				, p.FechaRegistro
				, (SELECT 0) AS Fotos
				-- , (SELECT TOP 1 1 FROM ParteImagen WHERE ParteID = p.ParteID AND Estatus = 1) AS Fotos
				, (SELECT TOP 1 1 FROM ParteEquivalente WHERE GrupoID = (SELECT GrupoID FROM ParteEquivalente
					WHERE ParteID = p.ParteID)) AS Equivalentes
				, (SELECT TOP 1 1 FROM ParteVehiculo WHERE ParteID = p.ParteID AND Estatus = 1) AS Aplicaciones
				, (SELECT TOP 1 1 FROM ParteCodigoAlterno WHERE ParteID = p.ParteID) AS Alternos
				, (SELECT TOP 1 1 FROM ParteComplementaria WHERE ParteID = p.ParteID) AS Complementarios
				, (SELECT TOP 1 1 FROM ParteCaracteristica WHERE ParteID = p.ParteID) AS Caracteristicas
			FROM Parte p
			WHERE p.Estatus = 1
		) pc
		LEFT JOIN Proveedor pv ON pv.ProveedorID = pc.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = pc.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = pc.LineaID AND l.Estatus = 1
		LEFT JOIN ParteCompletado pcm ON pcm.ParteID = pc.ParteID
		*/
GO

ALTER VIEW [dbo].[PartesView] AS
	SELECT -- TOP 1000000000000
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		/* ,MAX(CASE 
		WHEN (Linea.Alto = 1 AND Parte.Alto IS NULL) OR 
			(Linea.Diametro = 1 AND Parte.Diametro IS NULL) OR
			(Linea.Largo = 1 AND Parte.Largo IS NULL) OR
			(Linea.Dientes = 1 AND Parte.Dientes IS NULL) OR
			(Linea.Astrias = 1 AND Parte.Astrias IS NULL) OR
			(Linea.Sistema = 1 AND Parte.ParteSistemaID IS NULL) OR	
			(Linea.Amperaje = 1 AND Parte.Amperes IS NULL) OR
			(Linea.Voltaje = 1 AND Parte.Voltios IS NULL) OR
			(Linea.Watts = 1 AND Parte.Watts IS NULL) OR
			(Linea.Ubicacion = 1 AND Parte.ParteUbicacionID IS NULL) OR
			(Linea.Terminales = 1 AND Parte.Terminales IS NULL) THEN 'SI' ELSE 'NO' END) AS FaltanCaracteristicas
		*/
		-- , '' AS FaltanCaracteristicas
		, p.ParteEstatusID
		, p.Es9500
		-- , MAX(CASE WHEN Parte.ParteID = ParteVehiculo.ParteID THEN 'SI' ELSE 'NO' END) AS Aplicacion
		-- , MAX(CASE WHEN Parte.ParteID = ParteEquivalente.ParteID THEN 'SI' ELSE 'NO' END) AS Equivalente
		, p.FechaRegistro
		, p.NumeroParte
			+ p.NombreParte
			+ mp.NombreMarcaParte
			+ l.NombreLinea AS Busqueda
	FROM 
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1 
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		-- LEFT JOIN ParteEstatus pe ON pe.ParteEstatusID = p.ParteEstatusID AND pe.Estatus = 1
		-- LEFT JOIN ParteVehiculo pa ON pa.ParteID = p.ParteID AND pa.Estatus = 1
		LEFT JOIN ParteEquivalente pe ON pe.ParteID = p.ParteID
	WHERE p.Estatus = 1
	/* GROUP BY
		Parte.ParteID
		,Parte.NumeroParte 
		,Parte.NombreParte
		,MarcaParte.NombreMarcaParte
		,ParteEstatus.ParteEstatusID
		,Linea.NombreLinea
		,Parte.LineaID
		,Parte.FechaRegistro
	ORDER BY p.NombreParte
	*/
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

