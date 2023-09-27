FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY . /app
WORKDIR /app
ENV TZ "America/Lima"
ENTRYPOINT ["dotnet", "Facade.dll"]