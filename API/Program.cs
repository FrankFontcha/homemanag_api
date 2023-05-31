using System.Text;
using System.Text.Json.Serialization;
using API.Data;
using API.DTOs.Emails;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "HomeManag Backend API", Version = "v1.0.0" });

        // Add JWT bearer authentication scheme
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });
        // c.OperationFilter<SwaggerAuthorizeOperationFilter>();
});

builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<UserSeeder>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddTransient<IMailService, MailService>();

builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, Options =>
{
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

string[] _Auth_Type_User = { "Admin", "User" };
string[] _Auth_Type_Admin = { "Admin" };

builder.Services.AddAuthorization(Options =>
    {
        Options.AddPolicy("IsAdmin", policy => policy.RequireClaim("Type", _Auth_Type_Admin));
        Options.AddPolicy("IsUser", policy => policy.RequireClaim("Type", _Auth_Type_User));
    }
);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RoleAccessMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
