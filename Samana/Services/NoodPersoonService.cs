using DataSource;
using DataSource.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Samana.Services
{
    public class NoodPersoonService
    {
        private OkraContext _db = new OkraContext();
        public NoodPersoon GetNoodPersoon(int? id)
        {            
            return _db.NoodPersonen.Find(id);
        }

        public List<NoodPersoon> GetAlleNoodPersonen()
        {
            return _db.NoodPersonen.ToList();
        }
        public int GetMaxId()
        {
            if (!(_db.NoodPersonen.Count() == 0))
            {
                return _db.NoodPersonen.Max(m => m.Id);
            }
            else
            {
                return 0;
            }
        }
        public void SaveNoodPersoon(NoodPersoon pers)
        {
            _db.Leden.Find(pers.LidId).NoodPersonen.Add(pers);
            _db.SaveChanges();
        }
        public void RemoveNoodPersoon(int? id)
        {
            NoodPersoon pers = _db.NoodPersonen.Find(id);
            _db.NoodPersonen.Remove(pers);
            _db.SaveChanges();
        }
    }
}