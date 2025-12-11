using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Client.Models.File;

namespace Client.Models.Folders
{
    public class FolderTree
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<FolderTree> SubFolders { get; set; } = new List<FolderTree>();
        public List<FileDto> Files { get; set; } = new List<FileDto>();
    }
}

