// See https://aka.ms/new-console-template for more information

#region Program

using CS7_Alg_TextManipulation;

Console.WriteLine("Enter a secret Message");
var message = Console.ReadLine()!;
var encrypted = EncryptOrDecryptString(message, "my_password");
Console.WriteLine(encrypted);

var decrypted = EncryptOrDecryptString(encrypted, "my_password");
Console.WriteLine(decrypted);

#endregion

#region Methods


string EncryptOrDecryptString(string message, string password)
{
    throw new NotImplementedException("You gotta write this code!");
    //TODO:  Convert `message` and `password` to DATA and then encrypt/decrypt the message!
}

void EncryptOrDecrypteData(ref byte[] data, byte[] key)
{
    // TODO:  use the XOR operator to encrypt/decrypt the data using the key
}

#endregion
