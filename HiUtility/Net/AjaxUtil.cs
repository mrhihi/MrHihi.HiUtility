using System.Net.Http.Headers;
using System.Text.Json;

namespace MrHihi.HiUtility.Net;

public class AjaxUtil: HttpUtil
{
    public static AjaxUtil Ajax(Uri baseAddress, HttpClientHandler? handler = null)
    {
        var hc = new AjaxUtil(getClient(baseAddress, handler));
        hc._baseUri = baseAddress;
        hc._client.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header
        return hc;
    }

    public AjaxUtil(HttpClient client, IDictionary<string, string>? headers = null): base(client, headers)
    {
    }

    public T Post<T>(string method, string content)
    {
        var data = ToJsonContent(content);
        HttpResponseMessage response = _client.PostAsync(method, data).GetAwaiter().GetResult();

        string jsonStrResult = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        T? result = JsonSerializer.Deserialize<T>(jsonStrResult);
        if (result == null)
        {
            throw new InvalidOperationException("Deserialization returned null");
        }
        return result;
    }

    public T Get<T>(string method, IDictionary<string, string>? queryParams = null)
    {
        string jsonStrResult = GetRaw(method, queryParams);
        var result = ToObject<T>(jsonStrResult);
        if (result == null)
        {
            throw new InvalidOperationException("Deserialization returned null");
        }
        return result;
    }

}
