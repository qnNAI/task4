using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Identity;

namespace Application.Common.Contracts.Services
{
    public interface IIdentityService
    {
        Task<AuthenticateResponse> SignInAsync(SignInRequest request);
        Task<AuthenticateResponse> SignUpAsync(SignUpRequest request);

        Task<List<UserDto>> GetAllAsync();
        Task DeleteMultipleAsync(string[] users, CancellationToken cancellationToken = default);
        Task LockMultipleAsync(string[] users, CancellationToken cancellationToken = default);
        Task UnlockMultipleAsync(string[] users, CancellationToken cancellationToken = default);
    }
}
