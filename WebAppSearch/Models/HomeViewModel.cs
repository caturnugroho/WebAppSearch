using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppSearch.Models
{
    public class HomeViewModel
    {
        [Display(Name = "Import CSV File")]
        public HttpPostedFileBase FileAttach { get; set; }

        [Display(Name = "Search")]
        public String Keyword { get; set; }

        public List<contents> Data { get; set; }

        public List<contents> results { get; set; }
    }
}