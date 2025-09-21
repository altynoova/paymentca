namespace Domain.Entities
{
    public class LoginAttempt
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string? Reason { get; set; } = default!;
        public DateTimeOffset AttemptAt { get; set; } = DateTimeOffset.UtcNow;
        public bool Success { get; set; }
    }
}
