namespace Consultorio.Models;

public class Paciente{
    public int Id {get;set;}
    public string Name {get;set;} = "";
    public string Email {get;set;} = "";
    public DateTime EnrollmentDate {get;set;} = DateTime.UtcNow;
}
