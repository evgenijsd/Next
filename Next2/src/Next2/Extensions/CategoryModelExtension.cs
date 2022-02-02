using Next2.Models;

namespace Next2.Extensions
{
    public static class CategoryModelExtension
    {
        public static CategoryBindableModel ToBindableModel(this CategoryModel category)
        {
            return new CategoryBindableModel
            {
                Id = category.Id,
                Title = category.Title,
                IsSelected = false,
            };
        }
    }
}