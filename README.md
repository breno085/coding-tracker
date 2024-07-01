The project was completed with all requirements met (except for using Dapper as specified; instead, I used ADO.NET), including all the challenges.

## Tips

1. Recomended build order:
 - configuration file, model, database/table creation, CRUD controller (where the operations will happen), TableVisualisationEngine (where the consoleTableExt code will be run) and finally: validation of data.

2. Sqlite doesn't support dates. We recommend you store the datetime as a string in the database and then parse it using C#. You'll need to parse it to calculate the duration of your sessions.

3. Don't forget the user input's validation: Check for incorrect dates. What happens if a menu option is chosen that's not available? What happens if the users input a string instead of a number? Remember that the end date can't be before the start date.

## Folder Structure Description

- Models: Contains the entities that are mapped to database tables.
- Repositories: Contains the data access logic and database manipulation methods.
- Controllers: Manages business logic and user interactions.
- Services: Classes that provide important functionalities and services to the application.
- Utils/Helpers: Auxiliary classes that assist with various utility tasks within the project.

## Project Overview 

### Design and OOP Principles

This code was organized following the Separation of Concerns (SoC) design principle and Object-Oriented Programming (OOP) principles. I created separate classes for different responsibilities within my CRUD application, following this suggested structure:

- Main Program Class: Coordinates the application flow and user interactions via the console.
- Database Manager Class: Handles SQLite database interactions.
- User Interface Class: Manages user input and output.
- Coding Session Class: Represents a coding session object (CodingTracker class).
- Coding Repository Class: Manages CRUD operations for coding sessions.

Additional Classes:

- Generate Records Class: Creates 100 random coding session records for testing.
- StopWatch Class: Measures and manages the duration of coding sessions.
- Spectre Table Renderer Class: Renders data in a table format in the console using the Spectre.Console library.

## Some Resources
[Spectre Console](https://spectreconsole.net/) documentation.
Using Configuration Manager
[Parsing DateTime in C#](https://medium.com/@Has_San/datetime-in-c-1aef47db4feb)





