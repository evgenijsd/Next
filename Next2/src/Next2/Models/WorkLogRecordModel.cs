using Next2.Enums;
using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class WorkLogRecordModel : IBaseModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public DateTime Timestamp { get; set; }

        public EEmployeeRegisterState State { get; set; }
    }
}
