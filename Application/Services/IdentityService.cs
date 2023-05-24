using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Contracts.Contexts;
using Application.Common.Contracts.Services;
using Application.Models.Identity;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{

    internal class IdentityService : IIdentityService {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationDbContext _context;

        public IdentityService(UserManager<ApplicationUser> userManager, IApplicationDbContext context) {
            this._userManager = userManager;
            this._context = context;
        }

        public async Task<AuthenticateResponse> SignInAsync(SignInRequest request) {
            if(request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user is null) {
                return new AuthenticateResponse { Succeeded = false };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid) {
                return new AuthenticateResponse { Succeeded = false };
            }

            if (!user.IsActive) {
                return new AuthenticateResponse {
                    Succeeded = false,
                    Errors = new[] {
                        new IdentityError {
                            Code = "UserLocked",
                            Description = "User is locked"
                        }
                    }
                };
            }

            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new AuthenticateResponse { 
                Succeeded = true,
                Email = user.Email!,
                Username = user.UserName!
            };
        }

        public async Task<AuthenticateResponse> SignUpAsync(SignUpRequest request) {
            if(request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            var appUser = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(appUser, request.Password);

            if(!result.Succeeded) {
                return result.Adapt<AuthenticateResponse>();
            }

            var created = await _userManager.FindByEmailAsync(request.Email);
            created!.RegistrationDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(created);

            return new AuthenticateResponse {
                Succeeded = true,
                Email = request.Email,
                Username = request.Username
            };
        }
    }
}
