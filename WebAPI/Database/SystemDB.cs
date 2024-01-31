using Microsoft.Extensions.Hosting;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Contract;
using WebAPI.Controllers;

namespace WebAPI.Database
{
    public class SystemDB : ISystemDB
    {
        private readonly SQLiteConnection _con;
        public SystemDB(IHostEnvironment hostEnvironment)
        {
            string contentRootPath = hostEnvironment.ContentRootPath;
            _con = new SQLiteConnection(Path.Combine(contentRootPath, "database.db"));
            _con.CreateTable<Bet>();
            _con.CreateTable<SpinResult>();
        }

        public void ClearBetsFromUser(List<Bet> bets)
        {
            foreach (var bet in bets)
            {
                _con.Delete(bet);
            }
        }

        public List<SpinResult> GetAllSpins()
        {
            return _con.Table<SpinResult>().ToList();
        }

        public List<Bet> GetBetsFromUser(long userId)
        {
            return _con.Table<Bet>().Where(x => x.UserId == userId).ToList();
        }

        public List<Bet> GetBetsFromSpinId(long spinId)
        {
            return _con.Table<Bet>().Where(x => x.SpinId == spinId).ToList();
        }

        public List<SpinResult> GetSpinsFromSpinIds(List<long> spinIds)
        {
            return _con.Table<SpinResult>().Where(x => spinIds.Contains(x.SpinId)).ToList();
        }

        public void SaveBet(Bet bet)
        {
            _con.Insert(bet);
        }

        public void SaveSpinNumber(SpinResult spin)
        {
            _con.Insert(spin);
        }

    }
}
