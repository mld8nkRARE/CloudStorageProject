using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.User
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public long UsedStorage { get; set; }
        public int FilesCount { get; set; }
    }
}
