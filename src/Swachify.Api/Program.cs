using Microsoft.EntityFrameworkCore;
using Swachify.Infrastructure.Data;
using Swachify.Application;
using Swachify.Application.Interfaces;
using Swachify.Application.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Database Configuration
// ----------------------------------------------------
builder.Services.AddDbContext<MyDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

// ----------------------------------------------------
// CORS
// ----------------------------------------------------
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("spa", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// ----------------------------------------------------
// Controllers + JSON Options
// ----------------------------------------------------
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    // opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

// ----------------------------------------------------
// Swagger
// ----------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------------------------
// Dependency Injection
// ----------------------------------------------------
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICleaningService, CleaningService>();
builder.Services.AddScoped<IMasterService, MasterService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<ISMSService, SMSService>();

var app = builder.Build();

// ----------------------------------------------------
// Middleware Pipeline
// ----------------------------------------------------

// ⭐ REQUIRED: Swagger UI static files
app.UseStaticFiles();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseCors("spa");

app.UseAuthorization();

app.MapControllers();

app.Run();




