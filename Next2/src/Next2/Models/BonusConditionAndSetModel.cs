using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class BonusConditionAndSetModel : IBaseModel
    {
        public int Id { get; set; }

        public int SetId { get; set; }

        public int BonusId { get; set; }
    }
}
