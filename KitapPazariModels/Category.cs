using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KitapPazariModels
{
    public class Category
    {
        public int Id { get; set; }
        [DisplayName("Category Name")]
        [MaxLength(30)]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100)]
        public int DisplayOrder { get; set; }
    }
}
