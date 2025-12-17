# 🚀 Comandos para Subir TurnosApp a GitHub

## Paso 1: Inicializar Git (si no está inicializado)

```bash
cd c:\Users\juuan\TurnosApp
git init
```

## Paso 2: Agregar todos los archivos

```bash
git add .
```

## Paso 3: Hacer el commit inicial

```bash
git commit -m "Initial commit: TurnosApp - Sistema de gestión de turnos médicos fullstack

- Backend: .NET 9.0 con Clean Architecture
- Frontend: React 19 + TypeScript + Vite
- Funcionalidades completas para pacientes, profesionales y administradores
- Autenticación JWT con roles y permisos
- API RESTful documentada con Swagger"
```

## Paso 4: Crear repositorio en GitHub

1. Ve a [GitHub](https://github.com) y haz clic en el botón **"New"** o **"+"** > **"New repository"**
2. Nombre del repositorio: `TurnosApp` (o el que prefieras)
3. Descripción: "Sistema fullstack de gestión de turnos médicos con .NET 9.0 y React"
4. **IMPORTANTE**: NO marques las opciones de inicializar con README, .gitignore o licencia (ya los tenemos)
5. Haz clic en **"Create repository"**
6. Copia la URL del repositorio (ej: `https://github.com/tu-usuario/TurnosApp.git`)

## Paso 5: Conectar con GitHub y subir

```bash
# Agregar el repositorio remoto (reemplaza con tu URL)
git remote add origin https://github.com/tu-usuario/TurnosApp.git

# Cambiar a la rama main (si estás en master)
git branch -M main

# Subir el código
git push -u origin main
```

## ✅ Verificar que se subió correctamente

```bash
git remote -v  # Verifica que el remote esté configurado
git status     # Debe decir "Your branch is up to date with 'origin/main'"
```

## 🔐 Si necesitas autenticarte

Si GitHub te pide autenticación:
1. Ve a GitHub Settings > Developer settings > Personal access tokens > Tokens (classic)
2. Crea un token con permisos de `repo`
3. Úsalo como contraseña cuando Git te lo pida (el usuario es tu email de GitHub)

## 📝 Notas Importantes

- ✅ El `.gitignore` ya está configurado para ignorar archivos sensibles
- ✅ Los archivos `appsettings.Development.json` y `appsettings.Production.json` están en el .gitignore
- ✅ Si tienes datos sensibles en `appsettings.json`, asegúrate de no subirlos o usa variables de entorno

---

**¡Listo! Tu proyecto estará en GitHub y podrás compartirlo con recruiters y empleadores.**
