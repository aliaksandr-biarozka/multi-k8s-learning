FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS builder
WORKDIR /source
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -o app -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS runtime
EXPOSE 5000
WORKDIR /app
COPY --from=builder /source/app .
ENTRYPOINT [ "dotnet", "backend-server.dll" ]