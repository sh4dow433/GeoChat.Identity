using GeoChat.Identity.Api.DbAccess;
using GeoChat.Identity.Api.Extensions;
using GeoChat.Identity.Api.MessageQueue.Publisher;
using GeoChat.Identity.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterSwaggerWithAuthInformation();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb"));
});

builder.Services.RegisterAuthServices(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();
builder.Services.AddTransient<IMqPublisher, MqPublisher>();

builder.Services.AddCors();

var app = builder.Build();
app.UseCors(builder => builder
         //.AllowAnyOrigin()
         .WithOrigins("null")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
     );
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
