*Trabalho: Banco de Dados + API em C#
Alunas: Carolina Marques Vinhal de Carvalho, RA-22303413 e Leticia Seto, RA-22306613
Professor: Fabio Ramos

PASSOS:
1- Apos baixar o .NET na sua maquina, crie a estrutura da API com o comando no terminal do VsCode:
dotnet new console -n ConsultorioDB

2-Depois entre pasta ConsultorioDB e baixe alguns pacotes para a API e o banco de dados funcionarem:
cd .\ConsultorioDB\
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet new tool-manifest
dotnet tool install dotnet-ef
dotnet add package Swashbuckle.AspNetCore

3-Depois, adcione os arquivos de Models, Controllers e Data na mesma pasta que ConsultorioDB.

4-Depois rodar no terminal apenas uma vez para criar o banco de dados:
dotnet ef migrations add InitialCreate
dotnet ef database update

5-Por fim rode o programa todo:
dotnet run

ENTIDADES:

ROTAS:
<img width="1887" height="1692" alt="Screenshot 2025-10-23 151129" src="https://github.com/user-attachments/assets/d8897049-bf4e-4191-aca1-6b8ac43d2531" />

