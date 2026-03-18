using System;
using System.Text;
using UnityEngine.UIElements;

public class TestMsg : BaseMsg
{
    public string name;
    public int atk;
    public int lev;
    public override int GetBytesNum()
    {
        return 4 + 4 + 4 + 4 + 4 + Encoding.UTF8.GetBytes(name).Length;
    }

    public override int GetID()
    {
        return 1001;
    }

    public override int Reading(byte[] info, int beginIndex = 0)
    {
        int index = beginIndex;
        name = ReadString(info, ref index);
        atk = ReadInt(info, ref index);
        lev = ReadInt(info, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int length = GetBytesNum();
        byte[] bytes = new byte[length];
        //先写消息ID
        WriteInt(bytes, GetID(), ref index);
        //再写消息长度
        WriteInt(bytes, length - 8, ref index);
        //再写数据
        WriteString(bytes, name, ref index);
        WriteInt(bytes, atk, ref index);
        WriteInt(bytes, lev, ref index);
        return bytes;
    }
}