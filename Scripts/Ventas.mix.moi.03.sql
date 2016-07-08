/* Script con modificaciones para el módulo de ventas. Archivo 3
*/

USE ControlRefaccionaria
GO

/* *****************************************************************************
** Creación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY
	
	-- Ventas
	UPDATE VentaEstatus SET Descripcion = 'Facturada' WHERE VentaEstatusID = 4
	
	-- Búsqueda avanzada
	CREATE INDEX Ix_NombreParte ON Parte(NombreParte)
	
	-- 9500
	INSERT INTO Sistema (NombreSistema, UsuarioID, FechaRegistro) VALUES ('OTRO', 1, GETDATE())
	DECLARE @UltId INT = (SELECT @@IDENTITY)
	INSERT INTO Subsistema (SistemaID, NombreSubsistema, UsuarioID, FechaRegistro) VALUES
		(@UltId, 'OTRO', 1, GETDATE())

	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
END CATCH

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */

-- Ventas
-- Venta.SucursalID
ALTER TABLE Venta ADD SucursalID INT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
GO
UPDATE Venta SET SucursalID = 1
ALTER TABLE Venta ALTER COLUMN SucursalID INT NOT NULL
-- Venta.RealizoUsuarioID
ALTER TABLE Venta ADD RealizoUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
GO
UPDATE Venta SET RealizoUsuarioID = 1
ALTER TABLE Venta ALTER COLUMN RealizoUsuarioID INT NOT NULL
-- VentaPago.SucursalID
ALTER TABLE VentaPago ADD SucursalID INT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
GO
UPDATE VentaPago SET SucursalID = 1
ALTER TABLE VentaPago ALTER COLUMN SucursalID INT NOT NULL
-- VentaPagoDetalle.NotaDeCreditoID
ALTER TABLE VentaPagoDetalle ADD NotaDeCreditoID INT NULL FOREIGN KEY REFERENCES NotaDeCredito(NotaDeCreditoID)

-- 9500
EXEC sp_rename 'CatEstatusGenerico', 'EstatusGenerico'
EXEC sp_rename 'EstatusGenerico.EstatusID', 'EstatusGenericoID', 'COLUMN'
EXEC sp_rename 'Cotizacion9500.EstatusID', 'EstatusGenericoID', 'COLUMN'
ALTER TABLE Cotizacion9500 ALTER COLUMN Fecha DATETIME NOT NULL
ALTER TABLE Cotizacion9500 DROP COLUMN NombreDelCliente
ALTER TABLE Cotizacion9500 DROP COLUMN Celular
ALTER TABLE Cotizacion9500 DROP COLUMN Telefono
ALTER TABLE Cotizacion9500 ADD ClienteID INT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
ALTER TABLE Cotizacion9500 ADD RealizoUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
GO
UPDATE Cotizacion9500 SET ClienteID = 1
UPDATE Cotizacion9500 SET RealizoUsuarioID = 1
ALTER TABLE Cotizacion9500 ALTER COLUMN ClienteID INT NOT NULL
ALTER TABLE Cotizacion9500 ALTER COLUMN RealizoUsuarioID INT NOT NULL

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
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion AS Estatus
		, v.Subtotal
		, v.Iva
		, v.Total
		, ISNULL(SUM(vp.Importe), 0.00) AS Pagado
	FROM
		Venta v
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID AND ve.Estatus = 1
		LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
	WHERE v.Estatus = 1
	GROUP BY
		v.VentaID
		, v.Fecha
		, v.ClienteID
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion
		, v.Subtotal
		, v.Iva
		, v.Total
GO

-- Reimpresión
ALTER VIEW VentasPagosDetalleView AS
	SELECT
		vpd.VentaPagoDetalleID
		, vp.VentaID
		, vp.Fecha
		, vp.Importe AS ImporteTotal
		, c.ClienteID
		, vp.TipoPagoID AS TipoDePagoID
		, vpd.TipoFormaPagoID AS FormaDePagoID
		, vpd.Importe
		, b.NombreBanco AS Banco
		, vpd.Folio
		, vpd.Cuenta
		, vpd.NotaDeCreditoID
		, u.NombrePersona AS Vendedor
	FROM
		VentaPagoDetalle vpd
		LEFT JOIN VentaPago vp ON vp.VentaPagoID = vpd.VentaPagoID AND vp.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		-- LEFT JOIN TipoPago tp ON tp.TipoPagoID = vp.TipoPagoID AND tp.Estatus = 1
		-- LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vpd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
	WHERE vpd.Estatus = 1
GO

/* *****************************************************************************
** Crear Vistas
***************************************************************************** */

-- 9500

-- DROP VIEW Lista9500View
CREATE VIEW Lista9500View AS
	SELECT
		c9.Cotizacion9500ID AS Folio
		, c9.Fecha
		, p.NumeroParte
		, p.NombreParte
		, c9.Anticipo
		, (c.Nombre + ' ' + c.ApellidoPaterno + ' ' + c.ApellidoMaterno) AS Nombre
		, s.NombreSucursal AS Sucursal
		, v.NombrePersona AS Vendedor
	FROM
		Cotizacion9500 c9
		LEFT JOIN (
			SELECT
				Cotizacion9500ID
				, MIN(ParteID) OVER (PARTITION BY Cotizacion9500DetalleID) AS ParteID
			FROM
				Cotizacion9500Detalle c9d
			WHERE Estatus = 1
		) c9d ON c9d.Cotizacion9500ID = c9.Cotizacion9500ID
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = c9.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario v ON v.UsuarioID = c9.RealizoUsuarioID AND v.Estatus = 1
	WHERE
		c9.Estatus = 1
		AND c9.EstatusGenericoID = 2
GO

-- DROP VIEW Cotizaciones9500DetalleView
CREATE VIEW Cotizaciones9500DetalleView AS
	SELECT
		c9d.Cotizacion9500DetalleID
		, c9d.Cotizacion9500ID
		, c9d.ProveedorID
		, c9d.LineaID
		, c9d.MarcaParteID
		, c9d.ParteID
		, p.NumeroParte
		, p.NombreParte
		, c9d.Cantidad
		, c9d.Costo
		, c9d.PrecioAlCliente
	FROM
		Cotizacion9500Detalle c9d
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
	WHERE
		c9d.Estatus = 1
GO

-- Reimpresión
-- DROP VIEW VentasDetalleView
CREATE VIEW VentasDetalleView AS
	SELECT
		vd.VentaDetalleID
		, vd.VentaID
		, vd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, vd.Cantidad
		, vd.PrecioUnitario
		, vd.Iva
	FROM
		VentaDetalle vd
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
	WHERE vd.Estatus = 1
GO

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */

-- Búsqueda y búsqueda avanzada

-- DROP FUNCTION fnuDividirCadena
CREATE FUNCTION fnuDividirCadena(@Cadena NVARCHAR(1024), @Delimitador VARCHAR(4))
RETURNS @Partes TABLE (
	Parte NVARCHAR(512)
) AS BEGIN
	DECLARE @LongDelim INT = DATALENGTH(@Delimitador)
	DECLARE @Pos INT
	
	WHILE (@Cadena <> '') BEGIN
		SET @Pos = CHARINDEX(@Delimitador, @Cadena)
		IF @Pos = 0 BEGIN
			INSERT INTO @Partes VALUES (@Cadena)
			BREAK
		END ELSE BEGIN
			INSERT INTO @Partes VALUES (SUBSTRING(@Cadena, 1, @Pos - 1))
		END
		SET @Cadena = SUBSTRING(@Cadena, @Pos + @LongDelim, LEN(@Cadena))
	END
	
	RETURN
END
GO

CREATE FUNCTION fnuTextoEnCadena(@Cadena NVARCHAR(1024), @Texto NVARCHAR(1024))
RETURNS BIT
AS BEGIN
	DECLARE @Palabras TABLE (Palabra NVARCHAR(1024))
	DECLARE @Palabra NVARCHAR(1024)
	DECLARE @Coincide BIT = 1
	
	INSERT INTO @Palabras SELECT Parte FROM fnuDividirCadena(@Texto, ' ')
	
	WHILE 0 = 0 BEGIN
		SET @Palabra = (SELECT TOP 1 Palabra FROM @Palabras)
		IF @Palabra IS NULL BREAK
		DELETE TOP (1) FROM @Palabras
		IF CHARINDEX(@Palabra, @Cadena) <= 0 BEGIN
			SET @Coincide = 0
			BREAK
		END
	END
	
	RETURN @Coincide
END
GO

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
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
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
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
		WHERE NumeroParte = @Codigo
	END

END
GO