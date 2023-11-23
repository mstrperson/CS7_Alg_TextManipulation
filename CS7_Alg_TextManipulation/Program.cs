// See https://aka.ms/new-console-template for more information

#region Program

using System.Text;
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
    var data = Encoding.UTF8.GetBytes(message);
    var key = Encoding.UTF8.GetBytes(password);
    
    EncryptOrDecryptData(ref data, key);
    
    // Give back the encrypted String
    return Encoding.UTF8.GetString(data);
}

void EncryptOrDecryptData(ref byte[] data, byte[] key)
{
    for (int i = 0; i < data.Length; i++)
    {
        int j = i % key.Length;
        data[i] ^= key[j]; // XOR
    }
}

#endregion
