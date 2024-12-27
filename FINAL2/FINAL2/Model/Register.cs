namespace FINAL2.Model
{
    public class Register
    {

        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public double Salary { get; set; }
        public string Role { get; set; }

        public bool IsBlocked { get; set; } = false;
    }
}