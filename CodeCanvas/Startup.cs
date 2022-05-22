using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;

namespace CodeCanvas
{
    public partial class Startup
	{
		private readonly IConfiguration _config;

		public Startup(IConfiguration config)
		{
			_config = config;

			Log.Logger = new LoggerConfiguration()
										.WriteTo.Map("Date", "Other", (date, wt) => wt.File($"./logs/log_{date}.txt"))
										.CreateLogger();
		}
	}
}
