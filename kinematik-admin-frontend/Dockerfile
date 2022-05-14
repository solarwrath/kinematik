FROM node:lts

RUN mkdir /app
COPY package*.json /app
WORKDIR /app
RUN npm ci

COPY . .

EXPOSE 3000

CMD ["npm", "run", "dev"]