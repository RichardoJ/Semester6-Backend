# Semester6-Backend
 Backend of Online Paper Library System

## The Project
The project itself is Library System that uses using microservices approach. The services themselves consist of Catalog Service (CatalogNoSQL), Publish Service (PublishNoSQL), and User Service (UserService). The communication between the services uses RabbitMQ, and the API gateway uses nginx (can be viewed with the nginx).conf file)

## How to use
Create a .env file for each directory (CatalogNoSQL, PublishNoSQL, and UserService) or create a .env file for the docker-compose.yml

## How to run
By running the docker-compose.yml file or by running each of the services one by one, but we need to have rabbitMQ running.

## Env file example
MYSQL_ROOT_PASSWORD=""
MYSQL_DATABASE_USER=""
CONNECTIONSTRING_USER=""
RABBITMQHOST=""
RABBITMQPORT=""
CATALOGCONN_STRING=""
CATALOGDB_NAME=""
CATALOGCOLL_NAME=""
REDIS_CON="redis:6379"
PUBLISHCONN_STRING=""
PUBLISHDB_NAME=""
PUBLISHCOLL_NAME=""
BLOB_CONNECTION_STRING=""
FIREBASE_CONFIG=""
