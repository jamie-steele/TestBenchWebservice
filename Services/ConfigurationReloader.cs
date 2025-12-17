namespace TestBenchWebService.Services
{
    public interface IConfigurationReloader
    {
        void Reload();
    }

    public sealed class ConfigurationReloader : IConfigurationReloader
    {
        private readonly IConfiguration _configuration;

        public ConfigurationReloader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Reload()
        {
            if (_configuration is IConfigurationRoot root)
            {
                root.Reload();
            }
        }
    }
}


