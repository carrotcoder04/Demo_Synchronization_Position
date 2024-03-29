using System.Text;
using System;
using Newtonsoft.Json;
using MemoryPack;
public static class GameHelper
{
    public static string RandomString(int length)
    {
        var rand = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid() + $"{DateTime.Now}"));
        return rand[..length];
    }
}