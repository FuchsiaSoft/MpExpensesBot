using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TweetSharp;
using TweetyMp.FileHandling;

namespace TweetyMp
{
    class Expense : IExpense
    {
        #region Constructors

        public Expense(IMember owner)
        {
            OwningMember = owner;
        }

        #endregion

        #region Private Fields

        #endregion

        #region Properties

        public IMember OwningMember { get; set; }

        public Nullable<double> AmountClaimed { get; set; }

        public Nullable<double> AmountNotPaid { get; set; }

        public Nullable<double> AmountPaid { get; set; }

        public Nullable<double> AmountRepaid { get; set; }

        public string Category { get; set; }

        public string ClaimNumber { get; set; }

        public DateTime? Date { get; set; }

        public string Description { get; set; }

        public string Detail { get; set; }

        public string ReasonNotPaid { get; set; }

        public ExpenseStatus? Status { get; set; }

        public string Type { get; set; }

        #endregion

        #region Private Methods

        private string GetTweetFriendlyDescription()
        {
            string suitableString = Detail;

            if (string.IsNullOrEmpty(suitableString) == false &&
                suitableString.Length < 50)
            {
                return suitableString;
            }
            else
            {
                suitableString = Description;

                if (string.IsNullOrEmpty(suitableString) == false &&
                    suitableString.Length < 50)
                {
                    return suitableString;
                }
                else
                {
                    suitableString = Type;

                    if (string.IsNullOrEmpty(suitableString) == false &&
                        suitableString.Length < 50)
                    {
                        return suitableString;
                    }
                    else
                    {
                        suitableString = Category;

                        if (suitableString.Length < 50)
                        {
                            return suitableString;
                        }
                        else
                        {
                            return suitableString.Substring(0, 50);
                        }
                    }
                }
            }
        }

        private string GetTweetText(string previousTweeterName = null)
        {
            StringBuilder builder = new StringBuilder();

            if (previousTweeterName != null)
            {
                builder.Append("@");
                builder.Append(previousTweeterName + " ");
            }

            builder.Append("On ");
            builder.Append(Date?.ToString("dd/MM/yy") + " ");
            
            if (OwningMember.TwitterHandle != null)
            {
                builder.Append(OwningMember.TwitterHandle + " (");
                builder.Append(OwningMember.Name);
                builder.Append(")");
            }
            else
            {
                builder.Append(OwningMember.Name);
            }

            builder.Append(" claimed ");
            builder.Append(AmountClaimed?.ToString("£#,##0.00"));
            builder.Append(" for \"");
            builder.Append(GetTweetFriendlyDescription());
            builder.Append("\" #AccHack15");

            return builder.ToString();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Posts a status update on Facebook and/or twitter
        /// </summary>
        /// <param name="option"></param>
        /// <param name="previousTweetId"></param>
        /// <param name="previousFacebookId"></param>
        /// <param name="previousTweeterName"></param>
        /// <param name="previousFacebookName"></param>
        public void PostOnline(PostOption postOption,
            long previousTweetId = 0,
            long previousFacebookId = 0,
            string previousTweeterName = null,
            string previousFacebookName = null)
        {
            if (postOption == PostOption.Twitter ||
                postOption == PostOption.Both)
            {
                TwitterService service =
                    new TwitterService(Helper.GetPublicKey(), Helper.GetSecretKey());

                service.AuthenticateWith(Helper.GetPublicToken(), Helper.GetSecretToken());

                SendTweetOptions options = new SendTweetOptions()
                {
                    Status = GetTweetText(previousTweeterName)
                };

                if (previousTweetId != 0 && previousTweeterName != null)
                {
                    options.InReplyToStatusId = previousTweetId;
                }

                service.SendTweet(options);

                if (service.Response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    //probably hit a rate limit, chill out!
                    Thread.Sleep(600000);
                }
            }

            if (postOption == PostOption.Facebook ||
                postOption == PostOption.Both)
            {
                //TODO: implement facebook here
            }
            
        }

        #endregion
    }
}
