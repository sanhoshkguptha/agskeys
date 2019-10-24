using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace agskeys.Models
{
    public class Multiple_proofs_customer
    {
        public IEnumerable<proof_table> proof_table { get; set; }
        public IEnumerable<loan_table> loan_table { get; set; }
        public IEnumerable<proof_customer_table> proof_customer_table { get; set; }
        public answer answers { get; set; }

    }
    public enum answer
    {
        [Display(Name = "Not Applicable")]
        notapplicable,
        [Display(Name = "Pending")]
        pending,
        [Display(Name = "Submitted")]
        submitted

    }
}