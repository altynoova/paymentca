FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src/Application/Application.csproj ./Application/
COPY ./src/Domain/Domain.csproj ./Domain/
COPY ./src/Infrastructure/Infrastructure.csproj ./Infrastructure/
COPY ./src/WebApi/WebApi.csproj ./WebApi/

RUN dotnet restore ./WebApi/WebApi.csproj

COPY ./src ./ 
WORKDIR /src/WebApi

RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Docker
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApi.dll", "--urls", "http://0.0.0.0:8080"]
