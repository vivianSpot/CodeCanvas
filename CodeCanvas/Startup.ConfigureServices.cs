using CodeCanvas.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeCanvas
{
	public partial class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddSwaggerDocument();

			services.AddDbContext<ApplicationDbContext>(options =>
			{
				var connectionString = _config.GetConnectionString(Constants.Databases.Application);
				options.UseSqlite(connectionString);
			});

			// todo: register EuropeanCentralBankClient
			// todo: register UpdateRatesHostedService
		}
	}
}
