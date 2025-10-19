using System;
using System.Data.SqlTypes;

namespace APIClasses
{
    public class UserProfile
    {
        public int UserProfileId { get; set; } //PK
        public string name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }
        public byte[] profilePicture { get; set; }
        public string password { get; set; }
        public Boolean isActive { get; set; }
    }
}
