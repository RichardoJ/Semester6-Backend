using CatalogNoSQL.Authentication;
using CatalogNoSQL.Model;
using CatalogNoSQL.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using CatalogNoSQL.Repository;
using CatalogNoSQL.EventProcessing;
using CatalogNoSQL.RabbitMQ;
using DotNetEnv.Configuration;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Load Env file
DotNetEnv.Env.Load();

//Add MongoDB & RabbitMQ
var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddDotNetEnv()
    .Build();

var connectionString = config.GetValue<string>("CATALOGCONN_STRING");
var databaseName = config.GetValue<string>("CATALOGDB_NAME");
var collectionName = config.GetValue<string>("CATALOGCOLL_NAME");
var rabbitMQHost = config.GetValue<string>("RABBITMQHOST");
var rabbitMQPort = config.GetValue<string>("RABBITMQPORT");

builder.Services.Configure<RabbitMQSettings>(options =>
{
    options.HostName = rabbitMQHost;
    options.Port = rabbitMQPort;
});

builder.Services.Configure<PaperStoreDatabaseSettings>(options =>
{
    options.ConnectionString = connectionString;
    options.DatabaseName = databaseName;
    options.PapersCollectionName= collectionName;
});

//Add Service
builder.Services.AddSingleton<IPaperService, PaperService>();
builder.Services.AddSingleton<IPaperRepository, PaperRepository>();

// Add Redis caching
var redisCon = Environment.GetEnvironmentVariable("REDIS_CON");
var redisConfiguration = ConfigurationOptions.Parse(redisCon);
builder.Services.AddStackExchangeRedisCache(options => {
    options.ConfigurationOptions = redisConfiguration;
});

//Add Azure Blob Storage
builder.Services.AddTransient<IAzureStorage, AzureStorage>();


//Event Processor
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

//Add MessageSubscriber
builder.Services.AddHostedService<MessageBusSubcriber>();

//Add JWT Verification
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, (o) => { });

//Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("role", "ADMIN"));
    options.AddPolicy("Public", policy => policy.RequireClaim("role", "READER"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Setting CORS
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
