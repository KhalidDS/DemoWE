namespace DemoWE.Models
{
    public class Project
    {
        public int ProjectID { get; set; }
        public string? ProjectTitle { get; set; }
        public string? ProjectDescription { get; set; }
        public string? pfile { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public PriorityLevel Priority { get; set; }
        public DepartmentNames AssignedDepartmentID { get; set; }
        public Status Status { get; set; }
    }
}
