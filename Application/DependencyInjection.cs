using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using MapsterMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Contracts.Services;
using Application.Services;
using Application.Common.Mappings;

namespace Application {

    public static class DependencyInjection {

        public static IServiceCollection AddApplication(this IServiceCollection services) {
            var config = TypeAdapterConfig.GlobalSettings;
            MappingProfile.ApplyMappings();

            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IIdentityService, IdentityService>();

            return services;
        }
    }
}
