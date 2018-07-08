using AdaptiveCards;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.CognitiveServices.QnAMaker
{
    public static class AdaptiveCardHelper
    {
        public static AdaptiveCard GetAdaptiveCard(QnAMakerResult result)
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                // Defining the Body contents of the card 
                Body = new List<AdaptiveElement>()
    {
        new AdaptiveContainer()
        {
            Items = new List<AdaptiveElement>()
            {
                new AdaptiveTextBlock()
                {
                    Text = result.Questions[0],
                    Weight = AdaptiveTextWeight.Bolder,
                    Size = AdaptiveTextSize.Medium,
                    Wrap= true
                },
                new AdaptiveTextBlock()
                {
                    Text = result.Answer,
                    Weight = AdaptiveTextWeight.Normal,
                    Size = AdaptiveTextSize.Normal,
                    Wrap=true
                }
            }
        }
    },
            
            };

            return card;

        }

        public static AdaptiveCard AddFeedback(AdaptiveCard adaptiveCard)
        {
            adaptiveCard.Actions =    // Defining the actions (buttons) our card will have, as well as their functions 
                  new List<AdaptiveAction>()
    {
        new AdaptiveSubmitAction()
        {
            Title = "Order",
            // returning our StringData as JSON to the bot 
            DataJson = ""
        }
    };
            return adaptiveCard;
        }
       


    }
}
