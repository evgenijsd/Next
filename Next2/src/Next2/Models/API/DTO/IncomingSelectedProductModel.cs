using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.Api.DTO
{
    public class IncomingSelectedProductModel
    {
        public string? Comment
        {
            get => Comment;

            set => Comment = value.Length > 250

                    ? value.Substring(0, 250)
                    : value;
        }

        public Guid? ProductId { get; set; }
        public IEnumerable<Guid>? SelectedOptionsId { get; set; }
        public IEnumerable<Guid>? SelectedIngredientsId { get; set; }
        public IEnumerable<Guid>? AddedIngredientsId { get; set; }
        public IEnumerable<Guid>? ExcludedIngredientsId { get; set; }
    }
}
