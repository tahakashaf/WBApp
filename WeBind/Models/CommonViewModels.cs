using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class CommonViewModels
    {
        public RegisterViewModel RegisterVM { get; set; }
        public LoginViewModel LoginVM { get; set; }
        public ExpertProfileViewModel ExpertVM { get; set; }
        public WebinarViewModel WebinarVM { get; set; }
        public List<ExpertProfileViewModel> ExpertList { get; set; }
    }

    public class CommonMessageViewModel
    {
        public string Message { get; set; }
        public int Status { get; set; }
    }
}