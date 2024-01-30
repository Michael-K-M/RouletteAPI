using SQLite;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Contract
{
    public class Bet
    {
        [PrimaryKey][AutoIncrement]
        public long Id { get; set; }
        public long SpinId { get; set; }
        public long Amount { get; set; }
        public long ChosenNumber { get; set; }
        public long UserId { get; set; }
    }
}
