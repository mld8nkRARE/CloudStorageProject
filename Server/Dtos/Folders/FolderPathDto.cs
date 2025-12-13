using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Dtos.Folders
{
    public class FolderPathDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
