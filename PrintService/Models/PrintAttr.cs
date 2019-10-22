using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrintService.Models
{
    public struct PrintAttr
    {
        public string DocUrl { get; set; }

        public bool Vertical { get; set; }
    }
}