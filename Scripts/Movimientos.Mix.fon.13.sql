ALTER VIEW PartesVehiculosView
AS
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
GO