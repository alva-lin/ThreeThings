﻿using System.Security.Cryptography;
using System.Text;

namespace ThreeThings.Utils.Helpers;

public static class CryptoHelper
{
    private const string SEPARATE = "_";

    public static string Md5(params string[] args)
    {
        var md5 = MD5.Create();
        var str = string.Join(SEPARATE, args);
        var bytes = Encoding.UTF8.GetBytes(str);
        var newBytes = md5.ComputeHash(bytes);

        return Convert.ToHexString(newBytes);
    }
}
