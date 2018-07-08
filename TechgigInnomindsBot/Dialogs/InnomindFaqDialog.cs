using AdaptiveCards;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker.Resource;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechgigInnomindsBot.Dialogs
{
    [Serializable]
    [QnAMaker("92b91ea7d3e74103bb4de25e64c1f069", "d2526c2c-57b4-4ff0-a8bb-e20578e3282a", "I don't understand this right now! Try another query!", 0.50, 3)]
    //[QnAMaker("92b91ea7d3e74103bb4de25e64c1f069", "d41bc2ad-b8d1-4a3e-a63b-54ee20b0fb16", "I don't understand this right now! Try another query!", 0.50, 3)]

    public class InnomindFaqDialog : QnAMakerDialog
    {
        protected override async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            // responding with the top answer when score is above some threshold
            //if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score > 0.75)
            //{
            //    await context.PostAsync(qnaMakerResults.Answers.FirstOrDefault().Answer);
            //}
           // else
            //{
                var qnaList = qnaMakerResults.Answers;
                var questions = qnaList.Select(x => x.Answer).Concat(new[] { Resource.noneOfTheAboveOption }).ToArray();
                var cards = GetCardsAttachments(qnaMakerResults);
               
                await RespondFromQnAMakerResultAsync(context,context.Activity.AsMessageActivity(), qnaMakerResults);
                await base.QnAFeedbackStepAsync(context, qnaMakerResults);
           // }
        }

        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetAdaptiveCard(result);
            if (result.Answers.Count > 1 && result.Answers.First().Score<=0.95)
                await context.PostAsync($"Top {result.Answers.Count()} suggestions are given, please give feedback");

            await context.PostAsync(reply);

        }

        
        private static IList<Attachment> GetCardsAttachments(QnAMakerResults results)
        {
            var attachemnts = new List<Attachment>();
            foreach(var answer in results.Answers)
            {
                attachemnts.Add(GetHeroCard(
                    "Suggestions",
                     string.Join(",", answer.Questions),
                      answer.Answer,
                    new CardImage(url: "https://docs.microsoft.com/en-us/aspnet/aspnet/overview/developing-apps-with-windows-azure/building-real-world-cloud-apps-with-windows-azure/data-storage-options/_static/image5.png"),
                    new CardAction(ActionTypes.PostBack, "Feedback", value: answer)));
            }

            return attachemnts;

            //return new List<Attachment>()
            //{
            //    GetHeroCard(
            //        "Azure Storage",
            //        "Offload the heavy lifting of data center management",
            //        "Store and help protect your data. Get durable, highly available data storage across the globe and pay only for what you use.",
            //        new CardImage(url: "https://docs.microsoft.com/en-us/aspnet/aspnet/overview/developing-apps-with-windows-azure/building-real-world-cloud-apps-with-windows-azure/data-storage-options/_static/image5.png"),
            //        new CardAction(ActionTypes.OpenUrl, "Learn more", value: "https://azure.microsoft.com/en-us/services/storage/")),
            //    GetThumbnailCard(
            //        "DocumentDB",
            //        "Blazing fast, planet-scale NoSQL",
            //        "NoSQL service for highly available, globally distributed apps—take full advantage of SQL and JavaScript over document and key-value data without the hassles of on-premises or virtual machine-based cloud database options.",
            //        new CardImage(url: "https://docs.microsoft.com/en-us/azure/documentdb/media/documentdb-introduction/json-database-resources1.png"),
            //        new CardAction(ActionTypes.OpenUrl, "Learn more", value: "https://azure.microsoft.com/en-us/services/documentdb/")),
            //    GetHeroCard(
            //        "Azure Functions",
            //        "Process events with a serverless code architecture",
            //        "An event-based serverless compute experience to accelerate your development. It can scale based on demand and you pay only for the resources you consume.",
            //        new CardImage(url: "https://msdnshared.blob.core.windows.net/media/2016/09/fsharp-functions2.png"),
            //        new CardAction(ActionTypes.OpenUrl, "Learn more", value: "https://azure.microsoft.com/en-us/services/functions/")),
            //    GetThumbnailCard(
            //        "Cognitive Services",
            //        "Build powerful intelligence into your applications to enable natural and contextual interactions",
            //        "Enable natural and contextual interaction with tools that augment users' experiences using the power of machine-based intelligence. Tap into an ever-growing collection of powerful artificial intelligence algorithms for vision, speech, language, and knowledge.",
            //        new CardImage(url: "https://msdnshared.blob.core.windows.net/media/2017/03/Azure-Cognitive-Services-e1489079006258.png"),
            //        new CardAction(ActionTypes.OpenUrl, "Learn more", value: "https://azure.microsoft.com/en-us/services/cognitive-services/")),
            //};
        }

        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }

        private static Attachment GetThumbnailCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }

        private  List<Attachment> GetAdaptiveCard(QnAMakerResults result)
        {

            var attachments = new List<Attachment>();
            foreach (var res in result.Answers)
            {
                var card = AdaptiveCardHelper.GetAdaptiveCard(res);
                attachments.Add(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card,
                    Name= "Suggestion"
                    
                });

                if (res.Score >=0.95)
                    break;
            }

            return attachments;

        }

      

    }



}