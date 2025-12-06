using DailyAgriSupplyChain.DAL.Helper.Interfaces;
using DailyAgriSupplyChain.DAL.Helper;
using DailyAgriSupplyChain.DAL.Interfaces;
using DailyAgriSupplyChain.DAL;
using DailyAgriSupplyChain.BLL.Interfaces;
using DailyAgriSupplyChain.BLL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// ĐĂNG KÝ DEPENDENCY INJECTION CHO 3 LỚP 

// 1. Đăng ký Database Helper (DAL Utility)
builder.Services.AddScoped<IDatabaseHelper, DatabaseHelper>();

// 2. Đăng ký Repository (DAL Contract)
builder.Services.AddScoped<IDonHangDaiLyRepository, DonHangDaiLyRepository>();

// 3. Đăng ký Business/Service (BLL Contract)
builder.Services.AddScoped<IDonHangDaiLyBusiness, DonHangDaiLyBusiness>();


// CẤU HÌNH API MẶC ĐỊNH
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