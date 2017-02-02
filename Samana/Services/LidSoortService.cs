using DataSource;
using DataSource.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Samana.Services
{
    public class LidSoortService
    {
        OkraContext _db = new OkraContext();

        public Lidsoort GetLidSoort(int id)
        {
            return _db.Lidsoorten.Find(id);
        }
        public List<Lidsoort> GetLidSoorten()
        {
            return _db.Lidsoorten.ToList();
        }
    }
}