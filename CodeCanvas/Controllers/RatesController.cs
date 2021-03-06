using CodeCanvas.Models;
using CodeCanvas.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
			var rates = _currencyRateRepository.GetAllByDate(date)
				.Select(x => new CurrencyRateModel(x.Id, x.CurrencyCode, x.Rate, x.CreatedAt)).ToArray();

			// log each request along with its corresponding response
			_logger.LogInformation($"Request date: {date}");

			// or 404 (not found) in case requested date is missing
			if (rates?.Any() != true)
			{
				_logger.LogInformation($"No rates found for {date}.");
				return NotFound();
			}
			else
			{
				foreach (var rate in rates)
				{
					_logger.LogInformation(rate.ToString());
				}
			}

			return Ok(rates);
		}
	}
}
