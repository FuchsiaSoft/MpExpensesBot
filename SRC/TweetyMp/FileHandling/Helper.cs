using FileHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetyMp.FileHandling
{
    class Helper
    {
        /// <summary>
        /// The standard location that the program can expect to find the
        /// current flat CSV file to work from
        /// </summary>
        private const string EXPENSE_FILE_LOCATION = "C:\\ExpenseFiles\\Current.csv";

        /// <summary>
        /// The standard location that the program can expect to find the current
        /// set of Twitter handles to work from
        /// </summary>
        private const string TWITTER_HANDLE_FILE = "C:\\ExpenseFiles\\TwitterHandles.csv";

        /// <summary>
        /// The standard location that the program can expect to find the current 
        /// set of Facebook handles to work from
        /// </summary>
        private const string FACEBOOK_HANDLE_FILE = "C:\\ExpenseFiles\\FacebookHandles.csv";

        //Below are file locations for twitter access keys, designed to be
        //easily replaceable and not accessible from source code
        private const string PUBLIC_KEY_FILE = "C:\\ExpenseFiles\\public.key";
        private const string SECRET_KEY_FILE = "C:\\ExpenseFiles\\secret.key";
        private const string PUBLIC_TOKEN_FILE = "C:\\ExpenseFiles\\public.token";
        private const string SECRET_TOKEN_FILE = "C:\\ExpenseFiles\\secret.token";
        private const string LAST_RESPONDED_TWEET = "C:\\ExpenseFiles\\last.tweet";

        /// <summary>
        /// A list of all members and their twitter handles (if known)
        /// </summary>
        private static IList<BaseMember> _TwitterHandles = GetAllTwitterHandles();
        private static IList<BaseMember> GetAllTwitterHandles()
        {
            FileHelperEngine<BaseMember> engine =
                new FileHelperEngine<BaseMember>();

            engine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue;

            IEnumerable<BaseMember> members =
                engine.ReadFile(TWITTER_HANDLE_FILE);

            return members.ToList();
        }

        /// <summary>
        /// A list of all members and their Facebook handles (if known)
        /// </summary>
        private static IList<BaseMember> _FacebookHandles = GetAllFacebookHandles();
        private static IList<BaseMember> GetAllFacebookHandles()
        {
            FileHelperEngine<BaseMember> engine =
                new FileHelperEngine<BaseMember>();

            engine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue;

            IEnumerable<BaseMember> members =
                engine.ReadFile(FACEBOOK_HANDLE_FILE);

            return members.ToList();
        }

        internal static void MarkLastTweetId(long id)
        {
            File.WriteAllText(LAST_RESPONDED_TWEET, id.ToString());
        }

        /// <summary>
        /// A helper method that extracts the data from the CSV flat files
        /// and populates a simple local object model of Members and 
        /// their respective Expensive for programmatic access and logic
        /// </summary>
        /// <param name="members">The enumerable of members that
        /// will be populated by this method</param>
        public static void PrepDataModel(ref IList<IMember> members)
        {
            FileHelperEngine<ExpenseMapping> engine =
                new FileHelpers.FileHelperEngine<ExpenseMapping>();

            engine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue;

            IEnumerable<ExpenseMapping> flatExpenses =
                engine.ReadFile(EXPENSE_FILE_LOCATION);

            foreach (ExpenseMapping flatExpense in flatExpenses)
            {
                if (members.Count(m => m.Name == flatExpense.MemberName) == 0)
                {
                    //not got this member in the list yet, add them first
                    members.Add(new Member()
                    {
                        Name = flatExpense.MemberName,
                        Constituency = flatExpense.MemberConstituency,
                        TwitterHandle = FindTwitterHandle(flatExpense.MemberName),
                        FacebookHandle = FindFacebookHandle(flatExpense.MemberName)
                    });
                }

                IMember currentMember = 
                    members.First(m => m.Name == flatExpense.MemberName);

                currentMember.Expenses.Add(new Expense(currentMember)
                {
                    Date = flatExpense.Date,
                    ClaimNumber = flatExpense.ClaimNumber,
                    Type = flatExpense.Type,
                    Category = flatExpense.Category,
                    Description = flatExpense.Description,
                    Detail = flatExpense.Details,
                    Status = flatExpense.Status,
                    AmountClaimed = flatExpense.AmountClaimed,
                    AmountPaid = flatExpense.AmountPaid,
                    AmountNotPaid = flatExpense.AmountNotPaid,
                    AmountRepaid = flatExpense.AmountRepaid
                });
            }
        }

        /// <summary>
        /// Finds and returns a facebook handle for the member
        /// if it is in the standard file
        /// </summary>
        /// <param name="memberName">The name of the member</param>
        /// <returns></returns>
        public static string FindFacebookHandle(string memberName)
        {
            BaseMember member = _FacebookHandles
                                    .Where(m => m.Name == memberName)
                                    .FirstOrDefault();

            if (member == null || string.IsNullOrEmpty(member.Handle))
            {
                return null;
            }
            else
            {
                return member.Handle;
            }
        }

        /// <summary>
        /// Finds and returns a twitter handle for the member
        /// if it is in the standard file
        /// </summary>
        /// <param name="memberName">The name of the member</param>
        /// <returns></returns>
        public static string FindTwitterHandle(string memberName)
        {
            BaseMember member = _TwitterHandles
                                    .Where(m => m.Name == memberName)
                                    .FirstOrDefault();

            if (member == null || string.IsNullOrEmpty(member.Handle))
            {
                return null;
            }
            else
            {
                return member.Handle;
            }
        }


        public static string GetPublicKey()
        {
            return File.ReadAllText(PUBLIC_KEY_FILE);
        }

        public static string GetSecretKey()
        {
            return File.ReadAllText(SECRET_KEY_FILE);
        }

        public static string GetPublicToken()
        {
            return File.ReadAllText(PUBLIC_TOKEN_FILE);
        }

        public static string GetSecretToken()
        {
            return File.ReadAllText(SECRET_TOKEN_FILE);
        }

        public static long GetLastKnownTweetId()
        {
            string tweetId = File.ReadAllText(LAST_RESPONDED_TWEET);

            long id;

            long.TryParse(tweetId, out id);

            return id;
        }


    }
}
