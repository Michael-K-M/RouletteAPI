using SQLite;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Contract
{
    public class Payout
    {
        public long Amount { get; set; }
    }
}
