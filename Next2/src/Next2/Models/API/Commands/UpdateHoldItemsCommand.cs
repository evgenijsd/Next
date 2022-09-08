using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Next2.Models.API.Commands
{
    public class UpdateHoldItemsCommand
    {
        public IEnumerable<Guid> HoldItemsId { get; set; } = Enumerable.Empty<Guid>();

        public DateTime? NewHoldTime;
    }
}
