using SQLite;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Contract
{
    public class SpinResult
    {
        public SpinResult(long spinId, long wonNumber) 
        { 
            SpinId = spinId;
            WonNumber = wonNumber;
        }

        public SpinResult() { }


        [PrimaryKey][AutoIncrement]
        public long Id { get; set; }
        public long SpinId { get; set; }
        public long WonNumber { get; set; }

    }

}
