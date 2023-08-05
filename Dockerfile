FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /src
COPY . .
COPY Properties Properties/
RUN dotnet dev-certs https
CMD [ "dotnet", "run", "-lp", "https" ]