FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-environment
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /binary-app

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /binary-app
COPY --from=build-environment /binary-app .
CMD [ "dotnet", "docker-test.dll" ]