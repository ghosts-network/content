# GhostNetwork - Publications

Publications is a part of GhostNetwork education project for working with users publications such as publications to news feed

## Instalation

copy provided docker-compose.yml and customize for your needs

compile images from the sources - `docker-compose build && docker-compose up -d`

### Parameters

| Environment   | Description                 |
|---------------|---------------------------- |
| MONGO_ADDRESS | Address of MongoDb instance |

## Development

To run dependent environment use `docker-compose -f dev-compose.yml up -d --build`
