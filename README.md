# Unitess test task

Develop a GraphQL API for task management with support for authentication and role-based authorization.

## Features

- **Custom Identityerver** support based on Microsoft Identity
- TaskManager.API creates user based on incoming access_token
- Identityerver register User Event via Rabbitmq to consume
- Clean Arch, CQRS support
- Docker support
- Certificates support, security in storing keys
- Logging support
- Architecture tests

if finish all - it's a production ready code, but obviasly we need to change identity server flow to some sort of Client interaction

## How to run
Go to TaskManager.API Identityerver and root folders<br>
run PS from admin then run<br>
dotnet dev-certs https -ep "$env:APPDATA\ASP.NET\Https\TaskManager.API.pfx" -p "your_password"<br>
or<br>
dotnet dev-certs https -ep "$env:APPDATA\ASP.NET\Https\Identityerver.pfx" -p "your_password"<br>
then run<br>
dotnet user-secrets set "Kestrel:Certificates:Development:Password" "your_password"<br>
in .env file you can change your_password if needed<br>
docker-compose up -d --build taskmanager-db rabbitmq identityserver-db identityserver seq<br>
and then run locally TaskManager.API
