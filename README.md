## Tips

1. Recomended build order:
 - configuration file, model, database/table creation, CRUD controller (where the operations will happen), TableVisualisationEngine (where the consoleTableExt code will be run) and finally: validation of data.

2. Sqlite doesn't support dates. We recommend you store the datetime as a string in the database and then parse it using C#. You'll need to parse it to calculate the duration of your sessions.

3. Don't forget the user input's validation: Check for incorrect dates. What happens if a menu option is chosen that's not available? What happens if the users input a string instead of a number? Remember that the end date can't be before the start date.

## OOP Principles

This code was organized according to Object-Oriented Programming (OOP) principles. I created separate classes for different responsibilities within my CRUD application, following this suggested structure:

- Main Program Class: Responsible for coordinating the flow of the application and interacting with the user through the console.
- Database Manager Class: Handles all interactions with the SQLite database.
- User Interface Class: Manages user input and output operations.
- Coding Session Class: Represents a coding session object (CodingTracker class).
- Coding Controller Class: Manages the creation, retrieval, updating, and deletion (CRUD) operations for coding sessions.
  (Example: Retrieves user input, stores it in a Coding Session object, and sends it to the database.)
