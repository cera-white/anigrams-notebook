using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AnigramsNotebook.EF
{
    class NBCategoryMetaData
    {
        [Required(ErrorMessage = "*Name is required")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "*Icon is required")]
        public string IconName { get; set; }

        [Required(ErrorMessage = "*Color is required")]
        public string Color { get; set; }
    }

    [MetadataType(typeof(NBCategoryMetaData))]
    public partial class NBCategory { }
}
