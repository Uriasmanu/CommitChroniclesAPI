using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Adiciona a configuração do MongoDB a partir do appsettings.json
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");

// Registra o serviço MongoDB
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));

// Adiciona Razor Pages
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

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
