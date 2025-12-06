# Podman command
```
podman run -d --name questDB -p 9000:9000 -p 9009:9009 -p 8812:8812 -p 9003:9003 questdb/questdb:8.1.1
podman run -d -it --rm --name rabbitMQ -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
```
