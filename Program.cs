using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication;
using PosMobileApi;
using PosMobileApi.Constants;
using PosMobileApi.Data;
using PosMobileApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistance(builder.Configuration);

builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseStorage(
                    new MySqlStorage(
                        builder.Configuration["Database:HangfireDb"],
                        new MySqlStorageOptions
                        {
                            QueuePollInterval = TimeSpan.FromSeconds(10),
                            JobExpirationCheckInterval = TimeSpan.FromHours(1),
                            CountersAggregateInterval = TimeSpan.FromMinutes(5),
                            PrepareSchemaIfNecessary = true,
                            DashboardJobListLimit = 25000,
                            TransactionTimeout = TimeSpan.FromMinutes(1),
                            TablesPrefix = "Hangfire",
                        }
                    )
                ));
builder.Services.AddHangfireServer();

builder.Services.AddRedisService(builder.Configuration);

builder.Services.AddDALServices();

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

if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.UseAuthorization();

app.MapControllers();

app.Run();
