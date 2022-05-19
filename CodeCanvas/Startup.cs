using EuropeanCentralBank;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeCanvas
{
	public partial class Startup
	{
		private readonly IConfiguration _config;

		public Startup(IConfiguration config)
		{
			_config = config;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			ServiceCollectionExtensions.AddEuropeanCentralBank(services, _config);

			//services.AddHttpClient();
		}
	}
}
