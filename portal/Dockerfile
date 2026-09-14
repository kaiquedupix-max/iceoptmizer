FROM node:22-alpine

WORKDIR /app
COPY portal/package.json ./
COPY portal/server.js ./
COPY portal/public ./public

ENV NODE_ENV=production
EXPOSE 3000

CMD ["node", "server.js"]

