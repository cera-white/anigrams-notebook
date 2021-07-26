using System.ComponentModel.DataAnnotations;

namespace AnigramsNotebook.EF
{
    class NBImageMetaData
    {
        [Required(ErrorMessage = "*Project is required")]
        [Display(Name = "Project")]
        public int NBProjectId { get; set; }

        [Required(ErrorMessage = "*Category is required")]
        [Display(Name = "Category")]
        public int ObjectCategoryId { get; set; }
    }

    [MetadataType(typeof(NBImageMetaData))]
    public partial class NBImage { }
}
