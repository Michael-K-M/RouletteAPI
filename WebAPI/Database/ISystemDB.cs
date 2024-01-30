using WebAPI.Contract;

namespace WebAPI.Database
{
    public interface ISystemDB
    {
        public void SaveBet(Bet bet);
        public void SaveSpinNumber(SpinResult spin);
        public List<Bet> GetBetsFromUser(long userId);
        public void ClearBetsFromUser(List<Bet> bets);
        public List<SpinResult> GetSpinsFromSpinIds(List<long> spinIds);
        public List<SpinResult> GetAllSpins();
        public List<Bet> GetBetsFromSpinId(long spinId);
    }
}
