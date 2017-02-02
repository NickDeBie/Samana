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
            var leden = (_db.Leden.Include(l => l.GeboortePlaats).Include(l => l.Gemeente).Include(l => l.Lidsoort).Include(l => l.Mentor).Include(l => l.Verantwoordelijkheden)).ToList();
            return leden;
        }

        public ICollection<Lid> GetAlleMentoren()
        {
            ICollection<Lid> mentoren = _db.Leden.Where(m => m.LidsoortId == 1).Distinct().ToList();
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

        public void EditLid(LidViewModel lidViewModel)
        {
            Lid lid = GetLid(lidViewModel.Id);
            lid.Id = lidViewModel.Id;
            lid.Voornaam = lidViewModel.Voornaam;
            lid.Achternaam = lidViewModel.Achternaam;
            lid.Lidsoort = _db.Lidsoorten.Find(lidViewModel.LidsoortId);
            lid.Geslacht = lidViewModel.Geslacht;
            lid.GeboorteDatum = lidViewModel.GeboorteDatum;
            lid.GeboortePlaats = _db.Gemeenten.Find(lidViewModel.GeboortePostcode);
            lid.Adres = lidViewModel.Adres;
            lid.HuisNr = lidViewModel.HuisNr;
            lid.Gemeente = _db.Gemeenten.Find(lidViewModel.Postcode);
            lid.RijksregisterNr = lidViewModel.RijksregisterNr;
            lid.Tel = lidViewModel.Tel;
            lid.GSM = lidViewModel.GSM;
            lid.Email = lidViewModel.Email;
            lid.NoodNr = lidViewModel.NoodNr;
            lid.NoodNaam = lidViewModel.NoodNaam;
            lid.Rolstoel = lidViewModel.Rolstoel;
            lid.Rusthuis = lidViewModel.Rusthuis;
            lid.ThuisGebonden = lidViewModel.ThuisGebonden;
            lid.Verantwoordelijkheden = lidViewModel.Verantwoordelijkheden;
            lid.Beschermelingen = lidViewModel.Beschermelingen;
            lid.Mentor = _db.Leden.Find(lidViewModel.MentorId);
            lid.Opmerking = lidViewModel.Opmerking;
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
        public ICollection<Lid> GetBeschermelingenPerKernlid(int id)
        {
            ICollection<Lid> beschermelingen = _db.Leden.Where(m => m.MentorId == id).ToList();
            return beschermelingen;
        }
        public ICollection<Lid> GetLedenOpVoorNaam(string beginNaam)
        {
            return (from lid in _db.Leden where lid.Voornaam.StartsWith(beginNaam) orderby lid.Voornaam select lid).ToList();
        }
        public ICollection<Lid> GetLedenOpAchterNaam(string beginNaam)
        {
            return (from lid in _db.Leden where lid.Achternaam.StartsWith(beginNaam) orderby lid.Achternaam select lid).ToList();
        }
        public ICollection<Lid> GetLedenPerSoort(int? id)
        {
            return (from lid in _db.Leden where lid.LidsoortId == id orderby lid.Voornaam select lid).ToList();
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
                GeboortePostcode = Convert.ToInt16(lid.GeboortePlaats?.Postcode ?? null),
                GeboortePlaats = lid.GeboortePlaats?.Plaats ?? null,
                Adres = lid.Adres,
                HuisNr = lid.HuisNr,
                Postcode = Convert.ToInt16(lid.Gemeente?.Postcode ?? null),
                Gemeente = lid.Gemeente?.Plaats ?? null,
                RijksregisterNr = lid.RijksregisterNr,
                Tel = lid.Tel,
                GSM = lid.GSM,
                Email = lid.Email,
                NoodNr = lid.NoodNr,
                NoodNaam = lid.NoodNaam,
                Rolstoel = lid.Rolstoel,
                Rusthuis = lid.Rusthuis,
                ThuisGebonden = lid.ThuisGebonden,
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

        public Lid SaveLidViewModelToLid(LidViewModel lidViewModel)
        {
            Lid lid = _db.Leden.Find(lidViewModel.Id);
            lid.Voornaam = lidViewModel.Voornaam;
            lid.Achternaam = lidViewModel.Achternaam;
            lid.LidsoortId = lidViewModel.LidsoortId;
            lid.Geslacht = lidViewModel.Geslacht;
            lid.GeboorteDatum = lidViewModel.GeboorteDatum;
            lid.GeboortePlaatsId = lidViewModel.GeboortePostcode;
            lid.Adres = lidViewModel.Adres;
            lid.HuisNr = lidViewModel.HuisNr;
            lid.GemeenteId = lidViewModel.Postcode;
            lid.RijksregisterNr = lidViewModel.RijksregisterNr;
            lid.Tel = lidViewModel.Tel;
            lid.GSM = lidViewModel.GSM;
            lid.Email = lidViewModel.Email;
            lid.NoodNr = lidViewModel.NoodNr;
            lid.NoodNaam = lidViewModel.NoodNaam;
            lid.Rolstoel = lidViewModel.Rolstoel;
            lid.Rusthuis = lidViewModel.Rusthuis;
            lid.ThuisGebonden = lidViewModel.ThuisGebonden;
            lid.Verantwoordelijkheden = lidViewModel.Verantwoordelijkheden;
            //lid.Beschermelingen = lidViewModel.Beschermelingen;
            //lid.MentorId = lidViewModel.MentorId;
            lid.Opmerking = lidViewModel.Opmerking;

            _db.SaveChanges();
            return lid;
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
    }
}