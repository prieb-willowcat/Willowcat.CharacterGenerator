using Microsoft.Extensions.DependencyInjection;

namespace Willowcat.CharacterGenerator.Application.Extension
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection RegisterApplicationServices(this ServiceCollection services)
        {
            services.AddSingleton(new Random());
            return services;
        }
    }
}
