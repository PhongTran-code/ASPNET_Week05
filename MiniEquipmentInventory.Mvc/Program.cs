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
        context.Response.ContentType = "text/html; charset=utf-8";
        var html = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Health Check - MiniEquipmentInventory.Mvc</title>
    <link rel=""stylesheet"" href=""/css/site.css"" />
</head>
<body>
    <header class=""site-header"">
        <div class=""container header-content"">
            <div class=""brand"">
                <a href=""/"" style=""text-decoration: none; display: flex; align-items: baseline;"">
                    <span style=""font-weight: 800; color: white; font-size: 22px;"">AspNet Lab05 MVC</span>
                    <span style=""font-size: 15px; color: #9ca3af; font-weight: normal; margin-left: 6px;"">Dashboard</span>
                </a>
            </div>
            <nav class=""main-nav"">
                <a href=""/Equipment"">Equipment</a>
                <a href=""/Equipment/Create"">Create</a>
                <a href=""/Equipment/Trash"">Trash</a>
                <a href=""/AuditLogs"">Audit Logs</a>
                <a href=""/health/ready"">Health</a>
                <a href=""/api/equipment/9999"">API</a>
            </nav>
        </div>
    </header>
    <main class=""container main-content"" style=""margin-top: 20px;"">
        <h2 class=""page-title"" style=""margin: 0; font-size: 2rem; font-weight: bold; color: #1a202c;"">Health Check - /health/ready</h2>
        <table class=""table table-bordered table-striped mt-3"" style=""width: 100%; border-collapse: collapse; margin-top: 20px; background-color: white;"">
            <thead>
                <tr style=""background-color: #1a202c; color: white; border-bottom: 2px solid #ddd;"">
                    <th style=""padding: 12px 10px; text-align: left; color: white;"">Check</th>
                    <th style=""padding: 12px 10px; text-align: left; color: white;"">Status</th>
                    <th style=""padding: 12px 10px; text-align: left; color: white;"">Description</th>
                </tr>
            </thead>
            <tbody>";

        foreach (var entry in report.Entries)
        {
            var statusClass = entry.Value.Status == HealthStatus.Healthy ? "Healthy" : "Unhealthy";
            var color = entry.Value.Status == HealthStatus.Healthy ? "#2ec4b6" : "#e63946";
            var desc = entry.Value.Description ?? (entry.Key == "self" ? "Application is running." : "Database connection is healthy.");
            html += $@"
                <tr style=""border-bottom: 1px solid #e2e8f0;"">
                    <td style=""padding: 12px 10px; font-weight: bold;"">{entry.Key}</td>
                    <td style=""padding: 12px 10px;"">
                        <span class=""badge"" style=""background-color: {color}; color: white; padding: 4px 8px; border-radius: 4px; font-weight: bold;"">{statusClass}</span>
                    </td>
                    <td style=""padding: 12px 10px;"">{desc}</td>
                </tr>";
        }

        var overallColor = report.Status == HealthStatus.Healthy ? "#2ec4b6" : "#e63946";
        html += $@"
                <tr style=""border-bottom: 1px solid #e2e8f0; font-weight: bold;"">
                    <td style=""padding: 12px 10px;"">Overall Status</td>
                    <td style=""padding: 12px 10px;"">
                        <span class=""badge"" style=""background-color: {overallColor}; color: white; padding: 4px 8px; border-radius: 4px; font-weight: bold;"">{report.Status}</span>
                    </td>
                    <td style=""padding: 12px 10px;"">All checks are healthy.</td>
                </tr>
            </tbody>
        </table>
    </main>
    <footer class=""site-footer"" style=""margin-top: 40px;"">
        <div class=""container footer-content"">
            <span>&copy; 2026 - MiniEquipmentInventory.Mvc - Lab05</span>
        </div>
    </footer>
</body>
</html>";
        await context.Response.WriteAsync(html);
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
            instance: http.Request.Path);
    }

    return Results.Ok(equipment);
});

app.Run();
