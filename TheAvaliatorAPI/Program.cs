using Microsoft.EntityFrameworkCore;
using Npgsql;
using Hangfire;
using TheAvaliatorAPI.Model.Interface;
using TheAvaliatorAPI.Infra.Repositorios;
using TheAvaliatorAPI.Model;
using TheAvaliatorAPI.Infra;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));
builder.Services.AddScoped(typeof(AvaliacaoAlunos));
builder.Services.AddScoped(typeof(AvaliacaoProfessor));
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

var strBuilder = new NpgsqlConnectionStringBuilder()
{
    Port = 5432,
    Host = "100.68.8.230",
    Username = "postgres",
    Password = "3141516",
    Database = "postgres"
};

builder.Services.AddEntityFrameworkNpgsql().AddDbContext<Contexto>(options => options.UseNpgsql(strBuilder.ConnectionString).EnableSensitiveDataLogging());
builder.Services.AddHangfire(op => op.UsePostgreSqlStorage(strBuilder.ConnectionString));
builder.Services.AddHangfireServer();




using (var serviceProvider = builder.Services.BuildServiceProvider())
{
    var context = serviceProvider.GetRequiredService<Contexto>();
    context.Database.Migrate();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseCors();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
