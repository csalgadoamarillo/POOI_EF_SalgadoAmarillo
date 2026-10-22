-- codigo sql
USE master;
GO
IF DB_ID('ExamenFinal') IS NULL
    CREATE DATABASE ExamenFinal;
GO
USE ExamenFinal;
GO
IF OBJECT_ID('dbo.tb_paises', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_paises (
        idpais char(3) PRIMARY KEY,
        nombrepais varchar(40) NOT NULL
    );
    INSERT INTO dbo.tb_paises VALUES
        ('001', 'Perú'), ('002', 'Argentina'), ('003', 'Chile');
END;
GO
IF OBJECT_ID('dbo.tb_consumidor', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_consumidor (
        dnicont char(8) PRIMARY KEY,
        nomcont varchar(40) NOT NULL,
        apecont varchar(40) NOT NULL,
        emailcont varchar(250) UNIQUE,
        idpais char(3) REFERENCES dbo.tb_paises(idpais)
    );
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_listar_paises
AS
BEGIN
    SET NOCOUNT ON;
    SELECT idpais, nombrepais FROM dbo.tb_paises ORDER BY nombrepais;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_listar_consumidor
AS
BEGIN
    SET NOCOUNT ON;
    SELECT dnicont, nomcont, apecont, emailcont, idpais
    FROM dbo.tb_consumidor ORDER BY apecont, nomcont, dnicont;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_detalle_consumidor
    @dnicont varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT dnicont, nomcont, apecont, emailcont, idpais
    FROM dbo.tb_consumidor WHERE dnicont = @dnicont;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_agrega_consumidor
    @dnicont varchar(50),
    @nomcont varchar(100),
    @apecont varchar(100),
    @emailcont varchar(300),
    @idpais varchar(20),
    @mensaje varchar(250) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @dnicont IS NULL OR DATALENGTH(@dnicont) <> 8
        OR @dnicont COLLATE Latin1_General_BIN2 LIKE '%[^0-9]%'
    BEGIN
        SET @mensaje = 'El DNI debe tener 8 dígitos numéricos.';
        RETURN;
    END;
    SET @nomcont = LTRIM(RTRIM(@nomcont));
    SET @apecont = LTRIM(RTRIM(@apecont));
    SET @emailcont = NULLIF(LTRIM(RTRIM(@emailcont)), '');
    IF @nomcont IS NULL OR @nomcont = '' OR LEN(@nomcont) > 40
        OR @apecont IS NULL OR @apecont = '' OR LEN(@apecont) > 40
    BEGIN
        SET @mensaje = 'Ingrese nombres y apellidos de 1 a 40 caracteres.';
        RETURN;
    END;
    IF LEN(@emailcont) > 250
    BEGIN
        SET @mensaje = 'El correo debe tener como máximo 250 caracteres.';
        RETURN;
    END;
    IF @idpais IS NULL OR DATALENGTH(@idpais) <> 3
        OR NOT EXISTS (SELECT 1 FROM dbo.tb_paises WHERE idpais = @idpais)
    BEGIN
        SET @mensaje = 'Seleccione un país válido.';
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM dbo.tb_consumidor WHERE dnicont = @dnicont)
    BEGIN
        SET @mensaje = 'El DNI ya está registrado.';
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM dbo.tb_consumidor
        WHERE (emailcont = @emailcont OR (emailcont IS NULL AND @emailcont IS NULL)))
    BEGIN
        SET @mensaje = 'El correo ya está registrado. Ingrese otro correo.';
        RETURN;
    END;
    BEGIN TRY
        INSERT INTO dbo.tb_consumidor (dnicont, nomcont, apecont, emailcont, idpais)
        VALUES (@dnicont, @nomcont, @apecont, @emailcont, @idpais);
        SET @mensaje = 'Consumidor registrado correctamente.';
    END TRY
    BEGIN CATCH
        SET @mensaje = 'No se pudo guardar. Revise que el DNI y el correo no estén repetidos.';
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_actualiza_consumidor
    @dnicont varchar(50),
    @nomcont varchar(100),
    @apecont varchar(100),
    @emailcont varchar(300),
    @idpais varchar(20),
    @mensaje varchar(250) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @dnicont IS NULL OR DATALENGTH(@dnicont) <> 8
        OR @dnicont COLLATE Latin1_General_BIN2 LIKE '%[^0-9]%'
    BEGIN
        SET @mensaje = 'El DNI debe tener 8 dígitos numéricos.';
        RETURN;
    END;
    SET @nomcont = LTRIM(RTRIM(@nomcont));
    SET @apecont = LTRIM(RTRIM(@apecont));
    SET @emailcont = NULLIF(LTRIM(RTRIM(@emailcont)), '');
    IF @nomcont IS NULL OR @nomcont = '' OR LEN(@nomcont) > 40
        OR @apecont IS NULL OR @apecont = '' OR LEN(@apecont) > 40
    BEGIN
        SET @mensaje = 'Ingrese nombres y apellidos de 1 a 40 caracteres.';
        RETURN;
    END;
    IF LEN(@emailcont) > 250
    BEGIN
        SET @mensaje = 'El correo debe tener como máximo 250 caracteres.';
        RETURN;
    END;
    IF @idpais IS NULL OR DATALENGTH(@idpais) <> 3
        OR NOT EXISTS (SELECT 1 FROM dbo.tb_paises WHERE idpais = @idpais)
    BEGIN
        SET @mensaje = 'Seleccione un país válido.';
        RETURN;
    END;
    IF NOT EXISTS (SELECT 1 FROM dbo.tb_consumidor WHERE dnicont = @dnicont)
    BEGIN
        SET @mensaje = 'No se encontró el consumidor.';
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM dbo.tb_consumidor
        WHERE (emailcont = @emailcont OR (emailcont IS NULL AND @emailcont IS NULL)) AND dnicont <> @dnicont)
    BEGIN
        SET @mensaje = 'El correo ya está registrado. Ingrese otro correo.';
        RETURN;
    END;
    BEGIN TRY
        UPDATE dbo.tb_consumidor
        SET nomcont = @nomcont, apecont = @apecont,
            emailcont = @emailcont, idpais = @idpais
        WHERE dnicont = @dnicont;
        SET @mensaje = 'Consumidor actualizado correctamente.';
    END TRY
    BEGIN CATCH
        SET @mensaje = 'No se pudo guardar. Revise que el DNI y el correo no estén repetidos.';
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_eliminar_consumidor
    @dnicont varchar(50),
    @mensaje varchar(250) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.tb_consumidor WHERE dnicont = @dnicont)
    BEGIN
        SET @mensaje = 'No se encontró el consumidor.';
        RETURN;
    END;
    DELETE FROM dbo.tb_consumidor WHERE dnicont = @dnicont;
    SET @mensaje = 'Consumidor eliminado correctamente.';
END;
GO
