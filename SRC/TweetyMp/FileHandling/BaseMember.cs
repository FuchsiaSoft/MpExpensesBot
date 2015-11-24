using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace TweetyMp.FileHandling
{
    /// <summary>
    /// This is a mapping class just used to access a standard CSV
    /// file of all known members with twitter and facebook
    /// addresses, it works in the same way as
    /// ExpenseMapping class
    /// </summary>
    [DelimitedRecord(",")]
    [IgnoreEmptyLines(true)]
    [IgnoreFirst(1)]
    class BaseMember
    {
        [FieldQuoted(QuoteMode.OptionalForRead)]
        public string Name;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        public string Handle;
    }
}
