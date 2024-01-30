namespace WebAPI.Exceptions
{
    public class BetOutofRangeException : Exception
    {
        public BetOutofRangeException(long BetNum) : base($"Bet out of range: {BetNum}") { }

    }
}
