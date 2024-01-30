

using WebAPI.Contract;
using WebAPI.Controllers;
using WebAPI.Database;
using WebAPI.Exceptions;

namespace WebAPI.Service
{
    public class RouletteService : IRouletteService
    {
        private readonly ISystemDB _db;
        private long _loggedInUser = 1;
        public RouletteService(ISystemDB db)
        {
            _db = db;
        }

        public void PlaceBet(Bet bet)
        {
            if (bet.ChosenNumber > 36 || bet.ChosenNumber < 0) 
            {
                throw new InsufficientFunds(bet.ChosenNumber);
            }
            else if(bet.Amount <= 0) 
            {
                throw new InsufficientFunds(bet.Amount);
            }

            bet.UserId = _loggedInUser;
            _db.SaveBet(bet);  
        }

        public SpinResult Spin(long spinId)
        {
            if (_db.GetBetsFromSpinId(spinId).Any() == false) 
            {
                throw new InvalidSpinException(spinId);
            }
            Random random = new Random();
            // Generate a random number between 0 and 36
            int rollNumber = random.Next(0, 37);
            var spin = new SpinResult(spinId, rollNumber);

            _db.SaveSpinNumber(spin);
            return spin;
        }

        public Payout GetPayout()
        {
            var payout = new Payout();
            var existingBets = _db.GetBetsFromUser(_loggedInUser);
            var reliventSpins = _db.GetSpinsFromSpinIds(existingBets.Select(x => x.SpinId).ToList());

            foreach (var spin in reliventSpins)
            {
                foreach (var bet in existingBets.Where(x => x.SpinId == spin.SpinId))
                {
                    if (bet.ChosenNumber == spin.WonNumber)
                        payout.Amount += bet.Amount * 35;
                }
            }
            _db.ClearBetsFromUser(existingBets);
            return payout;
        }

        public List<SpinResult> GetPreviousSpins()
        {
            return _db.GetAllSpins();
        }
    }
}
