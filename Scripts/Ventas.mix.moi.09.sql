/* Script con modificaciones para el módulo de ventas. Archivo 9
 * Creado: 
 * Subido: 2013/02/02
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	EXEC sp_rename 'AutorizacionProceso.Descricpion', 'Descripcion', 'COLUMN'
	
	CREATE INDEX Ix_CodigoBarra ON Parte(CodigoBarra)

	ALTER TABLE Configuracion ALTER COLUMN Descripcion NVARCHAR(512)
	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Reportes.VentaTicket.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de venta (D - Diseño, P - Pantalla, I - Impresora).')
		, ('Reportes.VentaCotizacion.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de cotización de una venta (D - Diseño, P - Pantalla, I - Impresora).')
		, ('Reportes.9500Ticket.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de un 9500 (D - Diseño, P - Pantalla, I - Impresora).')
		, ('Reportes.VentaDevolucionTicket.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de una devolución / cancelación (D - Diseño, P - Pantalla, I - Impresora).')

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
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

-- DROP VIEW AutorizacionesView
CREATE VIEW AutorizacionesView AS
	SELECT
		a.AutorizacionID
		, a.AutorizacionProcesoID
		, ap.Descripcion AS Tipo
		, a.FechaRegistro AS Fecha
		, a.Tabla
		, a.TablaRegistroID
		, a.Autorizado
		, a.FechaAutorizo
		, a.AutorizoUsuarioID
		, u.NombrePersona AS Autorizo
	FROM
		Autorizacion a
		LEFT JOIN AutorizacionProceso ap ON ap.AutorizacionProcesoID = a.AutorizacionProcesoID AND ap.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = a.AutorizoUsuarioID AND u.Estatus = 1
	WHERE a.Estatus = 1
GO

ALTER VIEW Cotizaciones9500View AS
	SELECT
		c9.Cotizacion9500ID
		, c9.EstatusGenericoID
		, c9.Fecha
		, c9.SucursalID
		, (SELECT SUM(PrecioAlCliente) FROM Cotizacion9500Detalle 
			WHERE Cotizacion9500ID = c9.Cotizacion9500ID AND Estatus = 1) AS Total
		, c9.Anticipo
		, c9.ClienteID
		, c.Nombre AS Cliente
		, u.NombrePersona AS Vendedor
	FROM
		Cotizacion9500 c9
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = c9.RealizoUsuarioID AND u.Estatus = 1
	WHERE c9.Estatus = 1
GO

CREATE VIEW CajaEfectivoPorDiaView AS
	SELECT
		ce.CajaEfectivoPorDiaID
		, ce.Dia
		, ce.Inicio
		, ce.InicioUsuarioID
		, ui.NombrePersona AS UsuarioInicio
		, ce.Cierre
		, ce.CierreUsuarioID
		, uc.NombrePersona AS UsuarioCierre
	FROM
		CajaEfectivoPorDia ce
		LEFT JOIN Usuario ui ON ui.UsuarioID = ce.InicioUsuarioID AND ui.Estatus = 1
		LEFT JOIN Usuario uc ON uc.UsuarioID = ce.CierreUsuarioID AND uc.Estatus = 1
GO

CREATE VIEW ClientesCreditoView AS
	SELECT
		c.ClienteID
		, c.Nombre
		, c.TieneCredito
		, c.Credito
		, c.DiasDeCredito
		, c.Tolerancia
		, ca.Adeudo
		, ca.FechaPrimerAdeudo
		, CASE WHEN (ca.Adeudo >= c.Credito OR ca.FechaPrimerAdeudo >= (GETDATE() + c.DiasDeCredito))
			THEN 1 ELSE 0 END AS CreditoVencido
	FROM
		Cliente c
		LEFT JOIN (
			SELECT
				ClienteID
				, MIN(Fecha) AS FechaPrimerAdeudo
				, SUM(Total - Pagado) AS Adeudo
			FROM VentasView
			WHERE ACredito = 1 AND (Total - Pagado) > 0 AND VentaEstatusID = 2
			GROUP BY ClienteID
		) ca ON ca.ClienteID = c.ClienteID
GO

ALTER VIEW NotasDeCreditoView AS
	SELECT
		nc.NotaDeCreditoID
		, nc.FechaDeEmision
		, nc.Importe
		, nc.ClienteID
		, c.Nombre AS Cliente
		, nc.Valida
		, nc.FechaDeUso
		, nc.Observacion
		, u.NombrePersona AS Autorizo
		, v1.Folio AS OrigenVentaFolio
		, v2.Folio AS UsoVentaFolio
	FROM
		NotaDeCredito nc
		LEFT JOIN Autorizacion a ON a.Tabla = 'NotaDeCredito' AND a.TablaRegistroID = nc.NotaDeCreditoID AND a.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = nc.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = a.UsuarioID AND u.Estatus = 1
		LEFT JOIN Venta v1 ON v1.VentaID = nc.OrigenVentaID AND v1.Estatus = 1
		LEFT JOIN Venta v2 ON v2.VentaID = nc.UsoVentaID AND v2.Estatus = 1
GO

ALTER VIEW VentasCambiosView AS
	SELECT
		vc.VentaCambioID
		, vc.VentaPagoID
		, v.VentaID
		, v.Folio AS VentaFolio
		, vc.Fecha
		, c.Nombre AS Cliente
		, vc.FormasDePagoAntes
		, vc.FormasDePagoDespues
		, vc.RealizoIDAntes
		, ua.NombreUsuario AS VendedorAntes
		, vc.RealizoIDDespues
		, ud.NombreUsuario AS VendedorDespues
		, vc.ComisionistaIDAntes
		, cca.Alias AS ComisionistaAntes
		, vc.ComisionistaIDDespues
		, ccd.Alias AS ComisionistaDespues
	FROM
		VentaCambio vc
		LEFT JOIN VentaPago vp ON vp.VentaPagoID = vc.VentaPagoID AND vp.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = vc.RealizoIDAntes AND ua.Estatus = 1
		LEFT JOIN Usuario ud ON ud.UsuarioID = vc.RealizoIDDespues AND ud.Estatus = 1
		LEFT JOIN Cliente cca ON cca.ClienteID = vc.ComisionistaIDAntes AND cca.Estatus = 1
		LEFT JOIN Cliente ccd ON ccd.ClienteID = vc.ComisionistaIDDespues AND ccd.Estatus = 1
	WHERE vc.Estatus = 1
GO

ALTER VIEW ClientesDatosView AS
	SELECT
		c.ClienteID
		, c.Nombre
		, c.Calle + ' ' + c.NumeroExterior
			+ CASE WHEN ISNULL(c.NumeroInterior, '') = '' THEN '' ELSE ' - ' END
			+ ISNULL(c.NumeroInterior, '') AS Direccion
		, c.Colonia
		, c.CodigoPostal
		, m.NombreMunicipio AS Municipio
		, cd.NombreCiudad AS Ciudad
		, e.NombreEstado AS Estado
		, c.Telefono
		, c.Celular
		, cf.Rfc
		, c.TipoFormaPagoID
		, c.BancoID
		, c.CuentaBancaria
		, c.ListaDePrecios
		, c.TieneCredito
		, c.Credito
		, c.Tolerancia
	FROM
		Cliente c
		LEFT JOIN Municipio m ON m.MunicipioID = c.MunicipioID AND m.Estatus = 1
		LEFT JOIN Ciudad cd ON cd.CiudadID = c.CiudadID AND cd.Estatus = 1
		LEFT JOIN Estado e ON e.EstadoID = c.EstadoID AND e.Estatus = 1
		LEFT JOIN ClienteFacturacion cf ON cf.ClienteID = c.ClienteID AND cf.Estatus = 1
	WHERE c.Estatus = 1
GO

ALTER VIEW PartesVentasView AS
	SELECT
		p.ParteID
		, p.CodigoBarra
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

CREATE VIEW BancosView AS
	SELECT
		BancoID
		, NombreBanco
	FROM Banco
	WHERE Estatus = 1
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

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
	, @CodigoAlterno NVARCHAR(32) = NULL

	, @VehiculoModeloID INT = NULL -- Se debe incluir el ModeloID para que el filtro por vehículo tenga efecto
	, @VehiculoAnio INT = NULL
	, @VehiculoMotorID INT = NULL
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
			-- LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1 AND pi.Estatus = 1
			-- LEFT JOIN ParteCodigoAlterno pca ON pca.ParteID = p.ParteID AND pca.Estatus = 1
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
		WHERE
			NumeroParte = @Codigo
			OR CodigoBarra = @Codigo
	END

END
GO