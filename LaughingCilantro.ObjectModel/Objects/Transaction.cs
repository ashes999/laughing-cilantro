using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaughingCilantro.ObjectModel.Objects
{
    public class Transaction
    {
        public string Id { get; set; }

        // "FitId" in Quicken parlance
        [Required]
        public string ForeignId { get; set; }

        // Locale-specific date (not UTC)
        [Required]
        public DateTime TransactionDateUtc { get; set; }

        // "Name" in Quick parlance
        [Required]
        public string OriginalText { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}