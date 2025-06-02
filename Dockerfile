FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file(s)
COPY LMS/*.csproj ./LMS/

# Restore dependencies for your project
RUN dotnet restore ./LMS/LMS.csproj

# Copy all source files
COPY LMS/. ./LMS/

# Publish the project
RUN dotnet publish ./LMS/LMS.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "LMS.dll"]
