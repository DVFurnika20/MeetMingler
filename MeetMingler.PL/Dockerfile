FROM oven/bun:alpine AS build

ARG SERVER_NAME
ARG API_HOSTNAME
ARG API_UPSTREAM_PORT

WORKDIR /app
COPY . .

RUN bun install
RUN VITE_API_BASE_URL=https://meetminglerapi.${SERVER_NAME} bun run build



FROM alpine AS final

ARG SERVER_NAME
ARG API_HOSTNAME
ARG API_UPSTREAM_PORT

RUN apk update
RUN apk add nginx

COPY --from=build /app/dist /www/data

EXPOSE 443

RUN echo -ne ""\
    "server {\n"\
    "  listen 443 ssl;\n"\
    "  ssl_certificate /etc/ssl/${SERVER_NAME}.pem;\n"\
    "  ssl_certificate_key /etc/ssl/${SERVER_NAME}.key;\n"\
    "  server_name meetmingler.${SERVER_NAME};\n"\
    "\n"\
    "  root /www/data;\n"\
    "  index index.html;"\
    "\n"\
    "  location / {\n"\
    "    try_files \$uri /index.html;\n"\
    "  }\n"\
    "\n"\
    "  error_page 404 /index.html;"\
    "}\n"\
    "\n"\
    "server {\n"\
    "  listen 443 ssl;\n"\
    "  ssl_certificate /etc/ssl/${SERVER_NAME}.pem;\n"\
    "  ssl_certificate_key /etc/ssl/${SERVER_NAME}.key;\n"\
    "  server_name meetminglerapi.${SERVER_NAME};\n"\
    "  location /api {\n"\
    "    proxy_pass http://${API_HOSTNAME}:${API_UPSTREAM_PORT};\n"\
    "    proxy_set_header Host \$host;\n"\
    "    proxy_set_header X-Real-IP \$remote_addr;\n"\
    "    proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;\n"\
    "    proxy_set_header X-Forwarded-Proto \$scheme;\n"\
    "  }\n"\
    "}\n" > /etc/nginx/http.d/nginx.conf

ENTRYPOINT ["nginx", "-g", "daemon off;"]
