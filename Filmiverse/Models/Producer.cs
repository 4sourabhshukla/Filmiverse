using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmiverse.Models
{
    public class Producer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Producer's name required")]
        public string Name { get; set; }
        [DisplayName("Gender")]
        public Gender Sex { get; set; }
        [Required(ErrorMessage = "Date of birth required")]
        [DisplayName("Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        public string Bio { get; set; }
    }
}
