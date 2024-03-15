using System.ComponentModel.DataAnnotations;

namespace DemoWE.Models
{
    public class Request
    {
        [Key]
        public int RequestID { get; set; }
        public string RequestTitle { get; set; }
        public string RequestDescription { get; set; }
        public string Rfile { get; set; }
        public Status Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public int CreatedBy { get; set; }
    }
}