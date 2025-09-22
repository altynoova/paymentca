# ========== Build Stage ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# копируем csproj для кэширования restore
COPY ./Application/Application.csproj ./Application/
COPY ./Domain/Domain.csproj ./Domain/
COPY ./Infrastructure/Infrastructure.csproj ./Infrastructure/
COPY ./PaymentCA/WebApi.csproj ./PaymentCA/

RUN dotnet restore ./PaymentCA/WebApi.csproj

# копируем весь исходный код
COPY . .

WORKDIR /src/PaymentCA

# публикуем
RUN dotnet publish -c Release -o /app

# ========== Runtime Stage ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Docker
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApi.dll", "--urls", "http://0.0.0.0:8080"]
