using System.ComponentModel.DataAnnotations;

namespace Mango.Service.EmailAPI.Models
{
    public class EmailLogger
    {
        [Key]
        public int EmailId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime? EmailSent { get; set; }
    }
}
