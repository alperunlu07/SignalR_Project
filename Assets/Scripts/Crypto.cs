using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Crypto 
{  
    public static byte[] MD5Hash(string input)
    {
        using (var md5 = MD5.Create())
        { 
            return md5.ComputeHash(Encoding.ASCII.GetBytes(input)); 
        }
    }
}
