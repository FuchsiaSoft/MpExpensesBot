using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetyMp
{
    class Member : IMember
    {
        #region Constructors

        public Member() { Expenses = new List<IExpense>(); }

        #endregion

        #region Private Fields

        #endregion

        #region Properties

        public string Constituency { get; set; }

        public IList<IExpense> Expenses { get; set; }

        public string FacebookHandle { get; set; }

        public string Name { get; set; }

        public string TwitterHandle { get; set; }

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        #endregion
    }
}
