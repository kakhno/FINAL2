using FINAL2.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Diagnostics;

namespace FINAL2.Model
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        public int PersonId { get; set; }
        public LoanType Type { get; set; } = LoanType.QuickLoan;
        public Currency Currency { get; set; } = Currency.USD;
        public int Period { get; set; }
        public Status Status { get; set; } = Status.InProcess;
        public int Amount { get; set; }
        public Person Person { get; set; }

    }
    public enum Currency
    {
        USD = 0,
        EUR = 1,
        GBP = 3,
        JPY = 4,
        AUD = 5,
        CAD = 6
    }
    public enum Status
    {
        InProcess = 0,
        Approved = 1,
        Rejected = 2
    }
    public enum LoanType
    {
        QuickLoan = 0,
        CarLoan = 1,
        Installment = 2
    }

}