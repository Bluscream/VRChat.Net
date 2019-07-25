﻿using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VRChatApi.Classes;
using VRChatApi.Logging;
namespace VRChatApi.Endpoints
{
    public class UserApi
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        public string Username { get; set; }
        public string Password { get; set; }
        public UserApi(string username, string password)
        {
            Logger.Trace(() => $"Entering {nameof(UserApi)} constructor with username: {username}");
            Username = username;
            Password = password;
        }
        public async Task<UserResponse> Login()
        {
            Logger.Trace(() => "Getting current user details");
            HttpResponseMessage response = await Global.HttpClient.GetAsync($"auth/user?apiKey={Global.ApiKey}");
            UserResponse res = null;
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                Logger.Debug(() => $"JSON received: {json}");
                res = JsonConvert.DeserializeObject<UserResponse>(json);
            }
            UserResponse result = new UserResponse();
            result.Raw = response;
            if (res != null) result = res;
            return result;
        }
        public async Task<UserResponse> Register(string username, string password, string email, string birthday = null, string acceptedTOSVersion = null)
        {
            Logger.Debug(() => $"Registering new user with {nameof(username)} = {username}, {nameof(email)} = {email}, {nameof(birthday)} = {birthday}, {nameof(acceptedTOSVersion)} = {acceptedTOSVersion}");
            JObject json = new JObject();
            json["username"] = username;
            json["password"] = password;
            if (email != null)
                json["email"] = email;
            if (birthday != null )
                json["birthday"] = birthday;
            if (acceptedTOSVersion != null)
                json["acceptedTOSVersion"] = acceptedTOSVersion;
            Logger.Debug(() => $"Prepared JSON to post: {json}");
            StringContent content = new StringContent(json.ToString(), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await Global.HttpClient.PostAsync($"auth/register?apiKey={Global.ApiKey}", content);
            UserResponse res = null;
            if (response.IsSuccessStatusCode)
            {
                var receivedJson = await response.Content.ReadAsStringAsync();
                Logger.Debug(() => $"JSON received: {receivedJson}");
                res = JsonConvert.DeserializeObject<UserResponse>(receivedJson);
            }
            return res;
        }
        public async Task<List<UserBriefResponse>> Search(string pattern, int limit = 100, int offset = 0)
        {
            HttpResponseMessage response = await Global.HttpClient.GetAsync($"users/?apiKey={Global.ApiKey}&n={limit}&offset={offset}&search={pattern}");
            List<UserBriefResponse> res = null;
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                Logger.Debug(() => $"JSON received: {json}");
                res = JsonConvert.DeserializeObject<List<UserBriefResponse>>(json);
            }
            return res;
        }
        public async Task<UserBriefResponse> GetById(string userId)
        {
            Logger.Debug(() => $"Getting user info with ID: {userId}");
            HttpResponseMessage response = await Global.HttpClient.GetAsync($"users/{userId}?apiKey={Global.ApiKey}");
            UserBriefResponse res = null;
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                Logger.Debug(() => $"JSON received: {json}");
                res = JsonConvert.DeserializeObject<UserBriefResponse>(json);
            }
            return res;
        }
        public async Task<UserResponse> UpdateInfo(string userId, string email = null, string birthday = null, string acceptedTOSVersion = null, List<string> tags = null, string status = null, string statusDescription = null)
        {
            Logger.Debug(() => $"Updating user info for {nameof(userId)} = {userId} with {nameof(email)} = {email}, {nameof(birthday)} = {birthday}, {nameof(acceptedTOSVersion)} = {acceptedTOSVersion}, {nameof(tags)} = {tags}, {nameof(status)} = {status}, {nameof(statusDescription)} = {statusDescription}");
            JObject json = new JObject();
            if (email != null)
                json["email"] = email;
            if (birthday != null)
                json["birthday"] = birthday;
            if (acceptedTOSVersion != null)
                json["acceptedTOSVersion"] = acceptedTOSVersion;
            if (tags != null)
                json["tags"] = JToken.FromObject(tags);
            if (status != null)
                json["status"] = status;
            if (statusDescription != null)
                json["statusDescription"] = statusDescription;
            Logger.Debug(() => $"Prepared JSON to put: {json}");
            StringContent content = new StringContent(json.ToString(), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await Global.HttpClient.PutAsync($"users/{userId}?apiKey={Global.ApiKey}", content);
            UserResponse res = new UserResponse();
            res.Raw = response;
            // res = await VRChatApi.ParseResponseAsync(response);
            if (response.IsSuccessStatusCode)
            {
                var receivedJson = await response.Content.ReadAsStringAsync();
                Logger.Debug(() => $"JSON received: {receivedJson}");
                res = JsonConvert.DeserializeObject<UserResponse>(receivedJson);
            }
            return res;
        }
    }
}