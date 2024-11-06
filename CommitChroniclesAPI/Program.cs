using CommitChroniclesAPI.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Adiciona a configuração do MongoDB a partir do appsettings.json
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");

// Registra o serviço MongoDB
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));

// Registra o JogadorService
builder.Services.AddSingleton<JogadorService>();

// Configura o GuidSerializer para representar GUIDs de forma correta
BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));

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
builder.Services.AddControllers(); // Adiciona suporte para controllers
builder.Services.AddRazorPages();

// Adiciona Swagger
builder.Services.AddSwaggerGen();

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
app.UseAuthorization();

// Mapeia endpoints das controllers e Razor Pages
app.MapControllers(); // Mapeia os endpoints das controllers
app.MapRazorPages();

app.Run();
