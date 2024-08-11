using System;
using UsersManipulationWeb.Util;

namespace UsersManipulationWeb.Model
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? EMail { get; set; }
        public string? MobileNumber { get; set; }
        public Enums.Language Language { get; set; }
        public Enums.Culture Culture { get; set; }
        public string? Password { get; set; }
    }
}
