# Etapa de Build (com SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2019 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura dependências
COPY *.csproj ./ 
RUN dotnet restore

# Copia todo o código-fonte e realiza o build da aplicação
COPY . . 
RUN dotnet publish -c Release -o /app/publish

# Etapa de Runtime (somente o runtime, mais leve)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2019 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expõe a porta 8080 (a porta em que a aplicação estará ouvindo)
ENV ASPNETCORE_URLS=http://+:8081
EXPOSE 8081
