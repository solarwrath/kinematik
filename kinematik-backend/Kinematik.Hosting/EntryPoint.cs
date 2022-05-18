using System.Reflection;
using System.Text.Json.Serialization;

using Kinematik.Application.Ports;
using Kinematik.EntityFramework;
using Kinematik.FileStorage;

using kinematik_backend.Swagger;

using MediatR;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;

using Swashbuckle.AspNetCore.SwaggerGen;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

Assembly httpApiAssembly = Assembly.Load("Kinematik.HttpApi");
Assembly applicationAssembly = Assembly.Load("Kinematik.Application");

var builder = WebApplication.CreateBuilder(args);

ConfigureConfiguration(builder.Configuration);
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureMiddleware(app, builder.Services, builder.Environment);
ConfigureEndpoints(app, builder.Services);

app.Run();

void ConfigureConfiguration(ConfigurationManager configuration)
{

}

void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
{
    services.AddDbContext<KinematikDbContext>(options =>
    {
        options.UseSqlServer(Environment.GetEnvironmentVariable("KINEMATIK_CONNECTION_STRING")!);
    });
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.AddCors(options =>
    {
        options.AddDefaultPolicy(policyBuilder =>
        {
            policyBuilder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            // TODO Deal with this when running with reverse proxy
            /*
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
            */
        });
    });

    JsonConvert.DefaultSettings = () => new JsonSerializerSettings{
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    services
        .AddMvc()
        .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

    services
        .AddControllers()
        .AddApplicationPart(httpApiAssembly)
        .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

    services.AddRouting(options =>
    {
        options.LowercaseUrls = true;
    });

    services.AddSwaggerGen(configuration =>
    {
        configuration.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Description = "API для взаємодії з сервісом Kinematik"
        });

        configuration.EnableAnnotations();
        // TODO Maybe consider making examples later
        //configuration.ExampleFilters();
        configuration.CustomOperationIds(endpointDescription =>
        {
            return endpointDescription.TryGetMethodInfo(out MethodInfo methodInfo)
                ? methodInfo.Name
                : null;
        });
        configuration.SchemaFilter<EnumSchemaFilter>();
    });

    services.AddMediatR(applicationAssembly);

    services.TryAddSingleton<IFileStorageService, OnDiskFileStorageService>();
    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddHttpClient();
}

void ConfigureMiddleware(
    IApplicationBuilder app,
    IServiceCollection services,
    IWebHostEnvironment environment
)
{
    if (environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
    app.UseCors();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    string fileUploadsDirectoryName = "FileUploads";
    string fileUploadsDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "FileUploads");
    if (!Directory.Exists(fileUploadsDirectoryPath))
    {
        Directory.CreateDirectory(fileUploadsDirectoryPath);
    }
    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(fileUploadsDirectoryPath),
        RequestPath = new PathString($"/{fileUploadsDirectoryName}")
    });

    app.UseRouting();

    app.UseAuthorization();

    app.UseSwaggerUI(configuration =>
    {
        configuration.SwaggerEndpoint("/swagger/v1/swagger.json", "Kinematik API v1");
    });
}

void ConfigureEndpoints(IEndpointRouteBuilder app, IServiceCollection services)
{
    app.MapControllers();
    app.MapSwagger();
}