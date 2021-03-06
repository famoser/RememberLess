﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.RememberLess.Data.Entities.Communication;
using Newtonsoft.Json;

namespace Famoser.RememberLess.Data.Services
{
    public class DataService : IDataService
    {
        private const string ApiUrl = "https://api.rememberless.famoser.ch/";

        public Task<BooleanResponse> PostNote(NoteRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            return PostForBoolean(new Uri(ApiUrl + "notes/act"), json);
        }

        public Task<BooleanResponse> PostNoteCollection(NoteCollectionRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            return PostForBoolean(new Uri(ApiUrl + "notecollections/act"), json);
        }

        public async Task<NoteResponse> GetNotes(NoteRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var resp = await PostForString(new Uri(ApiUrl + "notes/act"), json);
            if (resp.IsSuccessfull)
            {
                try
                {
                    return JsonConvert.DeserializeObject<NoteResponse>(resp.Response);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.FatalError, "GetNotes failed with response: " + resp.Response, this, ex);
                    return new NoteResponse()
                    {
                        ErrorMessage = "Unserialisation failed for Content " + resp.Response
                    };
                }
            }
            return new NoteResponse()
            {
                ErrorMessage = resp.ErrorMessage
            };
        }

        public async Task<NoteCollectionResponse> GetNoteCollections(NoteCollectionRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var resp = await PostForString(new Uri(ApiUrl + "notecollections/act"), json);
            if (resp.IsSuccessfull)
            {
                try
                {
                    return JsonConvert.DeserializeObject<NoteCollectionResponse>(resp.Response);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.FatalError, "GetNotes failed with response: " + resp.Response, this, ex);
                    return new NoteCollectionResponse()
                    {
                        ErrorMessage = "Unserialisation failed for Content " + resp.Response
                    };
                }
            }
            return new NoteCollectionResponse()
            {
                ErrorMessage = resp.ErrorMessage
            };
        }

        private async Task<StringReponse> DownloadString(Uri url)
        {
            try
            {
                using (var client = new HttpClient(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip
                                                 | DecompressionMethods.Deflate
                    }))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                    var resp = await client.GetAsync(url);
                    var res = new StringReponse()
                    {
                        Response = await resp.Content.ReadAsStringAsync()
                    };
                    if (resp.IsSuccessStatusCode)
                        return res;
                    res.ErrorMessage = "Request not successfull: Status Code " + resp.StatusCode + " returned. Message: " + res.Response;
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "DownloadStringAsync failed for url " + url, this, ex);
                return new StringReponse()
                {
                    ErrorMessage = "Request failed for url " + url
                };
            }
        }

        private async Task<BooleanResponse> PostForBoolean(Uri url, string content)
        {
            BooleanResponse resp = null;
            try
            {
                using (var client = new HttpClient(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip
                                                 | DecompressionMethods.Deflate
                    }))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

                    var credentials = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("json", content)
                    });

                    var res = await client.PostAsync(url, credentials);
                    var respo = await res.Content.ReadAsStringAsync();
                    if (respo == "true")
                        resp = new BooleanResponse() { Response = true };
                    else
                    {
                        if (respo == "false")
                            resp = new BooleanResponse() { Response = false };
                        else
                        {
                            resp = new BooleanResponse() { ErrorMessage = respo };
                            LogHelper.Instance.Log(LogLevel.FatalError,
                                "Post failed for url " + url + " with json " + content + " Reponse recieved: " + respo, this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "Post failed for url " + url, this, ex);
                resp = new BooleanResponse() { ErrorMessage = "Post failed for url " + url };
            }
            return resp;
        }

        private async Task<StringReponse> PostForString(Uri url, string content)
        {
            try
            {
                using (var client = new HttpClient(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip
                                                 | DecompressionMethods.Deflate
                    }))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

                    var credentials = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("json", content)
                    });

                    var res = await client.PostAsync(url, credentials);
                    var resp = new StringReponse()
                    {
                        Response = await res.Content.ReadAsStringAsync()
                    };
                    if (res.IsSuccessStatusCode)
                        return resp;
                    resp.ErrorMessage = "Request not successfull: Status Code " + res.StatusCode + " returned. Message: " + resp.Response;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "DownloadStringAsync failed for url " + url, this, ex);
                return new StringReponse()
                {
                    ErrorMessage = "Request failed for url " + url
                };
            }
        }
    }
}
