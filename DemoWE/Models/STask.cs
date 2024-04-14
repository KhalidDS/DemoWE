using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DemoWE.Models
{
    public class STask
    {
        [Key]

        public int TaskID { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public string? sfile { get; set; }
        public Status Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }

        public int CreatedBy { get; set; }
        public int AssignedTo { get; set; }
        public PriorityLevel Priority { get; set; }

    }
}
