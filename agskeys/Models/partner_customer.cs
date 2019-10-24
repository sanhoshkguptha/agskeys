using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace agskeys.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public partial class partner_customer
    {
        public int id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "This field can not be empty.")]
        public string name { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "You must provide a phone number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string phoneno { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Please enter loan amount")]
        [Display(Name = "Request Loan Amount")]
        public string requestloanamt { get; set; }

        [Display(Name = "Internal Comment")]
        [Required(ErrorMessage = "You must provide a Comment")]
        [DataType(DataType.MultilineText)]
        public string internalcomment { get; set; }

        [Required(ErrorMessage = "You must choose loan type")]
        [Display(Name = "Loan Type")]
        public string loantype { get; set; }

        [Display(Name = "Added By")]
        public string addedby { get; set; }

        [Display(Name = "Added Date")]
        public string datex { get; set; }
    }
}