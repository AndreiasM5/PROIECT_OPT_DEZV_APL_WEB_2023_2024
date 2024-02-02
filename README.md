# Proiect_OptDezvAplWeb_2023-2024

## Project Setup

### Backend
1. Navigate to the Backend directory:
    ```bash
    cd Backend
    ```

2. Run Entity Framework migrations:
    ```bash
    dotnet ef migrations add InitialCreate
    ```

3. Apply migrations to update the database:
    ```bash
    dotnet ef database update InitialCreate
    ```

4. Run the project:
    ```bash
    dotnet run
    ```

### Frontend
1. Navigate to the Frontend directory:
    ```bash
    cd Frontend
    ```

2. Install npm packages:
    ```bash
    npm install
    ```

3. Run the Angular application:
    ```bash
    ng serve
    ```

## Authentication Process

The authentication process in this project is inspired by the following article: [JWT Authentication and Authorization in .NET 6.0 with Identity Framework](https://www.c-sharpcorner.com/article/jwt-authentication-and-authorization-in-net-6-0-with-identity-framework/).

Feel free to add more sections or details specific to your project as needed. Additionally, consider including information about environment variables, configuration files, or any other setup steps that might be relevant to users running or contributing to your project.
