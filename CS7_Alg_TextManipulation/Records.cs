using System.Collections.Immutable;

namespace CS7_Alg_TextManipulation;

/// <summary>
/// This file contains all the data types that are used
/// when sending and receiving messages.
/// </summary>
public partial class MessagingApi
{
    #region internal records
    private class ApiException : Exception
    {
        public ErrorRecord Error { get; protected set; }

        public ApiException(ErrorRecord error, 
            Exception? innerException = null) 
            : base($"{error}", innerException)
        {
            Error = error;
        }
    }
    
    /// <summary>
    /// This is what the API expects when you want to
    /// SEND a new message
    /// </summary>
    /// <param name="messageContent">raw data of a message</param>
    /// <param name="contentType">the encoding format of the message</param>
    /// <param name="recipients">who are you sending the message to?</param>
    private readonly record struct CreateMessageRecord(
        byte[] messageContent, string contentType, 
        ImmutableArray<string> recipients);

    private readonly record struct SentMessageRecord(string id, string sender, byte[] messageContent, string contentType, DateTime sent,
        ImmutableArray<string> recipients);

    private readonly record struct SentMessageStub(string id, string sender, string contentType, int contentLength, DateTime sent,
        ImmutableArray<string> recipients)
    {
        public static implicit operator SentMessageStub(SentMessageRecord message) =>
            new(message.id, message.sender, message.contentType, message.messageContent.Length, message.sent, message.recipients);
    }

    /// <summary>
    /// The Full Content of the message that you are have received
    /// </summary>
    /// <param name="id">unique identifier for this message, used for retrieving it</param>
    /// <param name="sender">who sent the message</param>
    /// <param name="messageContent">raw binary data of this message</param>
    /// <param name="contentType">encoding format of the data</param>
    /// <param name="sent">when was this message sent</param>
    /// <param name="recipient">who is reading this message</param>
    /// <param name="read">when was this first read</param>
    /// <param name="hidden"></param>
    public readonly record struct RecievedMessageRecord(string id, string sender, byte[] messageContent, string contentType, DateTime sent, 
        string recipient, DateTime read, bool hidden);

    /// <summary>
    /// Short Record that tells you that you /have/ a message
    /// and describes the contents.
    /// </summary>
    /// <param name="id">
    /// id used to retrieve the message in
    /// its entirety.
    /// </param>
    /// <param name="sender">Who sent this message?</param>
    /// <param name="contentType">What kind of data is here?</param>
    /// <param name="contentLength">number of Bytes in the content</param>
    /// <param name="sent">when was this sent?</param>
    /// <param name="unread">Have you already read this message?</param>
    /// <param name="hidden">Have you deleted this message?</param>
    public readonly record struct RecievedMessageStub(string id, string sender, 
        string contentType, int contentLength, DateTime sent, bool unread, bool hidden)
    {
        public static implicit operator RecievedMessageStub(RecievedMessageRecord message) =>
            new(message.id, message.sender, message.contentType, message.messageContent.Length,
                message.sent, message.read == default, message.hidden);

        public override string ToString() => $"[{id}] From:  {sender}  Content-Type:  {contentType}";
    }

    
    private readonly record struct AuthResponse(
        string userId, string jwt, DateTime expires, string refreshToken)
    {
        public bool IsExpired => expires <= DateTime.Now;
    }

    public readonly record struct ErrorRecord(string type, string error)
    {
        public override string ToString() => $"{type} : {error}";
    }
    
    private readonly record struct LoginRecord(string email, string password);

    #endregion

    
}

public static class Extensions
{
    public static string ToBinaryString(this byte b) => Convert.ToString(b, 2).PadLeft(8, '0');
}