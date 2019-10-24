using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace agskeys.Models
{
    public class MultipleEmpClass
    {
        public admin_table admin_table { get; set; }
        public emp_category_table emp_category_table { get; set; }
        public customer_profile_table customer_profile_table { get; set; }
        public vendor_table vendor_table { get; set; }

    }
}