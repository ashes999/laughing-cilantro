using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LaughingCilantro.ObjectModel.Objects
{
    public class FinancialAccount
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public AccountType AccountType { get; set; }
    }
    
    public enum AccountType
    {
        [Display(Name = "President's Choice")]
        PresidentsChoice,
        [Display(Name = "TD (Toronto Dominion)")]
        TorontoDominion,
        [Display(Name = "VISA")]
        Visa,
        [Display(Name = "MasterCard")]
        Mastercard,
        Other
    }
}