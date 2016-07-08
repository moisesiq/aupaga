/* Script con modificaciones para el módulo de ventas. Archivo 4
*/

USE ControlRefaccionaria
GO

/* *****************************************************************************
** Creación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY
	
	-- Devoluciones / Cancelaciones
	
	CREATE TABLE VentaDevolucionMotivo (
		VentaDevolucionMotivoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Descripcion NVARCHAR(32) NOT NULL

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)
	INSERT INTO VentaDevolucionMotivo (Descripcion, UsuarioID) VALUES
		('No es la pieza', 1)
		, ('No lo necesitó', 1)
		, ('Otro Cliente', 1)
		, ('Otro', 1)
	
	-- DROP TABLE VentaDevolucion
	CREATE TABLE VentaDevolucion (
		VentaDevolucionID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
		, Fecha DATETIME NOT NULL
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, MotivoID INT NOT NULL FOREIGN KEY REFERENCES VentaDevolucionMotivo(VentaDevolucionMotivoID)
		, Observacion NVARCHAR(512)
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)

	-- DROP TABLE VentaDevolucionDetalle
	CREATE TABLE VentaDevolucionDetalle (
		VentaDevolucionDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VentaDevolucionID INT NOT NULL FOREIGN KEY REFERENCES VentaDevolucion(VentaDevolucionID)
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, Cantidad INT NOT NULL
		, PrecioUnitario DECIMAL(12, 2) NOT NULL
		, Iva DECIMAL(12, 2) NOT NULL

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)

	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
END CATCH

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */

-- Ventas
ALTER TABLE Venta DROP COLUMN Subtotal
ALTER TABLE Venta DROP COLUMN Iva
ALTER TABLE Venta DROP COLUMN Total

GO

/* *****************************************************************************
** Modificar Vistas
***************************************************************************** */

-- Ventas
ALTER VIEW VentasView AS
	SELECT
		v.VentaID
		, v.Fecha
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion AS Estatus
		, SUM(vd.PrecioUnitario * vd.Cantidad) AS Subtotal
		, SUM(vd.Iva * vd.Cantidad) AS Iva
		, SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Total
		, ISNULL(SUM(vp.Importe), 0.00) AS Pagado
	FROM
		Venta v
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID AND ve.Estatus = 1
		LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID
	WHERE v.Estatus = 1
	GROUP BY
		v.VentaID
		, v.Fecha
		, v.ClienteID
		, c.Nombre
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion
GO

/* *****************************************************************************
** Crear Vistas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

-- Búsqueda y búsqueda avanzada
ALTER PROCEDURE pauParteBusquedaAvanzada (
	@Codigo NVARCHAR(32) = NULL
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
			, l.NombreLinea
			, mp.NombreMarcaParte
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1 AND pi.Estatus = 1
		WHERE
			(@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
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
				WHERE SistemaID = @SistemaID
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
			, l.NombreLinea
			, mp.NombreMarcaParte
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
		WHERE NumeroParte = @Codigo
	END

END
GO