namespace Domain.Entities
{
    public class Session
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = default!;
        public required string Jti { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
        public bool IsActive => RevokedAt == null && DateTimeOffset.UtcNow < ExpiresAt;
    }
}
