using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace agskeys.Models
{
    public class loan_track
    {
        public IEnumerable<loan_track_table> employee_track { get; set; }
        public IEnumerable<vendor_track_table> vendor_track { get; set; }
        public IEnumerable<loan_table> loan_details { get; set; }
    }
}