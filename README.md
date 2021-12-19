# ServicesDAO_VotingEngine
VotingEngine implements the core voting functionality in ServicesDAO. Repository contains two application for reputation management and voting process.

The project was developed with dotnet core 3.1 in C# as language.<br>
jQuery UI - v1.10.4 and Razor was used to develop the frontend.<br>

Test Environment<br>
Votin engine and reputation service is a seperated module that works integrated with the "Services DAO" project.<br>
The following test environment gives the opportunity to show and test how the module works integrated with the Services DAO <br>
Address: 193.140.239.52:1098
Test Admin User:
username: Ekin
password: 1parola1

Setup
Docker must be installed on your system. <br>
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
- voting engine : http://localhost:8897 
- reputation service: http://localhost:8898
- voting database : localhost:3312
- reputation database : localhost:3313
- RabbitMQ : http://localhost:15672

"Welcome page" shows the status and error logs of the application. <br>

To access databases, Mysql client must be installed on the environment.

After proper setup, the mysql instance should have a database named 'daorfpdb'. The daorfpdb database should have tables below;

Users
UserLogs
Rfps
RfpBids
ErrorLogs
ApplicationLogs
__EFMigrationHistory
Usage
Your smtp information must be entered to SMPT section in the appsettings.json file. This is required to create a user because the registration process is completed with the activation mail.
To activate a user without email registeration, the IsActive column in the daorfpdb.Users table in the database should be set to 1 manually.

Example:

	docker exec -it rfpportal-rfpportal_db bash -l
	/# mysql -u root -p daorfpdb
	> update Users set IsActive = 1 where Username = 'username of the user that is required to be activated';
Where "rfpportal-rfpportal_db" is the name of the container. All running containers can be listed with docker ps -a command from the terminal.

Email Information in appsettings.json

"EmailAddress": "info@ekonteknoloji.com",
"EmailDisplayName": "RFP Portal",
"EmailPassword": "********",
"EmailHost": "smtp.zoho.com",
"EmailPort": "587",
"EmailSSL": "true"
Creating a Public User
Simply Click the login button in the upper right corner. A model will appear with a 'Sign Up' tab. Fill out the form and hit the sign up button. If you have entered the necessary information in the relevant places in the appsettings.json file an activation email will be sent to you. Click on the link specified in the activation e-mail. You will be redirected to the application url using your default browser. Once the page appeared, you should see a toaster indicating that the activation was successful.

Creating an Internal User
Internal accounts are recognized by RFPPortal via the api below, provided by DEVxDAO.
https://backend.devxdao.com/api/va/email/
The api needs two parameters;

Email
DxDApiToken (The token is added to Headers/Authorization of the HttpWebRequest)
If DxDApiToken is provided signing up with an internal email automatically creates an internal user.
To create an internal user without the api call, the UserType column in the daorfpdb.Users table in the database should be set to Internal manually.

Example:

	docker exec -it rfpportal-rfpportal_db bash -l
	/# mysql -u root -p daorfpdb
	> update Users set UserType = 'Internal' where Username = 'username of the user that is required to be Internal';
Creating an Admin User
To create an admin user, first a public user should be created and user type should be updated to 'admin' manually from the database. The database can be accessed by docker cli.

	docker exec -it rfpportal-rfpportal_db bash -l
	/# mysql -u root -p daorfpdb
	> update Users set UserType = 'Admin' where Username = 'username of the user that is required to be admin';
Creating RFP
When logged in to the application as an Admin user, there appears a 'New RFP' navigation button on the top left corner in addition to 'My Bids' and 'RFP List' navigation buttons. Clicking it redirects to an RFP form page including the fields below;

Type of the Job
Job Description
Total Price for the Job
Job Start Date
Job Completion Date
Filling the form and submitting it creates an internal RFP which only the internal users are able to see and bid on.
The posted RFP is added to the list of RFPs shown in the main page.

RFP Card
RFP card shows;

Job Type
Posted date
Job Description
Time frame
Type of the RFP (public/internal)
Days remaining to type/status change (internal/public/expired)
Status of the RFP (completed/continues/expired)
VA users are expected to submit bids to rfps that are in internal state and the winning bid to be determined within 15 days.
If a winner still has not been determined at the end of the 15th day, the RFP becomes public. Now public users can view and bid the RFP on their page.

Creating, Editing and Deleteing a bid
Creating
Clicking on the RFP card that you want to bid, redirects you to RFP Detail page. RFP Detail page shows all RFP information and all the bids as a list.
If bidding continues, there is a New Bid button on the top-right corner. Clicking it brings a modal bidding form including fields below;

Name and Surname (autofilled)
Username (autofilled)
Email (autofilled)
Timeframe
Amount
Additional Notes
You can enter proposed amount, timeframe and additional notes (optional) and submit it by clicking the Submit Bid button placed on the bottom-right of the modal. After proper submitting, there toasts a green Success message on the top-right corner of the page and the submitted bid is added on the bid list below the RFP information pane.

Editing and Deleting
In the RFP Details page, if the user has an active bid, the row in the list below the RFP information pane, has 'Edit' and 'Delete' buttons which allows the user delete and edit their active bids.

Choosing the Winning Bid
Only an admin type of user is able to choose the winning bid.
Admin users' RFP detail page has a star button placed on the end of the every bid row. Clicking it brings a confirm pop-up. After confirming, there appears a green 'Success' toaster on the top-right corner of the page and the star in the selected bid row becomes yellow.
Choosing the winning bid changes the status of the RFP from 'bidding continues' to 'bidding ended'.

My Bids Page
Clicking on the 'My Bids' navigator button at the top-left corner of the home page, every user is redirected to the 'My Bids' page.
The users can see their bidding history and RFP details of the belonging bid.

Entities
ApplicationLog
ErrorLog
Rfp
RfpBid
User
UserLog
Testing
A mysql database should be up and running with a testing environment setup.
To run tests from terminal dotnet sdk should be installed on your system.

The easiest and recommended way is pulling a mysql docker image and run in a docker container with minimum parameters.

docker run --detach --name=test-mysql -p 3317:3306  --env="MYSQL_ROOT_PASSWORD=mypassword" mysql
To access the mysql instance in the container:

docker exec -it test-mysql bash -l
To access the database from the mysql container terminal :

mysql -u root -p
Enter Password: **********
mysql>
The root password, the expose port and many other parameters can be changed optionally. The test database connection string should be written under the PlatformSettings section taking place in the \PathToSolution\RFPPortalWebsite\appsettings.json file and rebuild with command dotnet build.
Example:

"PlatformSettings": {
    "DbConnectionString": "Server=localhost;Port=3317;Database=test_daorfpdb;Uid=root;Pwd=mypassword;",
    "InternalBiddingDays": 21,
    "PublicBiddingDays": 21
}
After configuring the database, run the following commands from the test project directory \PathToSolution\RFPPortal_Tests\.

dotnet test --filter DisplayName~Authorization_Tests
dotnet test --filter DisplayName~BidController_Tests
dotnet test --filter DisplayName~RfpController_Tests
Code documentation files in format is autogenerated everytime the project is build under bin folder.

HTTPS Configuration
To enable HTTPS, https settings of the application should be added to the Docker Container configuration and the an ssl certificate file should be introduced if necessary.

Example of enabling HTTPS using 'docker-compose.override.yml' file:

rfpportalwebsite:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+;http://+:80
      - ASPNETCORE_HTTPS_PORT=443
https://+ is added to ASPNETCORE_URLS and 443 port is defined as https port with adding ASPNETCORE_HTTPS_PORT=443

An ssl certificate can be generated and placed in a location on the machine where the docker container is running.
One way to generate an SSL certificate is explained here.

The definition of the generated ssl certificate in the docker compose file is as follows:

rfpportalwebsite:
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
