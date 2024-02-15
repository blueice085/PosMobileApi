using Microsoft.AspNetCore.Authentication;
using PosMobileApi;
using PosMobileApi.Constants;
using PosMobileApi.Data;
using PosMobileApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistance(builder.Configuration);

builder.Services.AddDALServices();

// Add services to the container.
builder.Services.AddAuthentication(ConstantStrings.AUTHBASIC)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>(ConstantStrings.AUTHBASIC, null);

builder.Services.AddTokenAuthentication(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(builder.Configuration);

var app = builder.Build();


//////////////////////////////////////////
// Configure the HTTP request pipeline. //
//////////////////////////////////////////
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
