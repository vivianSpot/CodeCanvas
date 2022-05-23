using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EuropeanCentralBank
{
    public class EuropeanCentralBankClient : IEuropeanCentralBankClient
	{
		private readonly HttpClient _client;
		private readonly EuropeanCentralBankSettings _settings;

		public EuropeanCentralBankClient(HttpClient client, IOptions<EuropeanCentralBankSettings> settings)
		{
			_client = client;
			_settings = settings.Value;
		}

		public async Task<RatesResponse> GetRates()
		{
			// 1) make http call to European Central Bank (_settings.Endpoint) to get the latest rates
			var ecbResponse = await _client.GetAsync(_settings.RatesEndpoint);

			if (!ecbResponse.IsSuccessStatusCode) 
				throw new Exception("Unable to retrieve rates from ECB.");

			// 2) parse response
			XDocument xml;

			try
			{
				xml = XDocument.Parse(ecbResponse.Content.ReadAsStringAsync().Result);
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to parse ECB response.", ex);
			}

			// 3) create RatesResponse
			var ecbRateDate = (DateTime?)xml.Descendants()?.SingleOrDefault(x => x.Attribute("time") != null)?.Attribute("time");
			if (ecbRateDate == null)
				throw new Exception("ECB response missing date.");

			var rates = xml.Descendants()?.Where(x => x.Attribute("currency") != null && x.Attribute("rate") != null)
				?.Select(x => new CurrencyRate((string)x.Attribute("currency"), (decimal)x.Attribute("rate")));

			if (rates?.Any() != true)
				throw new Exception("ECB response missing rates.");

			// 4) return RatesResponse
			return new RatesResponse(ecbRateDate.Value, rates.ToList().AsReadOnly());
		}
	}
}
