FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

RUN dotnet publish WebApi/WebApi.csproj -c release -o publish
COPY WebApi/appsettings.json .

ENTRYPOINT ["dotnet", "publish/WebApi.dll"]


