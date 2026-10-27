-- Ejecutar únicamente con autorización, en una instalación nueva.
-- Este script no elimina ni reemplaza tablas existentes.
IF DB_ID(N'CatalogoApartamentosDB') IS NULL
BEGIN
    CREATE DATABASE CatalogoApartamentosDB;
END;
GO

USE CatalogoApartamentosDB;
GO

CREATE TABLE dbo.Residencial
(
    Id INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Residencial PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Ubicacion NVARCHAR(250) NOT NULL,
    Descripcion NVARCHAR(1000) NOT NULL
);
GO

CREATE TABLE dbo.Apartamento
(
    Id INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Apartamento PRIMARY KEY,
    Numero NVARCHAR(20) NOT NULL,
    Habitaciones INT NOT NULL,
    Banos INT NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    Disponible BIT NOT NULL
        CONSTRAINT DF_Apartamento_Disponible DEFAULT (1),
    ResidencialId INT NOT NULL,
    CONSTRAINT CK_Apartamento_Habitaciones CHECK (Habitaciones >= 1),
    CONSTRAINT CK_Apartamento_Banos CHECK (Banos >= 1),
    CONSTRAINT CK_Apartamento_Precio CHECK (Precio > 0),
    CONSTRAINT FK_Apartamento_Residencial
        FOREIGN KEY (ResidencialId) REFERENCES dbo.Residencial(Id)
        ON DELETE NO ACTION
);
GO
