using System.ComponentModel.DataAnnotations;

namespace AnigramsNotebook.EF
{
    class NBPromptMetaData
    {
        [Required(ErrorMessage = "*Category is required")]
        public int NBCategoryId { get; set; }

        [MaxLength(200, ErrorMessage = "Source must be less than 200 characters.")]
        [DataType(DataType.Url)]
        public string Source { get; set; }

        [Required(ErrorMessage = "*Description is required")]
        [MaxLength(500, ErrorMessage = "Description must be less than 1000 characters.")]
        public string Text { get; set; }
    }

    [MetadataType(typeof(NBPromptMetaData))]
    public partial class NBPrompt { }
}
