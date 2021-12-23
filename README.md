# ServicesDAO_VotingEngine
VotingEngine implements the core voting functionality in ServicesDAO. Repository contains two application for reputation management and voting process.

The project was developed with dotnet core 3.1 in C# as language.<br>
jQuery UI - v1.10.4 and Razor was used to develop the frontend.<br>

Test Environment<br>
Votin engine and reputation service is a seperated module that works integrated with the "Services DAO" project.<br>
The following test environment gives the opportunity to show and test how the module works integrated with the Services DAO <br>
Address: 193.140.239.52:1098 <br>
Test Admin User:<br>
username: Ekin<br>
password: 1parola1<br>

## Prerequisites
To run the application, `Docker` should be installed and configured on your system. Necesseary information is [here](https://docs.docker.com/engine/install/).<br>
<br>
In order to access the database, mysql client is to be installed on your system. Necessary information is [here](https://dev.mysql.com/doc/mysql-shell/8.0/en/mysql-shell-install-linux-quick.html).<br>
Example for Ubuntu:
```shell
sudo apt-get update
sudo apt-get install mysql-client
```
In order to build microservices individually and run the tests from command prompt dotnet sdk 3.1 or higher should be installed on your system. Necessary information is [here](https://docs.microsoft.com/tr-tr/dotnet/core/install/linux-ubuntu).<br>
Example for Ubuntu:
```shell
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install apt-transport-https
sudo apt install dotnet-sdk-3.1
```

## Install and Run

Build and run with docker-compose <br>

Under project folder, type 
```shell
docker-compose up --build
```
Docker-compose downloads/creates(if necessary) builds and runs;

- reputation_db Mysql: 5.7 instance image 
- vote_db Mysql: 5.7 instance image 
- RabbitMQ instance
- votingengine instance
- reputationservice instance

The voting engine and reputation service microservices must join the network of the project they are integrated with and must subscribe to RabbitMQ in that network. <br>
Since this module has been developed as a separate project, it has been orchestrated to have its own network for testing purposes and to subscribe to RabbitMQ in that network.<br>

When all containers are up, you can access the application's "Welcome page"s and databases from addresses below;<br>
- voting engine       : http://localhost:8897 
- reputation service  : http://localhost:8898
- voting database     : localhost:3312
- reputation database : localhost:3313
- RabbitMQ            : http://localhost:15672

"Welcome page" shows the status and error logs of the application. <br>

After proper setup, the two mysql instances should have database named 'daovotesdb' and 'daoreputataiondb'. The databases should have tables below;<br>
```shell
daovotesdb<br>
- Votes
- Voting
- __EFMigrationHistory

daoreputataiondb<br>
- UserReputationHistories
- UserReputationStakes
- __EFMigrationHistory
```

## Usage

Voting engine and Reputation service has been developed so that users can vote for a post using their reputation and decide whether the post is appropriate or accepted. Voting engine and Reputation service microservices can be used in relation to an application database where users and jobs are registered.<br>
The main application that the voting engine will integrate with should use the database context of the voting engine application.<br>

Due to its microservice architecture, the voting engine comminucates with the reputation service internally thru address "http://dao_reputationservice".

## Testing
A mysql database should be up and running with a testing environment setup.
To run tests from terminal dotnet sdk should be installed on your system.

The easiest and recommended way is pulling a mysql docker image and run in a docker container with minimum parameters.
```shell
docker run --detach --name=test-mysql -p 3317:3306  --env="MYSQL_ROOT_PASSWORD=mypassword" mysql
```
To access the mysql instance in the container:

```shell
docker exec -it test-mysql bash -l
To access the database from the mysql container terminal :

mysql -u root -p
Enter Password: **********
mysql>
```
The root password, the expose port and many other parameters can be changed optionally. The test database connection string should be written under the PlatformSettings section taking place in the \PathToSolution\DAO_Votingengine\appsettings.test.json file and rebuild with command dotnet build.<br>
Example:
```json
"PlatformSettings": {
    "DbConnectionString": "Server=localhost;Port=3313;Database=test_votingdb;Uid=root;Pwd=mypassword;",
    ...
}
```
After configuring the database, run the following commands from the test project directory \PathToSolution\RFPPortal_Tests\.
```shell
dotnet test --filter DisplayName~
dotnet test --filter DisplayName~
dotnet test --filter DisplayName~
```
Code documentation files in format is autogenerated everytime the project is build under bin folder.

HTTPS Configuration
The process below is explained for dao_votingengine and is identical for dao_reputationservice.
To enable HTTPS, https settings of the application should be added to the Docker Container configuration and the an ssl certificate file should be introduced if necessary.

Example of enabling HTTPS using 'docker-compose.override.yml' file:
```yml
dao_votingengine:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+;http://+:80
      - ASPNETCORE_HTTPS_PORT=443
```
https://+ is added to ASPNETCORE_URLS and 443 port is defined as https port with adding ASPNETCORE_HTTPS_PORT=443

An ssl certificate can be generated and placed in a location on the machine where the docker container is running.
One way to generate an SSL certificate is explained here.

The definition of the generated ssl certificate in the docker compose file is as follows:
```yml
dao_votingengine:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+;http://+:80
      - ASPNETCORE_HTTPS_PORT=443

# Password for the certificate
      - ASPNETCORE_Kestrel__Certificates__Default__Password=< password of the generated certificate >
# Path of the certificate file
      - ASPNETCORE_Kestrel__Certificates__Default__Path= < location of the ssl certificate in docker container. Example: '/https/aspnetapp.pfx' > 
    volumes:
# Mount the local volume where the certificate exists to docker container
      - < location of the ssl certificate in the host machine> : < location of the ssl certificate in docker container. Example: '~/.aspnet/https:/https:ro'>
```
