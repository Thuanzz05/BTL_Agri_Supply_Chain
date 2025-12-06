using DailyAgriSupplyChain.DAL.Helper;
using DailyAgriSupplyChain.DAL.Helper.Interfaces;
using DailyAgriSupplyChain.DAL.Interfaces;
using DailyAgriSupplyChain.DAL; // Giả sử DonHangDaiLyRepository nằm ở đây
//using DailyAgriSupplyChain.BLL.Interfaces;
//using DailyAgriSupplyChain.BLL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
