using Samana.ViewModels;
using System.Web.Mvc;

namespace Samana.Controllers
{
    public class LedenController : Controller
    {
        private LedenViewModel _ledenViewModel = new LedenViewModel();
        // GET: Leden
        public ActionResult Index()
        {
            return View(_ledenViewModel);
        }
    }
}