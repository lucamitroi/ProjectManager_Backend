# Backend for the ProjectManager Web Application
The ProjectManager Applications is a Web Application that stores projects for different users. The projects also support the addition of specific tasks. The Application has complete User Registration and Authentification with JWT. All the Data is stored inside of an SQL Database that is connected to the Frontend using an API created using ASP.NET.

This is the Backend for the ProjectManager Web Application. This Backend contains the SQL configuration, the Authentification and Project Controllers and all the structures related to the stored data inside the Database. The User Passwords are secured using SHA256 and are not directly visible inside the Database.

## Backend Setup
1. Clone the repository in you local storage.
2. Run the sqlQueries.sql file using any Database Manager that supports MSSQL in order to create the Database in your localhost.
3. Run the command 'dotnet build' in your cloned folder.
4. Run the command 'dotnet watch run' to start the API.

## Endpoints
![image](https://github.com/user-attachments/assets/e65f62fe-0e42-4323-be8f-da0bc8d99e2e)
