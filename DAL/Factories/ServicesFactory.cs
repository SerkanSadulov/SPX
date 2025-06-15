using DAL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL.Factories
{
    public class ServicesFactory(IConfiguration configuration) : IFactory<string>
    {
        private readonly IConfiguration _configuration = configuration;

        public string Get()
        {
#if TARGET_LINUX && !TARGET_ANDROID
#if DEBUG
                        return string.Empty;
#else
                        return string.Empty;
#endif
#else
#if DEBUG
            return _configuration.GetSection("API:service-provider-dev:url").Value;
#else
            return _configuration.GetSection("API:service-provider-prod:url").Value;
#endif
#endif
        }
    }
}