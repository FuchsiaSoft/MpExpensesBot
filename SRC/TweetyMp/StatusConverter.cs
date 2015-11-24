using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetyMp
{
    class StatusConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            switch (from)
            {
                case "Paid":
                    return ExpenseStatus.Paid;
                case "NotPaid":
                    return ExpenseStatus.NotPaid;
                case "Repaid":
                    return ExpenseStatus.Repaid;
                default:
                    return null;
            }
        }
    }
}
