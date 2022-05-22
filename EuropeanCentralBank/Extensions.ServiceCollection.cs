using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EuropeanCentralBank
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddEuropeanCentralBank(this IServiceCollection services, IConfiguration configuration)
		{
			// 1) register EuropeanCentralBankClient
			services.AddSingleton<IEuropeanCentralBankClient, EuropeanCentralBankClient>();

			// 2) register the EuropeanCentralBankSettings 
			services.Configure<EuropeanCentralBankSettings>(configuration.GetSection("EuropeanCentralBankSettings"));

			return services;
		}
	}
}
