
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebAPI.Contract;
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
        public HttpResponseMessage PostPlaceBet([FromBody] Bet bet)
        {
            try
            {
                _rouletteService.PlaceBet(bet);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            //catch (MinimumDepositException ex)
            //{
            //    return new HttpResponseMessage(HttpStatusCode.NotAcceptable) { Content = new StringContent(ex.Message) };
            //}
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) };

            }

        }

        [HttpGet("{spinId:long}")]
        public IActionResult GetSpin(long spinId)
        {
            try
            {
                //Throw no bets have been placed :/
                var result = _rouletteService.Spin(spinId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                //return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) };
                return Ok();
            }

        }

        [HttpGet("Payout")]
        public HttpResponseMessage GetPayout()
        {
            try
            {
                var result = _rouletteService.GetPayout();
                string jsonContent = JsonConvert.SerializeObject(result);
                return new HttpResponseMessage(HttpStatusCode.Accepted) { Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json") };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) };

            }
        }

        [HttpGet("GetPreviousSpins")]
        public HttpResponseMessage GetPreviousSpins()
        {
            try
            {
                var result = _rouletteService.GetPreviousSpins();
                string jsonContent = JsonConvert.SerializeObject(result);
                return new HttpResponseMessage(HttpStatusCode.Accepted) { Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json") };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) };

            }
        }
    }

   
}
