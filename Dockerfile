FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY MessageTest.csproj ./

RUN dotnet restore "MessageTest.csproj"

COPY . .

RUN dotnet build "MessageTest.csproj" -c Release -o /app/build

RUN dotnet publish "MessageTest.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "MessageTest.dll"]
