using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CS7_Alg_TextManipulation;

public partial class MessagingApi
{
    #region Sending Messages
    
    public void SendMessage(byte[] data, string contentType, 
        params string[] recipients)
    {
        CreateMessageRecord newMessage = 
            new(data, contentType,
                recipients.ToImmutableArray());
        Send(HttpMethod.Post, "api/messages/send", newMessage,
            onErrorAction: (exception, code) => 
                DefaultErrorAction(exception, code));
    }

    public void SendMessage<T>(T serializableObject, params string[] recipients)
    {
        string json;
        try
        {
             json = JsonSerializer.Serialize(serializableObject);
        }
        catch (Exception e)
        {
            PrintError("Failed to serialize object...");
            PrintError(e.Message);
            return;
        }

        var data = Encoding.UTF8.GetBytes(json);
    }
    
    public void SendMessage(string message, params string[] recipients)
    {
        var data = Encoding.UTF8.GetBytes(message);
        SendMessage(data, "text/utf8", recipients);
    }
    #endregion
    
    #region Getting Recieved Messages

    public ImmutableArray<RecievedMessageStub> GetMyRecievedMessages(
        DateTime start = default, DateTime end = default,
        bool unreadOnly = true, bool hidden = false)
    {
        var queryParams = new List<string>();
        if(start != default)
            queryParams.Add($"start={start:yyyy-MM-dd}");
        if(end != default)
            queryParams.Add($"end={end:yyyy-MM-d}");
        if(!unreadOnly)
            queryParams.Add("unreadOnly=false");
        if(hidden)
            queryParams.Add("hidden=true");

        var queryString = queryParams.Any() ? 
            $"?{queryParams
                .Aggregate((a, b) => $"{a}&{b}")}" : "";
        
        var result = ApiCall<object, ImmutableArray<RecievedMessageStub>>(
            HttpMethod.Get, $"api/messages/inbox{queryString}",
            onErrorAction: (e, s) => DefaultErrorAction(e, s));

        return result;
    }

    public RecievedMessageRecord GetMessageContent(string messageId) =>
        ApiCall<object, RecievedMessageRecord>(HttpMethod.Get,
            $"api/messages/inbox/{messageId}",
            onErrorAction: (e, s) => DefaultErrorAction(e, s));
    
    public void DeleteMessage(string messageId) =>
        Send<object>(HttpMethod.Delete, $"api/messages/inbox/{messageId}",
            onErrorAction: (e, s) => DefaultErrorAction(e, s));


    #endregion

}