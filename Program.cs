using FluentAssertions;
using ProgrammingChallenge.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace programming_challenge
{
    public class Program
    {
        private const string baseUrl = "http://magazinestore.azurewebsites.net/";
        private static HttpClient httpClient;
        private static string token;

        private static void InitHttpClient()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static async Task<string> GetToken()
        {
            var response = await httpClient.GetAsync($"{baseUrl}/api/token");
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
                if (tokenResponse.Success) return tokenResponse.Token;
                throw new Exception($"Unsuccessful at fetching token. Message: {tokenResponse.Message}");
            }

            throw new Exception($"Api call to get token failed. Response Code: {response.StatusCode}");
        }

        private static async Task<IEnumerable<Subscriber>> GetSubscribers()
        {
            var response = await httpClient.GetAsync($"{baseUrl}/api/subscribers/{token}");
            if (response.IsSuccessStatusCode)
            {
                var subscriberResponse = await response.Content.ReadAsAsync<SubscriberResponse>();
                if (subscriberResponse.Success) return subscriberResponse.Subscribers;
                throw new Exception($"Unsuccessful at fetching subscribers. Message: {subscriberResponse.Message}");
            }

            throw new Exception($"Api call to get subscribers failed. Response Code: {response.StatusCode}");
        }

        private static async Task<IEnumerable<string>> GetCategories()
        {
            var response = await httpClient.GetAsync($"{baseUrl}/api/categories/{token}");
            if (response.IsSuccessStatusCode)
            {
                var categoryResponse = await response.Content.ReadAsAsync<CategoryResponse>();
                if (categoryResponse.Success) return categoryResponse.Categories;
                throw new Exception($"Unsuccessful at fetching categories. Message: {categoryResponse.Message}");
            }

            throw new Exception($"Api call to get categories failed. Response Code: {response.StatusCode}");
        }

        private static async Task<IEnumerable<Magazine>> GetMagazinesByCategory(string category)
        {
            var response = await httpClient.GetAsync($"{baseUrl}/api/magazines/{token}/{category}");
            if (response.IsSuccessStatusCode)
            {
                var magazineResponse = await response.Content.ReadAsAsync<MagazineResponse>();
                if (magazineResponse.Success) return magazineResponse.Magazines;
                throw new Exception($"Unsuccessful at fetching magazines for category {category}. Message: {magazineResponse.Message}");
            }

            throw new Exception($"Api call to get magazines for category {category} failed. Response Code: {response.StatusCode}");
        }

        private static async Task<AnswerCheckResponseDetail> SubmitAnswer(AnswerCheckRequest answerCheckRequest)
        {
            var response = await httpClient.PostAsJsonAsync<AnswerCheckRequest>($"{baseUrl}/api/answer/{token}",
                answerCheckRequest);
            if (response.IsSuccessStatusCode)
            {
                var answerCheckResponse = await response.Content.ReadAsAsync<AnswerCheckResponse>();
                if (answerCheckResponse.Success) return answerCheckResponse.Detail;
                throw new Exception($"Unsuccessful at fetching answer check response. Message: {answerCheckResponse.Message}");
            }

            throw new Exception($"Api call to get answer check response failed. Response Code: {response.StatusCode}");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to a new programming challenge!");

            InitHttpClient();
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            token = await GetToken();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var taskGetSubscribers = GetSubscribers();
            var taskGetCategories = GetCategories();
            await Task.WhenAll(taskGetSubscribers, taskGetCategories);

            var subscribers = taskGetSubscribers.Result;
            var categories = taskGetCategories.Result;
            var dictionaryMagazineIdsByCategory = categories.Distinct()
                .AsParallel()
                .Select(async q => new KeyValuePair<string, IEnumerable<Magazine>>(q, await GetMagazinesByCategory(q)))
                .Select(q => q.Result)
                .ToDictionary(q => q.Key, q => q.Value.Select(q1 => q1.Id));
            var filteredSubscribers = subscribers.Where(subscriber =>
            dictionaryMagazineIdsByCategory.Keys.All(category =>
            subscriber.MagazineIds.Any(magazineId => dictionaryMagazineIdsByCategory[category].Contains(magazineId))));

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            Console.WriteLine($"Total Categories: {categories.Count()}");
            Console.WriteLine($"Total Subscribers: {subscribers.Count()}");
            Console.WriteLine($"Filtered Subscribers: {filteredSubscribers.Count()}");

            Console.WriteLine("Submitting answer...");
            var filteredSubscriberIds = filteredSubscribers.Select(q => q.Id);
            var answerCheckResponseDetail = await SubmitAnswer(new AnswerCheckRequest { Subscribers = filteredSubscriberIds });
            Console.WriteLine("Answer Check Response is as follows: ");
            Console.WriteLine($"AnswerCorrect: {answerCheckResponseDetail.AnswerCorrect}");
            Console.WriteLine($"TotalTime: {answerCheckResponseDetail.TotalTime}");

            Console.ReadLine();
        }
    }
}
