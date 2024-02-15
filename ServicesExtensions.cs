using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PosMobileApi.Constants;
using PosMobileApi.DALs;
using PosMobileApi.Filters;
using PosMobileApi.Models.Responses;
using PosMobileApi.Services;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PosMobileApi
{
    public static class ServicesExtensions
    {
        #region App CORS
        public static void AddAppCors(this IServiceCollection services, string policyName = "AppCorsPolicy")
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(); // Safe
                });

                options.AddPolicy(policyName, builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }
        #endregion

        #region JWT Auth Token
        public static void AddTokenAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(ConstantStrings.OTPTOKENAUTH, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.Response.StatusCode = 401;
                            var response = new BaseResponse<object>
                            {
                                Code = 401,
                                Message = "Invalid OTP Token or Token Expire!"
                            };
                            context.HttpContext.Response.ContentType = "application/json";
                            context.HttpContext.Response.Headers.Add("error-code", "401");
                            await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)));
                            context.HandleResponse();
                        }
                    };
                })
                    .AddJwtBearer(ConstantStrings.AUTHACCESSTOKEN, x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
                x.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.Response.StatusCode = 401;
                        var response = new BaseResponse<object>
                        {
                            Code = 401,
                            Message = "Invalid Access Token or Token Expire!"
                        };
                        context.HttpContext.Response.ContentType = "application/json";
                        context.HttpContext.Response.Headers.Add("error-code", "401");
                        await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)));
                        context.HandleResponse();
                    },
                };
            })
                    .AddJwtBearer(ConstantStrings.REFRESHTOKENAUTH, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.Response.StatusCode = 401;
                            var response = new BaseResponse<object>
                            {
                                Code = 498,
                                Message = "Invalid Refresh Token or Refresh Token Expire!"
                            };
                            context.HttpContext.Response.ContentType = "application/json";
                            context.HttpContext.Response.Headers.Add("error-code", "401");
                            await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)));
                            context.HandleResponse();
                        }
                    };
                });
        }
        #endregion

        #region SwaggerGen Basic and JWT Auth config
        public static void AddSwaggerGen(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "POS.Mobile.API",
                    Version = "v1",
                    Description = configuration["APP:env"] + " " + configuration["APP:ver"] + "\n\r" + configuration["APP:desc"]
                });
                x.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);

                //JWT Token Basic Auth
                x.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Require to add basic auth to authorization header"
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new List<string>()
                    }
                });

                // JWT Access Token
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT Bearer access token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                };
                x.AddSecurityDefinition("Bearer", securityScheme);
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id="Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });

                x.SchemaFilter<EnumSchemaFilter>();
            });
        }
        #endregion

        #region Adding Redis
        public static void AddRedisService(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddStackExchangeRedisCache(options => {

                options.Configuration = Configuration["CacheConnection"];
            });
        }
        #endregion

        #region Adding DAL Servies
        public static void AddDALServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthDAL, AuthDAL>();

            services.AddTransient<IBasicAuthUserService, BasicAuthUserService>();

            services.AddTransient<IOTPCodeGenerator, OTPCodeGenerator>();

            services.AddTransient<ITokenGenerator, TokenGenerator>();

            //services.AddTransient<IAuthDAL, AuthDAL>();
        }
        #endregion
    }
}