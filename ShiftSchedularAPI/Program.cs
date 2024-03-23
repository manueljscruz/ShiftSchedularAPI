using Microsoft.EntityFrameworkCore;
using ShiftSchedularBLL.IService;
using ShiftSchedularBLL.Service;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Repositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularIL.IServices;
using ShiftSchedularIL.Mappers;
using ShiftSchedularIL.Services;
using System.ComponentModel;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Repositories
builder.Services.AddScoped<IGenericRepository<Gender>, GenericRepository<Gender>>();
builder.Services.AddScoped<IGenericRepository<GenderLocalization>, GenericRepository<GenderLocalization>>();
builder.Services.AddScoped<ILocalizationRepository, LocalizationRepository>();

// Services
builder.Services.AddScoped<IGenderService, GenderService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Infrastructure
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(ApplicationMapper)));
builder.Services.AddScoped<IGeneralService, GeneralService>();
builder.Services.AddScoped<ICryptographyService, CryptographyService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
