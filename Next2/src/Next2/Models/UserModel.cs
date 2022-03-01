using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class UserModel : IBaseModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
