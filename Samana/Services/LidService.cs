using DataSource;
using DataSource.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Samana.ViewModels;

namespace Samana.Services
{
    public class LidService
    {
        private OkraContext _db = new OkraContext();
        public Lid GetLid(int? id)
        {
            return _db.Leden.Find(id);
        }

        public List<Lid> GetAlleLeden()
        {
            var leden = (_db.Leden.Include(l => l.Gemeente).Include(l => l.Lidsoort).Include(l => l.Mentor).Include(l => l.Verantwoordelijkheden).Include(l=>l.NoodPersonen).OrderBy(o=>o.Achternaam)).ToList();
            return leden;
        }

        public List<Lid> GetAlleLedenByStep(int limit, int fromRowNumber)
        {
         
            var leden = (_db.Leden.Include(l => l.Gemeente).Include(l => l.Lidsoort).Include(l => l.Mentor).Include(l => l.Verantwoordelijkheden).Include(l => l.NoodPersonen).OrderBy(o=>o.Achternaam).Skip(fromRowNumber).Take(limit)).ToList();
            
            return leden;
        }

        public List<Lid> GetAlleMentoren()
        {
            List<Lid> mentoren = _db.Leden.Where(m => m.LidsoortId == 1).Distinct().ToList();
            return mentoren;
        }
        public int GetMaxId()
        {
            if (_db.Leden.Count() != 0)
            {
                return _db.Leden.Max(m => m.Id);
            }
            else
            {
                return 1;
            }
        }
        public void Createlid(Lid lid)
        {
            
            if (lid.MentorId == 0)
            {
                lid.MentorId = null;
            }
            _db.Leden.Add(lid);
            _db.SaveChanges();
        }
     
        public void BewaarVerantwoordelijkheden(LidViewModel lidViewModel)
        {
            Lid lid = _db.Leden.Find(lidViewModel.Id);
            lid.Verantwoordelijkheden.Clear();
            foreach (var verantw in lidViewModel.Verantwoordelijkheden)
            {
                lid.Verantwoordelijkheden.Add(_db.Verantwoordelijkheden.Find(verantw.Id));
            }
            _db.SaveChanges();
        }
        public List<Lid> GetBeschermelingenPerKernlid(int id)
        {
            List<Lid> beschermelingen = _db.Leden.Where(m => m.MentorId == id).ToList();
            return beschermelingen;
        }
        public List<Lid> GetLedenOpVoorNaam(string beginNaam)
        {
            return (from lid in _db.Leden where lid.Voornaam.StartsWith(beginNaam) orderby lid.Achternaam select lid).ToList();
        }
        public List<Lid> GetLedenOpVoorNaamByStep(string beginNaam, int limit, int fromRowNumber)
        {
            return _db.Leden.Include(l => l.Gemeente).Include(l => l.Lidsoort).Include(l => l.Mentor).Include(l => l.Verantwoordelijkheden).Include(l => l.NoodPersonen).Where(m => m.Voornaam.StartsWith(beginNaam)).OrderBy(o => o.Achternaam).Skip(fromRowNumber).Take(limit).ToList();
        }
        public List<Lid> GetLedenOpAchterNaam(string beginNaam)
        {
            return (from lid in _db.Leden where lid.Achternaam.StartsWith(beginNaam) orderby lid.Achternaam select lid).ToList();
        }
        public List<Lid> GetLedenOpAchterNaamByStep(string beginNaam, int limit, int fromRowNumber)
        {
            return _db.Leden.Include(l => l.Gemeente).Include(l => l.Lidsoort).Include(l => l.Mentor).Include(l => l.Verantwoordelijkheden).Include(l => l.NoodPersonen).Where(m => m.Achternaam.StartsWith(beginNaam)).OrderBy(o => o.Achternaam).Skip(fromRowNumber).Take(limit).ToList();
        }
        public List<Lid> GetLedenPerSoort(int? id)
        {
            return (from lid in _db.Leden where lid.LidsoortId == id orderby lid.Achternaam select lid).ToList();
        }
        public List<Lid> GetLedenPerSoortByStep(int? id, int limit, int fromRowNumber)
        {
            return _db.Leden.Include(l => l.Gemeente).Include(l => l.Lidsoort).Include(l => l.Mentor).Include(l => l.Verantwoordelijkheden).Include(l => l.NoodPersonen).OrderBy(o => o.Achternaam).Where(m=>m.LidsoortId == id).OrderBy(m=>m.Achternaam).Skip(fromRowNumber).Take(limit).ToList();            
        }
        public LidViewModel LidToLidViewModel(Lid lid)
        {
            LidViewModel lidViewModel = new LidViewModel
            {
                Id = lid.Id,
                Voornaam = lid.Voornaam,
                Achternaam = lid.Achternaam,
                Lidsoort = lid.Lidsoort?.Soort ?? null,
                LidsoortId = lid.LidsoortId,
                Geslacht = lid.Geslacht,
                GeboorteDatum = lid.GeboorteDatum,
                Adres = lid.Adres,
                HuisNr = lid.HuisNr,
                Postcode = Convert.ToInt16(lid.Gemeente?.Postcode ?? null),
                Gemeente = lid.Gemeente?.Plaats ?? null,
                RijksregisterNr = lid.RijksregisterNr,
                Tel = lid.Tel,
                GSM = lid.GSM,
                Email = lid.Email,
                Rolstoel = lid.Rolstoel,
                Rusthuis = lid.Rusthuis,
                ThuisGebonden = lid.ThuisGebonden,
                Rollator = lid.Rollator,
                LiggendeZieke = lid.LiggendeZieke,
                GaandeZieke = lid.GaandeZieke,
                Verantwoordelijkheden = lid.Verantwoordelijkheden?.ToList() ?? null,
                Beschermelingen = lid.Beschermelingen?.ToList() ?? null,
                Mentor = (lid.Mentor?.Voornaam + " " + lid.Mentor?.Achternaam) ?? null,
                MentorId = lid.MentorId,
                Opmerking = lid.Opmerking,
                Leeftijd = BerekenLeeftijd(lid.GeboorteDatum)
            };
            return lidViewModel;
        }
        public List<LidViewModel> LedenToViewModelList(List<Lid> leden)
        {
            List<LidViewModel> ledenViewModel = new List<LidViewModel>();
            foreach (var lid in leden)
            {
                ledenViewModel.Add(LidToLidViewModel(lid));
            }
            return ledenViewModel;
        }

        public void SaveLidViewModelToDb(LidViewModel lidViewModel)
        {
            Lid lid = _db.Leden.Find(lidViewModel.Id);
            lid.Voornaam = lidViewModel.Voornaam;
            lid.Achternaam = lidViewModel.Achternaam;
            lid.LidsoortId = lidViewModel.LidsoortId;
            lid.Geslacht = lidViewModel.Geslacht;
            lid.GeboorteDatum = lidViewModel.GeboorteDatum;
            lid.Adres = lidViewModel.Adres;
            lid.HuisNr = lidViewModel.HuisNr;
            lid.GemeenteId = lidViewModel.Postcode;
            lid.RijksregisterNr = lidViewModel.RijksregisterNr;
            lid.Tel = lidViewModel.Tel;
            lid.GSM = lidViewModel.GSM;
            lid.Email = lidViewModel.Email;
            lid.Rolstoel = lidViewModel.Rolstoel;
            lid.Rusthuis = lidViewModel.Rusthuis;
            lid.ThuisGebonden = lidViewModel.ThuisGebonden;
            lid.Rollator = lidViewModel.Rollator;
            lid.LiggendeZieke = lidViewModel.LiggendeZieke;
            lid.GaandeZieke = lidViewModel.GaandeZieke;
            lid.Verantwoordelijkheden = lidViewModel.Verantwoordelijkheden;
            lid.Beschermelingen = lidViewModel.Beschermelingen;
            lid.MentorId = lidViewModel.MentorId != 0 ? lidViewModel.MentorId:null ;
            lid.Opmerking = lidViewModel.Opmerking;
            _db.SaveChanges();
        }
        public void SaveNewLidViewModelToDb(LidViewModel lidViewModel)
        {
            Lid lid = ConvertLidviewModelToLid(lidViewModel);
            lid.Id = GetMaxId() + 1;
            _db.Leden.Add(lid);
            _db.SaveChanges();
        }
        public Lid ConvertLidviewModelToLid(LidViewModel lidViewModel)
        {
            Lid lid = new Lid();
            lid.Voornaam = lidViewModel.Voornaam;
            lid.Achternaam = lidViewModel.Achternaam;
            lid.LidsoortId = lidViewModel.LidsoortId;
            lid.Geslacht = lidViewModel.Geslacht;
            lid.GeboorteDatum = lidViewModel.GeboorteDatum;
            lid.Adres = lidViewModel.Adres;
            lid.HuisNr = lidViewModel.HuisNr;
            lid.GemeenteId = lidViewModel.Postcode;
            lid.RijksregisterNr = lidViewModel.RijksregisterNr;
            lid.Tel = lidViewModel.Tel;
            lid.GSM = lidViewModel.GSM;
            lid.Email = lidViewModel.Email;
            lid.Rolstoel = lidViewModel.Rolstoel;
            lid.Rusthuis = lidViewModel.Rusthuis;
            lid.ThuisGebonden = lidViewModel.ThuisGebonden;
            lid.Rollator = lidViewModel.Rollator;
            lid.LiggendeZieke = lidViewModel.LiggendeZieke;
            lid.GaandeZieke = lidViewModel.GaandeZieke;
            lid.Verantwoordelijkheden = lidViewModel.Verantwoordelijkheden;
            lid.Beschermelingen = lidViewModel.Beschermelingen;
            lid.MentorId = lidViewModel.MentorId != 0 ? lidViewModel.MentorId : null;
            lid.Opmerking = lidViewModel.Opmerking;
            return lid;
        }

        public void deleteLidViewModelFromDb(int? id)
        {
            _db.Leden.Remove(_db.Leden.Find(id));
            _db.SaveChanges();
        }

        public void BewaarMentor(LidViewModel lidviewmodel)
        {
            Lid lid = GetLid(lidviewmodel.Id);
            lid.MentorId = lidviewmodel.MentorId;
            _db.SaveChanges();
        }

        public string BerekenLeeftijd(DateTime geboortedatum)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - geboortedatum.Year;
            if (now < geboortedatum.AddYears(age)) age--;
            return age.ToString();
        }
        public string GetLidSoort(int id)
        {
            return _db.Lidsoorten.Find(id).Soort;
        }

        public int AantalHuizen(List<Lid> list)
        {
            List<Lid> ledenHuizen = new List<Lid>();
            foreach (var lid in list)
            {
                if (!ledenHuizen.Exists(s => s.Adres == lid.Adres && s.HuisNr == lid.HuisNr))
                {

                    ledenHuizen.Add(lid);
                }
            }
            return ledenHuizen.Count();
        }

        public LidViewModel GetOudsteLid(List<LidViewModel> ledenList)
        {
            LidViewModel oudsteLid = new LidViewModel();

            if (ledenList.Count != 0)
            {
                oudsteLid = ledenList.First();
                foreach (var lid in ledenList)
                {
                    if(Convert.ToInt16(lid.Leeftijd) > Convert.ToInt16(oudsteLid.Leeftijd))
                    {
                        oudsteLid = lid;
                    }
                }
            }
            return oudsteLid;
        }
        public LidViewModel GetjongsteLid(List<LidViewModel> ledenList)
        {
            LidViewModel oudsteLid = new LidViewModel();

            if (ledenList.Count != 0)
            {
                oudsteLid = ledenList.First();
                foreach (var lid in ledenList)
                {
                    if (Convert.ToInt16(lid.Leeftijd) < Convert.ToInt16(oudsteLid.Leeftijd))
                    {
                        oudsteLid = lid;
                    }
                }
            }
            return oudsteLid;
        }
    }
}