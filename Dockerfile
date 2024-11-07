# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o arquivo de solução para o diretório de trabalho
COPY *.sln .

# Copia os arquivos .csproj para a pasta src
COPY CommitChroniclesAPI/*.csproj ./CommitChroniclesAPI/

# Restaura as dependências do projeto
WORKDIR /app/CommitChroniclesAPI
RUN dotnet restore

# Publica o aplicativo
RUN dotnet publish -c Release -o /app/publish

# Estágio final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish . 
EXPOSE 80

# Configura a variável de ambiente para o MongoDB
ENV MongoConnectionString=mongodb://mongodb:27017

# Inicia o aplicativo
ENTRYPOINT ["dotnet", "CommitChroniclesAPI.dll"]
