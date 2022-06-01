# GhostNetwork - Content

Content is a part of [GhostNetwork](https://github.com/ghosts-network) education project for working with users publications, comments and reactions

### Parameters

| Environment                    | Description                                                                                         |
|--------------------------------|-----------------------------------------------------------------------------------------------------|
| MONGO_CONNECTION               | Connection string to MongoDb instance                                                               |
| PUBLICATION_CONTENT_MIN_LENGTH | Minimum length of publication text                                                                  |
| PUBLICATION_CONTENT_MAX_LENGTH | Maximum length of publication text. 5000 chars by default                                           |
| PUBLICATION_UPDATE_TIME_LIMIT  | Maximum time in second update action will be available. Unlimited by default                        |
| COMMENT_CONTENT_MIN_LENGTH     | Minimum length of comment                                                                           |
| COMMENT_CONTENT_MAX_LENGTH     | Maximum length of comment. 5000 chars by default                                                    |
| COMMENT_UPDATE_TIME_LIMIT      | Maximum time in second update action will be available. Unlimited by default                        |
| EVENTHUB_TYPE                  | Represent type of service for event bus. Options: rabbit, servicebus. By default all events ignored |
| RABBIT_CONNECTION              | Connection string to rabbitmq. Required for EVENTHUB_TYPE=rabbit                                    |
| SERVICEBUS_CONNECTION          | Connection string to azure service bus. Required for EVENTHUB_TYPE=servicebus                       |


## Development

To run development environment use

```bash
docker-compose -f dev-compose.yml pull
docker-compose -f dev-compose.yml up --force-recreate
```
