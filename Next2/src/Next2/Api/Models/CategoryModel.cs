using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Next2.Api.Models.Category
{
    [Table("categories")]
    public class CategoryModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<SubcategoryModel>? Subcategories { get; set; } = new();
    }
}
