using Microsoft.EntityFrameworkCore;
using MiniEquipmentInventory.Mvc.Data;
using MiniEquipmentInventory.Mvc.Settings;
using MiniEquipmentInventory.Mvc.Services;
using MiniEquipmentInventory.Mvc.Models;

using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/lab05-.txt", rollingInterval: RollingInterval.Day));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEquipmentService, EquipmentService>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running."), tags: new[] { "live" })
    .AddDbContextCheck<AppDbContext>("database", tags: new[] { "ready" });

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["timestamp"] =
            DateTimeOffset.UtcNow;
        context.ProblemDetails.Extensions.Remove("exception");
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description ?? (e.Key == "self" ? "Application is running." : "Database connection is healthy.")
            }).ToList()
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/api/equipment/{id:int}", async (int id, AppDbContext db, HttpContext http) =>
{
    var equipment = await db.Equipments.AsNoTracking().FirstOrDefaultAsync(e => e.EquipId == id);
    if (equipment == null)
    {
        var traceId = http.TraceIdentifier;
        var log = new AuditLog
        {
            Time = DateTime.Now,
            Level = "Error",
            Message = $"Invalid request: EquipmentId={id} TraceId={traceId}"
        };
        db.AuditLogs.Add(log);
        await db.SaveChangesAsync();

        return Results.Problem(
            type: "https://example.com/problems/equipment-not-found",
            title: "Equipment not found",
            detail: $"The equipment with id {id} was not found.",
            statusCode: StatusCodes.Status404NotFound,
            instance: http.Request.Path,
            extensions: new Dictionary<string, object?>
            {
                { "errorCode", "EQUIPMENT_NOT_FOUND" }
            });
    }

    return Results.Ok(equipment);
});

app.Run();
