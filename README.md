# 📒 Agenda Digital – Gestión de Clientes y Cobranzas

Proyecto académico desarrollado en **C# con Windows Forms y Entity Framework**, que implementa una **agenda digital** para la gestión de clientes, seguimiento de interacciones y recordatorios en el área de cobranzas de una empresa.

---

## Descripción
La aplicación permite a la empresa registrar, consultar y dar seguimiento a las interacciones realizadas con los clientes, optimizando los procesos de cobranza y mejorando la organización interna.  
Se ha diseñado bajo principios de **Programación Orientada a Objetos (POO)** y utiliza una **base de datos SQL Server** para persistencia de datos.

---

## Funcionalidades principales
-  **Login de usuarios** con control de roles (Admin / Usuario).
-  **Gestión de clientes**: alta, edición, eliminación y consulta.
-  **Gestión de interacciones**: registro de llamadas, correos, reuniones o WhatsApp, con resultados asociados.
-  **Gestión de recordatorios**: creación de tareas pendientes para clientes con fecha y estado.
- **Gestión de usuarios**: creación y administración de cuentas con contraseñas seguras (hashing con PBKDF2).
- **Dashboard inicial**: muestra clientes activos, interacciones recientes y recordatorios pendientes.
- **Seguridad**:
  - Contraseñas encriptadas.
  - Roles de usuario que controlan acceso al menú de administración.

---

## Tecnologías utilizadas
- **Lenguaje**: C#  
- **Framework**: .NET Framework / Windows Forms  
- **ORM**: Entity Framework 6 (Database First)  
- **Base de datos**: SQL Server  
- **Arquitectura**: POO con separación en `Models`, `Data`, `Utils` y `Forms`

---

##  Estructura del proyecto
AgendaDigital/
│
├── Data/ # Contexto de base de datos (AppDb)
├── Models/ # Clases de dominio (Usuario, Cliente, Interaccion, Recordatorio, Agenda)
├── Utils/ # Utilidades (PasswordHasher, etc.)
├── Forms/ # Formularios WinForms (Login, Principal, Clientes, Interacciones, Recordatorios, Usuarios)
├── Program.cs # Punto de entrada de la aplicación

##  Usuarios iniciales

Al ejecutar por primera vez, si la tabla de usuarios está vacía, se crea automáticamente:
Usuario: admin
Contraseña: admin123
Rol: ADMIN
