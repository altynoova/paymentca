using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public decimal BalanceCents { get; set; } = 800;
}