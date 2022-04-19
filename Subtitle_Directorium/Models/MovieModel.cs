using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Subtitle_Directorium.Models
{
    public class MovieModel
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "How does your movie have no Title?")]
        [StringLength(60, ErrorMessage = "Please make sure your title is shorter than 60 characters")]
        [Display (Name="Title: ")]
        public string Name { get; set; }
        [Required(ErrorMessage = "There should be a Rating number provided.")]
        [Range (0,10)]
        [Display (Name="Rating: ")]
        public float Rating { get; set; }
        [Required(ErrorMessage = "There needs to be an Image URL provided.")]
        [Display(Name = "Image URL: ")]
        public string URL { get; set; }
        //[Required(ErrorMessage = "There needs to be a description.")]
        [StringLength (250, ErrorMessage="Please make sure your description is not longer than 250 characters")]
        [Display(Name = "Short description: ")]
        [Required (ErrorMessage = "Please make sure there is text in the description field")]
        public string Description { get; set; }
        public static string[] AllGenres = { "Action", "Animated", "Comedy", "Drama", "Fantasy", "Horror", "Mystery", "Romance", "Thriller", "Western", "Sci-Fi"};
        [Required]
        [Display(Name = "Movie genre: ")]
        public string Genre { get; set; }
        public List<Subtitle> movieSubtitles { get; set; }
        public MovieModel()
        {
            movieSubtitles = new List<Subtitle>();
        }
        public MovieModel(string Name, float Rating, string URL, string Description, string Genre)
        {
            this.Name = Name;
            this.Rating = Rating;
            this.URL = URL;
            this.Description = Description;
            this.Genre = Genre;
            movieSubtitles = new List<Subtitle>();
        }
    }
}