/* Script con modificaciones a la base de datos de Theos. Archivo 054
 * Creado: 2015/12/10
 * Subido: 2015/12/10
 */

DECLARE @ScriptID INT = 54
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
** Crear / modificar Vista
***************************************************************************** */

GO



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauVentasPartesBusqueda] (
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
	
	, @Caracteristicas BIT = 0
	, @Car01ID INT = NULL
	, @Car01 NVARCHAR(64) = NULL
	, @Car02ID INT = NULL
	, @Car02 NVARCHAR(64) = NULL
	, @Car03ID INT = NULL
	, @Car03 NVARCHAR(64) = NULL
	, @Car04ID INT = NULL
	, @Car04 NVARCHAR(64) = NULL
	, @Car05ID INT = NULL
	, @Car05 NVARCHAR(64) = NULL
	, @Car06ID INT = NULL
	, @Car06 NVARCHAR(64) = NULL
	, @Car07ID INT = NULL
	, @Car07 NVARCHAR(64) = NULL
	, @Car08ID INT = NULL
	, @Car08 NVARCHAR(64) = NULL
	, @Car09ID INT = NULL
	, @Car09 NVARCHAR(64) = NULL
	, @Car10ID INT = NULL
	, @Car10 NVARCHAR(64) = NULL
	, @Car11ID INT = NULL
	, @Car11 NVARCHAR(64) = NULL
	, @Car12ID INT = NULL
	, @Car12 NVARCHAR(64) = NULL
	, @Car13ID INT = NULL
	, @Car13 NVARCHAR(64) = NULL
	, @Car14ID INT = NULL
	, @Car14 NVARCHAR(64) = NULL
	, @Car15ID INT = NULL
	, @Car15 NVARCHAR(64) = NULL
	, @Car16ID INT = NULL
	, @Car16 NVARCHAR(64) = NULL
	, @Car17ID INT = NULL
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

	-- Se verifica si es por código o 
	DECLARE @Partes TABLE (ParteID INT NOT NULL)
	IF @Codigo IS NULL BEGIN
		-- Se hace el filtro inicial
		INSERT INTO @Partes
		SELECT TOP 201 p.ParteID
		FROM
			Parte p
			LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
			LEFT JOIN ParteCodigoAlterno pca ON pca.ParteID = p.ParteID -- AND pca.Estatus = 1
			-- LEFT JOIN ParteVehiculo pv ON pv.ParteID = p.ParteID
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		WHERE
			p.Estatus = 1
			AND ParteEstatusID = @ParteEstActivo
			AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
			/* AND (@SistemaID IS NULL OR p.SubsistemaID IN (
				SELECT SubsistemaID
				FROM Subsistema
				WHERE SistemaID = @SistemaID AND Estatus = 1
			))
			*/
			AND (@SistemaID IS NULL OR ss.SistemaID = @SistemaID)
			AND (@LineaID IS NULL OR p.LineaID = @LineaID)
			AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
			/* AND (@CodigoAlterno IS NULL OR p.ParteID IN (
				SELECT DISTINCT ParteID
				FROM ParteCodigoAlterno
				WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
			))
			*/
			AND (@CodigoAlterno IS NULL OR pca.CodigoAlterno LIKE '%' + @CodigoAlterno + '%')
			AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
				SELECT DISTINCT ParteID
				FROM ParteVehiculo
				WHERE
					ModeloID = @VehiculoModeloID
					AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
					AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
					-- AND TipoFuenteID != @IdTipoFuenteMostrador
			))
			/* AND (@VehiculoModeloID IS NULL OR pv.ModeloID = @VehiculoModeloID)
			AND (@VehiculoAnio IS NULL OR pv.Anio = @VehiculoAnio)
			AND (@VehiculoMotorID IS NULL OR pv.MotorID = @VehiculoMotorID)
			*/
			
			AND (@Caracteristicas = 0 OR p.ParteID IN (
				SELECT DISTINCT ParteID
				FROM ParteCaracteristica
				WHERE
					(@Car01 IS NULL OR CaracteristicaID = @Car01ID AND Valor = @Car01)
					OR (@Car02 IS NULL OR CaracteristicaID = @Car02ID AND Valor = @Car02)
					OR (@Car03 IS NULL OR CaracteristicaID = @Car03ID AND Valor = @Car03)
					OR (@Car04 IS NULL OR CaracteristicaID = @Car04ID AND Valor = @Car04)
					OR (@Car05 IS NULL OR CaracteristicaID = @Car05ID AND Valor = @Car05)
					OR (@Car06 IS NULL OR CaracteristicaID = @Car06ID AND Valor = @Car06)
					OR (@Car07 IS NULL OR CaracteristicaID = @Car07ID AND Valor = @Car07)
					OR (@Car08 IS NULL OR CaracteristicaID = @Car08ID AND Valor = @Car08)
					OR (@Car09 IS NULL OR CaracteristicaID = @Car09ID AND Valor = @Car09)
					OR (@Car10 IS NULL OR CaracteristicaID = @Car10ID AND Valor = @Car10)
					OR (@Car11 IS NULL OR CaracteristicaID = @Car11ID AND Valor = @Car11)
					OR (@Car12 IS NULL OR CaracteristicaID = @Car12ID AND Valor = @Car12)
					OR (@Car13 IS NULL OR CaracteristicaID = @Car13ID AND Valor = @Car13)
					OR (@Car14 IS NULL OR CaracteristicaID = @Car14ID AND Valor = @Car14)
					OR (@Car15 IS NULL OR CaracteristicaID = @Car15ID AND Valor = @Car15)
					OR (@Car16 IS NULL OR CaracteristicaID = @Car16ID AND Valor = @Car16)
					OR (@Car17 IS NULL OR CaracteristicaID = @Car17ID AND Valor = @Car17)
				)
			)
		
		-- Se verifica el número de filas encontradas, si son más de 200, se sale
		IF (SELECT COUNT(*) FROM @Partes) > 200 BEGIN
			-- Se selecciona un registro vacío, sólo para mandar la estructura
			SELECT
				CONVERT(INT, 0) AS ParteID
				, CONVERT(NVARCHAR(64), '') AS NumeroDeParte
				, CONVERT(NVARCHAR(512), '* Se encontraron más de 200 resultados. Debes ser más específico.') AS Descripcion
				, CONVERT(NVARCHAR(128), '') AS Marca
				, CONVERT(NVARCHAR(128), '') AS Linea
				, CONVERT(DECIMAL(12, 2), 0) AS Existencia
				, CONVERT(DECIMAL(12, 2), 0) AS ExistenciaLocal
			RETURN
		END
			
		-- Se filtran las características, si aplica
		/* IF @Caracteristicas = 1 BEGIN
			DELETE FROM @Partes WHERE ParteID NOT IN (
				SELECT p.ParteID
				FROM
					@Partes p
					INNER JOIN ParteCaracteristica pc ON pc.ParteID = p.ParteID
				WHERE
					(@Car01 IS NULL OR CaracteristicaID = @Car01ID AND Valor = @Car01)
					OR (@Car02 IS NULL OR CaracteristicaID = @Car02ID AND Valor = @Car02)
					OR (@Car03 IS NULL OR CaracteristicaID = @Car03ID AND Valor = @Car03)
					OR (@Car04 IS NULL OR CaracteristicaID = @Car04ID AND Valor = @Car04)
					OR (@Car05 IS NULL OR CaracteristicaID = @Car05ID AND Valor = @Car05)
					OR (@Car06 IS NULL OR CaracteristicaID = @Car06ID AND Valor = @Car06)
					OR (@Car07 IS NULL OR CaracteristicaID = @Car07ID AND Valor = @Car07)
					OR (@Car08 IS NULL OR CaracteristicaID = @Car08ID AND Valor = @Car08)
					OR (@Car09 IS NULL OR CaracteristicaID = @Car09ID AND Valor = @Car09)
					OR (@Car10 IS NULL OR CaracteristicaID = @Car10ID AND Valor = @Car10)
					OR (@Car11 IS NULL OR CaracteristicaID = @Car11ID AND Valor = @Car11)
					OR (@Car12 IS NULL OR CaracteristicaID = @Car12ID AND Valor = @Car12)
					OR (@Car13 IS NULL OR CaracteristicaID = @Car13ID AND Valor = @Car13)
					OR (@Car14 IS NULL OR CaracteristicaID = @Car14ID AND Valor = @Car14)
					OR (@Car15 IS NULL OR CaracteristicaID = @Car15ID AND Valor = @Car15)
					OR (@Car16 IS NULL OR CaracteristicaID = @Car16ID AND Valor = @Car16)
					OR (@Car17 IS NULL OR CaracteristicaID = @Car17ID AND Valor = @Car17)
			)
		
		END
		*/

		-- Se obtienen los equivalente, si aplica
		IF @Equivalentes = 1 BEGIN
			DECLARE @ParaEquiv TABLE (ParteID INT NOT NULL)
			INSERT INTO @ParaEquiv SELECT * FROM @Partes
			DELETE FROM @Partes

			INSERT INTO @Partes
			SELECT DISTINCT ParteID FROM (
				SELECT
					ISNULL(pe.ParteID, p.ParteID) AS ParteID
					, p.GrupoID
					, ROW_NUMBER() OVER(PARTITION BY p.GrupoID ORDER BY 
						CASE WHEN pex.Existencia > 0 THEN 1 ELSE 2 END
						, pp.PrecioUno DESC
					) AS Fila
				FROM
					(
						SELECT p.ParteID, pe.GrupoID
						FROM @ParaEquiv p left JOIN ParteEquivalente pe ON pe.ParteID = p.ParteID
					) p
					LEFT JOIN ParteEquivalente pe ON pe.GrupoID = p.GrupoID
					LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
					LEFT JOIN ParteExistencia pex ON pex.ParteID = pe.ParteID 
						AND pex.SucursalID = @SucursalID AND pex.Estatus = 1
			) c
			WHERE (c.GrupoID IS NULL OR c.Fila = 1)
		END
	END ELSE BEGIN
		INSERT INTO @Partes
		SELECT ParteID
		FROM Parte p
		WHERE
			p.Estatus = 1
			AND ParteEstatusID = @ParteEstActivo
			AND (NumeroParte = @Codigo OR CodigoBarra = @Codigo)
	END

	-- Se realiza la consulta final, con todos los campos necesarios
	SELECT
		pc.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, SUM(pe.Existencia) AS Existencia
		, SUM(CASE WHEN pe.SucursalID = @SucursalID THEN pe.Existencia ELSE 0.00 END) AS ExistenciaLocal
	FROM
		@Partes pc
		LEFT JOIN Parte p ON p.ParteID = pc.ParteID AND p.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = pc.ParteID AND pe.Estatus = 1
	GROUP BY
		pc.ParteID
		, p.NumeroParte
		, p.NombreParte
		, mp.NombreMarcaParte
		, l.NombreLinea

END
GO
