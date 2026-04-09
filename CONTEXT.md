# AgendaYa — Context Transfer Document

> Pega este archivo en otro chat de Claude Code para continuar el trabajo sin re-explicar todo.
> **Lee primero este archivo, despues espera instrucciones.**

---

## Repositorio

**Nuevo repo dedicado:** `erg1996/AgendaYa` (https://github.com/erg1996/AgendaYa)

> NOTA: Este `CONTEXT.md` vive todavia en el repo viejo `erg1996/landing-new` (landing page) durante la migracion. Una vez el repo nuevo este pusheado, **toda la actividad pasa a `AgendaYa`**. El directorio standalone para el nuevo repo se prepara en `/home/user/agendaya/` (ver seccion "Migracion al repo nuevo" mas abajo).

### Estrategia de branching (a partir de ahora)

```
main                      <- protegida, solo merge via PR
  └─ develop              <- integracion (opcional, podemos saltarnos esto al inicio)
       └─ feat/<nombre>   <- features individuales
       └─ fix/<nombre>    <- bugfixes
       └─ chore/<nombre>  <- refactor, deps, configs
```

**Reglas:**
- NUNCA pushear directo a `main`
- Cada feature/fix vive en su propia rama, prefijada (`feat/`, `fix/`, `chore/`, `docs/`)
- PRs hacia `main` con descripcion clara
- Squash merge para mantener historia limpia
- Branch actual de trabajo en chats con Claude Code: `claude/<descripcion-corta>`

---

## Que es el proyecto

**AgendaYa** — SaaS multi-tenant de agendamiento de citas (estilo Calendly/Fresha para barberias, salones, clinicas).

**Stack:**
- Backend: .NET 8 + Clean Architecture + EF Core + SQLite (dev) / PostgreSQL (prod)
- Frontend: React 19 + Vite + Tailwind CSS + React Router
- Auth: JWT propio + BCrypt (sin Auth0/Firebase)
- Email: System.Net.Mail / SMTP (sin SendGrid)

**Estructura:**
```
agendaya/
├── src/
│   ├── AppointmentScheduler.Domain/          # Entidades puras
│   ├── AppointmentScheduler.Application/     # Services, DTOs, Interfaces
│   ├── AppointmentScheduler.Infrastructure/  # EF Core, Repositorios, SMTP
│   └── AppointmentScheduler.API/             # Controllers, Middleware, Program
├── tests/AppointmentScheduler.Tests/         # xUnit
└── frontend/                                  # React + Vite
```

> Los namespaces internos siguen siendo `AppointmentScheduler.*` por ahora — renombrarlos seria un refactor grande sin valor inmediato. Solo el branding visible (UI, emails, JWT issuer, package.json) es **AgendaYa**.

---

## Convenciones del proyecto (CRITICAS)

- **Idioma UI:** Español (sin tildes en codigo C#, si en UI React)
- **Branding:** AgendaYa (no SchedulePro, no AppointmentScheduler en lo visible)
- **NO emojis en codigo ni UI** — usar SVG icons en `frontend/src/components/Icons.jsx`. Cualquier emoji que aparezca se considera un bug.
- **Sin librerias pesadas** — calendario custom CSS Grid, SVG icons inline, no Chart.js, no FullCalendar, no lucide-react
- **Sin vendor lock-in** — todo self-hostable
- **Nunca hacer amend** — siempre crear nuevo commit
- **Nunca push a `main`** — siempre a una rama feature/fix/chore con PR

---

## Estado actual (post-sprint, abril 2026)

### Fases completadas

| Fase | Descripcion | Estado |
|------|------------|--------|
| 0 | Pagina publica de reserva (`/book/:slug`) | OK |
| 1 | CRUD + AppointmentStatus (Pending/Confirmed/Completed/Cancelled) | OK |
| 2 | Calendario semanal visual (CSS Grid) | OK |
| 3 | Auth + Multi-tenancy (JWT + UserBusinesses N:N) | OK |
| 4 | Emails de confirmacion + recordatorios 24h (background service) | OK |
| 5 | Analytics dashboard (temporal + status + revenue) | OK |
| 6 | Hardening parcial (rate limit, CORS, security headers, JWT secret env) | OK |

### Sprint mas reciente (commit `fe10f0e` + rebranding)

- **Dashboard arreglado:** "Activas" = solo Pending+Confirmed. Cards en 2 filas (Hoy/Semana/Mes/Servicios + Activas/Completadas/Canceladas/Ingresos).
- **Vista de calendario semanal** — `frontend/src/pages/CalendarView.jsx`, CSS Grid puro, 8am-9pm, navegacion prev/next/hoy, click en cita -> popup detalle.
- **Precios en servicios** — `Service.Price` decimal opcional, dashboard suma ingresos del mes (solo citas Completed).
- **Notas internas en citas** — `Appointment.Notes`, edit inline en `AppointmentsList.jsx` (click -> input -> Enter guarda).
- **Reportes CSV descargables** — `GET /api/reports/appointments.csv?businessId=&from=&to=` con BOM UTF-8 para Excel.
- **Recordatorios 24h** — `ReminderBackgroundService` corre cada hora, marca `Appointment.ReminderSent=true`.
- **Rebranding completo** — SchedulePro -> AgendaYa en todo el codigo (UI, emails, JWT issuer, package.json, index.html).
- **Iconos modernos** — todos los emojis reemplazados por SVG inline en `Icons.jsx` (Lucide-style, stroke 2, currentColor).

### Pendiente (proximo trabajo)

**Prioridad alta:**
1. **SuperAdmin panel** (planificado, no implementado) — ver `/root/.claude/plans/sharded-crunching-sprout.md`. Necesita: `BusinessStatus` enum (Trial/Active/PastDue/Suspended/Archived), global query filters, IgnoreQueryFilters para superadmin queries, **NO eliminar nada** cuando se suspende un cliente.
2. **Migracion a PostgreSQL real (Railway free tier)** — cambiar `UseSqlite` -> `UseNpgsql`, agregar package, crear migraciones EF Core formales (hoy usa `EnsureDeleted+EnsureCreated` en dev).
3. **Dockerfile + docker-compose + deploy a Railway**
4. **Editar/eliminar servicios desde UI** (solo crear hoy)
5. **PDF reports** (CSV ya existe, falta PDF — usar QuestPDF o similar minimal)

**Prioridad media (despues de prod):**
- Multi-staff (varios empleados por negocio, cada uno con calendario)
- Stripe deposits (cobrar al reservar para reducir no-shows)
- Google Calendar sync (2-way)
- Health checks formales (`/health` ya existe, mejorar con DbContext check)
- Serilog estructurado
- White-label (subdominio por negocio)

---

## Bugs criticos ya resueltos (NO repetir)

1. **Multi-tenant leak**: `EnsureCreated()` no actualiza DB existente -> tabla UserBusinesses no se creaba. Fix: `EnsureDeleted()+EnsureCreated()` en dev.
2. **Port mismatch 5000 vs 5030**: `.env` tenia `VITE_API_URL=http://localhost:5000`. Fix: vaciar `.env`, dejar Vite proxy.
3. **Logo broken image**: Vite proxy solo forwardeaba `/api`, no `/uploads`. Fix: agregar regla `/uploads` en `vite.config.js`.
4. **ECONNREFUSED Windows**: `localhost` -> IPv6 `::1`, backend en IPv4. Fix: proxy target a `http://127.0.0.1:5030`.
5. **JWT 401 en /api/business**: Secret mismatch entre `Program.cs` y `DependencyInjection.cs`. Fix: alinear fallback (`DEV-ONLY-SECRET-KEY-CHANGE-IN-PRODUCTION-Min32Chars!!`).
6. **Status congruence**: Email decia "Confirmada" pero cita quedaba "Pending". Fix: cambiar a "Agendada".
7. **Cancelled contaminaba stats**: Dashboard contaba canceladas. Fix: `AnalyticsService` filtra canceladas, card "Canceladas" separada.

---

## Archivos clave

**Backend:**
- `src/AppointmentScheduler.API/Program.cs` — startup, DI, middleware, rate limit, CORS, JWT issuer "AgendaYa"
- `src/AppointmentScheduler.API/BackgroundServices/ReminderBackgroundService.cs` — recordatorios 24h
- `src/AppointmentScheduler.API/Controllers/ReportsController.cs` — CSV export
- `src/AppointmentScheduler.Infrastructure/DependencyInjection.cs` — EF Core, JWT, repos
- `src/AppointmentScheduler.Infrastructure/Data/AppDbContext.cs` — esquema DB + indices
- `src/AppointmentScheduler.Infrastructure/Services/SmtpEmailService.cs` — emails (confirmacion + recordatorio)
- `src/AppointmentScheduler.Application/Services/AppointmentService.cs` — logica de citas
- `src/AppointmentScheduler.Application/Services/AnalyticsService.cs` — dashboard stats

**Frontend:**
- `frontend/src/components/Icons.jsx` — **TODOS los iconos del proyecto** (SVG inline)
- `frontend/src/components/Navbar.jsx` — navegacion + logo AgendaYa
- `frontend/src/api/client.js` — fetch wrapper + 401 handler + downloadReportCsv
- `frontend/src/components/BusinessContext.jsx` — estado global negocios
- `frontend/src/pages/Dashboard.jsx` — stats temporales + status + ingresos
- `frontend/src/pages/CalendarView.jsx` — calendario semanal CSS Grid
- `frontend/src/pages/AppointmentsList.jsx` — lista + cambio status + notas inline + export CSV
- `frontend/src/pages/PublicBooking.jsx` — flujo publico de reserva (sin navbar)
- `frontend/vite.config.js` — proxy (apunta a 127.0.0.1:5030)

---

## Como correr

```bash
# Backend
cd agendaya/src/AppointmentScheduler.API
dotnet run
# http://localhost:5030

# Frontend
cd agendaya/frontend
npm install
npm run dev
# http://localhost:3000 con proxy a 5030

# Tests
cd agendaya/tests/AppointmentScheduler.Tests
dotnet test
# 21 tests deben pasar
```

---

## Variables de entorno produccion

```
Jwt:Secret=<32+ char random>
ConnectionStrings:DefaultConnection=<postgres url>
Cors:AllowedOrigins:0=https://agendaya.app
Email:SmtpHost=smtp.gmail.com
Email:SmtpPort=587
Email:Username=<email>
Email:Password=<app password>
Email:From=noreply@agendaya.app
Email:FromName=AgendaYa
```

---

## Migracion al repo nuevo (estado actual)

El codigo esta en `/home/user/landing-new/appointment-scheduler/`. Para migrar al repo standalone:

```bash
# 1. Copiar carpeta limpia a /home/user/agendaya
cp -r /home/user/landing-new/appointment-scheduler /home/user/agendaya
cd /home/user/agendaya

# 2. Iniciar repo nuevo con historia limpia
rm -rf .git
git init
git add .
git commit -m "Initial commit: AgendaYa MVP

Features:
- Multi-tenant auth (JWT + BCrypt)
- Public booking page (/book/:slug)
- Weekly calendar view
- Service prices + revenue analytics
- Internal notes on appointments
- CSV report export
- 24h email reminders (background service)
- Dashboard with temporal + status metrics"

# 3. Conectar al repo de GitHub
git remote add origin https://github.com/erg1996/AgendaYa.git
git branch -M main
git push -u origin main

# 4. Crear branch de desarrollo
git checkout -b develop
git push -u origin develop
```

Despues de pushear, **el repo `landing-new` queda solo para la landing page** (no tocar mas appointment-scheduler/ ahi).

---

## Plan original

Ver `/root/.claude/plans/sharded-crunching-sprout.md` — roadmap completo por fases con detalles del SuperAdmin pendiente.
