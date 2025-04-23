using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Core.Models
{
    [Owned]
    public class RefreshTocken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public DateTime CreateOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired;

    }
}
