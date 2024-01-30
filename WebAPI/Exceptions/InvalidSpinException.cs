namespace WebAPI.Exceptions
{
    public class InvalidSpinException : Exception
    {
        public InvalidSpinException() { }

        public InvalidSpinException(long SpinId)
            : base(String.Format("Game can only run once a bet has been placed on spin: {0}", SpinId))
        {

        }
    }
}
