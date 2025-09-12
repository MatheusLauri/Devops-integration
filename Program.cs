using IntegracaoDevOps.Data;
using IntegracaoDevOps.Services;
using Microsoft.EntityFrameworkCore;

// Carrega as variáveis de ambiente do arquivo .env
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// O .NET agora encontrará a variável 'ConnectionStrings__DefaultConnection' no ambiente
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWebhookService, WebhookService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
