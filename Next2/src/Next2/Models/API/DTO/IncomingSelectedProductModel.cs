using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class IncomingSelectedProductModel
    {
        public Guid? ProductId { get; set; }

        public string? Comment { get; set; }

        public IEnumerable<Guid>? SelectedOptionsId { get; set; }

        public IEnumerable<Guid>? SelectedIngredientsId { get; set; }

        public IEnumerable<Guid>? AddedIngredientsId { get; set; }

        public IEnumerable<Guid>? ExcludedIngredientsId { get; set; }
    }
}
