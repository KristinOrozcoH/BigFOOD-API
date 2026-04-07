# BigFOOD API

API desarrollada en .NET para la gestión de un sistema tipo restaurante/negocio, que permite administrar clientes, productos, facturación, cuentas por cobrar y otros módulos relacionados.

---

## Tecnologías utilizadas

- .NET (ASP.NET Core)
- Entity Framework Core
- SQL Server
- JWT (Autenticación)
- Postman (pruebas de endpoints)

---

## Funcionalidades principales

- Gestión de clientes
- Gestión de productos
- Facturación
- Cuentas por cobrar
- Generación de PDF de facturas
- Consumo de servicios externos (tipo de cambio)
- Autenticación mediante JWT

---

## Estructura del proyecto

- Controllers: Manejo de endpoints de la API
- Model: Entidades y modelos de datos
- Services: Lógica de negocio y servicios auxiliares

---

## Configuración del proyecto

Antes de ejecutar el proyecto, se debe configurar:

1. Base de datos  
   Ejecutar el script incluido:  
   ScriptBD.sql

2. Configuración en appsettings.json  

   - Cadena de conexión a SQL Server  
   - Clave JWT  


---

## Ejecución del proyecto

1. Abrir la solución en Visual Studio  
2. Restaurar paquetes NuGet  
3. Ejecutar la API  


---

## Notas

- Este proyecto forma parte del sistema BigFOOD, junto con otros módulos como:
  - API de seguridad
  - Aplicación de escritorio

## Estado del proyecto

Este proyecto se encuentra actualmente en mantenimiento activo.
Las mejoras incluyen la corrección de errores y el perfeccionamiento de la arquitectura del backend.

---

