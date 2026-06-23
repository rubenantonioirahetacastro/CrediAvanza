-- Schema for departamentos and municipios - El Salvador
-- Creates tables and inserts sample data. Replace with full dataset as needed.

CREATE TABLE Departamentos (
    IdDepartamento INT IDENTITY(1,1) PRIMARY KEY,
    cNombre VARCHAR(150) NOT NULL
);

CREATE TABLE Municipios (
    IdMunicipio INT IDENTITY(1,1) PRIMARY KEY,
    IdDepartamento INT NOT NULL,
    cNombre VARCHAR(150) NOT NULL,
    CONSTRAINT FK_Municipios_Departamentos FOREIGN KEY (IdDepartamento)
        REFERENCES Departamentos(IdDepartamento)
);

-- Sample inserts (complete dataset should include 14 departamentos and ~262 municipios)
INSERT INTO Departamentos (cNombre) VALUES ('Ahuachapán');
INSERT INTO Departamentos (cNombre) VALUES ('Santa Ana');
INSERT INTO Departamentos (cNombre) VALUES ('Sonsonate');
INSERT INTO Departamentos (cNombre) VALUES ('Chalatenango');
INSERT INTO Departamentos (cNombre) VALUES ('La Libertad');
INSERT INTO Departamentos (cNombre) VALUES ('Cuscatlán');
INSERT INTO Departamentos (cNombre) VALUES ('San Salvador');
INSERT INTO Departamentos (cNombre) VALUES ('La Paz');
INSERT INTO Departamentos (cNombre) VALUES ('Cabañas');
INSERT INTO Departamentos (cNombre) VALUES ('San Vicente');
INSERT INTO Departamentos (cNombre) VALUES ('Usulután');
INSERT INTO Departamentos (cNombre) VALUES ('San Miguel');
INSERT INTO Departamentos (cNombre) VALUES ('Morazán');
INSERT INTO Departamentos (cNombre) VALUES ('La Unión');

-- Example municipios for San Salvador (adjust IdDepartamento accordingly)
-- Get IdDepartamento for 'San Salvador' then insert
DECLARE @idSanSal INT = (SELECT IdDepartamento FROM Departamentos WHERE cNombre = 'San Salvador');
INSERT INTO Municipios (IdDepartamento, cNombre) VALUES (@idSanSal, 'San Salvador');
INSERT INTO Municipios (IdDepartamento, cNombre) VALUES (@idSanSal, 'Soyapango');
INSERT INTO Municipios (IdDepartamento, cNombre) VALUES (@idSanSal, 'Mejicanos');
INSERT INTO Municipios (IdDepartamento, cNombre) VALUES (@idSanSal, 'Apopa');
INSERT INTO Municipios (IdDepartamento, cNombre) VALUES (@idSanSal, 'Ilopango');

-- Query to get municipios by departamento
-- SELECT m.IdMunicipio, m.cNombre
-- FROM Municipio m
-- JOIN Departamento d ON d.IdDepartamento = m.IdDepartamento
-- WHERE d.cNombre = 'San Salvador'
-- ORDER BY m.cNombre;

-- Simple API usage examples:
-- GET /api/ubigeo/departamentos  -> lista de departamentos
-- GET /api/ubigeo/municipios/{idDepartamento} -> municipios del departamento
