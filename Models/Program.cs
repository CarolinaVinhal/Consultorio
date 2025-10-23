using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Consultorio.Data;
using Consultorio.Models;

var builder = WebApplication.CreateBuilder(args);

// Porta fixa (opcional, facilita testes)
builder.WebHost.UseUrls("http://localhost:5099");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=escola.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

var webTask = app.RunAsync();
Console.WriteLine("API online em http://localhost:5099 (Swagger em /swagger)");

Console.WriteLine("=========== ConsultorioDb ===========");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Escolha uma opção:");
    Console.WriteLine("1 - Cadastrar paciente");
    Console.WriteLine("2 - Listar paciente");
    Console.WriteLine("3 - Atualizar paciente (por Id)");
    Console.WriteLine("4 - Remover paciente (por Id)");
    Console.WriteLine("0 - Sair");
    Console.Write("> ");

    var opt = Console.ReadLine();

    if (opt == "0") break;

    switch (opt)
    {
        case "1": await CreatePacienteAsync(); break;
        case "2": await ListPacienteAsync(); break;
        case "3": await UpdatePacienteAsync(); break;
        case "4": await DeletePacienteAsync(); break;
        default: Console.WriteLine("Opção inválida."); break;
    }
}

await app.StopAsync();
await webTask;

async Task CreatePacienteAsync()
{
    Console.Write("Nome: ");
    var name = (Console.ReadLine() ?? "").Trim();

    Console.Write("Email: ");
    var email = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
  

    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
    {
        Console.WriteLine("Nome e Email são obrigatórios.");
        return;
    }
    

    using var db = new AppDbContext();
    var exists = await db.Pacientes.AnyAsync(s => s.Email == email);
    if (exists) { Console.WriteLine("Já existe um paciente com esse email."); return; }

    var paciente = new Paciente { Name = name, Email = email, EnrollmentDate = DateTime.UtcNow };
    db.Pacientes.Add(paciente);
    await db.SaveChangesAsync();
    Console.WriteLine($"Paciente cadastrado com sucesso! Id: {paciente.Id}");
}

async Task ListPacienteAsync()
{
    using var db = new AppDbContext();
    var pacientes = await db.Pacientes.OrderBy(s => s.Id).ToListAsync();

    if (pacientes.Count == 0) { Console.WriteLine("Nenhum paciente encontrado."); return; }

    Console.WriteLine("Id | Name                 | Email                    | EnrollmentDate (UTC)");
    foreach (var s in pacientes)
        Console.WriteLine($"{s.Id,2} | {s.Name,-20} | {s.Email,-24} | {s.EnrollmentDate:yyyy-MM-dd HH:mm:ss}");
}

async Task UpdatePacienteAsync()
{
    Console.Write("Informe o Id do paciente a atualizar: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); return; }

    using var db = new AppDbContext();
    var paciente = await db.Pacientes.FirstOrDefaultAsync(s => s.Id == id);
    if (paciente is null) { Console.WriteLine("Paciente não encontrado."); return; }

    Console.WriteLine($"Atualizando Id {paciente.Id}. Deixe em branco para manter.");
    Console.WriteLine($"Nome atual : {paciente.Name}");
    Console.Write("Novo nome  : ");
    var newName = (Console.ReadLine() ?? "").Trim();

    Console.WriteLine($"Email atual: {paciente.Email}");
    Console.Write("Novo email : ");
    var newEmail = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    if (!string.IsNullOrWhiteSpace(newName)) paciente.Name = newName;
    if (!string.IsNullOrWhiteSpace(newEmail))
    {
        var emailTaken = await db.Pacientes.AnyAsync(s => s.Email == newEmail && s.Id != id);
        if (emailTaken) { Console.WriteLine("Já existe outro paciente com esse email."); return; }
        paciente.Email = newEmail;
    }

    await db.SaveChangesAsync();
    Console.WriteLine("Paciente atualizado com sucesso.");
}

async Task DeletePacienteAsync()
{
    Console.Write("Informe o Id do paciente a remover: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); return; }

    using var db = new AppDbContext();
    var paciente = await db.Pacientes.FirstOrDefaultAsync(s => s.Id == id);
    if (paciente is null) { Console.WriteLine("Estudante não encontrado."); return; }

    db.Pacientes.Remove(paciente);
    await db.SaveChangesAsync();
    Console.WriteLine("Paciente removido com sucesso.");
}
