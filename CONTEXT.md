# SchedulePro — Context Transfer Document

> Pega este archivo en otro chat de Claude Code para continuar el trabajo sin re-explicar todo.

## Qué es el proyecto

**SchedulePro** — SaaS multi-tenant de agendamiento de citas (estilo Calendly/Fresha para barberías, salones, clínicas).

**Stack:**
- Backend: .NET 8 + Clean Architecture + EF Core + SQLite (dev) / PostgreSQL (prod)
- Frontend: React 19 + Vite + Tailwind CSS + React Router
- Auth: JWT propio + BCrypt (sin Auth0/Firebase)
- Email: MailKit/SMTP (sin SendGrid)

**Estructura:**
```
appointment-scheduler/
├── src/
│   ├── AppointmentScheduler.Domain/          # Entidades puras
│   ├── AppointmentScheduler.Application/     # Services, DTOs, Interfaces
│   ├── AppointmentScheduler.Infrastructure/  # EF Core, Repositorios, SMTP
│   └── AppointmentScheduler.API/             # Controllers, Middleware, Program
├── tests/AppointmentScheduler.Tests/         # xUnit
└── frontend/                                  # React + Vite
```

## Branch activa

`claude/saas-appointment-scheduling-mvp-bbiiB` — TODO el trabajo va aquí, nunca a main.

## Estado actual (abril 2026)

### Completado

**Fase 0 — Página pública de reserva** ✅
- Slug por negocio, `/book/:slug` sin navbar
- Flujo 3 pasos: servicio → horario → confirmar
- Envío de email SMTP con branding (logo + color)

**Fase 1 — CRUD + Status de citas** ✅
- Enum `AppointmentStatus` (Pending/Confirmed/Cancelled/Completed)
- PATCH `/api/appointments/{id}/status`
- Soft delete en servicios (`IsDeleted` + query filter)
- Validación de solapamiento excluye cancelled

**Fase 3 — Auth + Multi-tenancy** ✅
- JWT + BCrypt, tabla join `UserBusinesses` (N:N)
- `[Authorize]` en todos los controllers excepto públicos
- Frontend AuthContext + interceptor 401 → redirect login
- **CRÍTICO fixed:** usuario B ya no ve negocios de usuario A

**Fase 4 — Emails** ✅
- `SmtpEmailService` envía confirmación al crear cita
- Template HTML inline con branding

**Fase 5 — Analytics Dashboard** ✅
- `AnalyticsService.GetDashboardAsync` con: total, today, cancelled, topService, busiestHour, quietestHour
- Dashboard excluye cancelled de stats activos, muestra contador separado

**Fase 6 — Hardening parcial** ✅
- Rate limiting (.NET 8 built-in): global 100/min, auth 10/min
- Security headers (X-Frame-Options, HSTS, XSS, etc.)
- CORS restrictivo vía `Cors:AllowedOrigins`
- JWT secret por env var (`Jwt:Secret`), dev tiene fallback
- Password complexity (8+ chars, mayus/minus/dígito)
- DataAnnotations en DTOs
- Paginación en GET appointments
- ExceptionHandlingMiddleware

### Pendiente

- **Fase 2 — Calendario visual** (tipo Google Calendar, sin libs pesadas)
- **Fase 6 — Production hardening faltante:**
  - Migrar a PostgreSQL real (Railway)
  - Dockerfile + docker-compose
  - Migraciones EF Core formales (ahora usa `EnsureCreated` en dev, `Migrate` en prod)
  - Serilog estructurado
  - Health checks (`/health`)
  - Background service para recordatorios 24h antes

## Bugs críticos ya resueltos (NO repetir)

1. **Multi-tenant leak**: `EnsureCreated()` no actualiza DB existente → tabla UserBusinesses no se creaba. Fix: `EnsureDeleted()+EnsureCreated()` en dev + rewrite del query (sin Include+Select).
2. **Port mismatch 5000 vs 5030**: `.env` tenía `VITE_API_URL=http://localhost:5000`. Fix: vaciar `.env`, dejar que Vite proxy maneje todo.
3. **Logo broken image**: Vite proxy sólo forwardeaba `/api`, no `/uploads`. Fix: agregar regla `/uploads` en `vite.config.js`.
4. **ECONNREFUSED Windows**: `localhost` resolvía a IPv6 `::1`, backend en IPv4. Fix: cambiar proxy target a `http://127.0.0.1:5030`.
5. **JWT 401 en /api/business**: Secret mismatch entre `Program.cs` y `DependencyInjection.cs`. Fix: alinear fallback en ambos (`DEV-ONLY-SECRET-KEY-CHANGE-IN-PRODUCTION-Min32Chars!!`).
6. **Status congruence**: Email/confirmación decían "Confirmada" pero cita quedaba "Pending". Fix: cambiar a "Agendada" en `BookingConfirmation.jsx` y `SmtpEmailService.cs`.
7. **Cancelled contaminaba stats**: Dashboard contaba cancelladas. Fix: `AnalyticsService` filtra cancelladas, nueva card "Canceladas" separada.

## Convenciones del proyecto

- **Idioma UI**: Español (sin tildes en código, sí en UI)
- **Sin vendor lock-in**: Todo self-hostable
- **Sin librerías pesadas**: Calendario custom con CSS Grid, no FullCalendar/Chart.js
- **Nunca hacer amend**: Siempre nuevo commit
- **Nunca push a main**: Siempre a la branch de desarrollo asignada
- **Commits con footer**: `https://claude.ai/code/session_...`

## Archivos clave que tocar frecuentemente

**Backend:**
- `src/AppointmentScheduler.API/Program.cs` — Startup, DI, middleware, rate limit, CORS
- `src/AppointmentScheduler.Infrastructure/DependencyInjection.cs` — EF Core, JWT, repos
- `src/AppointmentScheduler.Infrastructure/Data/AppDbContext.cs` — Esquema DB + índices
- `src/AppointmentScheduler.Application/Services/AppointmentService.cs` — Lógica de citas
- `src/AppointmentScheduler.Application/Services/AnalyticsService.cs` — Dashboard stats
- `src/AppointmentScheduler.Infrastructure/Services/SmtpEmailService.cs` — Emails
- `src/AppointmentScheduler.API/Middleware/ExceptionHandlingMiddleware.cs` — Manejo errores

**Frontend:**
- `frontend/src/api/client.js` — Fetch wrapper + 401 handler
- `frontend/src/components/BusinessContext.jsx` — Estado global negocios
- `frontend/src/pages/Dashboard.jsx` — Stats + próximas citas
- `frontend/src/pages/AppointmentsList.jsx` — Lista + cambio de status
- `frontend/src/pages/PublicBooking.jsx` — Flujo público de reserva
- `frontend/src/pages/BookingConfirmation.jsx` — Pantalla post-reserva
- `frontend/vite.config.js` — Proxy (debe apuntar a 127.0.0.1:5030)

## Cómo correr el proyecto

```bash
# Backend
cd appointment-scheduler/src/AppointmentScheduler.API
dotnet run
# Escucha en http://localhost:5030

# Frontend
cd appointment-scheduler/frontend
npm install
npm run dev
# Escucha en http://localhost:3000 con proxy a 5030

# Tests
cd appointment-scheduler/tests/AppointmentScheduler.Tests
dotnet test
```

## DB en producción

Plan: PostgreSQL en **Railway** (free tier suficiente para MVP).
- Cambiar `UseSqlite` → `UseNpgsql` en `DependencyInjection.cs`
- Agregar `Npgsql.EntityFrameworkCore.PostgreSQL` package
- Crear migración inicial con `dotnet ef migrations add Initial`
- `Database.Migrate()` en prod (ya está en Program.cs)
- Connection string vía env var `ConnectionStrings__DefaultConnection`

## Variables de entorno de producción

```
Jwt:Secret=<32+ char random>
ConnectionStrings:DefaultConnection=<postgres url>
Cors:AllowedOrigins:0=https://schedulepro.app
Email:SmtpHost=smtp.gmail.com
Email:SmtpPort=587
Email:Username=<email>
Email:Password=<app password>
Email:From=noreply@schedulepro.app
Email:FromName=SchedulePro
```

## Próximo trabajo sugerido

1. **Calendario visual (Fase 2)** — Mayor diferenciador vs competencia
2. **Migración a PostgreSQL** — Requisito previo al deploy
3. **Dockerfile + Railway deploy** — Shipping real
4. **Background service de recordatorios** — Reduce no-shows ~30%
5. **Editar/eliminar servicios y horarios desde UI** — Falta cerrar el CRUD completo en frontend

## Plan completo

Ver `/root/.claude/plans/sharded-crunching-sprout.md` — tiene el roadmap original por fases con todos los detalles.
