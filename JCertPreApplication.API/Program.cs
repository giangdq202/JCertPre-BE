using JCertPreApplication.Persistence;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDbContext<JCertPreDatabaseContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("JCertPreDB")));
builder.Services.AddPersistenceService(builder.Configuration);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// C?u hình DbContext v?i PostgreSQL
//builder.Services.AddDbContext<JCertPreDatabaseContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("JCertPreDB")));
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("all");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
