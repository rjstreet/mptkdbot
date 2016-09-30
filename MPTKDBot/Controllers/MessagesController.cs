using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;
using MPTKDDataEntry.Models;
using System.IO;
using System.Collections.Generic;

namespace MPTKDBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        static string LUIS_URI = "https://api.projectoxford.ai";
        static string MPTKDDE_URI = "https://mptkddataentry.azurewebsites.net";
        static string LUIS_PARSE = "/luis/v1/application?id=546857e1-1860-485d-ba79-0fc5f00afad8&subscription-key=a5574211c74f42a4a0e8907b17909efd&q={query}&timezoneOffset=-5.0";
        static string MPTKDDE_PARSE = "/api/BotInfo/{id}";
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                string aLine;
                StringReader strReader = new StringReader(parseIntent(activity.Text));
                while (true)
                {
                    aLine = strReader.ReadLine();
                    if (aLine != null)
                    {
                        Activity reply = activity.CreateReply(aLine);
                        //activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    else
                        break;
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Dictionary<DateTime, string>getHolidayDictionary(BotInfo input)
        {
            Dictionary<DateTime, string> dict = new Dictionary<DateTime, string>();
            string aLine;
            StringReader strReader = new StringReader(input.Holidays);
            while (true)
            {
                aLine = strReader.ReadLine();
                if (aLine != null)
                {
                    char[] delim = { ':' };
                    string[] breakdown = aLine.Split(delim);
                    dict.Add(DateTime.Parse(breakdown[0]), breakdown[1].Trim());
                }
                else
                    break;
            }


            return dict;
        }
        private BotInfo getInfo()
        {
            var client = new RestClient(MPTKDDE_URI);
            var request = new RestRequest(MPTKDDE_PARSE, Method.GET);
            request.AddUrlSegment("id", "1");
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            return JsonConvert.DeserializeObject<BotInfo>(content);
        }

        private string isInHoliday(BotInfo info, DateTime day)
        {
            string output;
            Dictionary<DateTime, string> dict = getHolidayDictionary(info);
            foreach(DateTime key in dict.Keys)
            {
                if (key.Date == day.Date)
                {
                    dict.TryGetValue(key, out output);
                    return output;
                }
            }
            return null;
        }

        private string getClassTimes(DateTime dayOfWeek)
        {
            BotInfo info = getInfo();
            string retText = isInHoliday(info, dayOfWeek);
            if(retText != null)
                return String.Format("There won't be any class on {0: MMMM d} because ", dayOfWeek) + retText;
            switch(dayOfWeek.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "There is no class on Sunday";
                case DayOfWeek.Monday:
                    if (info.Monday == null)
                        return "There is no class on Monday";
                    return String.Format("The class schedule for {0:MMMM d} is:\n", dayOfWeek) + info.Monday;
                case DayOfWeek.Tuesday:
                    if (info.Tuesday == null)
                        return "There is no class on Tuesday";
                    return String.Format("The class schedule for {0:MMMM d} is:\n", dayOfWeek) + info.Tuesday;
                case DayOfWeek.Wednesday:
                    if (info.Wedensday == null)
                        return "There is no class on Wednesday";
                    return String.Format("The class schedule for {0:MMMM d} is:\n", dayOfWeek) + info.Wedensday;
                case DayOfWeek.Thursday:
                    if (info.Thursday == null)
                        return "There is no class on Thursday";
                    return String.Format("The class schedule for {0:MMMM d} is:\n", dayOfWeek) + info.Thursday;
                case DayOfWeek.Friday:
                    if (info.Friday == null)
                        return "There is no class on Friday";
                    return String.Format("The class schedule for {0:MMMM d} is:\n", dayOfWeek) + info.Friday;
                case DayOfWeek.Saturday:
                    if (info.Saturday == null)
                        return "There is no class on Saturday";
                    return String.Format("The class schedule for {0:MMMM d} is:\n", dayOfWeek) + info.Saturday;
                default:
                    return "I'm not real sure what you're asking for.";
            }
        }

        private string getPromotionInformation()
        {
            return getInfo().Promotion;
        }

        private string getTestingInformation()
        {
            return getInfo().Testing;
        }

        private DateTime getDateFromValue(dynamic val)
        {
            if (val == null)
                return DateTime.Now;
            else
            {
                if (((string)val[0].resolution.date).Contains("XXXX-WXX-"))
                {
                    string str = ((string)val[0].resolution.date);
                    int dow = Int32.Parse(str.Substring(str.Length - 1));
                    if (dow > (int)DateTime.Now.DayOfWeek)
                    {
                        int off = dow - (int)DateTime.Now.DayOfWeek;
                        return DateTime.Now.AddDays(off);
                    }
                    else
                    {
                        int off = 7 - (int)DateTime.Now.DayOfWeek;
                        return DateTime.Now.AddDays(off + dow);
                    }
                }
                else
                    return DateTime.Parse((string)val[0].resolution.date);
            }
        }

        private string getHelp()
        {
            return "Hi! I'm the MP Ninja Bot!  I can answer basic questions about class times, testings and promotions!";
        }

        private string parseIntent(string user_request)
        {
            var client = new RestClient(LUIS_URI);
            var request = new RestRequest(LUIS_PARSE, Method.GET);
            request.AddUrlSegment("query", user_request);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            dynamic json = JObject.Parse(content);
            switch ((string)json["intents"][0].intent)
            {
                case "class":
                    dynamic val = json["intents"][0]["actions"][0]["parameters"][0].value;
                    return getClassTimes(getDateFromValue(val));
                case "promotion ceremony":
                    return getPromotionInformation();
                case "testing time":
                    return getTestingInformation();
                case "help":
                    return getHelp();
                case "thanks":
                    return "You're welcome!";
                default:
                    return "I have no idea.";
            }
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}