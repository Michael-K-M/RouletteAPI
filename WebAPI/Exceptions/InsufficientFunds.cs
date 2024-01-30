namespace WebAPI.Exceptions
{
    public class InsufficientFunds : Exception
    {
        public InsufficientFunds(long BetNum) : base($"Bet is insufficent: {BetNum}") { }

    }
}
