# Basic RabbitMQ with .NET Core API
Basic Web API .NET CORE with queue implementation using RabbitMQ

To implement this project is necessary run the RabbitMQ server with docker.
Run
```sh
docker run -d --hostname rabbitserver --name rabbitmq-server -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```

## RabbitMQ Server
To see your server management, you should to go to your localhost server with the port configured on docker image
```sh
http://localhost:15672/
```

## API
Run the project of API to send requests to yor server of RabbitMQ, you can use any request submission tool.
#### Request body
```json
{
    "id": 0,
    "itemName": "",
    "price": 0
}
```
After send your request, you can see the queue being filled on the management server of RabbitMQ

## Worker
To get your data in queue with the worker, you need to run the console application on project OrderWorker and run, when you run it, all the data that be in queue will get and presented on console.
