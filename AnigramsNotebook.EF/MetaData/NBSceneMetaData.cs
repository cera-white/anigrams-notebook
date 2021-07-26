using System.ComponentModel.DataAnnotations;

namespace AnigramsNotebook.EF
{
    class NBSceneMetaData
    {
        [Required(ErrorMessage = "*Project is required")]
        [Display(Name = "Project")]
        public int NBProjectId { get; set; }

        [Required(ErrorMessage = "*Name is required")]
        [MaxLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Description must be less than 1000 characters.")]
        public string Description { get; set; }
    }

    [MetadataType(typeof(NBSceneMetaData))]
    public partial class NBScene { }
}
