Sistema de Gestión de Bibliotecas
Este es un sistema completo de gestión de bibliotecas desarrollado en ASP.NET Core 9.0 con SQLite como base de datos. El sistema permite la gestión de libros, préstamos y reservas de manera eficiente.

Funcionalidades Principales
Gestión de Libros: Permite crear, leer, actualizar y eliminar libros, con búsqueda avanzada por título o autor y control automático de inventario.

Gestión de Préstamos: Registra préstamos, controla el estado de los mismos (Activo, Devuelto, Atrasado) y automatiza las devoluciones.

Gestión de Reservas: Permite reservar libros que no están disponibles y convierte automáticamente la reserva en un préstamo cuando el libro vuelve a estar en inventario.

Cómo Empezar
Prerrequisitos
Asegúrate de tener instalado lo siguiente:

.NET 9.0 SDK

Git

Un editor de código (Visual Studio Code, Visual Studio, etc.)

Pasos para Clonar y Ejecutar
Clonar el repositorio:

Bash

git clone https://github.com/TU_USUARIO/sistema-gestion-biblioteca.git
cd sistema-gestion-biblioteca
Restaurar los paquetes de NuGet:

Bash

dotnet restore
Aplicar las migraciones de la base de datos:

Bash

dotnet ef database update
Ejecutar la aplicación:

Bash

dotnet run
Abrir en el navegador:

La aplicación estará disponible en http://localhost:5037/
