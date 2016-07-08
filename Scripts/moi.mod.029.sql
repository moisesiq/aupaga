/* Script con modificaciones a la base de datos de Theos. Archivo 029
 * Creado: 2015/08/18
 * Subido: 2015/08/20
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- DROP TABLE VentaParteAplicacion
CREATE TABLE VentaParteAplicacion (
	VentaParteAplicacion INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
	, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, ModeloID INT NULL FOREIGN KEY REFERENCES Modelo(ModeloID)
	, Anio INT NULL
	, MotorID INT NULL FOREIGN KEY REFERENCES Motor(MotorID)
	, Cantidad DECIMAL(12, 2) NOT NULL
)

CREATE NONCLUSTERED INDEX Ix_SegunPlan_BusquedaVenta ON ParteCaracteristica (Valor) INCLUDE (ParteID, CaracteristicaID)
DROP INDEX Ix_SegunPlan_BusquedaVenta ON ParteExistencia
CREATE NONCLUSTERED INDEX Ix_SegunPlan_BusquedaVenta ON ParteExistencia (Estatus) INCLUDE (ParteID, SucursalID, Existencia)
CREATE NONCLUSTERED INDEX Ix_SegunPlan_BusquedaVenta ON ParteVehiculo (ModeloID)
	INCLUDE (ParteVehiculoID, ParteID, MotorID, Anio)

/* *****************************************************************************
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
	/* , @Car01 NVARCHAR(64) = NULL
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
	*/
	, @Car01ID INT = NULL
	, @Car01Val NVARCHAR(64) = NULL
	, @Car02ID INT = NULL
	, @Car02Val NVARCHAR(64) = NULL
	, @Car03ID INT = NULL
	, @Car03Val NVARCHAR(64) = NULL
	, @Car04ID INT = NULL
	, @Car04Val NVARCHAR(64) = NULL
	, @Car05ID INT = NULL
	, @Car05Val NVARCHAR(64) = NULL
	, @Car06ID INT = NULL
	, @Car06Val NVARCHAR(64) = NULL
	, @Car07ID INT = NULL
	, @Car07Val NVARCHAR(64) = NULL
	, @Car08ID INT = NULL
	, @Car08Val NVARCHAR(64) = NULL
	, @Car09ID INT = NULL
	, @Car09Val NVARCHAR(64) = NULL
	, @Car10ID INT = NULL
	, @Car10Val NVARCHAR(64) = NULL
	, @Car11ID INT = NULL
	, @Car11Val NVARCHAR(64) = NULL
	, @Car12ID INT = NULL
	, @Car12Val NVARCHAR(64) = NULL
	, @Car13ID INT = NULL
	, @Car13Val NVARCHAR(64) = NULL
	, @Car14ID INT = NULL
	, @Car14Val NVARCHAR(64) = NULL
	, @Car15ID INT = NULL
	, @Car15Val NVARCHAR(64) = NULL
	, @Car16ID INT = NULL
	, @Car16Val NVARCHAR(64) = NULL
	, @Car17ID INT = NULL
	, @Car17Val NVARCHAR(64) = NULL

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
					/* LEFT JOIN (
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
					*/
					LEFT JOIN ParteCaracteristica pc ON pc.ParteID = p.ParteID AND pc.Valor != ''
					LEFT JOIN ParteVehiculo pv ON pv.ParteID = p.ParteID AND pv.ModeloID = @VehiculoModeloID
						AND (@VehiculoAnio IS NULL OR pv.Anio = @VehiculoAnio)
						AND (@VehiculoMotorID IS NULL OR pv.MotorID = @VehiculoMotorID)
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
					-- AND (@Caracteristicas IS NULL OR pcc.ParteID IS NOT NULL)
					AND (@Caracteristicas IS NULL OR (
						(@Car01ID IS NULL OR pc.CaracteristicaID = @Car01ID AND pc.Valor = @Car01Val)
						AND (@Car02ID IS NULL OR pc.CaracteristicaID = @Car02ID AND pc.Valor = @Car02Val)
						AND (@Car03ID IS NULL OR pc.CaracteristicaID = @Car03ID AND pc.Valor = @Car03Val)
						AND (@Car04ID IS NULL OR pc.CaracteristicaID = @Car04ID AND pc.Valor = @Car04Val)
						AND (@Car05ID IS NULL OR pc.CaracteristicaID = @Car05ID AND pc.Valor = @Car05Val)
						AND (@Car06ID IS NULL OR pc.CaracteristicaID = @Car06ID AND pc.Valor = @Car06Val)
						AND (@Car07ID IS NULL OR pc.CaracteristicaID = @Car07ID AND pc.Valor = @Car07Val)
						AND (@Car08ID IS NULL OR pc.CaracteristicaID = @Car08ID AND pc.Valor = @Car08Val)
						AND (@Car09ID IS NULL OR pc.CaracteristicaID = @Car09ID AND pc.Valor = @Car09Val)
						AND (@Car10ID IS NULL OR pc.CaracteristicaID = @Car10ID AND pc.Valor = @Car10Val)
						AND (@Car11ID IS NULL OR pc.CaracteristicaID = @Car11ID AND pc.Valor = @Car11Val)
						AND (@Car12ID IS NULL OR pc.CaracteristicaID = @Car12ID AND pc.Valor = @Car12Val)
						AND (@Car13ID IS NULL OR pc.CaracteristicaID = @Car13ID AND pc.Valor = @Car13Val)
						AND (@Car14ID IS NULL OR pc.CaracteristicaID = @Car14ID AND pc.Valor = @Car14Val)
						AND (@Car15ID IS NULL OR pc.CaracteristicaID = @Car15ID AND pc.Valor = @Car15Val)
						AND (@Car16ID IS NULL OR pc.CaracteristicaID = @Car16ID AND pc.Valor = @Car16Val)
						AND (@Car17ID IS NULL OR pc.CaracteristicaID = @Car17ID AND pc.Valor = @Car17Val)
					))
					
					AND (@CodigoAlterno IS NULL OR p.ParteID IN (
						SELECT DISTINCT ParteID
						FROM ParteCodigoAlterno
						WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
					))
					/* AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
						SELECT ParteID
						FROM ParteVehiculo
						WHERE
							TipoFuenteID <> @IdTipoFuenteMostrador
							AND ModeloID = @VehiculoModeloID
							AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
							AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
					))
					*/
					AND (@VehiculoModeloID IS NULL OR pv.ParteVehiculoID IS NOT NULL)
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
				/* LEFT JOIN (
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
				*/
				LEFT JOIN ParteCaracteristica pc ON pc.ParteID = p.ParteID AND pc.Valor != ''
				LEFT JOIN ParteVehiculo pv ON pv.ParteID = p.ParteID AND pv.ModeloID = @VehiculoModeloID
					AND (@VehiculoAnio IS NULL OR pv.Anio = @VehiculoAnio)
					AND (@VehiculoMotorID IS NULL OR pv.MotorID = @VehiculoMotorID)
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
				-- AND (@Caracteristicas IS NULL OR pcc.ParteID IS NOT NULL)
				AND (@Caracteristicas IS NULL OR (
					(@Car01ID IS NULL OR pc.CaracteristicaID = @Car01ID AND pc.Valor = @Car01Val)
					AND (@Car02ID IS NULL OR pc.CaracteristicaID = @Car02ID AND pc.Valor = @Car02Val)
					AND (@Car03ID IS NULL OR pc.CaracteristicaID = @Car03ID AND pc.Valor = @Car03Val)
					AND (@Car04ID IS NULL OR pc.CaracteristicaID = @Car04ID AND pc.Valor = @Car04Val)
					AND (@Car05ID IS NULL OR pc.CaracteristicaID = @Car05ID AND pc.Valor = @Car05Val)
					AND (@Car06ID IS NULL OR pc.CaracteristicaID = @Car06ID AND pc.Valor = @Car06Val)
					AND (@Car07ID IS NULL OR pc.CaracteristicaID = @Car07ID AND pc.Valor = @Car07Val)
					AND (@Car08ID IS NULL OR pc.CaracteristicaID = @Car08ID AND pc.Valor = @Car08Val)
					AND (@Car09ID IS NULL OR pc.CaracteristicaID = @Car09ID AND pc.Valor = @Car09Val)
					AND (@Car10ID IS NULL OR pc.CaracteristicaID = @Car10ID AND pc.Valor = @Car10Val)
					AND (@Car11ID IS NULL OR pc.CaracteristicaID = @Car11ID AND pc.Valor = @Car11Val)
					AND (@Car12ID IS NULL OR pc.CaracteristicaID = @Car12ID AND pc.Valor = @Car12Val)
					AND (@Car13ID IS NULL OR pc.CaracteristicaID = @Car13ID AND pc.Valor = @Car13Val)
					AND (@Car14ID IS NULL OR pc.CaracteristicaID = @Car14ID AND pc.Valor = @Car14Val)
					AND (@Car15ID IS NULL OR pc.CaracteristicaID = @Car15ID AND pc.Valor = @Car15Val)
					AND (@Car16ID IS NULL OR pc.CaracteristicaID = @Car16ID AND pc.Valor = @Car16Val)
					AND (@Car17ID IS NULL OR pc.CaracteristicaID = @Car17ID AND pc.Valor = @Car17Val)
				))
				
				AND (@CodigoAlterno IS NULL OR p.ParteID IN (
					SELECT DISTINCT ParteID
					FROM ParteCodigoAlterno
					WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
				))
				
				/* AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
					SELECT ParteID
					FROM ParteVehiculo
					WHERE
						TipoFuenteID <> @IdTipoFuenteMostrador
						AND ModeloID = @VehiculoModeloID
						AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
						AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
				))
				*/
				AND (@VehiculoModeloID IS NULL OR pv.ParteVehiculoID IS NOT NULL)
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