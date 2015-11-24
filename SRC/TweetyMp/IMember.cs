using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetyMp
{
    interface IMember
    {
        /// <summary>
        /// The name of the Member
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The name of the member's constituency
        /// </summary>
        string Constituency { get; set; }

        /// <summary>
        /// The twitter handle for the member (if known)
        /// </summary>
        string TwitterHandle { get; set; }

        /// <summary>
        /// The Facebook handle for the member (if known)
        /// </summary>
        string FacebookHandle { get; set; }

        /// <summary>
        /// An IList of all the member's expenses
        /// </summary>
        IList<IExpense> Expenses { get; set; }
    }
}
