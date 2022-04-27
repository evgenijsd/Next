using Next2.Enums;
using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class UserModel : IBaseModel
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public DateTime CreationTime { get; set; }

        public EUserType UserType { get; set; } = EUserType.User;
    }
}
