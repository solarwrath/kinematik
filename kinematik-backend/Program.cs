using System.Reflection;
using Kinematik.Hosting.Swagger;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        /*
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod();
        */
    });
});

builder.Services.AddMvc();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services
    .AddControllers()
    .AddApplicationPart(Assembly.Load("Kinematik.HttpApi"));

builder.Services.AddSwaggerGen(configuration =>
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


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapSwagger();
});

app.UseSwaggerUI(configuration =>
{
    configuration.SwaggerEndpoint("/swagger/v1/swagger.json", "Kinematik API v1");
});

app.Run();