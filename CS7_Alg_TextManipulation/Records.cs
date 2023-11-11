using System.Collections.Immutable;

namespace CS7_Alg_TextManipulation;

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

    public readonly record struct RecievedMessageRecord(string id, string sender, byte[] messageContent, string contentType, DateTime sent, 
        string recipient, DateTime read, bool hidden);

    public readonly record struct RecievedMessageStub(string id, string sender, 
        string contentType, int contentLength, DateTime sent, bool unread, bool hidden)
    {
        public static implicit operator RecievedMessageStub(RecievedMessageRecord message) =>
            new(message.id, message.sender, message.contentType, message.messageContent.Length,
                message.sent, message.read == default, message.hidden);
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