# Online Learning Management System (LMS)

## Project Overview
The LMS is an online platform that facilitates the selling and purchasing of courses. Teachers can create and sell their courses while students can enroll and track their progress. The system aims to enhance the learning experience through interactive features and streamlined processes.

## Tech Stack
- **Backend**: ASP.NET Core, C#  
- **Frontend**: HTML, CSS, JavaScript  
- **Database**: SQL Server

## Key Features
- **Course Marketplace**: A platform where teachers can showcase and sell their courses.
- **Student Progress Tracking**: Students can monitor their progress throughout courses.
- **Enrollments**: Students can easily enroll in courses of their choice.
- **Payments**: Secure payment processing for course purchases.
- **Course Management**: Teachers can manage their courses, including updates and modifications.

## Authentication Details
- **ASP.NET Identity** is used for authentication.
- **Google OAuth** login is available for user convenience.

## Default Seeded Accounts
- **Admin**:  
  Login: admin@lms.com  
  Password: Admin123@
- **Sample Teacher Account**:  
  teacher1@lms.com / Teacher@123
- **Sample Student Account**:  
  student1@lms.com / Student@123  

## Project Setup and Installation Instructions
1. Clone the repository:  
   `git clone https://github.com/PasanNevinda/LMS.git`
2. Navigate to the project directory:  
   `cd LMS`
3. Restore dependencies:  
   `dotnet restore`
4. Build the project:  
   `dotnet build`
5. Update the configuration settings as needed.
6. Run the application:  
   `dotnet run`

## Database Seeding Information
Upon running the application for the first time, the database will be seeded with the default accounts outlined above to allow for easy testing and development. Make sure to check your connection strings in the appsettings.json file.

## Running and Testing Instructions
- To run the application, make sure the web server is running, and navigate to the given localhost URL in your browser.  
- For testing, use the default seeded accounts to log in and explore the functionalities.
