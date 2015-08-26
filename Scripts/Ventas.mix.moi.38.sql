/* Script con modificaciones para el módulo de ventas. Archivo 38
 * Creado: 2014/04/29
 * Subido: 2014/04/29
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Para contabilidad
	
	ALTER TABLE ContaCuentaAuxiliar ADD
		DevengarAut BIT NULL
		, CalculoSemanal BIT NULL
		, DiasMovimiento INT NULL
	
	CREATE TABLE ContaCuentaAuxiliarDevengadoAutomatico (
		ContaCuentaAuxiliarDevengadoAutomaticoID INT NOT NULL IDENTITY(1, 1) -- PRIMARY KEY
		, ContaCuentaAuxiliarID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaAuxiliar(ContaCuentaAuxiliarID)
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, Porcentaje DECIMAL(5, 2) NOT NULL
		
		, CONSTRAINT PK_ContaCuentaAuxiliarDevengadoAutomatico_ContaCuentaAuxiliarID_SucursalID 
			PRIMARY KEY (ContaCuentaAuxiliarID, SucursalID)
	)

	CREATE TABLE SucursalGastoFijo (
		SucursalGastoFijoID INT NOT NULL IDENTITY(1, 1)
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, ContaCuentaAuxiliarID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaAuxiliar(ContaCuentaAuxiliarID)
		, Importe DECIMAL(12, 2)

		, CONSTRAINT PK_SucursalGastoFijo_SucursalID_ContaCuentaAuxiliarID
			PRIMARY KEY (SucursalID, ContaCuentaAuxiliarID)
	)
	
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

DROP VIEW PartesDetalleView

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

-- DROP PROCEDURE pauPartesMaster
-- EXEC pauPartesMaster '2013-04-01', '2014-03-31'
CREATE PROCEDURE pauPartesMaster (
	@Desde DATE
	, @Hasta DATE
	, @ProveedorID INT = NULL
	, @MarcaID INT = NULL
	, @LineaID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2013-02-01'
	DECLARE @Hasta DATE = '2014-03-31'
	DECLARE @ProveedorID INT = NULL
	DECLARE @MarcaID INT = NULL
	DECLARE @LineaID INT = NULL
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Procedimiento
	
	SELECT
		p.ParteID
		, p.CodigoBarra AS CodigoDeBara
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, m.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, p.MedidaID
		, md.NombreMedida AS UnidadDeMedida
		, p.UnidadEmpaque AS UnidadDeEmpaque
		, p.AplicaComision
		, p.EsServicio
		, p.Etiqueta
		, p.SoloUnaEtiqueta
		, p.EsPar
		, pec.Existencia
		, ISNULL(vc.Ventas, 0) AS Ventas
		, pp.Costo
		, pp.PorcentajeUtilidadUno
		, pp.PrecioUno
		, pp.PorcentajeUtilidadDos
		, pp.PrecioDos
		, pp.PorcentajeUtilidadTres
		, pp.PrecioTres
		, pp.PorcentajeUtilidadCuatro
		, pp.PrecioCuatro
		, pp.PorcentajeUtilidadCinco
		, pp.PrecioCinco
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte m ON m.MarcaParteID = p.MarcaParteID AND m.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Medida md ON md.MedidaID = p.MedidaID AND md.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(Existencia) AS Existencia
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = p.ParteID
		LEFT JOIN (
			SELECT
				vd.ParteID
				, COUNT(v.VentaID) AS Ventas
			FROM
				VentaDetalle vd
				INNER JOIN Venta v ON v.VentaID = vd.VentaID
			WHERE
				vd.Estatus = 1
				AND v.Estatus = 1
				AND v.VentaEstatusID = @EstPagadaID
				AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			GROUP BY vd.ParteID
		) vc ON vc.ParteID = p.ParteID
	WHERE
		p.Estatus = 1
		AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		AND (@LineaID IS NULL OR p.LineaID = @LineaID)

END
GO