using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using OracleCMS_DealerAPI.Data;
using OracleCMS_DealerAPI.Models;
using OracleCMS_DealerAPI.Repositories;
using OracleCMS_DealerAPI.Services;
using System.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/dealers/login";
        options.AccessDeniedPath = "/error/403";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<DatabaseContext>();
builder.Services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<DatabaseContext>().CreateConnection());
builder.Services.AddScoped<CarRepository>();
builder.Services.AddScoped<AuthService>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});

// Add Swagger generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Car Stock Management API",
        Version = "v1",
        Description = "API for managing dealers, cars, and authentication.",
    });
});

// Build the application
WebApplication app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Stock Management API V1");
    c.RoutePrefix = "swagger";
});

app.UseRouting(); 

app.UseAuthentication();
app.UseAuthorization(); 

app.UseCors();

app.MapControllers();

// Error routes
app.MapFallback(async context =>
{
    switch(context.Response.StatusCode)
    {
        case StatusCodes.Status403Forbidden:
            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                StatusCode = 403,
                Message = "Access to resource denied."
            });
            break;
        default:
            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                StatusCode = 404,
                Message = "The resource you are looking for could not be found."
            });
            break;
    }


});

app.Run();
