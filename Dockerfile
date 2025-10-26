# Use SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY src/MssqlOperator/MssqlOperator.csproj ./src/MssqlOperator/

RUN dotnet restore src/MssqlOperator/MssqlOperator.csproj

COPY src/ ./src/

RUN dotnet build src/MssqlOperator/MssqlOperator.csproj -c Release -o /app/build

RUN dotnet publish src/MssqlOperator/MssqlOperator.csproj -c Release -o /app/publish

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MssqlOperator.dll"]
