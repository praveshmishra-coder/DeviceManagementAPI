using DeviceManagementAPI.Data;
using DeviceManagementAPI.Data.Interfaces;
using DeviceManagementAPI.Middlewares; // ? Add this

var builder = WebApplication.CreateBuilder(args);
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DatabaseHelper and Repositories
builder.Services.AddScoped<DatabaseHelper>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ISignalMeasurementRepository, SignalMeasurementRepository>();

var app = builder.Build();

// ? Global error handling middleware
app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
