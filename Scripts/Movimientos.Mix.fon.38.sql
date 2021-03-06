ALTER TABLE ParteCodigoAlterno ADD MarcaParteID INT NOT NULL
GO

CREATE VIEW [dbo].[PartesCodigosAlternosView] AS
	SELECT
		ParteCodigoAlterno.ParteCodigoAlternoID,
		ParteCodigoAlterno.ParteID,
		ParteCodigoAlterno.MarcaParteID,		
		MarcaParte.NombreMarcaParte,
		ParteCodigoAlterno.CodigoAlterno
	FROM
		ParteCodigoAlterno
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = ParteCodigoAlterno.MarcaParteID
	WHERE
		ParteCodigoAlterno.Estatus = 1
GO

INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
		('Administracion.CatalogosPartes.CodigosAlternos.Ver', '')
		,('Administracion.CatalogosPartes.CodigosAlternos.Agregar', '')
		,('Administracion.CatalogosPartes.CodigosAlternos.Modificar', '')
		,('Administracion.CatalogosPartes.CodigosAlternos.Eliminar', '')
		,('Administracion.CatalogosPartes.Equivalentes.Eliminar', '')
		,('Administracion.CatalogosPartes.Aplicaciones.Eliminar', '')
GO
