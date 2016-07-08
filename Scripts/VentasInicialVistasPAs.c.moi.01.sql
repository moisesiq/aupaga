/* Script inicial para crear Vistas y Procedimientos almacenados relacionados con el 
   módulo de punto de venta.
*/

USE ControlRefaccionaria
GO

-- 0. Venta

-- DROP VIEW PartesVentasView
CREATE VIEW PartesVentasView AS
	SELECT
		p.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.EsServicio
		, p.Es9500
		, mp.NombreMarcaParte AS Marca
		, pp.PrecioUno
		, pp.PrecioDos
		, pp.PrecioTres
		, pp.PrecioCuatro
		, pp.PrecioCinco
		, pe1.Existencia AS ExistenciaSuc01
		, pe2.Existencia AS ExistenciaSuc02
		, pe3.Existencia AS ExistenciaSuc03
	FROM
		Parte p
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
		LEFT JOIN ParteExistencia pe1 ON pe1.ParteID = p.ParteID AND pe1.SucursalID = 1
			AND pe1.Estatus = 1
		LEFT JOIN ParteExistencia pe2 ON pe2.ParteID = p.ParteID AND pe2.SucursalID = 2
			AND pe2.Estatus = 1
		LEFT JOIN ParteExistencia pe3 ON pe3.ParteID = p.ParteID AND pe3.SucursalID = 3
			AND pe3.Estatus = 1
	WHERE
		p.Estatus = 1
GO

-- 1. Indentificar vehículo

-- Va con el punto 6

-- 4. Búsqueda y búsqueda avanzada

-- DROP PROCEDURE pauParteBusquedaAvanzada
CREATE PROCEDURE pauParteBusquedaAvanzada (
	@Codigo NVARCHAR(32) = NULL
	, @Descripcion NVARCHAR(256) = NULL
	, @SistemaID INT = NULL
	, @LineaID INT = NULL
	, @MarcaID INT = NULL
	, @Cilindros INT = NULL
	, @Largo INT = NULL
	, @Alto INT = NULL
	, @Dientes INT = NULL
	, @Amperes INT = NULL
	, @Watts INT = NULL
	, @Diametro INT = NULL
	, @Astrias INT = NULL
	, @Terminales INT = NULL
	, @Voltios INT = NULL

	, @VehiculoAnio INT = NULL
	, @VehiculoMotorID INT = NULL  -- Se debe incluir el MotorID forsozamente para que el filtro por vehículo tenga efecto
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4

	IF @Codigo IS NULL BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte
			, p.NombreParte
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
		WHERE
			(@Descripcion IS NULL OR p.NombreParte LIKE '%' + @Descripcion + '%')
			AND (@SistemaID IS NULL OR p.SubsistemaID IN (
				SELECT SubsistemaID
				FROM Subsistema
				WHERE SistemaID = @SistemaID
				))
			AND (@LineaID IS NULL OR p.LineaID = @LineaID)
			AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
			-- AND (@Cilindros IS NULL OR p.Cilindros = @Cilindros)
			AND (@Largo IS NULL OR p.Largo = @Largo)
			AND (@Alto IS NULL OR p.Alto = @Alto)
			AND (@Dientes IS NULL OR p.Dientes = @Dientes)
			AND (@Amperes IS NULL OR p.Amperes = @Amperes)
			AND (@Watts IS NULL OR p.Watts = @Watts)
			AND (@Diametro IS NULL OR p.Diametro = @Diametro)
			AND (@Astrias IS NULL OR p.Astrias = @Astrias)
			AND (@Terminales IS NULL OR p.Terminales = @Terminales)
			AND (@Voltios IS NULL OR p.Voltios = @Voltios)
			
			AND (@VehiculoMotorID IS NULL OR p.ParteID IN (
				SELECT ParteID
				FROM ParteVehiculo
				WHERE
					TipoFuenteID <> @IdTipoFuenteMostrador
					AND MotorID = @VehiculoMotorID
					AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
				))
	END ELSE BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte
			, p.NombreParte
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
		WHERE NumeroParte = @Codigo
	END

END
GO

-- 6. Apliciones a vehículos

-- DROP VIEW ModelosAniosView
CREATE VIEW ModelosAniosView AS
	SELECT DISTINCT
		m.ModeloID
		, ma.Anio
	FROM
		Motor m
		INNER JOIN MotorAnio ma ON ma.MotorID = m.MotorID
	WHERE
		m.Estatus = 1
		AND ma.Estatus = 1
GO

-- DROP VIEW MotoresBasicoView
CREATE VIEW MotoresBasicoView AS
	SELECT
		m.MotorID
		, m.ModeloID
		, m.NombreMotor
		, MIN(ma.Anio) AS AnioInicial
		, MAX(ma.Anio) AS AnioFinal
	FROM
		Motor m
		INNER JOIN MotorAnio ma ON ma.MotorID = m.MotorID
	WHERE
		m.Estatus = 1
		AND ma.Estatus = 1
	GROUP BY
		m.MotorID
		, m.ModeloID
		, m.NombreMotor
GO

-- DROP VIEW PartesVehiculosBasicoView
CREATE VIEW PartesVehiculosBasicoView AS
	SELECT
		pv.ParteVehiculoID
		, pv.ParteID
		, p.NumeroParte
		, p.NombreParte
		, mr.NombreMarca AS Marca
		, md.NombreModelo AS Modelo
		, mt.NombreMotor AS Motor
		, pv.Anio
		, pv.TipoFuenteID
		, tf.NombreTipoFuente AS Fuente
	FROM
		ParteVehiculo pv
		LEFT JOIN Parte p ON p.ParteID = pv.ParteID AND p.Estatus = 1
		LEFT JOIN Motor mt ON mt.MotorID = pv.MotorID AND mt.Estatus = 1
		LEFT JOIN Modelo md ON md.ModeloID = mt.ModeloID AND md.Estatus = 1
		LEFT JOIN Marca mr ON mr.MarcaID = md.MarcaID AND mr.Estatus = 1
		LEFT JOIN TipoFuente tf ON tf.TipoFuenteID = pv.TipoFuenteID AND tf.Estatus = 1
	WHERE
		pv.Estatus = 1
GO

-- 7. ePoints

-- Pendiente. A ser analizado por Isidro

-- 8. Equivalentes

-- 9. Cotizaciones (Condiciones proveedores)

-- Va con punto 11

-- 10. Alta de Clientes

-- DROP VIEW ClientesDatosView
CREATE VIEW ClientesDatosView AS
	SELECT
		c.ClienteID
		, RTRIM(c.Nombre + ' ' + ISNULL(c.ApellidoPaterno, '') + ' ' + ISNULL(c.ApellidoMaterno, '')) AS Nombre
		, c.Calle + ' ' + c.NumeroExterior
			+ CASE WHEN ISNULL(c.NumeroInterior, '') = '' THEN '' ELSE ' - ' END
			+ ISNULL(c.NumeroInterior, '') AS Direccion
		, c.Colonia
		, c.CodigoPostal
		, c.Ciudad
		, c.Telefono
		, c.Celular
		, c.RFC
		
		, c.ListaDePrecios
		, c.TieneCredito
		, c.ePoints
	FROM
		Cliente c
	WHERE
		c.Estatus = 1
GO

-- 12. Flotilla del Cliente

-- DROP VIEW ClientesFlotillasView
CREATE VIEW ClientesFlotillasView AS
	SELECT
		cf.ClienteFlotillaID
		, cf.ClienteID
		, cf.NumeroEconomico
		, cf.MotorID
		, cf.Anio
		, m.NombreMotor
		, md.NombreModelo
		, mr.NombreMarca
	FROM
		ClienteFlotilla cf
		LEFT JOIN Motor m ON m.MotorID = cf.MotorID
		LEFT JOIN Modelo md ON md.ModeloID = m.ModeloID
		LEFT JOIN Marca mr ON mr.MarcaID = md.MarcaID
	WHERE
		cf.Estatus = 1
GO