# Используем официальный образ Node.js
FROM node:20 AS build

# Устанавливаем рабочую директорию и копируем файлы проекта
WORKDIR /app
COPY package.json package-lock.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем весь проект в контейнер
COPY . .

# Копируем кастомный конфиг Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Строим фронтенд
RUN npm run build

# Используем Nginx для сервировки статического контента
FROM nginx:alpine

# Копируем билд в Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Устанавливаем права для Nginx
RUN chown -R nginx:nginx /usr/share/nginx/html

EXPOSE 80

# Настроим Nginx для фронтенда
CMD ["nginx", "-g", "daemon off;"]
