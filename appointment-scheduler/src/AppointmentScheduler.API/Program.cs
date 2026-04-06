using System.Text;
using AppointmentScheduler.API.Middleware;
using AppointmentScheduler.Infrastructure;
using AppointmentScheduler.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o => o.MultipartBodyLengthLimit = 5 * 1024 * 1024);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "SchedulePro-Default-Secret-Key-Change-In-Production-Min32Chars!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "SchedulePro",
            ValidAudience = "SchedulePro",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// DEV: Force clean database to ensure schema is up-to-date
// EnsureCreated() does NOT update existing databases — it only creates if missing.
// This guarantees the UserBusinesses table (and all new columns) exist.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    Console.WriteLine("[STARTUP] Database recreated with fresh schema.");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
