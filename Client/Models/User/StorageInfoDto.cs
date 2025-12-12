using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.User
{
    public class StorageInfoDto
    {
        public long UsedBytes { get; set; }
        public long MaxBytes { get; set; }
        public double Percentage => MaxBytes > 0 ? (double)UsedBytes / MaxBytes * 100 : 0;
    }
}
