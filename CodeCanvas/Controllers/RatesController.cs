using System;
using CodeCanvas.Models;
using EuropeanCentralBank;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeCanvas.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RatesController : ControllerBase
	{
		private readonly IEuropeanCentralBankClient _europeanCentralBankClient;
		private readonly ILogger<RatesController> _logger;

		public RatesController(IEuropeanCentralBankClient europeanCentralBankClient, ILogger<RatesController> logger)
		{
			_europeanCentralBankClient = europeanCentralBankClient;
			_logger = logger;
		}

		[HttpGet]
		[ProducesResponseType(typeof(CurrencyRateModel[]), StatusCodes.Status200OK)]
		public async IActionResult GetRates([FromQuery] DateTime date)
		{
			// todo: implement RatesController.GetRates()

			// get rates for the requested date, or 404 (not found) in case requested date is missing

			// log each request along with its corresponding response

			if(date == default(DateTime))
				throw new ArgumentNullException(nameof(date));

			(await _europeanCentralBankClient.GetRates()).;
		}
	}
}
