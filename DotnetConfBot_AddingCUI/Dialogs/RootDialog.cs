using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace DotnetConfBot_AddingCUI.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
           
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {

            PromptDialog.Choice(
                   context: context,
                   resume: GetContentAsync,
                   options: Options,
                   prompt: "I can help you with Following",
                   retry: "Currently I dont Understand that. Please input what i understand");
        }
        public List<string> Options = new List<string>() {
           
            "View Sessions",
            "Verify Payment",
            "I wanna Chat",
            "List Attendees"

            
        };
        public async Task GetContentAsync(IDialogContext context, IAwaitable<string> result)
        {
            string selectedOption = await result;
            switch (selectedOption.ToString())
            {
                
                case "View Sessions":
                    var reply = context.MakeMessage();
                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    reply.Attachments =await GetSessionDetail();
                    await context.PostAsync(reply);
                    await MessageReceivedAsync(context,result);
                    break;
                case "Verify Payment":
                    await context.PostAsync("File upload");
                  context.Wait(RecieveFileAsync);
                  
                    break;
                case "I wanna Chat":
                    await context.PostAsync("So how may i help you?");
                    context.Wait(Chat);
                    break;
                case "List Attendees":
                    var attendeereply = context.MakeMessage();
                    attendeereply.AttachmentLayout = AttachmentLayoutTypes.List;
                    attendeereply.Attachments = await GetAttendes();
                    await context.PostAsync(attendeereply);
                    await MessageReceivedAsync(context, result);
                    break;
                default:
                    break;
            }
           
        }

       

        private async Task<List<Attachment>> GetSessionDetail()
        {
            List<Attachment> attachmentList = new List<Attachment>();
            var alokIntroCard = new HeroCard()
            {
                Title = "C# Internals",
                Subtitle ="Alok Kumar Pandey",
                Text = "CTO @ Braindigit IT Colutions",
                Images = new List<CardImage> { new CardImage("http://dotnetconf.aspnetcommunity.org/Media/aloksir_2017_10_05_01_37_15.png") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View Bio", value: "http://dotnetconf.aspnetcommunity.org/blog/Post/14/Net-Conf-2017/") }
            }; ;
            
            var devIntroCard = new HeroCard()
            {
                Title = "Intelligent Bots",
                Subtitle = "Dev Raj Gautam",
                Text = "Project Manager @ Braindigit IT Colutions",
                Images = new List<CardImage> { new CardImage("http://dotnetconf.aspnetcommunity.org/Media/devsir_2017_10_05_01_37_16.png") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View Bio", value: "http://dotnetconf.aspnetcommunity.org/blog/Post/14/Net-Conf-2017/") }
            }; ;
            var ranjanIntroCard = new HeroCard()
            {
                Title = "React With .NET",
                Subtitle = "Ranjan Shrestha",
                Text = "CTO @ Bhoos Entertinment",
                Images = new List<CardImage> { new CardImage("http://dotnetconf.aspnetcommunity.org/Media/ranjan_2017_10_05_01_37_18.png") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View Bio", value: "http://dotnetconf.aspnetcommunity.org/blog/Post/14/Net-Conf-2017/") }
            }; ;
            attachmentList.Add(alokIntroCard.ToAttachment());
            attachmentList.Add(devIntroCard.ToAttachment());

            attachmentList.Add(ranjanIntroCard.ToAttachment());
           return  attachmentList;
        }


        private async Task RecieveFileAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
           
            var message = await argument;
            if (message.Attachments != null && message.Attachments.Any())
            {
                var attachment = message.Attachments.First();
                using (HttpClient httpClient = new HttpClient())
                {
                    // Skype & MS Teams attachment URLs are secured by a JwtToken, so we need to pass the token from our bot.
                    if ((message.ChannelId.Equals("skype", StringComparison.InvariantCultureIgnoreCase) || message.ChannelId.Equals("msteams", StringComparison.InvariantCultureIgnoreCase))
                        && new Uri(attachment.ContentUrl).Host.EndsWith("skype.com"))
                    {
                        var token = await new MicrosoftAppCredentials().GetTokenAsync();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    var responseMessage = await httpClient.GetAsync(attachment.ContentUrl);

                    var contentLenghtBytes = responseMessage.Content.Headers.ContentLength;
                    
                    await context.PostAsync($"Voucher Image of {attachment.ContentType} type and size of {contentLenghtBytes} bytes received. We will verify and Send an email for confirmation");
                    
                }
            }
            await MessageReceivedAsync(context, argument);

        }

        private async Task Chat(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            LuisResult Data = new LuisResult();
            using (HttpClient client = new HttpClient())
            {
                string query = message.Text;
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/a652e4d3-5cb5-451b-aeb1-a97acde26463?subscription-key=f1ffd7409ec54fec83494fa66187fd28&timezoneOffset=0&verbose=true&q=" + query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<LuisResult>(JsonDataResponse);
                }
                string intent = Data.topScoringIntent.intent;
                switch (intent)
                {
                    case "FindSpeakerNAme":
                        string speaker = GetSpeakerName(Data.entities[0].entity.ToLower());
                        await context.PostAsync(speaker);
                        break;
                    case "List Session":
                        var reply = context.MakeMessage();
                        reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        reply.Attachments = await GetSessionDetail();
                        await context.PostAsync(reply);
                        break;
                   

                    default:
                        break;
                }
                

            }
            await MessageReceivedAsync(context, result);
        }
        private string GetSpeakerName(string sessionname)
        {
            string speaker = string.Empty;
            switch (sessionname)
            {
                case "react":
                     speaker="Ranjan Shrestha";
                    break;
                case "c#":
                    speaker= "Alok Pandey";
                    break;
                case "bot":
                    speaker= "Dev Raj Gautam";
                    break;
                default:
                    break;
            };
           return speaker;
        }

        private async Task<List<Attachment>>  GetAttendes()
        {
            using (HttpClient client = new HttpClient())
            {
                AttendeeList Data = new AttendeeList();
                string RequestURI = "http://localhost:8356/odata/Attendes";
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<AttendeeList>(JsonDataResponse);


                }
                List<Attachment> list = new List<Attachment>();
                foreach (Attendee attendee in Data.value)
                {
                    var articleCard = new ThumbnailCard
                    {
                        Title = attendee.Name,
                        Subtitle = attendee.Id.ToString()
                       
                        

                    };
                    list.Add(articleCard.ToAttachment());
                }
                return list;
            }
       
            }

    }
}