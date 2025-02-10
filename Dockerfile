# Используем официальный образ .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Устанавливаем рабочую директорию внутри контейнера
WORKDIR /src

# Копируем файл проекта в контейнер
COPY MessageTest.csproj ./

# Выполняем восстановление зависимостей
RUN dotnet restore "MessageTest.csproj"

# Копируем все файлы в контейнер
COPY . .

# Строим проект
RUN dotnet build "MessageTest.csproj" -c Release -o /app/build

# Строим публикацию
RUN dotnet publish "MessageTest.csproj" -c Release -o /app/publish

# Используем образ ASP.NET для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем опубликованные файлы в финальный контейнер
COPY --from=build /app/publish .

# Открываем порт 80
EXPOSE 80

# Команда запуска
ENTRYPOINT ["dotnet", "MessageTest.dll"]
