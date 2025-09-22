# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia o codigo e restaura dependências
COPY . .
RUN dotnet restore Devops-integration.sln

# compila
RUN dotnet publish Devops-integration.sln -c Release -o /app /p:UseAppHost=false

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app ./

EXPOSE 3050

ENTRYPOINT ["dotnet", "IntegracaoDevOps.dll"]

