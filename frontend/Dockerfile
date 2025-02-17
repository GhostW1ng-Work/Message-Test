FROM node:20 AS build

WORKDIR /app
COPY package.json package-lock.json ./

RUN npm install

COPY . .

COPY nginx.conf /etc/nginx/conf.d/default.conf

RUN npm run build

FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html

RUN chown -R nginx:nginx /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
