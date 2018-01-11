using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmiverse.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Movie title required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Producer required")]
        public string Producer { get; set; }
        [DisplayName("Year of Release")]
        [Range(0, 2020, ErrorMessage = "Can only be upto year 2020 ")]
        public int YearOfRelease { get; set; }
        [Required(ErrorMessage = "Running time required")]
        [DisplayName("Running Time")]
        [Range(1, 300, ErrorMessage = "Can only be between 1 to 300 minutes ")]
        public int RunningTime { get; set; }
        public string Plot { get; set; }
        public byte[] Poster { get; set; }
    }
    public class MovieViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Movie title required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Producer required")]
        public string Producer { get; set; }
        [AtleastOneItem(ErrorMessage = "Atleast one actor required")]
        public List<string> Actors { get; set; }
        [DisplayName("Year of Release")]
        [Range(0, 2020, ErrorMessage = "Can only be upto year 2020 ")]
        public int YearOfRelease { get; set; }
        [Required(ErrorMessage = "Running time required")]
        [DisplayName("Running Time")]
        [Range(1, 300, ErrorMessage = "Can only be between 1 to 300 minutes ")]
        public int RunningTime { get; set; }
        public string Plot { get; set; }
        public byte[] Poster { get; set; }
    }

    //create a custom validation attribute to check if list has atleast one item
    public class AtleastOneItem : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                return list.Count > 0;
            }
            return false;
        }
    }
}