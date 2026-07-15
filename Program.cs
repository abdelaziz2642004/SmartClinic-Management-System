using Clinic.Data;
using Clinic.Middlewares;
using Clinic.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer followed by the token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Identity
builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<Clinic.Repositories.AppointmentRepository>();
builder.Services.AddScoped<Clinic.Repositories.AppointmentTagRepository>();       // Dev 6: Decorator Pattern
builder.Services.AddScoped<Clinic.Repositories.CancellationCommandRepository>();  // Dev 6: Command Pattern
builder.Services.AddScoped<Clinic.Services.RoleFactory>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
//dev 5 notifications

builder.Services.AddScoped<Clinic.Observers.IAppointmentSubject, Clinic.Observers.AppointmentNotifier>();
builder.Services.AddScoped<Clinic.Observers.IAppointmentObserver, Clinic.Observers.NotificationObserver>();

//dev 7 reporting - excel export
builder.Services.AddScoped<Clinic.Reports.IReportExporter, Clinic.Reports.ExcelReportExporter>();
//dev 7 reporting - google calendar sync
builder.Services.AddScoped<Clinic.Calendar.ICalendarAdapter, Clinic.Calendar.GoogleCalendarAdapter>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var notifier = scope.ServiceProvider.GetRequiredService<Clinic.Observers.IAppointmentSubject>();
    var observer = scope.ServiceProvider.GetRequiredService<Clinic.Observers.IAppointmentObserver>();
    notifier.Subscribe(observer);
}


app.UseCors();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();



using (var scope = app.Services.CreateScope())
{
    var roleFactory = scope.ServiceProvider
        .GetRequiredService<Clinic.Services.RoleFactory>();
    await roleFactory.CreateRolesAsync();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("home.html");

app.Run();
