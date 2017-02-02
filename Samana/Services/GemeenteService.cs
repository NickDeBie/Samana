using DataSource;
using DataSource.Model;
using System.Collections.Generic;
using System.Linq;

namespace Samana.Services
{
    public class GemeenteService
    {
        private OkraContext _db = new OkraContext();
        public Gemeente GetGemeente(int id)
        {
            return _db.Gemeenten.Find(id);
        }
        public List<Gemeente> GetAlleGemeenten()
        {
            List<Gemeente> gemeenten = _db.Gemeenten.ToList();
            return gemeenten;
        }
        public void EditGemeente(Gemeente gemeente)
        {
            Gemeente oudeGemeente = _db.Gemeenten.Find(gemeente.Postcode);
            oudeGemeente = gemeente;
            _db.SaveChanges();
        }
        public void CreateGemeente(Gemeente gemeente)
        {
            _db.Gemeenten.Add(gemeente);
            _db.SaveChanges();
        }
    }
}