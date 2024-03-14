using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServcies;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQL Server");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));
}
Console.WriteLine($"--> CommandService Endpoint: {builder.Configuration["CommandService"]}");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

PrepDb.PrepPopulation(app, app.Environment.IsProduction());


app.Run();
