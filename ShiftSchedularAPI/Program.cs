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
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{

    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Use camelCase or any other naming policy
    options.JsonSerializerOptions.IgnoreNullValues = true;
});



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
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
builder.Services.AddScoped<IGenericRepository<EntityType>, GenericRepository<EntityType>>();
builder.Services.AddScoped<IGenericRepository<EntityTypeLocalization>, GenericRepository<EntityTypeLocalization>>();
builder.Services.AddScoped<IGenericRepository<Entity>, GenericRepository<Entity>>();
builder.Services.AddScoped<IEntityWorkerRepository, EntityWorkerRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IGenericRepository<SkillLocalization>, GenericRepository<SkillLocalization>>();
builder.Services.AddScoped<IEntityTypeLocalizationRepository, EntityTypeLocalizationRepository>();
builder.Services.AddScoped<ISQLRawRepository<object>, SqlRawRepository<object>>();
builder.Services.AddScoped<IEntityWorkerInvitationRepository, EntityWorkerInvitationRepository>();
// Repositories - Shifts
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IShiftBreakRepository, ShiftBreakRepository>();
builder.Services.AddScoped<IGenericRepository<ShiftBreakType>, GenericRepository<ShiftBreakType>>();
builder.Services.AddScoped<IShiftBreakTypeLocalizationRepository, ShiftBreakTypeLocalizationRepository>();
builder.Services.AddScoped<IGenericRepository<ShiftTemplate>, GenericRepository<ShiftTemplate>>();
builder.Services.AddScoped<IGenericRepository<ShiftBreakTemplate>, GenericRepository<ShiftBreakTemplate>>();


// Services
builder.Services.AddScoped<IGenderService, GenderService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IEntityTypeService, EntityTypeService>();
builder.Services.AddScoped<IEntityService, EntityService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IShiftBreakTypeService, ShiftBreakTypeService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Infrastructure
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(ApplicationMapper)));
builder.Services.AddScoped<IGeneralService, GeneralService>();
builder.Services.AddScoped<ICryptographyService, CryptographyService>();


var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
