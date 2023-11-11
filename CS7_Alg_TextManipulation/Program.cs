// See https://aka.ms/new-console-template for more information

#region Program

using CS7_Alg_TextManipulation;

Console.WriteLine("Enter a secret Message");
var message = Console.ReadLine()!;
var encrypted = EncryptOrDecrypt(message, "my_password");
Console.WriteLine(encrypted);
#endregion

#region Methods

string EncryptOrDecrypt(string message, string password)
{
    string output = message;
    
    // TODO:  Implement Encryption using your Password and the XOR operator
    
    return output;
}

#endregion
