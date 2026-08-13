using FluentValidation;
using FluentValidation.AspNetCore;
using ManagementSystem.Api.Data;
using ManagementSystem.Api.Extensions;
using ManagementSystem.Api.Models.Entities;
using ManagementSystem.Api.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAppAuthorization();

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

app.UseAuthentication();   // must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
