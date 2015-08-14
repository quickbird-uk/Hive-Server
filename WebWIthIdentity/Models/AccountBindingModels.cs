using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebWIthIdentity.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel : IValidatableObject
    {
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public long Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Twitter")]
        public string Twitter { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string notnullEmail = Email ?? string.Empty; 

                //phone is missing                    Email is invalid
            if (Phone < 10000 && (notnullEmail.Length > 5 && !notnullEmail.Contains("@")))
            {
                yield return new ValidationResult("Provide valid Phone or Email");
            }
        }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SearchBindingModel  
    {
        [Required]
        public virtual List<string> SearchContacts { get; set; }

        public SearchBindingModel()
        {
            SearchContacts = new List<string>();
        }

    }

    public class ChangeEmailBindingModel : IValidatableObject
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string notnullEmail = Email ?? string.Empty;

            // if it's too short or does not have at character
            if (notnullEmail.Length < 5 || !notnullEmail.Contains("@"))
            {
                yield return new ValidationResult("Email is invalid");
            }
        }
    }

    public class ChangePhoneBindingModel : IValidatableObject
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public long PhoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // if it's too short or does not have at character
            if (PhoneNumber < 50000 )
            {
                yield return new ValidationResult("Phone is invalid");
            }
        }
    }

    public class ChangeMiscBindingModel : IValidatableObject
    {
     
        public string Twitter { get; set; }


        public string Name { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // if it's too short or does not have at character
            if (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Twitter))
            {
                yield return new ValidationResult("You provided no information");
            }
        }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
