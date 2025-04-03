# Backend for the ProjectManager Web Application
The ProjectManager Applications is a Web Application that store projects for different users. The projects also support the addition of specific tasks. The Application has complete User Registration and Authentification with JWT. All the Data is stored inside of an SQL Database that is connected to the Frontend using an API created using ASP.NET.

This is the Backend for the ProjectManager Web Application. This Backend contains the SQL configuration, the Authentification and Project Controllers and all the structures related to the stored data inside the Database.

## Backend Setup
1. Clone the repository in you local storage.
2. Run the sqlQueries.sql file using any Database Manager that supports MSSQL in order to create the Database in your localhost.
3. Run the command 'dotnet build' in your cloned folder.
4. Run the command 'dotnet watch run' to start the API.
