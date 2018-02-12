using LaughingCilantro.ObjectModel.Objects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaughingCilantro.Infrastructure.Ofx.Tests
{
    [TestFixture]
    class OfxParserTests
    {
        private const string TestOfxFilesPath = "TestOfxFiles";

        [Test]
        public void MinimalTransactionsParseCorrectly()
        {
            // Timestmap format has milliseconds
            var actual = GetTransactions("MinimalTransactions.txt");

            // Five types of transactions: check, deposit, payment, interest, and point-of-sale
            Assert.That(actual.Count, Is.EqualTo(5));

            var transaction = actual.Single(t => t.OriginalText.StartsWith("PAYPAL"));
            VerifyValues(transaction, "PAYPAL PTE LTD", -13.37, "0000291637142016072000001", new DateTime(2016, 7, 20));

            var deposit = actual.Single(t => t.OriginalText.Contains("EMPLOYER"));
            VerifyValues(deposit, "AWESOME EMPLOYER CANADA", 614.53, "0000291637142016072200001", new DateTime(2016, 7, 22));

            var payment = actual.Single(t => t.OriginalText.Contains("VISA"));
            VerifyValues(payment, "VISA, CIBC/BANQUE CIBC", -368.64, "0000291637142016072500001", new DateTime(2016, 7, 25));

            var interest = actual.Single(t => t.OriginalText.Contains("INTEREST"));
            VerifyValues(interest, "INTEREST", 2.34, "0000291637142016072900001", new DateTime(2016, 7, 29));

            var pointOfSale = actual.Single(t => t.OriginalText.Contains("WHOLESAL"));
            VerifyValues(pointOfSale, "COSTCO WHOLESAL", -133.66, "0000291637142016080100001", new DateTime(2016, 8, 1));
        }

        [Test]
        public void ShorterTimestampTransactionsParseCorrectlyAndTrimsWhitespaceFromNames()
        {
            // Timestamp format doesn't have milliseconds
            // Names contain lots of whitespace in the middle
            var actual = GetTransactions("ShorterTimestampTransactions.txt");
            // Two debits, one credit
            Assert.That(actual.Count, Is.EqualTo(3));

            var credit = actual.Single(t => t.Amount > 0);
            VerifyValues(credit, "CIBC TORONTO", 2000, "02016070600000000000001000", new DateTime(2016, 7, 6));

            var debit = actual.Single(t => t.OriginalText.Contains("GENEVA"));
            VerifyValues(debit, "WP-UNHCR GENEVA", -17.54, "02016070700000000000003000", new DateTime(2016, 7, 7));

            debit = actual.Single(t => t.OriginalText.Contains("KABOB"));
            VerifyValues(debit, "WATAN KABOB MARKHAM", -23.14, "02016070800000000000004000", new DateTime(2016, 7, 8));
        }

        private void VerifyValues(Transaction transaction, string text, double amount, string fitId, DateTime postedDate)
        {
            Assert.That(transaction.OriginalText, Is.EqualTo(text));
            Assert.That(transaction.Amount, Is.EqualTo(amount));
            Assert.That(transaction.ForeignId, Is.EqualTo(fitId));

            Assert.That(transaction.TransactionDateUtc.Year, Is.EqualTo(postedDate.Year));
            Assert.That(transaction.TransactionDateUtc.Month, Is.EqualTo(postedDate.Month));
            Assert.That(transaction.TransactionDateUtc.Day, Is.EqualTo(postedDate.Day));
        }

        private List<Transaction> GetTransactions(string fileName)
        {
            string path = string.Format("{0}/{1}", TestOfxFilesPath, fileName);
            string ofxContent = File.ReadAllText(path);
            var transactions = new OfxParser().GetBankTransactionList(ofxContent);
            return transactions;
        }
    }
}
