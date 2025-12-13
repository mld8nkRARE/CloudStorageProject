using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Dtos.User
{
    public class StorageInfoDto
    {
        public long UsedBytes { get; set; }
        public long MaxBytes { get; set; } = 100 * 1024 * 1024; // 100MB фиксированный лимит
        public double Percentage => MaxBytes > 0 ? (double)UsedBytes / MaxBytes * 100 : 0;
    }
}