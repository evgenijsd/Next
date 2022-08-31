using Next2.Models.API.DTO;
using Next2.Models.Bindables;

namespace Next2.Extensions
{
    public static class OptionExtension
    {
        public static OptionModelDTO ToOptionModelDTO(this OptionBindableModel option)
        {
            return new()
            {
                Id = option.Id,
                Name = option.Name,
            };
        }

        public static OptionBindableModel ToOptionBindableModel(this OptionModelDTO option)
        {
            return new()
            {
                Id = option.Id,
                Name = option.Name,
            };
        }
    }
}
