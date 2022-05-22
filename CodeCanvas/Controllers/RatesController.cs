using CodeCanvas.Models;
using CodeCanvas.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;

namespace CodeCanvas.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class RatesController : ControllerBase
	{
		private readonly ICurrencyRateRepository _currencyRateRepository;
		private readonly ILogger<RatesController> _logger;

		public RatesController(ICurrencyRateRepository currencyRateRepository, ILogger<RatesController> logger)
		{
			_currencyRateRepository = currencyRateRepository;
			_logger = logger;
		}

		[HttpGet]
		[ProducesResponseType(typeof(CurrencyRateModel[]), StatusCodes.Status200OK)]
		public IActionResult GetRates([FromQuery] DateTime date)
		{
			if(date == default)
				throw new ArgumentNullException(nameof(date));

			// get rates for the requested date
			var rates = _currencyRateRepository.GetCurrencyRatesByDateAsync(date)
				.Select(x => new CurrencyRateModel(x.Id, x.CurrencyCode, x.Rate, x.CreatedAt)).ToArray();

			// or 404 (not found) in case requested date is missing
			if (rates?.Any() != true)
				return NotFound();

			// log each request along with its corresponding response
			Log.Information("Request Body {Date}!", date);
			Log.Information($"Response Body: {rates}");

			return Ok(rates);
		}
	}
}
