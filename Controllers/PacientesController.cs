using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Consultorio.Data;
using Consultorio.Models;

namespace Consultorio.Controllers;

[ApiController]
[Route("api/v1/[controller]")] // Utiliza nome  do arquivo/classe
public class PacientesController : ControllerBase{
    private readonly AppDbContext _db;
    public PacientesController(AppDbContext db)=>_db =db;

    //GET /api/v1/pacientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Paciente>>> GetAll()
        => Ok(await _db.Pacientes.OrderBy(s=>s.Id).ToListAsync());
    
    //GET /api/v1/pacientes/1(id)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Paciente>> GetById(int id)
        => await _db.Pacientes.FindAsync(id) is { } s ? Ok(s) : NotFound();

    //POST  /api/v1/pacientes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Paciente s){
        if(!string.IsNullOrWhiteSpace(s.Email) &&
            await _db.Pacientes.AnyAsync(x=>x.Email == s.Email)){
                return Conflict(new {error = "Email já cadastrado"});
            }
        _db.Pacientes.Add(s);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof (GetById), new {id = s.Id}, s);
    }

    //PUT /api/v1/[pacientes]/1(id)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Paciente s){
        s.Id = id;

         if(!string.IsNullOrWhiteSpace(s.Email) &&
            await _db.Pacientes.AnyAsync(x=>x.Email == s.Email && x.Id != id)){
                return Conflict(new {error = "Email já cadastrado."});
            }

        if(!await _db.Pacientes.AnyAsync(x=> x.Id == id)) return NotFound();

        _db.Entry(s).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return Ok();
    } 

    // DELETE /api/v1/pacientes/1(id)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete (int id){
        var s = await _db.Pacientes.FindAsync(id);
        if(s is null) return NotFound();

        _db.Pacientes.Remove(s);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
