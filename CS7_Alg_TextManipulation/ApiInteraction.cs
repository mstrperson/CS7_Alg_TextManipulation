using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CS7_Alg_TextManipulation;

/// <summary>
/// This file is a magic black box for you right now.
/// It will allow you to connect to the web service that we
/// will use to send and receive our messages!
///
/// We will go over how to use it in class, but gloss over the
/// details of how it works.  If you are interested in how it
/// works, stick around ;) cause we'll do this in Data Structures!
/// </summary>
public partial class MessagingApi
{
    #region Properties
    private readonly HttpClient client = new() {BaseAddress = new("https://forms-dev.winsor.edu")};
    private AuthResponse? _auth;

    
    private AuthenticationHeaderValue AuthHeader =>
        new AuthenticationHeaderValue("Bearer", _auth?.jwt ?? "");

    #endregion
    
    #region Public Interface
    /// <summary>
    /// Register your account.  You can only do this once!
    /// Make sure that you remember your password when you create it!
    /// </summary>
    /// <param name="email">your email address</param>
    /// <param name="password">A new Strong Password</param>
    /// <returns>true if you are successfully registered</returns>
    public bool RegisterAccount(string email, string password)
    {
        LoginRecord login = new(email, password);
        bool success = true;
        success = Send(HttpMethod.Post, 
            "api/auth/register", login, false,
            (e, s) => 
                success = DefaultErrorAction(e, s));
        
        return success;
    }

    /// <summary>
    /// If you forget your password... call this method!
    /// </summary>
    /// <param name="email">your email</param>
    /// <param name="password">a new password!a</param>
    public void ForgotPassword(string email, string password)
    {
        LoginRecord login = new(email, password);
        Send(HttpMethod.Post, "api/auth/forgot", 
            login, false,
            (e, s) => DefaultErrorAction(e, s));
        
    }
    
    /// <summary>
    /// Log In! (You have to do this before you can do anything else)
    /// </summary>
    /// <param name="email">Your Email</param>
    /// <param name="password">The password you registered with.</param>
    /// <returns>true if successful</returns>
    public bool Login(string email, string password)
    {
        LoginRecord login = new(email, password);
        bool success = true;
        _auth = ApiCall<LoginRecord, AuthResponse>(
            HttpMethod.Post, "api/auth", login, false,
            (e, s) => 
                success = DefaultErrorAction(e, s));
        return success;
    }
    
    #endregion
    
    #region Helper Methods
    
    private HttpRequestMessage BuildRequest<TIn>(HttpMethod method,
        string endpoint, TIn? content = default, bool authorize = true)
    {
        HttpRequestMessage request = new(method, endpoint);
        if (authorize && !_auth.HasValue)
        {
            PrintError("You can't authorize a request until you log in.");
        }

        if (authorize && (_auth?.IsExpired ?? false))
            RenewToken();
        
        if (authorize)
                request.Headers.Authorization = AuthHeader;

        if (content is not null)
        {
            var json = JsonSerializer.Serialize(content);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private bool Send<TIn>(HttpMethod method, string endpoint,
        TIn? content = default, bool authorize = true,
        Action<Exception, HttpStatusCode>? onErrorAction = null)
    {
        var request = BuildRequest(method, endpoint, content, authorize);
        var response = client.Send(request);
        return HandleNonSuccessStatus(onErrorAction, response);
    }

    private TOut? ApiCall<TIn, TOut>(HttpMethod method, string endpoint,
        TIn? content = default, bool authorize = true,
        Action<Exception, HttpStatusCode>? onErrorAction = null)
    {
        var request = BuildRequest(method, endpoint, content, authorize);
        var response = client.Send(request);
        string json;
        try
        {
            json = response.Content.ReadAsStringAsync().Result;
        }
        catch (Exception e)
        {
            if (onErrorAction is null) 
                throw;
            
            onErrorAction(e, response.StatusCode);
            return default;

        }
    
        if (!HandleNonSuccessStatus(onErrorAction, response)) 
            return default;

        try
        {
            return JsonSerializer.Deserialize<TOut>(json);
        }
        catch (Exception e)
        {
            if (onErrorAction is not null)
                onErrorAction(e, response.StatusCode);

            return default;
        }
    }

    private static bool HandleNonSuccessStatus(
        Action<Exception, HttpStatusCode>? onErrorAction, 
        HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) 
            return true;

        try
        {
            var json = response.Content.ReadAsStringAsync().Result;
            var error = JsonSerializer.Deserialize<ErrorRecord>(json);
            PrintError($"{error}");
            throw new ApiException(error);
        }
        catch (Exception e)
        {
            if (onErrorAction is not null)
                onErrorAction(e, response.StatusCode);
        }

        return false;

    }

    private static void PrintError(string error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ResetColor();
    }
    

    

    private bool RenewToken()
    {
        if (string.IsNullOrEmpty(_auth?.refreshToken))
        {
            PrintError("You cannot renew a token without first logging in.");
            return false;
        }
        
        bool success = true;
        _auth = ApiCall<LoginRecord, AuthResponse>(
            HttpMethod.Get, $"api/auth/renew?refreshToken={_auth?.refreshToken ?? ""}",
            onErrorAction: (e, s) => 
                success = DefaultErrorAction(e, s));
        return success;
    }

    private static bool DefaultErrorAction(Exception exception, HttpStatusCode status)
    {
        PrintError(
                $"Api Call Failed: {exception.Message} " +
                $"Status Code {status}");
            return false;
    }
    
    #endregion
}