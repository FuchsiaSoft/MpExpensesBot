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

    class Program
    {
        private static IList<IMember> _AllMembers = new List<IMember>();
        private static Random _Random = new Random();

        static void Main(string[] args)
        {
            Helper.PrepDataModel(ref _AllMembers);

            Thread randomThread = new Thread(DoMainRandomLoop);

            randomThread.Start();

            Thread checkerThread = new Thread(DoCheckingLoop);

            checkerThread.Start();
        }

        private static void DoCheckingLoop()
        {
            do
            {
                try
                {
                    TwitterStatus nextDue = GetNextResponseDue();

                    if (nextDue != null)
                    {
                        TwitterService service =
                            new TwitterService(Helper.GetPublicKey(), Helper.GetSecretKey());

                        service.AuthenticateWith(Helper.GetPublicToken(), Helper.GetSecretToken());

                        IMember mentionedMember = FindMember(nextDue.Text);

                        if (mentionedMember != null)
                        {
                            IList<IExpense> availableExpenses = mentionedMember.Expenses
                               .Where(e => e.AmountClaimed > 0 &&
                                           e.Status == ExpenseStatus.Paid).ToList();

                            if (availableExpenses.Count > 0)
                            {
                                int chosenExpenseIndex = _Random.Next(0, availableExpenses.Count);
                                IExpense chosenExpense = availableExpenses[chosenExpenseIndex];
                                chosenExpense.PostOnline
                                    (PostOption.Twitter, nextDue.Id, 0, 
                                    nextDue.User.ScreenName);
                            }
                            else
                            {
                                service.SendTweet(new SendTweetOptions()
                                {
                                    InReplyToStatusId = nextDue.Id,
                                    Status = "@" + nextDue.User.ScreenName +
                                    " sorry we can't find any expenses for that MP"
                                });
                            }
                        }
                        else
                        {
                            service.SendTweet(new SendTweetOptions()
                            {
                                InReplyToStatusId = nextDue.Id,
                                Status = "@" + nextDue.User.ScreenName + 
                                " sorry we couldn't find that MP, check https://goo.gl/YPdS7C " +
                                "for valid MPs"
                            });
                        }


                        Helper.MarkLastTweetId(nextDue.Id);
                    }

                    
                }
                catch (Exception)
                {
                    //do nothing at the minute
                }

                Thread.Sleep(180000);

            } while (true);
        }

        private static IMember FindMember(string text)
        {
            foreach (IMember member in _AllMembers)
            {
                if (text.ToUpper().Contains(member.Name.ToUpper()) ||
                                text.ToUpper().Contains
                                (member.TwitterHandle == null ? member.Name : member.TwitterHandle))
                {
                    return member;
                }
            }

            return null;
        }

        private static TwitterStatus GetNextResponseDue()
        {
            long lastTweetId = Helper.GetLastKnownTweetId();

            TwitterService service =
                new TwitterService(Helper.GetPublicKey(), Helper.GetSecretKey());

            service.AuthenticateWith(Helper.GetPublicToken(), Helper.GetSecretToken());

            ListTweetsMentioningMeOptions options = new ListTweetsMentioningMeOptions()
            {
                SinceId = lastTweetId,
                Count = 1
            };

            IEnumerable<TwitterStatus> myTweets =
                service.ListTweetsMentioningMe(options);

            return myTweets.FirstOrDefault();
        }

        private static void DoMainRandomLoop()
        {
            do
            {
                try
                {
                    int chosenMpIndex = _Random.Next(0, _AllMembers.Count);
                    IMember chosenMember = _AllMembers[chosenMpIndex];

                    IList<IExpense> availableExpenses = chosenMember.Expenses
                        .Where(e => e.AmountClaimed > 0 &&
                                    e.Status == ExpenseStatus.Paid).ToList();

                    if (availableExpenses.Count > 0)
                    {
                        int chosenExpenseIndex = _Random.Next(0, availableExpenses.Count);
                        IExpense chosenExpense = availableExpenses[chosenExpenseIndex];
                        chosenExpense.PostOnline(PostOption.Twitter);
                    }

                    Thread.Sleep(2400000); //wait 40 mins between
                }
                catch (Exception)
                {
                    //nothing at the minute
                    //Still at hack stage
                }

            } while (true);
        }
    }
}
