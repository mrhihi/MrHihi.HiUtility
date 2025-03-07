using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MrHihi.HiUtility.Net;

public class HttpUtil
{
    private readonly static ConcurrentDictionary<Uri, HttpClient> httpClients = new ConcurrentDictionary<Uri, HttpClient>();
    protected static HttpClient getClient(Uri baseAddress, HttpClientHandler? handler = null)
    {
        HttpClient result;
        if (httpClients.ContainsKey(baseAddress))
        {
            result = httpClients[baseAddress];
        }
        else
        {
            var client = (handler==null) ? new HttpClient() : new HttpClient(handler);
            client.BaseAddress = baseAddress;
            httpClients.TryAdd(baseAddress, client);
            result = client;
        }
        return result;
    }

    public static HttpUtil Http(Uri baseAddress, HttpClientHandler? handler = null)
    {
        var hc = new HttpUtil(HttpUtil.getClient(baseAddress, handler));
        hc._baseUri = baseAddress;
        return hc;
    }

    public HttpUtil SetBasicAuth(string userName, string password)
    {
        string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
        return this;
    }

    public HttpUtil ResetHeaders(IDictionary<string, string> headers)
    {
        _client.DefaultRequestHeaders.Clear();
        if (headers != null && headers.Count > 0)
        {
            foreach (var header in headers)
            {
                if (_client.DefaultRequestHeaders.Contains(header.Key))
                {
                    _client.DefaultRequestHeaders.Remove(header.Key);
                }
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
        return this;
    }

    protected Uri? _baseUri { get; set; }
    protected readonly HttpClient _client;
    protected Dictionary<string, string> _headers { get; set; } = new Dictionary<string, string>();
    protected HttpUtil(HttpClient client, IDictionary<string, string>? headers = null)
    {
        if (headers != null)
        {
            _headers.Clear();
            foreach (string key in headers.Keys)
            {
                _headers.Add(key, headers[key]);
            }
        }
        _client = client;
    }

    public HttpClient Client { get { return _client; } }

    public StringContent ToJsonContent(string content)
    {
        return new StringContent(content, Encoding.UTF8, "application/json");
    }

    public T? ToObject<T>(string json)
    {
        T? result = JsonSerializer.Deserialize<T>(json)??default(T);
        return result;
    }

    public HttpResponseMessage Post(string method, FormUrlEncodedContent formContent)
    {
        HttpResponseMessage response = _client.PostAsync(method, formContent).GetAwaiter().GetResult();
        return response;
    }

    public HttpResponseMessage Get(string method, IDictionary<string, string>? queryParams)
    {
        return Get(method, queryParams?.ToArray());
    }
    public HttpResponseMessage Get(string method, params KeyValuePair<string, string>[]? queryParams)
    {
        string querystr = "";
        if (queryParams != null && queryParams.Length > 0)
        {
            querystr = "?" + string.Join("&", (from a in queryParams select $"{WebUtility.UrlEncode(a.Key)}={WebUtility.UrlEncode(a.Value)}").ToArray());
        }
        string q = $"{method}{querystr}";

        HttpResponseMessage response = _client.GetAsync(q).GetAwaiter().GetResult();
        return response;
    }
    public string GetRaw(string method, IDictionary<string, string>? queryParams)
    {
        return GetRaw(method, queryParams?.ToArray());
    }
    public string GetRaw(string method, params KeyValuePair<string, string>[]? queryParams)
    {
        var response = Get(method, queryParams);
        string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        return result;
    }
}