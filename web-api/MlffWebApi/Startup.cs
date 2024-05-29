using System.Reflection;
using System.Text.Json.Serialization;
using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO;
using MlffWebApi.Hubs;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Interfaces.Services;
using MlffWebApi.Repositories;
using MlffWebApi.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace MlffWebApi;

public class Startup
{
    private readonly ILogger<Startup> _logger;
    private const string API_NAME = "MLFF Web API";

    public Startup()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole(ConfigureLoggerFormat));
        _logger = loggerFactory.CreateLogger<Startup>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        try
        {
            // Add services to the container.
            services.AddControllersWithViews()
                .AddNewtonsoftJson(option => { option.SerializerSettings.Formatting = Formatting.Indented; })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState.SelectMany(t =>
                            t.Value?.Errors.Select(m => m));
                        var response =
                            new ApiResponse<IEnumerable<ModelError>>(StatusCodes.Status400BadRequest, errors);
                        return new BadRequestObjectResult(response);
                    };
                });

            //http logging
            services.AddHttpLogging(options => { options.LoggingFields = HttpLoggingFields.All; });

            services.AddLogging(builder => { builder.AddSimpleConsole(ConfigureLoggerFormat); });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = API_NAME
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            services.AddApiVersioning(s =>
            {
                s.DefaultApiVersion = new ApiVersion(1, 0);
                s.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddDbContext<MlffDbContext>((option) =>
            {
                var connectionStr = Environment.GetEnvironmentVariable(Constants.DB_CONNECTION_STRING) ?? string.Empty;

                if (string.IsNullOrEmpty(connectionStr))
                {
                    throw new ArgumentException($"No environment variable \"{Constants.DB_CONNECTION_STRING}\" found");
                }

                option.UseLazyLoadingProxies().UseNpgsql(connectionStr);
            });

            services.AddRouting(t => t.LowercaseUrls = true);

            // services
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<IUploaderService, UploaderService>();

            // signalr
            var signalRServerBuilder = services.AddSignalR();
            var useAzureSignalR =
                bool.Parse(Environment.GetEnvironmentVariable(Constants.USE_AZURE_SIGNAL_R_SERVICE) ?? "false");
            if (useAzureSignalR)
            {
                signalRServerBuilder.AddAzureSignalR(x =>
                {
                    x.ConnectionString = Environment.GetEnvironmentVariable(Constants.SIGNALR_CONNECTION_STRING);
                });
            }

            services.AddScoped<DetectionHub>();

            // cors
            services.AddCors();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddRazorPages().AddRazorRuntimeCompilation();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    private void ConfigureLoggerFormat(SimpleConsoleFormatterOptions options)
    {
        options.TimestampFormat = "[yyyy/MM/dd hh:mm:ss tt] ";
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (bool.Parse(Environment.GetEnvironmentVariable(Constants.ENABLE_SWAGGER_UI) ?? "true"))
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (bool.Parse(Environment.GetEnvironmentVariable(Constants.USE_HTTPS_REDIRECTION) ?? "true"))
        {
            app.UseHttpsRedirection();
        }

        app.UseHttpLogging();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseCors(x => x.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider =
                new PhysicalFileProvider(Environment.GetEnvironmentVariable(Constants.IMAGE_OUTPUT_DIRECTORY)),
            RequestPath = "/images",
            DefaultContentType = "application/image"
        });

        app.UseEndpoints(endpoints =>
        {
            // RESTful URL
            endpoints.MapControllers();
            // UI URL
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            // SignalR URL
            endpoints.MapHub<DetectionHub>("/detection");
        });
    }
}