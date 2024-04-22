using System.ComponentModel.DataAnnotations;

namespace DemoWE.Models
{
    public class Department
    {

        [Key]
        public int DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? DepartmentManagerID { get; set; }

    }
}
