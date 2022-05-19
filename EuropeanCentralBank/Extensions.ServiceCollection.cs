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
			//we choose AddSingleton because only one instance of EuropeanCentralBankClient is needed
			//we only get the rates and at any given time the result will be the same
			services.AddSingleton<IEuropeanCentralBankClient, EuropeanCentralBankClient>();

			// 2) register the EuropeanCentralBankSettings 
			services.Configure<EuropeanCentralBankSettings>(configuration.GetSection("EuropeanCentralBankSettings"));

			return services;
		}
	}
}
