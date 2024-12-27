using System.ComponentModel.DataAnnotations;

namespace FINAL2.Model
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        // public int PersonId {  get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public double Salary { get; set; }
        public string Role { get; set; }
        public bool IsBlocked { get; set; } = false;
        //public ICollection<Loan> Loans { get; set; }


    }
    public static class Role
    {
        public const string Accountant = "Accountant";
        public const string User = "User";
    }


}
