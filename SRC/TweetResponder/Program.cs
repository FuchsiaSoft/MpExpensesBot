using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TweetSharp;

namespace TweetResponder
{
    class Program
    {
        private static List<string> _AllMpNames = new List<string>();
        private static List<string> _AllMpTwitterHandles = new List<string>();

        static void Main(string[] args)
        {
            using (TweetyMpEntities2 db = new TweetyMpEntities2())
            {
                IEnumerable<MpExpensesTable> rows = db.MpExpensesTables;

                foreach (MpExpensesTable row in rows)
                {
                    if (_AllMpNames.Contains(row.C_MP_s_Name))
                    {
                        // do nothing
                    }
                    else
                    {
                        _AllMpNames.Add(row.C_MP_s_Name);
                    }

                    if (_AllMpTwitterHandles.Contains(row.C_MP_s_Twitter))
                    {
                        //do nothing
                    }
                    else
                    {
                        _AllMpTwitterHandles.Add(row.C_MP_s_Twitter);
                    }
                }
            }

            Thread autoThread = new Thread(() =>
            {
                do
                {

                    using (TweetyMpEntities2 db = new TweetyMpEntities2())
                    {
                        int randomMpIndex = ChooseRandomInt(0, _AllMpTwitterHandles.Count - 1);

                        string chosenTwitterHandle = _AllMpTwitterHandles[randomMpIndex];

                        List<MpExpensesTable> rows = db.MpExpensesTables
                            .Where(m => m.C_MP_s_Twitter.ToUpper() == chosenTwitterHandle.ToUpper()
                                    && m.C_Details != null && m.C_Details != ""
                                    && m.C_MP_s_Twitter != null && m.C_MP_s_Twitter != "TWITTER_UNKNOWN")
                                    .ToList();

                        if (rows.Count == 0)
                        {
                            //do nothing
                        }
                        else
                        {

                            TwitterService service = new TwitterService(GetPublicKey(), GetSecretKey());
                            service.AuthenticateWith(GetPublicToken(), GetSecretToken());

                            int chosenIndex = ChooseRandomInt(0, rows.Count - 1);

                            MpExpensesTable expense = rows[chosenIndex];

                            StringBuilder builder = new StringBuilder();

                            builder.Append("On ");
                            builder.Append(expense.RealDate.ToString("dd/MM/yy") + " ");
                            builder.Append(expense.C_MP_s_Twitter + " ");
                            builder.Append(" (" + expense.C_MP_s_Name + ") ");
                            builder.Append("claimed ");
                            builder.Append(expense.Amount_ClaimedReal.ToString("£#,##0.00") + " ");
                            builder.Append("for ");
                            builder.Append("\"");
                            builder.Append(expense.C_Details);
                            builder.Append("\"");
                            builder.Append("  #AccHack15");

                            SendTweetOptions options = new SendTweetOptions()
                            {
                                Status = builder.ToString()
                            };

                            service.SendTweet(options);
                        }

                        
                        
                    }

                    Thread.Sleep(600000);

                } while (true);

                
            });
            autoThread.Start();

            do
            {

                try
                {
                    CheckAndRespond();
                }
                catch (Exception)
                {
                    //do nothing, no time to error handle in hacks baby!
                }

                Thread.Sleep(180000);

            } while (true);
        }

        private static string GetSecretKey()
        {
            return File.ReadAllText("C:\\TwitterKeys\\secret.key");
        }

        private static string GetPublicKey()
        {
            return File.ReadAllText("C:\\TwitterKeys\\public.key");
        }

        private static string GetSecretToken()
        {
            return File.ReadAllText("C:\\TwitterKeys\\secret.token");
        }

        private static string GetPublicToken()
        {
            return File.ReadAllText("C:\\TwitterKeys\\public.token");
        }

        static void CheckAndRespond()
        {
            TwitterService service = new TwitterService(GetPublicKey(), GetSecretKey());
            service.AuthenticateWith(GetPublicToken(), GetSecretToken());

            ListTweetsMentioningMeOptions options = new ListTweetsMentioningMeOptions();

            if (Properties.Settings.Default.LastRespondedTweet != 0)
            {
                options.SinceId = Properties.Settings.Default.LastRespondedTweet;
            }

            IEnumerable<TwitterStatus> myMentions = service.ListTweetsMentioningMe(options);

            if (myMentions.Count() > 0)
            {
                myMentions = myMentions.OrderBy(o => o.Id);

                RespondToTweet(myMentions.First(), service);
            }

        }

        private static void RespondToTweet(TwitterStatus twitterStatus, TwitterService service)
        {

            //does the tweet contain an MP name we know of?
            foreach (string mpName in _AllMpNames)
            {
                if (twitterStatus.Text.ToUpper().Contains(mpName.ToUpper()))
                {
                    //found an MP, tweet an expense

                    TweetExpenseFromName(twitterStatus, service, mpName);

                    return;
                }

            }

            foreach (string twitterHandle in _AllMpTwitterHandles)
            {
                if (twitterStatus.Text.ToUpper().Contains(twitterHandle.ToUpper()))
                {
                    //found an MP tweet an expense

                    TweetExpenseFromTwitterHandle(twitterStatus, service, twitterHandle);

                    return;
                }
            }

            //if it gets here, then didn't match anything
            SendTweetOptions options = new SendTweetOptions()
            {
                InReplyToStatusId = twitterStatus.Id,
                Status = "@" + twitterStatus.User.ScreenName + " sorry we couldn't find that MP, check https://goo.gl/YPdS7C for valid MPs"
            };

            service.SendTweet(options);

            Properties.Settings.Default.LastRespondedTweet = twitterStatus.Id;
            Properties.Settings.Default.Save();

            
        }

        private static void TweetExpenseFromTwitterHandle(TwitterStatus twitterStatus, TwitterService service, string twitterHandle)
        {
            using (TweetyMpEntities2 db = new TweetyMpEntities2())
            {
                List<MpExpensesTable> rows = db.MpExpensesTables
                    .Where(m => m.C_MP_s_Twitter.ToUpper() == twitterHandle.ToUpper()
                            && m.C_Details != null && m.C_Details != "" 
                            && m.C_MP_s_Twitter != null && m.C_MP_s_Twitter != "TWITTER_UNKNOWN")
                    .ToList();

                if (rows.Count == 0)
                {
                    SendTweetOptions failOptions = new SendTweetOptions()
                    {
                        InReplyToStatusId = twitterStatus.Id,
                        Status = "Sorry we couldn't find an expense for that MP"
                    };

                    service.SendTweet(failOptions);

                    Properties.Settings.Default.LastRespondedTweet = twitterStatus.Id;
                    Properties.Settings.Default.Save();

                    return;
                }

                int chosenIndex = ChooseRandomInt(0, rows.Count - 1);

                MpExpensesTable expense = rows[chosenIndex];

                StringBuilder builder = new StringBuilder();

                builder.Append("@" + twitterStatus.User.ScreenName);
                builder.Append(" on ");
                builder.Append(expense.RealDate.ToString("dd/MM/yy") + " ");
                builder.Append(expense.C_MP_s_Twitter + " ");
                builder.Append(" (" + expense.C_MP_s_Name + ") ");
                builder.Append("claimed ");
                builder.Append(expense.Amount_ClaimedReal.ToString("£#,##0.00") + " ");
                builder.Append("for ");
                builder.Append("\"");
                builder.Append(expense.C_Details);
                builder.Append("\"");
                builder.Append("  #AccHack15");

                SendTweetOptions options = new SendTweetOptions()
                {
                    InReplyToStatusId = twitterStatus.Id,
                    Status = builder.ToString()
                };

                service.SendTweet(options);

                Properties.Settings.Default.LastRespondedTweet = twitterStatus.Id;
                Properties.Settings.Default.Save();

                return;
            }

            
        }

        private static void TweetExpenseFromName(TwitterStatus twitterStatus, TwitterService service, string mpName)
        {

            using (TweetyMpEntities2 db = new TweetyMpEntities2())
            {
                List<MpExpensesTable> rows = db.MpExpensesTables
                    .Where(m => m.C_MP_s_Name.ToUpper() == mpName.ToUpper()
                            && m.C_Details != null && m.C_Details != ""
                            && m.C_MP_s_Twitter != null && m.C_MP_s_Twitter != "TWITTER_UNKNOWN")
                    .ToList();

                if (rows.Count == 0)
                {
                    SendTweetOptions failOptions = new SendTweetOptions()
                    {
                        InReplyToStatusId = twitterStatus.Id,
                        Status = "Sorry we couldn't find an expense for that MP"
                    };

                    service.SendTweet(failOptions);

                    Properties.Settings.Default.LastRespondedTweet = twitterStatus.Id;
                    Properties.Settings.Default.Save();

                    return;
                }

                int chosenIndex = ChooseRandomInt(0, rows.Count - 1);

                MpExpensesTable expense = rows[chosenIndex];

                StringBuilder builder = new StringBuilder();

                builder.Append("@" + twitterStatus.User.ScreenName);
                builder.Append(" on ");
                builder.Append(expense.RealDate.ToString("dd/MM/yy") + " ");
                builder.Append(expense.C_MP_s_Twitter + " ");
                builder.Append(" (" + expense.C_MP_s_Name + ") ");
                builder.Append("claimed ");
                builder.Append(expense.Amount_ClaimedReal.ToString("£#,##0.00") + " ");
                builder.Append("for ");
                builder.Append("\"");
                builder.Append(expense.C_Details);
                builder.Append("\"");
                builder.Append("  #AccHack15");

                SendTweetOptions options = new SendTweetOptions()
                {
                    InReplyToStatusId = twitterStatus.Id,
                    Status = builder.ToString()
                };

                service.SendTweet(options);

                Properties.Settings.Default.LastRespondedTweet = twitterStatus.Id;
                Properties.Settings.Default.Save();

                return;
            }
        }

        private static Random _Random = new Random();
        private static int ChooseRandomInt(int min, int max)
        {
            return _Random.Next(min, max);
        }
    }
}
