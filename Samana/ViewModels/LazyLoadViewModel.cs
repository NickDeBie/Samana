using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Samana.ViewModels
{
    public class LazyLoadViewModel
    {
        public LazyLoadViewModel()
        {
            Leden = new List<LidViewModel>();
        }
        public LazyLoadViewModel(int limit, int fromRowNumber, string containerId, string ajaxActionUrl, List<LidViewModel> leden)
        {
            Limit = limit;
            FromRowNumber = fromRowNumber;
            ContainerId = containerId;
            AjaxActionUrl = ajaxActionUrl;
            Leden = leden;
        }
        public int Limit { get; set; }
        public int FromRowNumber { get; set; }
        public string ContainerId { get; set; }
        public string AjaxActionUrl { get; set; }
        public List<LidViewModel> Leden { get; set; }
    }
}