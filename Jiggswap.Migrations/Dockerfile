FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine as build

WORKDIR /app
COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /publish ./
ENTRYPOINT [ "dotnet", "Jiggswap.Migrations.dll" ]
