using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace EuropeanCentralBank
{
	public class EuropeanCentralBankClient : IEuropeanCentralBankClient
	{
		private HttpClient _client;
		private readonly EuropeanCentralBankSettings _settings;

		public EuropeanCentralBankClient(HttpClient client, IOptions<EuropeanCentralBankSettings> settings)
		{
			_client = client;
			_settings = settings.Value;
		}

		public Task<RatesResponse> GetRates()
		{
			// todo: implement EuropeanCentralBankClient.GetRates()

			// 1) make http call to European Central Bank (_settings.Endpoint) to get the latest rates
			var response = _client.GetAsync(_settings.RatesEndpoint);

			// 2) parse response


			// 3) create RatesResponse
			// 4) return RatesResponse

			throw new NotImplementedException();
		}
	}
}
