using DeviceManagementAPI.Data;
using DeviceManagementAPI.Middlewares;
using DeviceManagementAPI.Services;
using DeviceManagementAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ? Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ? Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// ? Add Controllers, Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ? Enable CORS for React App (localhost:5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // if you send cookies or tokens
    });
});

// ? Register DatabaseHelper and Repositories
builder.Services.AddScoped<DatabaseHelper>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ISignalMeasurementRepository, SignalMeasurementRepository>();

var app = builder.Build();

// ? Global Error Handling Middleware
app.UseGlobalExceptionHandler();

// ? Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ? Use CORS before routing and endpoints
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
