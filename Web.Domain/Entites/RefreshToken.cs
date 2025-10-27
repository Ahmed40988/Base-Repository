using Microsoft.EntityFrameworkCore;

namespace Web.Domain.Entites
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expiereson { get; set; }

        public DateTime Createdon { get; set; } = DateTime.UtcNow;
        public DateTime? Revokedon { get; set; }

        public bool IsExpiered => DateTime.UtcNow >= Expiereson;
        public bool isActive => Revokedon is null && !IsExpiered;

    }
}
