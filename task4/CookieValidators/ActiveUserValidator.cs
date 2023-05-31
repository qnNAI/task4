using System.Security.Claims;
using Application.Common.Contracts.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace task4.CookieValidators;


public static class ActiveUserValidator {


    public static async Task ValidateAsync(CookieValidatePrincipalContext context) {
        var userPrincipal = context.Principal;
        var appContext = context.HttpContext.RequestServices.GetRequiredService<IApplicationDbContext>();

        var userId = userPrincipal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var user = await appContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if(user is null || !user.IsActive) {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync();
        }
    }
}
