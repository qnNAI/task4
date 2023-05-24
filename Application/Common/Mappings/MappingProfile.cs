using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Identity;
using Domain.Entities;
using Mapster;

namespace Application.Common.Mappings {

    public static class MappingProfile {

        public static void ApplyMappings() {
            TypeAdapterConfig<ApplicationUser, UserDto>
                .NewConfig()
                .Map(dest => dest.Username, src => src.UserName);

            TypeAdapterConfig<SignUpRequest, ApplicationUser>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.Username);
        }
    }
}
