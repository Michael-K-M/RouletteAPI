using WebAPI.Contract;
using WebAPI.Controllers;

namespace WebAPI.Service

{
    public interface IRouletteService
    {
        public void PlaceBet(Bet bet);
        public SpinResult Spin(long spinId);
        public Payout GetPayout();
        public List<SpinResult> GetPreviousSpins();
    }
}
