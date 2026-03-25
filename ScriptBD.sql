-- Script para la creación de la Base de Datos BigFood 
USE [master] 
GO

IF EXISTS (SELECT name FROM dbo.sysdatabases WHERE name = N'BigFood')
    DROP DATABASE [BigFood]
GO

CREATE DATABASE [BigFood]
GO

USE [BigFood]
GO

-- Tabla Usuarios
CREATE TABLE [Usuarios](
    Id INT NOT NULL PRIMARY KEY IDENTITY, 
    login VARCHAR(50) NOT NULL,           
    password VARCHAR(100) NOT NULL,       
    fechaRegistro DATETIME NOT NULL DEFAULT GETDATE(), 
    estado CHAR(1) NOT NULL DEFAULT 'A'   
)
GO

-- Tabla Bitacora 
CREATE TABLE [Bitacora](
    Tabla VARCHAR(100) NOT NULL,           
    Usuario INT NOT NULL,                  
    Maquina VARCHAR(100) NOT NULL,         
    Fecha DATETIME NOT NULL DEFAULT GETDATE(), 
    TipoMov CHAR(1) NOT NULL,              
    Registro VARCHAR(MAX) NOT NULL,       
    CONSTRAINT [FK_Bitacora_Usuario] FOREIGN KEY (Usuario)
        REFERENCES [Usuarios](Id)
)
GO

-- Tabla Clientes
CREATE TABLE [Clientes](
    cedulaLegal VARCHAR(15) NOT NULL PRIMARY KEY, 
    tipoCedula CHAR(2) NOT NULL,                  
    NombreCompleto VARCHAR(100) NOT NULL,         
    Email VARCHAR(100) NOT NULL,                 
    fechaRegistro DATETIME NOT NULL DEFAULT GETDATE(), 
    estado CHAR(1) NOT NULL DEFAULT 'A',         
    Usuario INT NOT NULL,                        
    CONSTRAINT [FK_Cliente_Usuario] FOREIGN KEY (Usuario)
        REFERENCES [Usuarios](Id)
)
GO

-- Tabla Productos
CREATE TABLE [Productos](
    CodigoInterno INT NOT NULL PRIMARY KEY IDENTITY, 
    CodigoBarra VARCHAR(30) NULL,                    
    Descripcion VARCHAR(150) NOT NULL,              
    PrecioVenta DECIMAL(12,2) NOT NULL,            
    Descuento DECIMAL(5,2) NOT NULL,                 
    Impuesto DECIMAL(5,2) NOT NULL,                  
    UnidadMedida VARCHAR(10) NOT NULL,              
    PrecioCompra DECIMAL(12,2) NOT NULL,             
    Usuario INT NOT NULL,                           
    Existencia INT NOT NULL DEFAULT 0,             
    CONSTRAINT [FK_Producto_Usuario] FOREIGN KEY (Usuario)
        REFERENCES [Usuarios](Id)
)
GO

-- Tabla Facturas
CREATE TABLE [Facturas](
    numero INT NOT NULL PRIMARY KEY IDENTITY, 
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),    
    codCliente VARCHAR(15) NOT NULL,               
    Subtotal DECIMAL(12,2) NOT NULL,               
    MontoDescuento DECIMAL(12,2) NOT NULL,         
    MontoImpuesto DECIMAL(12,2) NOT NULL,         
    Total DECIMAL(12,2) NOT NULL,                
    estado CHAR(1) NOT NULL DEFAULT 'A',          
    Usuario INT NOT NULL,                        
    TipoPago CHAR(1) NOT NULL,                     
    Condicion CHAR(1) NOT NULL,                    
    CONSTRAINT [FK_Factura_Cliente] FOREIGN KEY (codCliente)
        REFERENCES [Clientes](cedulaLegal),
    CONSTRAINT [FK_Factura_Usuario] FOREIGN KEY (Usuario)
        REFERENCES [Usuarios](Id)
)
GO

-- Tabla Det_Facturas
CREATE TABLE [Det_Facturas](
    numFactura INT NOT NULL,           
    codInterno INT NOT NULL,            
    cantidad INT NOT NULL,             
    PrecioUnitario DECIMAL(12,2) NOT NULL, 
    PorImp DECIMAL(5,2) NOT NULL,      
    PorDescuento DECIMAL(5,2) NOT NULL, 
    CONSTRAINT [PK_Det_Facturas] PRIMARY KEY (numFactura, codInterno), 
    CONSTRAINT [FK_Det_Factura_Factura] FOREIGN KEY (numFactura)
        REFERENCES [Facturas](numero),
    CONSTRAINT [FK_Det_Fac_Productos] FOREIGN KEY (codInterno)
        REFERENCES [Productos](CodigoInterno)
)
GO

-- Tabla CuentasPorCobrar
CREATE TABLE [CuentasPorCobrar](
    numFactura INT NOT NULL PRIMARY KEY,  
    codCliente VARCHAR(15) NOT NULL,    
    FechaFactura DATETIME NOT NULL,      
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(), 
    montoFactura DECIMAL(12,2) NOT NULL,  
    Usuario INT NOT NULL,                
    estado CHAR(1) NOT NULL DEFAULT 'A', 
    CONSTRAINT [FK_CuentasPorCobrar_Factura] FOREIGN KEY (numFactura)
        REFERENCES [Facturas](numero),
    CONSTRAINT [FK_CuentasPorCobrar_Cliente] FOREIGN KEY (codCliente)
        REFERENCES [Clientes](cedulaLegal),
    CONSTRAINT [FK_CuentasPorCobrar_Usuario] FOREIGN KEY (Usuario)
        REFERENCES [Usuarios](Id)
)
GO

