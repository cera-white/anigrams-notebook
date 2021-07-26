using System.ComponentModel.DataAnnotations;

namespace AnigramsNotebook.EF
{
    class NBResourceMetaData
    {
        [Required(ErrorMessage = "*Category is required")]
        public int NBCategoryId { get; set; }

        [MaxLength(200, ErrorMessage = "URL must be less than 200 characters.")]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Required(ErrorMessage = "*Name is required")]
        [MaxLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string DisplayName { get; set; }
    }

    [MetadataType(typeof(NBResourceMetaData))]
    public partial class NBResource { }
}
