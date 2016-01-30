using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
            return Post(new Uri(ApiUrl + "beers/act"), json);
        }

        public async Task<NoteResponse> GetNotes(Guid guid)
        {
            var resp = await DownloadString(new Uri(ApiUrl + "drinkers/" + guid));
            if (resp.IsSuccessfull)
            {
                try
                {
                    return JsonConvert.DeserializeObject<NoteResponse>(resp.Response);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.ApiError, this, "GetDrinker failed with response: " + resp.Response, ex);
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
                LogHelper.Instance.Log(LogLevel.Error, this, "DownloadStringAsync failed for url " + url, ex);
                return new StringReponse()
                {
                    ErrorMessage = "Request failed for url " + url
                };
            }
        }

        private async Task<BooleanResponse> Post(Uri url, string content)
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
                            LogHelper.Instance.Log(LogLevel.ApiError, this,
                                "Post failed for url " + url + " with json " + content + " Reponse recieved: " + respo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Post failed for url " + url, ex);
                resp = new BooleanResponse() { ErrorMessage = "Post failed for url " + url };
            }
            return resp;
        }
    }
}
