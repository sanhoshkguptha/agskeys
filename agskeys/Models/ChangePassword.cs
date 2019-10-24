using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace agskeys.Models
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "You must enter a password")]
        [MembershipPassword(
        MinRequiredNonAlphanumericCharacters = 1,
        MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
        ErrorMessage = "Your password must be 6 characters long and contain at least one symbol (!, @, #, etc).",
        MinRequiredPasswordLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        //[Remote("passwordmatch", "UserProfile", HttpMethod = "POST")]

        public string password { get; set; }

        [Required(ErrorMessage = "You must enter a password")]
        [MembershipPassword(
       MinRequiredNonAlphanumericCharacters = 1,
       MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
       ErrorMessage = "Your password must be 6 characters long and contain at least one symbol (!, @, #, etc).",
       MinRequiredPasswordLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string newpassword { get; set; }

        [Required(ErrorMessage = "You must enter a password")]
        [MembershipPassword(
       MinRequiredNonAlphanumericCharacters = 1,
       MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
       ErrorMessage = "Your password must be 6 characters long and contain at least one symbol (!, @, #, etc).",
       MinRequiredPasswordLength = 6)]
        [System.ComponentModel.DataAnnotations.Compare("newpassword", ErrorMessage = "Both NewPassword and RetypePassword Must be Same")]
        [DataType(DataType.Password)]
        [Display(Name = "Retype Password")]
        
        public string retypepassword { get; set; }
    }
}