# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY . /app
WORKDIR /app
ENV TZ "America/Lima"
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "Facade.dll"]
