FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore TestBenchWebService.csproj

COPY . ./
RUN dotnet publish TestBenchWebService.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/out .

EXPOSE 8080
EXPOSE 443

ENTRYPOINT ["dotnet", "TestBenchWebService.dll"]