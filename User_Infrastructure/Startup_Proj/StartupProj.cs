using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IO.Compression;
using System.Net;
using System.Text;
using User_Database.Domain;

namespace User_Infrastructure.Startup_Proj
{
    public static class StartupProj
    {
        public static void AddStartup(WebApplicationBuilder builder)
        {
            _ = builder.Configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true);

            #region SetAPPSetting

            APISetting config = new();
            builder.Configuration.Bind("AppSettings", config);
            APISetting.WWWRoot = builder.Environment.WebRootPath;

            #endregion SetAPPSetting
        }

        [Obsolete]
        public static void AddSerilog(WebApplicationBuilder builder)
        {
            _ = builder.Host.UseSerilog((hostingContext, loggerConfig) =>
            loggerConfig
            .WriteTo.MSSqlServer(
                connectionString: APISetting.LogDBConnection,
                tableName: "Logs",
                schemaName: "dbo",
                autoCreateSqlTable: true,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
                )
            .WriteTo.File(
                path: builder.Environment.WebRootPath + @"\Logs\Error\",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                outputTemplate: "{NewLine}{Timestamp:o} {NewLine}" +
                "--------------------SourceContext [{Level}]--------------------{NewLine}" +
                "({SourceContext}) {NewLine}" +
                "--------------------Message--------------------{NewLine}" +
                "{Message}{NewLine}" +
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX{NewLine}",
                rollingInterval: RollingInterval.Day
                )
            //.WriteTo.Email(
            //    new EmailConnectionInfo
            //    {
            //        FromEmail = "howard0@ethereal.email",
            //        ToEmail = "dipeshpansuriya@ymail.com",
            //        MailServer = APISetting.EmailConfiguration.SMTPAddress,
            //        NetworkCredentials = new NetworkCredential
            //        {
            //            UserName = APISetting.EmailConfiguration.UserId,
            //            Password = APISetting.EmailConfiguration.Password,
            //        },
            //        EnableSsl = APISetting.EmailConfiguration.SSL,
            //        IsBodyHtml = false,
            //        Port = APISetting.EmailConfiguration.Port,
            //        EmailSubject = "[{Level}] <{MachineName}> Log Email",
            //    },
            //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm} [{Level}] <{MachineName}> {Message}{NewLine}{Exception}",
            //    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
            //    mailSubject: "[{Level}] <{MachineName}> Log Email"
            //    )
            .WriteTo.Console(
                outputTemplate: "{NewLine}{Timestamp:o} {NewLine}" +
                "--------------------SourceContext [{Level}]--------------------{NewLine}" +
                "({SourceContext}) {NewLine}" +
                "--------------------Message--------------------{NewLine}" +
                "{Message}{NewLine}" +
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX{NewLine}"
                )
            );
        }

        public static void AddSwagger(WebApplicationBuilder builder)
        {
            _ = builder.Services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.FullName);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                        new string[] { }
                  }
                });
            });
        }

        public static void AddToken(WebApplicationBuilder builder)
        {
            builder.Services.AddJwtAuthentication(builder.Configuration);
        }

        public static void AddHangfire(WebApplicationBuilder builder)
        {
            _ = builder.Services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(APISetting.HangFireDBConnection, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
            }));

            _ = builder.Services.AddHangfireServer();
        }

        public static void AddCORS(WebApplicationBuilder builder)
        {
            if (!string.IsNullOrEmpty(APISetting.CORSAllowOrigin))
            {
                // Allowed specified domain to access the API
                _ = builder.Services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.WithOrigins(APISetting.CORSAllowOrigin)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            //.AllowCredentials()
                            .WithExposedHeaders("X-Pagination"));
                });
            }
            else
            {
                // Allowed all domain to access the API
                _ = builder.Services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            //.AllowCredentials()
                            .WithExposedHeaders("X-Pagination"));
                });
            }
        }

        public static void AddGenricAppBuilder(WebApplicationBuilder builder)
        {
            #region Cookie Policy

            _ = builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #endregion Cookie Policy

            #region Kestrel & IIS Server

            // If using Kestrel:
            _ = builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            _ = builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            #endregion Kestrel & IIS Server

            _ = builder.Services.AddHttpContextAccessor();
            _ = builder.Services.AddResponseCaching();
            _ = builder.Services.AddResponseCompression();

            _ = builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            _ = builder.Services.AddControllers()
                           .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            #region Caching

            // In-Memory Caching
            _ = builder.Services.AddMemoryCache();

            // Redis Caching
            _ = builder.Services.AddDistributedRedisCache(options =>
            {
                options.Configuration = APISetting.CacheConfiguration.CacheURL;
            });

            #endregion Caching

            // Customize default API behavior Start
            _ = builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = builder.Services.AddEndpointsApiExplorer();
        }

        public static void AddException(WebApplication app)
        {
            _ = app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = (c) =>
                {
                    IExceptionHandlerFeature exception = c.Features.Get<IExceptionHandlerFeature>();
                    HttpStatusCode statusCode = exception.Error.GetType().Name switch
                    {
                        "ArgumentException" => HttpStatusCode.BadRequest,
                        _ => HttpStatusCode.ServiceUnavailable
                    };

                    c.Response.StatusCode = (int)statusCode;
                    byte[] content = Encoding.UTF8.GetBytes($"Error [{exception.Error.Message}]");
                    _ = c.Response.Body.WriteAsync(content, 0, content.Length);

                    return Task.CompletedTask;
                }
            });
        }

        public static void AddMiddleware(WebApplication app)
        {
            // Store Response & Request
            _ = app.UseMiddleware<HttpLoggingMiddleware>();
        }

        public static void AddGenricApp(WebApplication app)
        {
            _ = app.UseResponseCompression();
            _ = app.UseHttpsRedirection();
            _ = app.UseRouting();
            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            StartupProj.AddMiddleware(app);

            // Cors
            _ = app.UseCors("CorsPolicy");

            // Hangfire
            _ = app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        RequireSsl = false,
                        SslRedirect = false,
                        LoginCaseSensitive = false,
                        Users = new []
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = "Dipesh",
                                PasswordClear =  "Pansuriya",
                            }
                        }
                    }) }
            });
            _ = app.UseHangfireServer();

            _ = app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllers();
                _ = endpoints.MapHangfireDashboard("/hangfire");
            });
        }
    }
}