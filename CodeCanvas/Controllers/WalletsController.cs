using CodeCanvas.Entities;
using CodeCanvas.Models;
using CodeCanvas.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CodeCanvas.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class WalletsController : ControllerBase
	{
		private readonly IWalletAdjustmentService _walletAdjustmentService;
		private readonly ILogger<WalletsController> _logger;

		public WalletsController(IWalletAdjustmentService walletAdjustmentService, ILogger<WalletsController> logger)
		{
			_walletAdjustmentService = walletAdjustmentService;
			_logger = logger;
		}

		[HttpPost]
		[ProducesResponseType(typeof(CurrencyRateModel[]), StatusCodes.Status200OK)]
		public async Task<IActionResult> AdjustBalance([FromQuery] string exchangeRateStrategy, [FromBody] AdjustBalancePayload payload)
		{
			// use IWalletAdjustmentService.AdjustBalance() to adjust the balance of the wallet
			var adjustedBalance = await _walletAdjustmentService.AdjustBalance(exchangeRateStrategy, payload.WalletId, payload.CurrencyCode, payload.Amount);

			// return 400 (bad request) in case there is no sufficient balance to subtract
			if (adjustedBalance < 0)
			{
				_logger.LogInformation($"No sufficient balance to subtract.");
				return BadRequest();
			}
			else
			{
				_logger.LogInformation($"Adjusted balance: {adjustedBalance}.");
			}

			// return new balance
			return Ok(adjustedBalance);
		}

		[HttpGet]
		[ProducesResponseType(typeof(WalletEntity), StatusCodes.Status200OK)]
		public IActionResult GetRates([FromQuery] int id)
		{
			var wallet = _walletAdjustmentService.GetWallet(id);

			if (wallet == null)
				return NotFound();

			return Ok(wallet);
		}

		[HttpPost]
		[Route("/create")]
		public IActionResult CreateWallet([FromBody] WalletEntity entity)
		{
			_walletAdjustmentService.CreateWallet(entity);

			return Ok(entity);
		}
	}
}
