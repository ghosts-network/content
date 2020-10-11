# GhostNetwork - Publications

Publications is a part of GhostNetwork education project for working with users publications such as publications to news feed

## Installation

copy provided docker-compose.yml and customize for your needs

compile images from the sources - `docker-compose build && docker-compose up -d`

### Parameters

| Environment                    | Description                                               |
|--------------------------------|-----------------------------------------------------------|
| MONGO_ADDRESS                  | Address of MongoDb instance                               |
| PUBLICATION_CONTENT_MIN_LENGTH | Minimum length of publication text                        |
| PUBLICATION_CONTENT_MAX_LENGTH | Maximum length of publication text. 5000 chars by default |
| COMMENT_CONTENT_MIN_LENGTH     | Minimum length of comment                                 |
| COMMENT_CONTENT_MAX_LENGTH     | Maximum length of comment. 5000 chars by default          |

## Development

To run dependent environment use `docker-compose -f dev-compose.yml up -d --build`
