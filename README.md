# 🩺 TurnosApp – Sistema de Gestión de Turnos Médicos

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-19.2-61DAFB?logo=react&logoColor=black)
![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?logo=typescript&logoColor=white)
![C#](https://img.shields.io/badge/C%23-9.0-239120?logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoft-sql-server&logoColor=white)
![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-512BD4?logo=dotnet&logoColor=white)

Aplicación **fullstack** para la gestión de turnos médicos, orientada a **pacientes, profesionales de la salud y administradores**, desarrollada con **.NET 9.0** (Backend) y **React + TypeScript** (Frontend).

El objetivo del proyecto es **simular un sistema real de producción**, aplicando buenas prácticas de **arquitectura, seguridad y diseño de APIs**, y demostrar competencias como **desarrollador Backend / Fullstack con .NET**.

---

## ⭐ Características Destacadas

- 🔐 **Autenticación JWT** con roles y permisos granulares (Admin, Professional, Patient)
- 🏗️ **Clean Architecture** con separación de responsabilidades en capas
- 📊 **Validación de negocio** compleja (solapamiento de turnos, disponibilidad)
- 🎨 **UI Responsive** con Tailwind CSS (mobile-first)
- 📚 **API Documentada** con Swagger/OpenAPI interactiva
- 🔒 **Seguridad** implementada en múltiples capas (BCrypt, JWT, validaciones)
- 🚀 **Performance** optimizado con React Query para cache y refetch inteligente

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

### Backend - Clean Architecture
```
src/
├── TurnosApp.API/              # Capa de presentación (Controllers)
├── TurnosApp.Application/      # Lógica de aplicación
├── TurnosApp.Domain/           # Entidades y enums
├── TurnosApp.Infrastructure/   # Acceso a datos (DbContext, Services)
└── TurnosApp.Contracts/       # DTOs (Requests/Responses)
```

### Frontend - Component-Based Architecture
```
src/
├── api/              # Clientes API organizados por dominio
├── components/       # Componentes React reutilizables
├── pages/            # Páginas de la aplicación
├── store/            # Estado global (Zustand)
├── types/            # Definiciones TypeScript
└── lib/              # Utilidades y configuración
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

## 📚 API Endpoints Principales

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

## ▶️ Cómo ejecutar el proyecto

### Backend
1. Configurar la conexión a SQL Server en `appsettings.json`
2. Ejecutar migraciones:
```bash
dotnet ef database update
```
3. Iniciar la API:
```bash
dotnet run
```

Swagger disponible en `/swagger`

### Frontend
1. Instalar dependencias:
```bash
npm install
```
2. Configurar `.env` con `VITE_API_URL=http://localhost:5294/api`
3. Ejecutar la aplicación:
```bash
npm run dev
```

---

## 🏁 Sobre el proyecto

Este proyecto forma parte de mi **portfolio profesional**, orientado a posiciones **Backend / Fullstack con .NET**.

Está desarrollado aplicando buenas prácticas de arquitectura, seguridad y diseño de APIs, simulando escenarios reales de sistemas en produccion.
