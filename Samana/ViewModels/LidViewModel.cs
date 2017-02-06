using DataSource.Enums;
using DataSource.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Samana.ViewModels
{
    public class LidViewModel
    {
        public LidViewModel()
        {
            NoodPersonen = new List<NoodPersoon>();
        }

        //TODO: Id's van lidsoort etc toevoegen...
        public int Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public int LidsoortId { get; set; }
        public string Lidsoort { get; set; }
        public Geslacht Geslacht { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Datum is fout.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "Geboortedatum is verplicht.")]
        [Range(typeof(DateTime), "1/1/1850", "1/1/2017", ErrorMessage = "datum ligt te ver in het verleden...")]
        public DateTime GeboorteDatum { get; set; }
        public string Adres { get; set; }
        public string HuisNr { get; set; }
        public int Postcode { get; set; }
        public string Gemeente { get; set; }
        public string RijksregisterNr { get; set; }
        public string Tel { get; set; }
        public string GSM { get; set; }
        public string Email { get; set; }
        public bool Rolstoel { get; set; }
        public bool Rusthuis { get; set; }
        public bool ThuisGebonden { get; set; }
        public bool Rollator { get; set; }
        public bool GaandeZieke { get; set; }
        public bool LiggendeZieke { get; set; }
        public List<NoodPersoon> NoodPersonen { get; set; }
        public List<Verantwoordelijkheid> Verantwoordelijkheden { get; set; }
        public List<Lid> Beschermelingen { get; set; }
        public string Mentor { get; set; }
        public int? MentorId { get; set; }
        public string Opmerking { get; set; }
        public List<Verantwoordelijkheid> AlleVerantwoordelijkheden { get; set; }
        public string Leeftijd { get; set; }
        public bool IsSelected { get; set; }

        public List<NoodPersoon> GetNoodPersonen()
        {
            List<NoodPersoon> persList = new List<NoodPersoon>();
            
            return persList;
        }
    }
}