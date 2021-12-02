# GhostNetwork - Content

Content is a part of GhostNetwork education project for working with users publications, comments and reactions

## Installation

copy provided docker-compose.yml and customize for your needs

compile images from the sources - `docker-compose build && docker-compose up -d`

### Parameters

| Environment                    | Description                                               |
|--------------------------------|-----------------------------------------------------------|
| MONGO_CONNECTION               | Connection string to MongoDb instance                     |
| ~~MONGO_ADDRESS~~              | Address of MongoDb instance (OBSOLETE)                    |
| PUBLICATION_CONTENT_MIN_LENGTH | Minimum length of publication text                        |
| PUBLICATION_CONTENT_MAX_LENGTH | Maximum length of publication text. 5000 chars by default |
| COMMENT_CONTENT_MIN_LENGTH     | Minimum length of comment                                 |
| COMMENT_CONTENT_MAX_LENGTH     | Maximum length of comment. 5000 chars by default          |

## Development

To run dependent environment use

```bash
docker-compose -f dev-compose.yml pull
docker-compose -f dev-compose.yml up --force-recreate
```
