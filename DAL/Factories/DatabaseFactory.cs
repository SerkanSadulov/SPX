using DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DAL.Factories
{
    public class DatabaseFactory(IConfiguration configuration) : IFactory<SqlConnection>
    {
        private readonly IConfiguration _configuration = configuration;

        public SqlConnection Get()
        {
#if TARGET_LINUX && !TARGET_ANDROID
#if DEBUG
                        return new SqlConnection(_configuration.GetConnectionString("LinuxDev"));
#else
                        return new SqlConnection(_configuration.GetConnectionString("LinuxProd"));
#endif
#else
#if DEBUG
            return new SqlConnection(_configuration.GetConnectionString("WindowsDev"));
#else
                        return new SqlConnection(_configuration.GetConnectionString("WindowsProd"));
#endif
#endif
        }
    }
}
