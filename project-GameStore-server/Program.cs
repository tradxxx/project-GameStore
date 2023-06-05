using Newtonsoft.Json.Serialization;
using project_GameStore_server.Service;
using System.Reflection;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.UseInlineDefinitionsForEnums();
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
});

builder.Services.AddControllers().AddNewtonsoftJson(x =>
{
    x.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    x.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
    x.SerializerSettings.ContractResolver = new DefaultContractResolver()
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    };

});

builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(cors => cors
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseWebSockets();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
