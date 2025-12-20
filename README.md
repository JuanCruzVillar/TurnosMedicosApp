# 🩺 TurnosApp – Sistema de Gestión de Turnos Médicos

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-19.2-61DAFB?logo=react&logoColor=black)
![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?logo=typescript&logoColor=white)
![C#](https://img.shields.io/badge/C%23-9.0-239120?logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoft-sql-server&logoColor=white)
![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-512BD4?logo=dotnet&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)
![CI/CD](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?logo=github-actions&logoColor=white)

**Aplicación fullstack** para la gestión de turnos médicos desarrollada con **.NET 9.0** y **React 19 + TypeScript**

[Características](#-características-destacadas) • [Arquitectura](#-arquitectura) • [Quick Start](#-inicio-rápido) • [API Docs](#-documentación-de-api)

</div>

---

## 📋 Sobre el Proyecto

Aplicación **fullstack** para la gestión de turnos médicos, orientada a **pacientes, profesionales de la salud y administradores**. 

Este proyecto demuestra competencias en:
- ✅ **Clean Architecture** con separación de responsabilidades
- ✅ **APIs REST** bien diseñadas y documentadas
- ✅ **Autenticación y autorización** con JWT y roles
- ✅ **Frontend moderno** con React, TypeScript y mejores prácticas
- ✅ **DevOps básico** con Docker y CI/CD

> 💡 **Nota**: Este es un proyecto de **portfolio** diseñado para demostrar habilidades técnicas y buenas prácticas de desarrollo.

---

## ⭐ Características Destacadas

### 🔐 Seguridad y Autenticación
- **JWT Authentication** con roles granulares (Admin, Professional, Patient)
- **BCrypt** para hashing de contraseñas
- **Autorización basada en roles** en frontend y backend
- **CORS configurado** de forma segura

### 🏗️ Arquitectura y Diseño
- **Clean Architecture** con separación clara de responsabilidades
- **DTOs** para no exponer entidades del dominio
- **Validación de negocio** compleja (solapamiento de turnos, disponibilidad)
- **Manejo centralizado de errores** con middleware personalizado

### 🎨 Frontend Moderno
- **React 19** con TypeScript para type-safety
- **UI Responsive** con Tailwind CSS (mobile-first)
- **React Query** para cache y refetch inteligente
- **Rutas protegidas** por rol
- **Error Boundaries** para manejo de errores

### 📚 Backend Robusto
- **API REST** bien diseñada y documentada
- **Swagger/OpenAPI** interactivo
- **Entity Framework Core** con Code First y migraciones
- **Validaciones** en múltiples capas

### 🚀 DevOps y Deployment
- **Docker Compose** para setup fácil
- **CI/CD** con GitHub Actions
- **Multi-stage Docker builds** para optimización

---

## 🚀 ¿Qué problema resuelve?

- Centraliza la gestión de turnos médicos
- Evita solapamientos de horarios
- Permite control de acceso por roles
- Automatiza flujos reales de negocio (reservas, cancelaciones, estados)

## 🧠 Decisiones Técnicas

- Se utilizó **Clean Architecture** para desacoplar la lógica de negocio del framework y la base de datos.
- **JWT** fue elegido por ser stateless y adecuado para APIs REST.
- **DTOs** separados para no exponer entidades del dominio.
- Validaciones críticas (solapamiento de turnos, disponibilidad) se realizan **en backend**.
- **React Query** gestiona el estado del servidor (cache, refetch).
- **Zustand** se utiliza para estado global simple (auth, usuario).
- Autorización basada en roles para proteger rutas y endpoints.

---

## 💻 Stack Tecnológico

### Backend
- **.NET 9.0** - Framework principal
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 8.0** - ORM y migraciones
- **JWT Bearer Authentication** - Autenticación segura
- **BCrypt.Net** - Hashing de contraseñas
- **Swagger/OpenAPI** - Documentación interactiva de API
- **SQL Server** - Base de datos relacional

### Frontend
- **React 19.2** - Biblioteca UI
- **TypeScript 5.9** - Tipado estático
- **Vite 7.2** - Build tool y dev server
- **Tailwind CSS 3.4** - Framework CSS utility-first
- **Zustand 5.0** - Gestión de estado global
- **TanStack React Query 5.90** - Data fetching y cache
- **React Router DOM 7.10** - Routing con protección de rutas
- **Axios 1.13** - Cliente HTTP con interceptors
- **date-fns 4.1** - Manipulación de fechas

---

## 🏗️ Arquitectura

### Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                        FRONTEND (React)                      │
├─────────────────────────────────────────────────────────────┤
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐    │
│  │  Pages   │  │Components│  │  Store   │  │   API    │    │
│  │          │  │          │  │(Zustand)│  │  Clients │    │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘    │
│       │              │             │              │           │
│       └──────────────┴─────────────┴──────────────┘           │
│                            │                                  │
│                    React Query (Cache)                        │
└────────────────────────────┼──────────────────────────────────┘
                             │ HTTP/REST
                             │ JWT Bearer Token
┌────────────────────────────┼──────────────────────────────────┐
│                        BACKEND (.NET 9)                        │
├────────────────────────────┼──────────────────────────────────┤
│  ┌──────────────────────────────────────────────────────┐   │
│  │              TurnosApp.API (Controllers)              │   │
│  │  • AuthController  • AppointmentsController           │   │
│  │  • ProfessionalsController  • ScheduleController       │   │
│  └──────────────────────────────────────────────────────┘   │
│                            │                                  │
│  ┌──────────────────────────────────────────────────────┐   │
│  │         TurnosApp.Application (Business Logic)         │   │
│  │  • Validators  • IJwtService                           │   │
│  └──────────────────────────────────────────────────────┘   │
│                            │                                  │
│  ┌──────────────────────────────────────────────────────┐   │
│  │         TurnosApp.Infrastructure (Data Access)         │   │
│  │  • TurnosDbContext  • JwtService  • Migrations         │   │
│  └──────────────────────────────────────────────────────┘   │
│                            │                                  │
│  ┌──────────────────────────────────────────────────────┐   │
│  │            TurnosApp.Domain (Entities)                  │   │
│  │  • User  • Professional  • Appointment  • Schedule     │   │
│  └──────────────────────────────────────────────────────┘   │
│                            │                                  │
│  ┌──────────────────────────────────────────────────────┐   │
│  │          TurnosApp.Contracts (DTOs)                    │   │
│  │  • Requests  • Responses                                │   │
│  └──────────────────────────────────────────────────────┘   │
└────────────────────────────┼──────────────────────────────────┘
                             │
                             │ Entity Framework Core
                             │
┌────────────────────────────┼──────────────────────────────────┐
│                    SQL Server Database                         │
│  • Users  • Professionals  • Appointments  • Schedules        │
└────────────────────────────────────────────────────────────────┘
```

### Estructura de Capas (Clean Architecture)

**Backend:**
```
src/
├── TurnosApp.API/              # Capa de presentación (Controllers, Middleware)
│   ├── Controllers/             # Endpoints REST
│   ├── Middleware/              # ExceptionHandlingMiddleware
│   └── Program.cs               # Configuración y startup
│
├── TurnosApp.Application/      # Lógica de aplicación
│   ├── Common/                  # Interfaces (IJwtService)
│   └── Validators/              # FluentValidation
│
├── TurnosApp.Domain/            # Entidades y enums (Core)
│   ├── Entities/                # User, Professional, Appointment, etc.
│   ├── Enums/                   # UserRole, AppointmentStatus
│   └── Constants/              # Roles
│
├── TurnosApp.Infrastructure/   # Acceso a datos
│   ├── Data/                    # DbContext, Configurations, Migrations
│   └── Services/               # Implementaciones (JwtService)
│
└── TurnosApp.Contracts/         # DTOs (Requests/Responses)
    ├── Requests/                # DTOs de entrada
    └── Responses/               # DTOs de salida
```

**Frontend:**
```
src/
├── api/              # Clientes API organizados por dominio
├── components/       # Componentes React reutilizables
│   ├── layout/      # Header, Layout
│   ├── ui/          # Componentes base (Button, Card, Input)
│   ├── patient/     # Componentes específicos de paciente
│   └── professional/# Componentes específicos de profesional
├── pages/           # Páginas de la aplicación
├── store/           # Estado global (Zustand)
├── types/           # Definiciones TypeScript
└── lib/             # Utilidades y configuración (axios, utils)
```

---

## 🧩 Habilidades Técnicas Aplicadas

### Backend Development
- ✅ Clean Architecture
- ✅ Diseño de APIs REST
- ✅ Entity Framework Core (Code First, migraciones, relaciones)
- ✅ Autenticación y autorización con JWT y roles
- ✅ Seguridad (BCrypt, validaciones, protección de endpoints)
- ✅ Manejo centralizado de errores
- ✅ Documentación con Swagger

### Frontend Development
- ✅ React con TypeScript
- ✅ Rutas protegidas por rol
- ✅ Integración con APIs REST
- ✅ Estado global y estado del servidor
- ✅ UI responsive (mobile-first)
- ✅ Manejo de formularios y validaciones
- ✅ Uso consistente de tipos

### Fullstack
- ✅ Comunicación frontend-backend mediante DTOs
- ✅ Autenticación end-to-end
- ✅ Diseño de entidades y relaciones
- ✅ Implementación de flujos reales de negocio

---

## 🚀 Funcionalidades

### Pacientes
- Registro y login con JWT
- Búsqueda de profesionales por especialidad
- Reserva y cancelación de turnos
- Historial de turnos
- Gestión de perfil y contraseña

### Profesionales
- Gestión de horarios semanales
- Visualización de turnos por día/semana
- Actualización de estado de turnos
- Notas por turno
- Validación de solapamientos

### Administradores
- Dashboard con estadísticas
- CRUD de profesionales
- CRUD de especialidades
- Visualización global de turnos

---

## 📚 Documentación de API

La API está completamente documentada con **Swagger/OpenAPI**. Una vez que inicies el backend, visita:

**🔗 http://localhost:5294/swagger**

### Endpoints Principales

### Autenticación
- `POST /api/Auth/register`
- `POST /api/Auth/login`

### Turnos
- `GET /api/Appointments/my-appointments`
- `GET /api/Appointments/{id}`
- `POST /api/Appointments`
- `POST /api/Appointments/{id}/cancel`
- `PATCH /api/Appointments/{id}/status`
- `PATCH /api/Appointments/{id}/notes`

### Horarios (Professional)
- `GET /api/Schedule/my-schedule`
- `POST /api/Schedule`
- `PUT /api/Schedule`
- `DELETE /api/Schedule/{id}`

### Perfil
- `GET /api/Profile`
- `PUT /api/Profile`
- `POST /api/Profile/change-password`

### Profesionales
- `GET /api/Professionals`
- `GET /api/Professionals/{id}`
- `GET /api/Professionals/{id}/available-slots`

### Especialidades
- `GET /api/Specialties`
- `GET /api/Specialties/{id}`

---

## 🔐 Seguridad

- JWT Authentication con expiración
- Autorización basada en roles
- Hashing de contraseñas con BCrypt
- Validaciones de datos en backend y frontend
- CORS configurado
- Protección de endpoints con `[Authorize]`

Header de autorización:
```
Authorization: Bearer <token>
```

---

## 📝 Modelo de Datos

### Entidades
- **User** (Admin, Professional, Patient)
- **Professional**
- **Patient**
- **Specialty**
- **Schedule**
- **Appointment**

---

## 🚀 Inicio Rápido

### Opción 1: Docker Compose (Recomendado - Más fácil) 🐳

```bash
# Clonar el repositorio
git clone <url-del-repositorio>
cd TurnosApp

# Iniciar todo con Docker Compose
docker-compose up -d

# La API estará en http://localhost:5294
# Swagger en http://localhost:5294/swagger
```

Luego, en otra terminal, iniciar el frontend:
```bash
cd TurnosApp-Frontend
npm install
npm run dev
```

### Opción 2: Setup Manual

## ▶️ Cómo ejecutar el proyecto

### Prerrequisitos
- **.NET 9.0 SDK** instalado
- **SQL Server** (local o remoto)
- **Node.js 18+** y npm
- **Git** para clonar el repositorio

### Backend

1. **Clonar el repositorio** (si aún no lo has hecho):
```bash
git clone <url-del-repositorio>
cd TurnosApp
```

2. **Configurar la base de datos**:
   - Editar `src/TurnosApp.API/appsettings.json` con tu cadena de conexión:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=TurnosAppDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

3. **Configurar JWT** (importante para producción):
   - Editar `appsettings.json` y cambiar `JwtSettings:SecretKey` por una clave segura de al menos 32 caracteres
   - **NUNCA** subas `appsettings.Production.json` con claves reales al repositorio

4. **Ejecutar migraciones**:
```bash
cd src/TurnosApp.API
dotnet ef database update
```

5. **Iniciar la API**:
```bash
dotnet run
```

La API estará disponible en `http://localhost:5294`  
Swagger/OpenAPI disponible en `http://localhost:5294/swagger`

### Frontend

1. **Navegar al directorio del frontend**:
```bash
cd ../TurnosApp-Frontend
```

2. **Instalar dependencias**:
```bash
npm install
```

3. **Configurar variables de entorno**:
   - Crear archivo `.env` en la raíz del proyecto frontend:
   ```
   VITE_API_URL=http://localhost:5294/api
   ```
   - Para producción, usar la URL de tu API desplegada:
   ```
   VITE_API_URL=https://api.turnosapp.com/api
   ```

4. **Ejecutar la aplicación**:
```bash
npm run dev
```

La aplicación estará disponible en `http://localhost:5173`

### Usuarios de Prueba

El seed de la base de datos crea usuarios de prueba:

- **Paciente**: `paciente@test.com` / `Paciente123!`
- **Profesional**: `dr.gomez@hospitalsanjuan.com` / `Doctor123!`
- **Admin**: `admin@hospitalsanjuan.com` / `Admin123!`

---

## 🚀 Deployment

### Backend (Producción)

1. **Configurar variables de entorno**:
   - Usar `appsettings.Production.json` o variables de entorno del sistema
   - Configurar `ConnectionStrings:DefaultConnection` con la cadena de conexión de producción
   - Configurar `JwtSettings:SecretKey` con una clave segura (al menos 32 caracteres)
   - Configurar `CORS:AllowedOrigins` con los orígenes permitidos

2. **Publicar la aplicación**:
```bash
dotnet publish -c Release -o ./publish
```

3. **Ejecutar migraciones en producción**:
```bash
dotnet ef database update --project src/TurnosApp.Infrastructure --startup-project src/TurnosApp.API
```

### Frontend (Producción)

1. **Configurar variables de entorno**:
   - Crear `.env.production` con `VITE_API_URL` apuntando a tu API de producción

2. **Build para producción**:
```bash
npm run build
```

3. **Servir los archivos estáticos**:
   - Los archivos generados estarán en `dist/`
   - Puedes servirlos con cualquier servidor estático (Nginx, IIS, Vercel, Netlify, etc.)

### Consideraciones de Seguridad

- ✅ **NUNCA** subas archivos `.env` o `appsettings.Production.json` con credenciales reales
- ✅ Usa claves JWT seguras y únicas en producción
- ✅ Configura CORS solo con los orígenes necesarios
- ✅ Usa HTTPS en producción
- ✅ Mantén las dependencias actualizadas
- ✅ Configura rate limiting en producción (recomendado)

---

## 📋 Mejores Prácticas Implementadas

### Backend
- ✅ **Clean Architecture** - Separación clara de responsabilidades
- ✅ **DTOs** - No se exponen entidades del dominio directamente
- ✅ **Validación de negocio** - Validaciones complejas en backend
- ✅ **Manejo centralizado de errores** - Middleware de excepciones
- ✅ **Seguridad** - JWT, BCrypt, validaciones
- ✅ **Documentación** - Swagger/OpenAPI integrado
- ✅ **Migraciones** - Entity Framework Core con Code First

### Frontend
- ✅ **TypeScript** - Tipado fuerte en toda la aplicación
- ✅ **React Query** - Cache y refetch inteligente
- ✅ **Rutas protegidas** - Control de acceso por roles
- ✅ **Error Boundaries** - Manejo de errores de React
- ✅ **UI Responsive** - Mobile-first design
- ✅ **Optimización** - Lazy loading y code splitting (preparado)

### Seguridad
- ✅ **Autenticación JWT** - Tokens con expiración
- ✅ **Autorización por roles** - Control granular de acceso
- ✅ **Hashing de contraseñas** - BCrypt
- ✅ **CORS configurado** - Restrictivo en producción
- ✅ **Validaciones** - Frontend y backend

---

## 🎯 Para Recruiters y Entrevistadores

Este proyecto demuestra:

### 💼 Habilidades Técnicas Comprobadas
- ✅ **Clean Architecture** - Separación de responsabilidades y desacoplamiento
- ✅ **APIs REST** - Diseño de endpoints siguiendo convenciones
- ✅ **Autenticación/Autorización** - JWT, roles, middleware personalizado
- ✅ **Entity Framework Core** - Code First, migraciones, relaciones
- ✅ **TypeScript** - Type-safety en todo el frontend
- ✅ **React Moderno** - Hooks, Context, React Query
- ✅ **DevOps Básico** - Docker, CI/CD con GitHub Actions

### 🧠 Decisiones de Diseño
- **Clean Architecture**: Elegida para mantener el código mantenible y testeable
- **JWT**: Stateless, escalable, adecuado para APIs REST
- **DTOs**: Separación entre entidades de dominio y contratos de API
- **React Query**: Cache inteligente y sincronización de estado del servidor
- **Zustand**: Estado global simple y performante

### 📊 Complejidad del Proyecto
- **3 roles diferentes** con permisos granulares
- **Validaciones de negocio** complejas (solapamiento de turnos)
- **Múltiples entidades** relacionadas (User, Professional, Patient, Appointment, Schedule)
- **Frontend completo** con rutas protegidas y manejo de estado

---

## 🏁 Sobre el proyecto

Este proyecto forma parte de mi **portfolio profesional**, diseñado para demostrar competencias como **desarrollador Backend / Fullstack con .NET**.

> 💡 **Objetivo**: Mostrar capacidad para desarrollar aplicaciones fullstack aplicando buenas prácticas de arquitectura, seguridad y diseño, incluso siendo nuevo en la industria.

### Tecnologías y Patrones Demostrados

- **Arquitectura**: Clean Architecture, Repository Pattern (implícito)
- **Backend**: .NET 9, ASP.NET Core, Entity Framework Core
- **Frontend**: React 19, TypeScript, Vite
- **Estado**: Zustand (global), React Query (servidor)
- **Estilos**: Tailwind CSS
- **Autenticación**: JWT Bearer
- **Base de datos**: SQL Server con migraciones

---

## 📝 Notas Adicionales

- El proyecto incluye seed de datos para facilitar las pruebas
- Los logs de desarrollo están condicionados a `import.meta.env.DEV`
- CORS está configurado para ser más restrictivo en producción
- Se recomienda usar variables de entorno para configuración sensible
