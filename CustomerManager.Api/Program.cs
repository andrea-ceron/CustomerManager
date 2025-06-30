using CustomerManager.Business.Abstraction;
using CustomerManager.Business;
using CustomerManager.Repository;
using CustomerManager.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using CustomerManager.Api.Middlewares;
using Utility.Kafka.DependencyInjection;
using CustomerManager.Business.Kafka;
using CustomerManager.Business.Kafka.MessageHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<ClientsDbContext>(options => options.UseSqlServer("name=ConnectionStrings:ClientsDbContext", b =>b.MigrationsAssembly("CustomerManager.Api")));
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IBusiness, Business>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddKafkaConsumer<KafkaTopicInput, MessageHandlerFactory>(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<ClientsDbContext>();
	db.Database.Migrate();
}
app.UseMiddleware<ExceptionMiddleware>();

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
