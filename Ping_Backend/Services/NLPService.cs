﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;

namespace Ping_Backend.Services
{
    public static class NLPService
    {
        static ITextAnalyticsClient _client;
        static NLPService()
        {
            _client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                Endpoint = "https://westus.api.cognitive.microsoft.com/"
            };

        }


        public static async Task<string[]> GetCategoriesFromTextAsync(string text)
        {
            var entitiesResult = await _client.EntitiesAsync(
                    false,
                    new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                        new MultiLanguageInput("en", "1", text),
                        }));
            var result = new List<string>();

            foreach(var e in entitiesResult.Documents.First().Entities)
            {
                if (e.Name.ToCharArray().Count() <= 30 && e.Name.ToCharArray().Count() >= 3)
                {
                    result.Add(e.Name);
                }
            }
            if (result.Count() != 0)
            {
                result.RemoveAt(result.Count() - 1);
            }
            if(result.Count() >= 6)
            {
                var count = result.Count() - 5;
                result.RemoveRange(5, count);
            }

            return result.ToArray();
        }
    }

    class ApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Ocp-Apim-Subscription-Key", "367330fcbf8d4619b4f7d76627c958fd");
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}
