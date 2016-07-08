/* Script con modificaciones para el módulo de ventas. Archivo 14
 * Creado: 2013/12/12
 * Subido: 2013/12/13
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY
	DECLARE @Hoy DATETIME = GETDATE()	INSERT INTO Permiso (NombrePermiso, MensajeDeError, FechaRegistro) VALUES
		('AplicarPartes.Agregar', 'No tienes permisos para aplicar partes a vehículos.', @Hoy)
		, ('Ventas.FondoDeCaja.Agregar', 'No tienes permisos para especificar el Fondo de Caja.', @Hoy)
		, ('Ventas.Refuerzo.Agregar', 'No tienes permisos para hacer Refuerzos.', @Hoy)
		, ('Ventas.Resguardo.Agregar', 'No tienes permisos para hacer Resguardos.', @Hoy)
		, ('Ventas.Cambios.Agregar', 'No tienes permisos para cambiar las caracteríasticas de una venta.', @Hoy)
		, ('Ventas.CorteDeCaja.Agregar', 'No tienes permisos para hecer un Corte de Caja.', @Hoy)
		, ('Ventas.Comisiones.Ver', 'No tienes permisos ver las Comisiones.', @Hoy)
		, ('Ventas.Devolucion.Agregar', 'No tienes permisos para hecer Devoluciones / Cancelaciones.', @Hoy)
		, ('Ventas.ReporteDeFaltante.Agregar', 'No tienes permisos para reportar un faltante.', @Hoy)
		, ('Ventas.FacturarTickets.Agregar', 'No tienes permisos para facturar tickets.', @Hoy)
		, ('Ventas.Ingresos.Agregar', 'No tienes permisos para agregar Ingresos de Caja.', @Hoy)
		, ('Ventas.Egresos.Agregar', 'No tienes permisos para agregar Egresos de Caja.', @Hoy)

	CREATE TABLE AutorizacionProcesoPerfil (
		AutorizacionProcesoPerfilID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, AutorizacionProcesoID INT NOT NULL FOREIGN KEY REFERENCES AutorizacionProceso(AutorizacionProcesoID)
		, PerfilID INT NOT NULL FOREIGN KEY REFERENCES Perfil(PerfilID)
	
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)

	INSERT INTO AutorizacionProcesoPerfil (AutorizacionProcesoID, PerfilID, UsuarioID)
		SELECT AutorizacionProcesoID, 1, 1 FROM AutorizacionProceso WHERE Estatus = 1
	INSERT INTO AutorizacionProcesoPerfil (AutorizacionProcesoID, PerfilID, UsuarioID)
		SELECT AutorizacionProcesoID, 3, 1 FROM AutorizacionProceso 
			WHERE AutorizacionProcesoID IN (2, 12) AND Estatus = 1
	INSERT INTO AutorizacionProcesoPerfil (AutorizacionProcesoID, PerfilID, UsuarioID)
		SELECT AutorizacionProcesoID, 4, 1 FROM AutorizacionProceso 
			WHERE AutorizacionProcesoID NOT IN (1) AND Estatus = 1

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

ALTER VIEW Lista9500View AS
	SELECT
		c9.Cotizacion9500ID AS Folio
		, c9.Fecha
		, p.NumeroParte
		, p.NombreParte
		, c9.Anticipo
		, c.Nombre AS Cliente
		, s.NombreSucursal AS Sucursal
		, v.NombrePersona AS Vendedor
	FROM
		Cotizacion9500 c9
		LEFT JOIN (
			SELECT
				Cotizacion9500ID
				, ParteID
			FROM (
				SELECT
					Cotizacion9500ID
					, ParteID
					, ROW_NUMBER() OVER (PARTITION BY Cotizacion9500ID ORDER BY Cotizacion9500DetalleID)
						AS Numero
				FROM Cotizacion9500Detalle c9d
				WHERE Estatus = 1
			) c9d
			WHERE Numero = 1
		) c9d ON c9d.Cotizacion9500ID = c9.Cotizacion9500ID
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = c9.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario v ON v.UsuarioID = c9.RealizoUsuarioID AND v.Estatus = 1
	WHERE
		c9.Estatus = 1
		AND c9.EstatusGenericoID = 2
GO

ALTER VIEW ClientesCreditoView AS
	SELECT
		c.ClienteID
		, c.Nombre
		, c.TieneCredito
		, c.Credito
		, c.DiasDeCredito
		, c.Tolerancia
		, ca.Adeudo
		, ca.FechaPrimerAdeudo
		, ISNULL(CONVERT(BIT, CASE WHEN (ca.Adeudo >= c.Credito OR ca.FechaPrimerAdeudo >= (GETDATE() + c.DiasDeCredito))
			THEN 1 ELSE 0 END), 0) AS CreditoVencido
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

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

