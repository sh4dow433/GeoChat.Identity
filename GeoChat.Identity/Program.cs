using GeoChat.Identity.Api.DbAccess;
using GeoChat.Identity.Api.EventBus;
using GeoChat.Identity.Api.AuthExtensions;
using GeoChat.Identity.Api.RabbitMqEventBus;
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

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IEventBus, MockEventBus>();
}
else
{
    builder.Services.RegisterEventBus();
}

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
