using System;
using System.Text;

public static class BinaryConvert
{
    public static byte[] Add(byte[] array1, byte[] array2)
    {
        byte[] result = new byte[array1.Length + array2.Length];
        Array.Copy(array1, 0, result, 0, array1.Length);
        Array.Copy(array2, 0, result, array1.Length, array2.Length);
        return result;
    }
    public static byte[] Append(byte[] array, byte value)
    {
        byte[] result = new byte[array.Length + 1];
        Array.Copy(array, result, array.Length);
        result[result.Length - 1] = value;
        return result;
    }
    public static byte[] AddFirst(byte newByte, byte[] originalArray)
    {
        byte[] newArray = new byte[originalArray.Length + 1];
        newArray[0] = newByte;
        Array.Copy(originalArray, 0, newArray, 1, originalArray.Length);
        return newArray;
    }
    public static float ConvertFloat(byte[] bytes, byte startIndex, byte length)
    {
        byte[] result = new byte[length];
        Array.Copy(bytes, startIndex, result, 0, length);
        return BitConverter.ToSingle(result);
    }
    public static string ConvertString(byte[] bytes)
    {
        string result = Encoding.UTF8.GetString(bytes,1,bytes.Length - 1);
        return result;
    }
}
public static class ConvertDirect
{
    public static Direct ByteToDirect(byte data)
    {
        Direct direct = new Direct();
        byte x = (byte)((data & 12) >> 2);
        byte y = (byte)(data & 3);
        byte _anim = (byte)(data >> 4);
        if (x == 0)
        {
            direct.x = 0;
        }
        else if (x == 1)
        {
            direct.x = 1;
        }
        else if (x == 2)
        {
            direct.x = -1;
        }
        if (y == 0)
        {
            direct.y = 0;
        }
        else if (y == 1)
        {
            direct.y = 1;
        }
        else if (y == 2)
        {
            direct.y = -1;
        }
        Anim anim = (Anim)_anim;
        direct.anim = anim;
        return direct;
    }
    public static byte DirectToByte(Direct direct)
    {
        byte result = 0;
        if (direct.x == 0)
        {

        }
        else if (direct.x == 1)
        {
            result = 4;
        }
        else if (direct.x == -1)
        {
            result = 8;
        }
        if (direct.y == 0)
        {

        }
        else if (direct.y == 1)
        {
            result |= 1;
        }
        else if (direct.y == -1)
        {
            result |= 2;
        }
        byte _anim = (byte)((byte)(direct.anim) << 4);
        result |= _anim;
        return result;
    }
    public static byte[] PackTransform(byte id, byte direct)
    {
        byte[] data = new byte[]
        {
            (byte)Game.PLAYER_POSITION,id, direct
        };
        return data;
    }
}
