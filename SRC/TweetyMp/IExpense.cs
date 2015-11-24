using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetyMp
{
    /// <summary>
    /// This interface broadly matches the relevant information from
    /// the parliamentary CSV files on Expenses but in a more useful
    /// way for interaction
    /// </summary>
    interface IExpense
    {
        /// <summary>
        /// The member to which the expense belongs
        /// </summary>
        IMember OwningMember { get; set; }

        /// <summary>
        /// The date of the claim (NULLABLE)
        /// </summary>
        Nullable<DateTime> Date { get; set; }

        /// <summary>
        /// The Claim number string, not always
        /// provided and cannot be relied on as a 
        /// primary key of any sort
        /// </summary>
        string ClaimNumber { get; set; }

        /// <summary>
        /// The high level category of the claim
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// The type of claim within the category
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// The description of the claim
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The detail of the claim
        /// </summary>
        string Detail { get; set; }

        /// <summary>
        /// The amount of money that was claimed,
        /// this number can be negative as there is
        /// inconsistency with how repayments
        /// are recorded
        /// </summary>
        Nullable<double> AmountClaimed { get; set; }

        /// <summary>
        /// Of what was claimed, the amount of money
        /// that was paid
        /// </summary>
        Nullable<double> AmountPaid { get; set; }

        /// <summary>
        /// The amount of money that was not paid, although
        /// this field is not reliable
        /// </summary>
        Nullable<double> AmountNotPaid { get; set; }

        /// <summary>
        /// The amount of money that was repaid if the status
        /// of the claim is Repaid.  However this field
        /// is not reliable and there is inconsistency
        /// with how it is recorded
        /// </summary>
        Nullable<double> AmountRepaid { get; set; }

        /// <summary>
        /// The status of the claim (Paid, Not Paid and Repaid).
        /// This field is not reliable, sometimes a claim is simply
        /// given a negative claim value rather than being flagged
        /// as repaid
        /// </summary>
        Nullable<ExpenseStatus> Status { get; set; }

        /// <summary>
        /// If the claim is not paid, the reason
        /// </summary>
        string ReasonNotPaid { get; set; }

        /// <summary>
        /// Posts an expense online
        /// </summary>
        /// <param name="option"></param>
        /// <param name="previousTweetId"></param>
        /// <param name="previousFacebookId"></param>
        /// <param name="previousTweeterName"></param>
        /// <param name="previousFacebookName"></param>
        void PostOnline(PostOption option, 
            long previousTweetId = 0, 
            long previousFacebookId = 0,
            string previousTweeterName = null,
            string previousFacebookName = null);
    }
}
