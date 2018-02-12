using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LaughingCilantro.ObjectModel.Interfaces;
using LaughingCilantro.ObjectModel.Objects;

namespace LaughingCilantro.Infrastructure.Ofx
{
    public class OfxParser : IOfxParser
    {
        /// <summary>
        /// Regular expression to match <stmttrn>...</stmttrn> blocks (statement transactions).  With PC, we get one line per transaction.
        /// With TD, we get better XML with one tag per line; so we have to use Singleline to match everything. But if we do that, (.*) is
        /// greedy and matches the first opening tag with the last closing tag. Using .*? makes it non-greedy, and everything Just Works.
        /// </summary>
        private static Regex statementRegex = new Regex(@"<stmttrn>(.*?)<\/stmttrn>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static Regex whitespaceRunsRegex = new Regex(@"\s{2,}");
         
        
        // TODO: make this more generic. We just care about transactions.

        public List<Transaction> GetBankTransactionList(string contents)
        {
            var upperCaseContents = contents.ToUpper();
            var toReturn = new List<Transaction>();
            var startIndex = upperCaseContents.IndexOf("<BANKTRANLIST>");
            var stopIndex = upperCaseContents.IndexOf("</BANKTRANLIST>");

            if (startIndex > -1 && stopIndex > -1)
            {
                string list = contents.Substring(startIndex, stopIndex - startIndex);
                var groups = statementRegex.Matches(list);
                foreach (Match group in groups)
                {
                    var value = group.Value;
                    var start = value.IndexOf("<DTPOSTED>") + 10;
                    var stop = value.IndexOf("<", start);
                    var rawDate = value.Substring(start, stop - start).Trim();

                    start = value.IndexOf("<TRNAMT>") + 8;
                    stop = value.IndexOf("<", start);
                    var rawAmount = value.Substring(start, stop - start).Trim();

                    start = value.IndexOf("<FITID>") + 7;
                    stop = value.IndexOf("<", start);
                    var rawId = value.Substring(start, stop - start).Trim();

                    // Look for NAME tag
                    start = value.IndexOf("<NAME>");
                    if (start == -1)
                    {
                        start = value.IndexOf("<MEMO>");
                    }
                    if (start == -1)
                    {
                        throw new InvalidOperationException("Statement doesn't have a NAME or MEMO tag: " + value);
                    }
                    start += 6; // Length of tag
                    stop = value.IndexOf("<", start);
                    var rawName = value.Substring(start, stop - start).Trim();
                    // Condense multiple runs of spaces into single spaces
                    rawName = whitespaceRunsRegex.Replace(rawName, " ");

                    toReturn.Add(new Transaction() {
                        Amount = decimal.Parse(rawAmount),
                        ForeignId = rawId,
                        OriginalText = rawName,
                        TransactionDateUtc = ParseDateTime(rawDate)
                    });
                }
            }

            return toReturn;
        }

        // Looks like this is ISO-8601 date-time, with the timezone in square brackets
        private DateTime ParseDateTime(string rawDate)
        {
            var timezoneStart = rawDate.IndexOf('[');
            var timezoneEnd = rawDate.IndexOf(':', timezoneStart);

            var timestamp = rawDate.Substring(0, timezoneStart);
            var timezoneOffset = rawDate.Substring(timezoneStart + 1, timezoneEnd - timezoneStart - 1);

            DateTime toReturn = DateTime.MinValue;
            if (!DateTime.TryParseExact(timestamp, "yyyyMMddhhmmss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out toReturn))
            {
                toReturn = DateTime.ParseExact(timestamp, "yyyyMMddhhmmss", System.Globalization.CultureInfo.InvariantCulture);
            }
            toReturn.AddHours(int.Parse(timezoneOffset));
            return toReturn;
        }
    }
}
