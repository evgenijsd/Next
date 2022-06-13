using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.DTO
{
    public class IncomingSelectedProductModel
    {
        public string? Comment
        {
            get => Comment;
            set => Comment = value.Length > Constants.Limits.COMMENT_LENGTH
                    ? value.Substring(0, Constants.Limits.COMMENT_LENGTH)
                    : value;
        }

        public Guid? ProductId { get; set; }

        public IEnumerable<Guid>? SelectedOptionsId { get; set; }

        public IEnumerable<Guid>? SelectedIngredientsId { get; set; }

        public IEnumerable<Guid>? AddedIngredientsId { get; set; }

        public IEnumerable<Guid>? ExcludedIngredientsId { get; set; }
    }
}
