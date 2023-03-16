# EtherScanTest

EtherScanTest is a C# .Net program for getting blocks and transactions data from https://api.etherscan.io and insert to database (mysql).

## Configuration
1. Make sure you have mysql installed
2. Create a database name 'etherscan'. For the step to create mysql database could be found in https://dev.mysql.com/doc/refman/8.0/en/creating-database.html
3. Import the database scripts in folder DbScripts
3. Open the file appsettings.json and modify the connnection string to fit with your local database.
For exammple: "ConnectionString": "Server=localhost;Database=etherscan;user=root; password=12345678"
- Change Server (localhost) to your local server name
- Database should be 'etherscan' as created at step 1
- user (root): change with your database username
- password (12345678) : change with your database password

## Run the program
1. Option 1: Run by Visual Studio. 
Open the project with Visual Studio 2022 or above and run the program

2. Option 2: Run by command line.
Open the command line as Administrator, navigate to the project folder.
Type the command line to execute the program: dotnet run -c Local 

## Specs
1. Mysql version 8.0.2
2. .Net Core version 6.0.101
3. Visual Studio 2022 or above