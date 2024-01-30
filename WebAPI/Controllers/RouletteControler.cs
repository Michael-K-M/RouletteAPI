
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebAPI.Contract;
using WebAPI.Exceptions;
using WebAPI.Service;
using static SQLite.SQLite3;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RouletteControler : ControllerBase
    {

        private readonly IRouletteService _rouletteService;
        public RouletteControler(IRouletteService rouletteService)
        {
            _rouletteService = rouletteService;
        }

        [HttpPost("PlaceBet")]
        public IActionResult PostPlaceBet([FromBody] Bet bet)
        {
            try
            {
                _rouletteService.PlaceBet(bet);
                return Ok();
            }
            catch (BetOutofRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InsufficientFunds ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Exception = ex.Message });
            }

        }

        [HttpGet("{spinId:long}")]
        public IActionResult GetSpin(long spinId)
        {

            try
            {
                var result = _rouletteService.Spin(spinId);
                return Ok(result);
            }
            catch (InvalidSpinException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
               return StatusCode(500, new { Message = "Internal Server Error", Exception = ex.Message });
            }
        }

        [HttpGet("Payout")]
        public IActionResult GetPayout()
        {
            try
            {
                var result = _rouletteService.GetPayout();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Exception = ex.Message });
            }
        }

        [HttpGet("GetPreviousSpins")]
        public IActionResult GetPreviousSpins()
        {
            try
            {
                var result = _rouletteService.GetPreviousSpins();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Exception = ex.Message });
            }
        }
    }
}
