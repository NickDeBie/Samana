using DataSource.Enums;
using DataSource.Model;
using System;
using System.Collections.Generic;

namespace Samana.ViewModels
{
    public class LidViewModel
    {

        //TODO: Id's van lidsoort etc toevoegen...
        public int Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public int LidsoortId { get; set; }
        public string Lidsoort { get; set; }
        public Geslacht Geslacht { get; set; }
        public DateTime GeboorteDatum { get; set; }
        public int GeboortePostcode { get; set; }
        public String GeboortePlaats { get; set; }
        public string Adres { get; set; }
        public string HuisNr { get; set; }
        public int Postcode { get; set; }
        public string Gemeente { get; set; }
        public string RijksregisterNr { get; set; }
        public string Tel { get; set; }
        public string GSM { get; set; }
        public string Email { get; set; }
        public string NoodNr { get; set; }
        public string NoodNaam { get; set; }
        public bool Rolstoel { get; set; }
        public bool Rusthuis { get; set; }
        public bool ThuisGebonden { get; set; }
        public virtual ICollection<Verantwoordelijkheid> Verantwoordelijkheden { get; set; }
        public virtual ICollection<Lid> Beschermelingen { get; set; }
        public string Mentor { get; set; }
        public int? MentorId { get; set; }
        public string Opmerking { get; set; }
        public List<Verantwoordelijkheid> AlleVerantwoordelijkheden { get; set; }
        public string Leeftijd { get; set; }
        public bool IsSelected { get; set; }

    }
}