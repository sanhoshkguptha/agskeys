using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace agskeys.Models
{
    public class ProcessExecutiveLoan
    {
        public int id { get; set; }

        //[Required(ErrorMessage = "You must choose customer id")]
        [Display(Name = "Customer Id")]
        public string customerid { get; set; }


        [Display(Name = "Partner")]
        public string partnerid { get; set; }

        //[Required(ErrorMessage = "You must choose Bank")]
        [Display(Name = "Bank")]
        public string bankid { get; set; }

        //[Required(ErrorMessage = "You must choose loan type")]
        [Display(Name = "Loan Type")]
        public string loantype { get; set; }

        //[Required(ErrorMessage = "Please enter loan amount")]
        [Display(Name = "Request Loan Amount")]
        public string requestloanamt { get; set; }

        // [Required(ErrorMessage = "Please enter loan amount")]
        [Display(Name = "Loan Amount")]
        public string loanamt { get; set; }

        // [Required(ErrorMessage = "Please enter disbursement amount")]
        [Display(Name = "Disbursement Amount")]
        public string disbursementamt { get; set; }

        // [Required(ErrorMessage = "Please enter rate of intrest")]
        [Display(Name = "Disbursement Rate Of Intrest")]
        public string rateofinterest { get; set; }

        [Display(Name = "Sactioned Copy")]
        public string sactionedcopy { get; set; }

        public HttpPostedFileBase sactionedCopyFile { get; set; }

        [Display(Name = "DD Copy")]
        public string idcopy { get; set; }

        public HttpPostedFileBase idCopyFile { get; set; }

        [Display(Name = "Added By")]
        public string addedby { get; set; }

        [Display(Name = "Added Date")]
        public string datex { get; set; }

        [Display(Name = "Employee Category")]
        public string employeetype { get; set; }

        [Display(Name = "Employee")]
        public string employee { get; set; }

        [Display(Name = "Internal Comment")]
        [DataType(DataType.MultilineText)]
        public string internalcomment { get; set; }

        [Display(Name = "External Comment")]
        [DataType(DataType.MultilineText)]
        public string externalcomment { get; set; }

        [Display(Name = "Loan Status")]
        public string loanstatus { get; set; }

        [Display(Name = "Property Details")]
        [DataType(DataType.MultilineText)]
        public string propertydetails { get; set; }

        [Display(Name = "Follow up Date")]
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]

        //[DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM / dd / yy H: mm:ss tt}", ApplyFormatInEditMode = true)]
        public string followupdate { get; set; }

        [Display(Name = "Property Documents")]
        public string propertydocuments { get; set; }

        public HttpPostedFileBase propertyDocumentsFile { get; set; }

        public string proofid { get; set; }
        public string proofans { get; set; }

        [Display(Name = "Technical")]
        public string technical { get; set; }

        [Display(Name = "Legal")]
        public string legal { get; set; }

        [Display(Name = "RCU")]
        public string rcu { get; set; }

    }
}