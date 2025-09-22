using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, decimal>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<decimal> Handle(CreateUserCommand request, CancellationToken ct)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = $"{request.UserName}@gmail.com",
                BalanceCents = 800
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            return user.BalanceCents;
        }
    }
}
