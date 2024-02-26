using System.ComponentModel.DataAnnotations;

namespace DemoWE.Models
{
    public class User
    {

        [Key]
        public int EmployeeNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int DepartmentID { get; set; }

    }
}
