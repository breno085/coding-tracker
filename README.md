The project was completed with all requirements met, except for using Dapper as specified; instead, I used ADO.NET, including all the challenges.

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

Restando:
Challenge 4 - Colocar metas de programação (vou ver se eu faço com C# ou usando SQL queries):
 - Criar um menu com duas opções: 1 - Insert new goal e 2 - Visualize currenty goal que tem duas opções: 1 Insert today hours e 2 - show progress
Selecionando 1 - Inserir metas:
 - Pedir para inserir o numero de horas total
 - Pedir para inserir quantas horas pretente programar por dia (colocar um limite de 12)
 - Pedir para inserir quantos dias pretende estudar por semana
 - Mostrar o dia e quanto dias será necessário para atingir a meta estudando a quantidade de horas necessárias (Your estimate closing date {data} studying {horas} per day)
- Como fazer o cálculo: numeros de dias para atingir a meta será igual ao número de horas totais dividido pelo numero de horas por dia.

Selecionando 2 - Visualize currenty goal progress:
1 - Insert how many hours you coded today (devo pedir pro usuário inserir start time e end time para armazenar na tabela já criada)
o usuário insere quantas horas programou hoje (start time and end time) e o programa mostra a mensagem: Your estimate closing date {data} studying {hours} per day. (essa data vai se atualizar constatemente baseado no número de horas inseridas diariamente)
Hours left: {hours} (vai subtrair o numero de horas total pelas horas inseridas)
COMO FAZER: 
1. Usar o Insert() método para o usuário colocar as horas no banco de dados.
2. eu pego as horas totais - totalHours que é studyGoalUserInput[0] e subtraio pela soma do Duration dos dados de todos os dias desde startDate.
3. as horas restantes acima eu divido por hoursPerDay(studyGoalUserInput[1]) e com isso eu tenho a quantidade de dias restantes, somo essa quantidade
com o dia de hoje para pegar close date.
4. Após isso mostrar a mensagem pro usuário: Your estimate closing date is{date} studying hoursPerDay(studyGoalUserInput[1]) per day
5. Eu precisaria criar um banco de dados para armazenar totalHours, hoursPerDay e talvez startDate


2 - Show progress (talvez não precise)
Mostrar uma tabelinha com Id, Date e Total Hours (aacho que devo reaproveitar a tabela já criada a invés de criar uma nova)
Em seguida a mesma informação acima:
Your estimate closing date {data} studying {hours} per day.
Hours left: {hours}




