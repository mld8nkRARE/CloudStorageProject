using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Folders
{
    public class FolderItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "folder"; // "folder" или "file"
        public long? Size { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsFolder => Type == "folder";
    }
}

