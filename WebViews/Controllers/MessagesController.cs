using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;

namespace WebViews
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        ///     POST: api/Messages
        ///     Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            
            if (activity.ChannelData != null)
            {
                var jobj = JObject.FromObject(activity.ChannelData);
                var fbAccountLinking = jobj.SelectToken("$..account_linking");

                if (fbAccountLinking != null && fbAccountLinking.HasValues)
                {
                    var linkedUnlinked = fbAccountLinking.Value<string>("status");

                    var reply2 = activity.CreateReply();

                    if (linkedUnlinked == "unlinked")
                    {
                        reply2.Text = "user just unlinked!";
                    }
                    else
                    {
                        reply2.Text = "user just linked!";
                    }

                    await connector.Conversations.ReplyToActivityAsync(reply2);
                }
                else
                {
                    var reply = activity.CreateReply();
                    var attachment = CreateLoginLogoutPayload();

                    reply.ChannelData = JObject.FromObject(new { attachment });

                    await connector.Conversations.ReplyToActivityAsync(reply);
                }


                // when we link or unlink our facebook account,
                // we get a callback ala https://developers.facebook.com/docs/messenger-platform/webhook-reference/account-linking
                // here, i am writing it out, but we can make some intelligent decisions based on it.
            }


            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private static object CreateLoginLogoutPayload()
        {
            var attachment = new
            {
                type = "template",
                payload = new
                {
                    template_type = "button",
                    text = "log in or out",
                    buttons = new[]
                    {
                        new
                        {
                            type = "account_link",
                            url = "https://pizzadeliverybot.azurewebsites.net/fakelogin"
                        },
                        new
                        {
                            type = "account_unlink",
                            url = ""
                        }
                    }
                }
            };
            return attachment;
        }
    }
}