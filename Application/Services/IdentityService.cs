﻿using System;
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
using Microsoft.EntityFrameworkCore;

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
                Id = user.Id,
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
            created!.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(created);

            return new AuthenticateResponse {
                Succeeded = true,
                Id = created.Id,
                Email = request.Email,
                Username = request.Username
            };
        }

        public async Task<List<UserDto>> GetAllAsync() {
            var users = await _userManager.Users.ProjectToType<UserDto>().ToListAsync();
            return users;
        }

        public async Task DeleteMultipleAsync(string[] users, CancellationToken cancellationToken = default) {
            this._context.Users.RemoveRange(
                await _GetUsersByIdAsync(users, cancellationToken));

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task LockMultipleAsync(string[] users, CancellationToken cancellationToken = default) {
            var existingUsers = await _GetUsersByIdAsync(users, cancellationToken);
            foreach(var user in existingUsers) {
                user.IsActive = false;
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UnlockMultipleAsync(string[] users, CancellationToken cancellationToken = default) {
            var existingUsers = await _GetUsersByIdAsync(users, cancellationToken);
            foreach(var user in existingUsers) {
                user.IsActive = true;
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<List<ApplicationUser>> _GetUsersByIdAsync(string[] users, CancellationToken cancellationToken = default) {
            return await _context.Users.Where(x => users.Contains(x.Id)).ToListAsync(cancellationToken);
        }
    }
}
