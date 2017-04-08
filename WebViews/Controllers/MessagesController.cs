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

            var reply = activity.CreateReply();

            reply.Text = JObject.FromObject(activity.ChannelData).ToString();
            // when we link or unlink our facebook account,
            // we get a callback ala https://developers.facebook.com/docs/messenger-platform/webhook-reference/account-linking
            // here, i am writing it out, but we can make some intelligent decisions based on it.

            var attachment = new
            {
                type = "template",
                payload = new
                {
                    template_type = "button",
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

            reply.ChannelData = JObject.FromObject(new {attachment});

            await connector.Conversations.ReplyToActivityAsync(reply);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}