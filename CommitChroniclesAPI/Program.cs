using CommitChroniclesAPI.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adiciona a configuração do MongoDB a partir do appsettings.json
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");

// Registra o serviço MongoDB
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));

// Registra o JogadorService
builder.Services.AddSingleton<JogadorService>();
builder.Services.AddSingleton<MissaoService>();


// Configura o GuidSerializer para representar GUIDs de forma correta
BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));

// Recupera a chave secreta do arquivo de configuração
var chaveSecreta = builder.Configuration["ChaveSecreta"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "system_tasks",
        ValidAudience = "seus_usuarios",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta))
    };
});

// Adiciona o CORS antes de qualquer outro serviço que precise
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Adiciona Controllers e Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Adiciona Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Horta Facil", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Name = "JWT Autenticação",
        Description = "Entre com o JWT Bearer token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securitySchema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new string[] { } }
    });
});


var app = builder.Build();

// Configura o middleware Swagger apenas no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
        c.RoutePrefix = string.Empty; // Define a página inicial do Swagger em "/"
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use o CORS antes do roteamento
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication(); // Adicionado o middleware de autenticação
app.UseAuthorization();

// Mapeia endpoints das controllers e Razor Pages
app.MapControllers();
app.MapRazorPages();

app.Run();
