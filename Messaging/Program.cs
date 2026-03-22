using Messaging.Common.Extensions;
using Messaging.Common.Options;
using Messaging.Common.Publishing;
using Messaging.Common.Topology;
using RabbitMQ.Client;
using Messaging.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đọc cấu hình RabbitMQ từ appsettings.json
builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

var rabbitOptions = builder.Configuration
    .GetSection("RabbitMq")
    .Get<RabbitMqOptions>()!;

// Đăng ký RabbitMQ vào DI
builder.Services.AddRabbitMq(
    rabbitOptions.HostName,
    rabbitOptions.UserName,
    rabbitOptions.Password,
    rabbitOptions.VirtualHost);

// Đăng ký Publisher
builder.Services.AddSingleton<Publisher>();
builder.Services.AddSingleton<ProductOrderPlacedConsumer>();

var app = builder.Build();

// Tạo exchange, queue, binding khi app khởi động
var channel = app.Services.GetRequiredService<IModel>();
RabbitTopology.EnsureAll(channel, rabbitOptions);
var consumer = app.Services.GetRequiredService<ProductOrderPlacedConsumer>();
consumer.Start();
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