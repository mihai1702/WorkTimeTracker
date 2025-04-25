using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        "server=localhost;database=worktimedb;user=root;password=0000",
        new MySqlServerVersion(new Version(8, 0, 3))
    ));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.OpenConnection();
    Console.WriteLine("Conexiunea la baza de date a fost realizată cu succes.");
}

//app.UseHttpsRedirection();

app.MapControllers(); 

app.Run();
