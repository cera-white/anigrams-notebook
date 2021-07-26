using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AnigramsNotebook.EF
{
    class NBNoteMetaData
    {
        [Required(ErrorMessage = "*Project is required")]
        [Display(Name = "Project")]
        public int NBProjectId { get; set; }

        [Required(ErrorMessage = "*Category is required")]
        [Display(Name = "Category")]
        public int ObjectCategoryId { get; set; }

        [Required(ErrorMessage = "*Name is required")]
        [MaxLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string Name { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "*Description is required")]
        public string Description { get; set; }
    }

    [MetadataType(typeof(NBNoteMetaData))]
    public partial class NBNote { }
}
