using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.Results
{
    public class GuidExecutionResult
    {
        public Guid Id { get; set; }

        public List<SeatsGuid>? Seats { get; set; }
    }

    public class SeatsGuid
    {
        public Guid Id { get; set; }

        public List<Guid>? SelectedDishes { get; set; }
    }
}
