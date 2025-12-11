using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Folders
{
    public class FolderContent
    {
        public Guid FolderId { get; set; }
        public string FolderName { get; set; } = string.Empty;
        public Guid? ParentFolderId { get; set; }
        public List<FolderItem> Items { get; set; } = new List<FolderItem>();
    }
}
