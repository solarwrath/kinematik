using System.Reflection;

using Kinematik_EntityFramework;

using Kinematik_Hosting.Swagger;

using MediatR;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

Assembly httpApiAssembly = Assembly.Load("Kinematik-HttpApi");
Assembly applicationAssembly = Assembly.Load("Kinematik-Application");

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
    services.AddMediatR(applicationAssembly);

    services.AddDbContext<KinematikDbContext>(options =>
    {
        options.UseSqlServer(Environment.GetEnvironmentVariable("KINEMATIK_CONNECTION_STRING")!);
    });

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

    services.AddMvc();
    services.AddRouting(options =>
    {
        options.LowercaseUrls = true;
    });

    services
        .AddControllers()
        .AddApplicationPart(httpApiAssembly);

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

    services.AddDatabaseDeveloperPageExceptionFilter();
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