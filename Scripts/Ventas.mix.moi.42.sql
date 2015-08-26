/* Script con modificaciones para el módulo de ventas. Archivo 42
 * Creado: 2014/05/21
 * Subido: 2014/05/26
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Agregar Costo y CostoConDescuento a tablas de Ventas

	ALTER TABLE VentaDetalle ADD
		Costo DECIMAL(12, 2) NOT NULL CONSTRAINT DF_Costo DEFAULT 0
		, CostoConDescuento DECIMAL(12, 2) NOT NULL CONSTRAINT DF_CostoConDescuento DEFAULT 0
	ALTER TABLE VentaDetalle DROP CONSTRAINT DF_Costo, DF_CostoConDescuento
	
	ALTER TABLE VentaDevolucionDetalle ADD
		Costo DECIMAL(12, 2) NOT NULL CONSTRAINT DF_Costo DEFAULT 0
		, CostoConDescuento DECIMAL(12, 2) NOT NULL CONSTRAINT DF_CostoConDescuento DEFAULT 0
	ALTER TABLE VentaDevolucionDetalle DROP CONSTRAINT DF_Costo, DF_CostoConDescuento

	-- Para las leyendas
	
	CREATE TABLE VentaTicketLeyenda (
		VentaTicketLeyendaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, NombreLeyenda NVARCHAR(32) NOT NULL
		, Leyenda NVARCHAR(512) NOT NULL
		, LineaID INT NULL FOREIGN KEY REFERENCES Linea(LineaID)
	)

	CREATE TABLE Temporal (
		Proceso NVARCHAR(64) NOT NULL
		, Identificador INT NOT NULL
		, Valor NVARCHAR(512) NULL
		
		, CONSTRAINT Pk_Proceso_Identificador PRIMARY KEY (Proceso, Identificador)
	)

	-- Para lo de los vehículos
	
	CREATE TABLE VehiculoTipo (
		VehiculoTipoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Tipo NVARCHAR(32) NOT NULL
	)
	
	ALTER TABLE ClienteFlotilla ADD
		Vin NVARCHAR(32) NULL
		, Kilometraje INT NULL
		, VehiculoTipoID INT NULL FOREIGN KEY REFERENCES VehiculoTipo(VehiculoTipoID)
	ALTER TABLE ClienteFlotilla ALTER COLUMN MotorID INT NULL
	ALTER TABLE ClienteFlotilla ALTER COLUMN Anio INT NULL

	ALTER TABLE Venta ADD
		ClienteVehiculoID INT NULL FOREIGN KEY REFERENCES ClienteFlotilla(ClienteFlotillaID)
		, Kilometraje INT NULL

	-- Para lo de la garantía

	CREATE TABLE VentaGarantiaMotivo (
		VentaGarantiaMotivoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Motivo NVARCHAR(32) NOT NULL
	)
	INSERT INTO VentaGarantiaMotivo (Motivo) VALUES
		('PIEZA DAÑADA')
		, ('MAL FUNCIONAMIENTO')
		, ('DEJÓ DE FUNCIONAR')
		, ('OTRO')

	CREATE TABLE VentaGarantiaAccion (
		VentaGarantiaAccionID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Etapa INT NOT NULL
		, Accion NVARCHAR(32) NOT NULL
	)
	INSERT INTO VentaGarantiaAccion (Etapa, Accion) VALUES
		(0, 'ARTÍCULO NUEVO')
		, (0, 'NOTA DE CRÉDITO')
		, (0, 'DEVOLUCIÓN DE DINERO')
		, (1, 'REVISIÓN PROVEEDOR')
		, (2, 'NO PROCEDE')

	CREATE TABLE VentaGarantia (
		VentaGarantiaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
		, Fecha DATETIME NOT NULL
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, MotovioID INT NOT NULL FOREIGN KEY REFERENCES VentaGarantiaMotivo(VentaGarantiaMotivoID)
		, MotivoObservacion NVARCHAR(512) NULL
		, AccionID INT NOT NULL FOREIGN KEY REFERENCES VentaGarantiaAccion(VentaGarantiaAccionID)
		, AccionPosterior BIT NOT NULL
		, AccionFechaCompletado DATETIME NULL
		, AccionObservacion NVARCHAR(512) NULL
		, NotaDeCreditoID INT NOT NULL FOREIGN KEY REFERENCES NotaDeCredito(NotaDeCreditoID)
		, EstatusGenericoID INT NOT NULL FOREIGN KEY REFERENCES EstatusGenerico(EstatusGenericoID)
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
		, Estatus BIT NOT NULL DEFAULT 1
	)
	CREATE INDEX Ix_VentaGarantia_VentaID ON VentaGarantia(VentaID)
	CREATE INDEX Ix_VentaGarantia_Estatus ON VentaGarantia(Estatus)
	
	CREATE TABLE VentaGarantiaDetalle (
		VentaGarantiaDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VentaGarantiaID INT NOT NULL FOREIGN KEY REFERENCES VentaGarantia(VentaGarantiaID)
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, Costo DECIMAL(12, 2) NOT NULL
		, CostoConDescuento DECIMAL(12, 2) NOT NULL
		, Cantidad DECIMAL(9, 2) NOT NULL
		, PrecioUnitario DECIMAL(14, 4) NOT NULL
		, Iva DECIMAL(14, 4) NOT NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
		, Estatus BIT NOT NULL DEFAULT 1
	)
	CREATE INDEX Ix_VentaGarantiaDetalle_VentaGarantiaID ON VentaGarantiaDetalle(VentaGarantiaID)
	CREATE INDEX Ix_VentaGarantiaDetalle_Estatus ON VentaGarantiaDetalle(Estatus)

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

ALTER VIEW PartesVentasView AS
	SELECT
		p.ParteID
		, p.CodigoBarra
		, p.NumeroParte
		, p.NombreParte
		, p.EsServicio
		, p.Es9500
		, p.AGranel
		, mp.NombreMarcaParte AS Marca
		, pp.PrecioUno
		, pp.PrecioDos
		, pp.PrecioTres
		, pp.PrecioCuatro
		, pp.PrecioCinco
		, pe1.Existencia AS ExistenciaSuc01
		, pe2.Existencia AS ExistenciaSuc02
		, pe3.Existencia AS ExistenciaSuc03
		, pp.Costo
		, pp.CostoConDescuento
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

ALTER VIEW VentasDetalleView AS
	SELECT
		vd.VentaDetalleID
		, vd.VentaID
		, vd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, vd.Costo
		, vd.CostoConDescuento
		, vd.Cantidad
		, vd.PrecioUnitario
		, vd.Iva
		, m.NombreMedida AS Medida
	FROM
		VentaDetalle vd
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN Medida m ON m.MedidaID = p.MedidaID AND p.Estatus = 1
	WHERE vd.Estatus = 1
GO

ALTER VIEW ClientesFlotillasView AS
	SELECT
		cf.ClienteFlotillaID
		, cf.ClienteID
		, cf.NumeroEconomico
		, mr.MarcaID
		, mr.NombreMarca AS Marca
		, md.ModeloID
		, md.NombreModelo AS Modelo
		, cf.MotorID
		, m.NombreMotor AS Motor
		, cf.Anio
		, cf.VehiculoTipoID
		, vt.Tipo
		, cf.Vin
		, cf.Color
		, cf.Placa
		, cf.Kilometraje
		, (cf.NumeroEconomico + ' - ' + md.NombreModelo + ' - ' + cf.Placa) AS Etiqueta
	FROM
		ClienteFlotilla cf
		LEFT JOIN Motor m ON m.MotorID = cf.MotorID AND m.Estatus = 1
		LEFT JOIN Modelo md ON md.ModeloID = m.ModeloID AND md.Estatus = 1
		LEFT JOIN Marca mr ON mr.MarcaID = md.MarcaID AND mr.Estatus = 1
		LEFT JOIN VehiculoTipo vt ON vt.VehiculoTipoID = cf.VehiculoTipoID
	WHERE
		cf.Estatus = 1
GO

ALTER VIEW VentasView AS
	SELECT
		v.VentaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, v.Folio
		, v.Fecha
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion AS Estatus
		, ISNULL(CONVERT(DECIMAL(12, 2), vd.Subtotal), 0) AS Subtotal
		, ISNULL(CONVERT(DECIMAL(12, 2), vd.Iva), 0) AS Iva
		, ISNULL(CONVERT(DECIMAL(12, 2), vd.Total), 0) AS Total
		, ISNULL(CONVERT(DECIMAL(12, 2), vpt.Importe), 0) AS Pagado
		, v.ACredito
		, v.RealizoUsuarioID AS VendedorID
		, u.NombrePersona AS Vendedor
		, u.NombreUsuario AS VendedorUsuario
		, v.ComisionistaClienteID AS ComisionistaID
		, v.ClienteVehiculoID
		, v.Kilometraje
	FROM
		Venta v
		-- LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID -- AND vfdv.EstatusGenericoID != 4
		LEFT JOIN (
			SELECT
				VentaID
				, SUM(PrecioUnitario * Cantidad) AS Subtotal
				, SUM(Iva * Cantidad) AS Iva
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
			FROM VentaDetalle
			WHERE Estatus = 1
			GROUP BY VentaID
		) vd ON vd.VentaID = v.VentaID
		LEFT JOIN (
			SELECT
				vp.VentaID
				, SUM(vpd.Importe) AS Importe
			FROM
				VentaPago vp
				LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE vp.Estatus = 1
			GROUP BY vp.VentaID
		) vpt ON vpt.VentaID = v.VentaID
		-- LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		-- LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
	WHERE v.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE pauContaCuentasPorSemana (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON
	
	/* EXEC pauContaCuentasPorSemana '2014-01-01', '2014-12-31'
	DECLARE @Desde DATE = '2014-01-01'
	DECLARE @Hasta DATE = '2014-12-31'
	*/
	
	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	-- Inicio del Procedimiento
	SELECT
		ced.SucursalID
		, s.NombreSucursal AS Sucursal
		, cc.ContaCuentaID
		, cc.Cuenta
		, cs.ContaSubcuentaID
		, cs.Subcuenta
		, ccm.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		-- , ced.Fecha
		, DATEPART(WEEK, ced.Fecha) AS Semana
		, SUM(ced.Importe) AS ImporteDev
		-- , ce.Observaciones AS Egreso
		-- , cca.CalculoSemanal
		-- , cca.DiasMovimiento
		, (ISNULL(cca.DiasMovimiento, 7) / 7) AS Semanas
		, SUM(CASE WHEN cca.CalculoSemanal = 1 THEN (ced.Importe / (cca.DiasMovimiento / 7)) ELSE ced.Importe END) AS Importe
	FROM
		ContaEgresoDevengado ced
		INNER JOIN ContaEgreso ce ON ce.ContaEgresoID = ced.ContaEgresoID AND ce.Estatus = 1
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID

		LEFT JOIN Sucursal s ON s.SucursalID = ced.SucursalID AND s.Estatus = 1
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE
		(ced.Fecha >= @Desde AND ced.Fecha < @Hasta)
	GROUP BY
		ced.SucursalID
		, s.NombreSucursal
		, cc.ContaCuentaID
		, cc.Cuenta
		, cs.ContaSubcuentaID
		, cs.Subcuenta
		, ccm.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, DATEPART(WEEK, ced.Fecha)
		, cca.DiasMovimiento
	ORDER BY
		Sucursal
		, Cuenta
		, Subcuenta
		, CuentaDeMayor
		, CuentaAuxiliar

END
GO