using System.ComponentModel.DataAnnotations;

namespace AnigramsNotebook.EF
{
    class NBAssociationMetaData
    {
        [Required(ErrorMessage = "*Parent is required")]
        [Display(Name = "Parent")]
        public int ParentId { get; set; }

        [Required(ErrorMessage = "*Child is required")]
        [Display(Name = "Child")]
        public int ChildId { get; set; }
    }

    [MetadataType(typeof(NBAssociationMetaData))]
    public partial class NBAssociation { }
}
