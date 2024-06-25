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
~~Challenge 1 - Stop Watch~~

~~Inserir uns 100 valores aleatórios desde começo do ano até hoje para facilitar no restante das challenges~~

Challenge 2 - Ideia para filtrar os records:
por dias:
 - pedir pra inserir o primeiro dia até a data final que quero filtrar
por semanas:
- pedir pra inserir a semana inicial e a semana final do ano (vai mostrar todos os dias daquelas semanas ?)
por ano:
 - pedir para inserir o ano (vai mostrar todos os dias daquele ano que programei)

Challenge 3 - Calcular a média por período:
 - No final dos filtros diário, semanal e anual, mostrar um texto dizendo
qual foi o total de horas programando e a média diária/semanal/anual
- Diário: Somar a duration the todos os records e dividir pelo número de dias percorridos desde o primeiro record inserido até datetime.now. Mostrar o número de dias percorridos, o número de dias que programei, as horas totais que programei e a média diária.

Challenge 4 - Colocar metas de programação:
 - Criar um menu com duas opções: 1 - Inserir metas e 2 - mostrar progresso
Selecionando 1 - Inserir metas:
 - Pedir para inserir o prazo final
 - Pedir para inserir o numero de horas total
 - Calcular quantas horas o usuário precisará programar por dia até o prazo final, a partir de hoje, para atingir a meta, e mostrar para o usuário (fazer uns testes com números de horas e prazos absurdos na plataforma da DIO para ver como ela responde e eu ter uma ideia do que fazer no meu projeto)
Selecionando 2 - mostrar progresso:
 - Eu vou subtrair o número de horas total pelo número de horas que foram inseridas a partir do dia inicial até o dia final e calcular a média
de horas com esse valor dividido pelo numero de dias restantes e mostrar pro usuário
 - caso o número de horas que o usuário precise programar para atingir a meta for maior que 24horas, mostrar uma mensagem pro usuário para criar novas metas, pois infelizmente não vai ser possível cumprir essa


