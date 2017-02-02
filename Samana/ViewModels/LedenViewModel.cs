using Samana.Services;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Samana.ViewModels
{
    public class LedenViewModel
    {
        private LidService _lidService = new LidService();
        private LidSoortService _lidsoortService = new LidSoortService();
        private GemeenteService _gemeenteService = new GemeenteService();
        private LidViewModel _lidViewModel = new LidViewModel();
        public LedenViewModel()
        {
            MentorenSelectList = CreateMentorenSelectList();
            LidsoortenSelectList = CreateLidsoortenSelectList();
            GemeenteSelectList = CreateGemeentenSelectList();
            LedenToegevoegd = new List<LidViewModel>();
            Leden = CreateLeden();
        }
        public List<LidViewModel> Leden { get; set; }
        public ICollection<SelectListItem> MentorenSelectList { get; set; }
        public ICollection<SelectListItem> LidsoortenSelectList { get; set; }
        public ICollection<SelectListItem> GemeenteSelectList { get; set; }
        public ICollection<SelectListItem> VerantwoordelijkheidSelectList { get; set; }
        public List<LidViewModel> LedenToegevoegd { get; set; }
        public List<LidViewModel> LedenZoeken { get; set; }


        private List<LidViewModel> CreateLeden()
        {
            List<LidViewModel> leden = new List<LidViewModel>();
            leden = _lidService.LedenToViewModelList(_lidService.GetAlleLeden());
            return leden;
        }

        private List<SelectListItem> CreateLidsoortenSelectList()
        {
            var lidsoortenList = new List<SelectListItem>();
            foreach (var lidsoort in _lidsoortService.GetLidSoorten())
            {
                lidsoortenList.Add(new SelectListItem { Text = lidsoort.Soort, Value = lidsoort.Id.ToString() });
            }
            return lidsoortenList;
        }

        private List<SelectListItem> CreateMentorenSelectList()
        {
            var mentorenlist = new List<SelectListItem>();
            mentorenlist.Add(new SelectListItem { Text = "", Value = "0" });
            foreach (var mentor in _lidService.GetAlleMentoren())
            {
                mentorenlist.Add(new SelectListItem { Text = mentor.Voornaam + " " + mentor.Achternaam, Value = mentor.Id.ToString() });
            }
            return mentorenlist;
        }

        private List<SelectListItem> CreateGemeentenSelectList()
        {
            var gemeentenlist = new List<SelectListItem>();
            foreach (var gemeente in _gemeenteService.GetAlleGemeenten())
            {
                gemeentenlist.Add(new SelectListItem { Text = gemeente.Plaats, Value = gemeente.Postcode.ToString() });
            }
            return gemeentenlist;
        }      
    }
}