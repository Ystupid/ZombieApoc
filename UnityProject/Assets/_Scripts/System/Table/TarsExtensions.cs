using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tup.Tars;
using System.Text;

public class TarsExtensions
{
    public static string TarsStructToString(TarsStruct msg)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(msg.GetType().Name + "=>");
        msg.Display(sb, 0);
        return sb.ToString();
    }

    public static byte[] TarsStructToBytes(TarsStruct msg)
    {
        TarsOutputStream _os = new TarsOutputStream(0);
        msg?.WriteTo(_os);
        return _os.toByteArray();
    }

    private static TarsInputStream kTarsInputStream = new TarsInputStream();

    public static void BytesToTarsStruct(byte[] msgBytes, TarsStruct msg)
    {
        lock (kTarsInputStream)
        {
            kTarsInputStream.wrap(msgBytes, 0);
            msg.ReadFrom(kTarsInputStream);
        }
    }

    public static void BytesToTarsStruct(byte[] msgBytes, int len, TarsStruct msg)
    {
        lock (kTarsInputStream)
        {
            kTarsInputStream.wrap(msgBytes, 0, len);
            msg.ReadFrom(kTarsInputStream);
        }
    }
}
