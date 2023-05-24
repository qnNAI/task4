using Application.Common.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace task4.ViewComponents {


    public class UsersComponent : ViewComponent {
        private readonly IIdentityService _identityService;

        public UsersComponent(IIdentityService identityService) {
            _identityService = identityService;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            var users = await _identityService.GetAllAsync();
            return View(users);
        }
    }
}
