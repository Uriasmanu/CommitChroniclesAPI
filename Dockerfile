# Est�gio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o arquivo de solu��o para o diret�rio de trabalho
COPY *.sln .

# Copia os arquivos .csproj para a pasta src
COPY CommitChroniclesAPI/*.csproj ./CommitChroniclesAPI/

# Restaura as depend�ncias do projeto
WORKDIR /app/CommitChroniclesAPI
RUN dotnet restore

# Publica o aplicativo
RUN dotnet publish -c Release -o /app/publish

# Est�gio final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish . 
EXPOSE 80

# Configura a vari�vel de ambiente para o MongoDB
ENV MongoConnectionString=mongodb://mongodb:27017

# Inicia o aplicativo
ENTRYPOINT ["dotnet", "CommitChroniclesAPI.dll"]
