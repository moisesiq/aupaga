/* Script con modificaciones para el módulo de ventas. Archivo 66
 * Creado: 2014/11/19
 * Subido: 2014/11/21
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY



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

ALTER VIEW [dbo].[PartesView] AS
	SELECT TOP 1000000000000
		Parte.ParteID
		,Parte.NumeroParte 
		,Parte.NombreParte AS Descripcion
		,MarcaParte.NombreMarcaParte AS Marca
		,Linea.NombreLinea AS Linea
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
		, '' AS FaltanCaracteristicas
		,ParteEstatus.ParteEstatusID
		,Parte.LineaID
		,MAX(CASE WHEN Parte.ParteID = ParteVehiculo.ParteID THEN 'SI' ELSE 'NO' END) AS Aplicacion
		,MAX(CASE WHEN Parte.ParteID = ParteEquivalente.ParteID THEN 'SI' ELSE 'NO' END) AS Equivalente
		,Parte.FechaRegistro
		,Parte.NumeroParte 
		+ Parte.NombreParte
		+ MarcaParte.NombreMarcaParte
		+ Linea.NombreLinea AS Busqueda
	FROM 
		Parte
		INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
		INNER JOIN ParteEstatus ON ParteEstatus.ParteEstatusID = Parte.ParteEstatusID
		LEFT JOIN ParteVehiculo ON ParteVehiculo.ParteID = Parte.ParteID
		LEFT JOIN ParteEquivalente ON ParteEquivalente.ParteID = Parte.ParteID	
	WHERE
		Parte.Estatus = 1
	GROUP BY
		Parte.ParteID
		,Parte.NumeroParte 
		,Parte.NombreParte
		,MarcaParte.NombreMarcaParte
		,ParteEstatus.ParteEstatusID
		,Linea.NombreLinea
		,Parte.LineaID
		,Parte.FechaRegistro
	ORDER BY 
		Parte.NombreParte
GO

ALTER VIEW [dbo].[PartesVehiculosView] AS
	/* Forma anterior
	SELECT ISNULL(ROW_NUMBER() OVER(ORDER BY NombreModelo), -1)  AS GenericoID, *
	FROM
	(
		SELECT DISTINCT	
			ParteVehiculo.ParteID
			,ParteVehiculo.ModeloID	
			,Modelo.NombreModelo	
			,ParteVehiculo.MotorID
			,Motor.NombreMotor				
			,STUFF((SELECT DISTINCT ', ' + CAST(pv.Anio AS VARCHAR)
			FROM
				ParteVehiculo pv
			WHERE
				pv.ParteID = ParteVehiculo.ParteID			
				AND pv.MotorID = ParteVehiculo.MotorID
				AND pv.ModeloID = ParteVehiculo.ModeloID
			FOR XML PATH('')), 1, 1, '') AS Anio	
		FROM 
			ParteVehiculo
			LEFT JOIN Modelo ON Modelo.ModeloID = ParteVehiculo.ModeloID
			LEFT JOIN Motor ON Motor.MotorID = ParteVehiculo.MotorID
		WHERE
			ParteVehiculo.MotorID IS NOT NULL
			AND ParteVehiculo.Estatus = 1
			
		UNION

		SELECT DISTINCT
			ParteVehiculo.ParteID
			,ParteVehiculo.ModeloID	
			,Modelo.NombreModelo
			,ParteVehiculo.MotorID
			,Motor.NombreMotor			
			,STUFF((SELECT DISTINCT ', ' + CAST(pv.Anio AS VARCHAR)
			FROM
				ParteVehiculo pv
			WHERE
				pv.ParteID = ParteVehiculo.ParteID					
				AND pv.ModeloID = ParteVehiculo.ModeloID
				AND pv.MotorID IS NULL
				AND pv.Anio IS NOT NULL
			FOR XML PATH('')), 1, 1, '') AS Anio
		FROM 
			ParteVehiculo
			LEFT JOIN Modelo ON Modelo.ModeloID = ParteVehiculo.ModeloID
			LEFT JOIN Motor ON Motor.MotorID = ParteVehiculo.MotorID
		WHERE
			ParteVehiculo.MotorID IS NULL	
			AND ParteVehiculo.Anio IS NOT NULL
			AND ParteVehiculo.Estatus = 1

		UNION

		SELECT 	
			ParteVehiculo.ParteID
			,ParteVehiculo.ModeloID	
			,Modelo.NombreModelo
			,ParteVehiculo.MotorID
			,Motor.NombreMotor			
			,CAST(ParteVehiculo.Anio AS VARCHAR) AS Anio
		FROM 
			ParteVehiculo
			LEFT JOIN Modelo ON Modelo.ModeloID = ParteVehiculo.ModeloID
			LEFT JOIN Motor ON Motor.MotorID = ParteVehiculo.MotorID
		WHERE
			ParteVehiculo.MotorID IS NULL	
			AND ParteVehiculo.Anio IS NULL
			AND ParteVehiculo.Estatus = 1
	) AS x
	*/

	SELECT
		pv.ParteVehiculoID AS GenericoID
		, pv.ParteID
		, pv.ModeloID
		, m.NombreModelo
		, pv.MotorID
		, mt.NombreMotor
		, CONVERT(NVARCHAR(4), pv.Anio) AS Anio
	FROM
		ParteVehiculo pv
		LEFT JOIN Modelo m ON m.ModeloID = pv.ModeloID AND m.Estatus = 1
		LEFT JOIN Motor mt ON mt.MotorID = pv.MotorID AND mt.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

