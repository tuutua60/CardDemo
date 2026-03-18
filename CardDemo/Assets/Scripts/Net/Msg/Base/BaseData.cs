using System;
using System.Text;

public abstract class BaseData
{
    /// <summary>
    /// 用于子类重写的 获取字节数组容器大小的方法
    /// </summary>
    /// <returns></returns>
    public abstract int GetBytesNum();
    /// <summary>
    /// 把成员变量序列化为对应字节数组
    /// </summary>
    /// <returns></returns>
    public abstract byte[] Writing();
    /// <summary>
    /// 把二进制字节数组 反序列化到 成员变量中
    /// </summary>
    /// <param name="info"></param>
    /// <param name="beginIndex"></param>
    /// <returns></returns>
    public abstract int Reading(byte[] info, int beginIndex = 0);
    #region Write
    protected void WriteInt(byte[] bytes, int value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += 4;
    }
    protected void WriteShort(byte[] bytes, short value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += 2;
    }
    protected void WriteFloat(byte[] bytes, float value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += 4;
    }
    protected void WriteBool(byte[] bytes, bool value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += 1;
    }
    protected void WriteByte(byte[] bytes, byte value, ref int index)
    {
        bytes[index] = value;
        index += 1;
    }
    protected void WriteString(byte[] bytes, string value, ref int index)
    {
        byte[] strBytes = Encoding.UTF8.GetBytes(value);
        int size = strBytes.Length;
        WriteInt(bytes, size, ref index);
        strBytes.CopyTo(bytes, index);
        index += size;
    }
    protected void WriteData(byte[] bytes, BaseData data, ref int index)
    {
        data.Writing().CopyTo(bytes, index);
        index += data.GetBytesNum();
    }
    #endregion

    #region Read
    protected int ReadInt(byte[] info, ref int index)
    {
        int result = BitConverter.ToInt32(info, index);
        index += 4;
        return result;
    }
    protected short ReadShort(byte[] info, ref int index)
    {
        short result = BitConverter.ToInt16(info, index);
        index += 2;
        return result;
    }
    protected float ReadFloat(byte[] info, ref int index)
    {
        float result = BitConverter.ToSingle(info, index);
        index += 4;
        return result;
    }
    protected bool ReadBool(byte[] info, ref int index)
    {
        bool result = BitConverter.ToBoolean(info, index);
        index += 1;
        return result;
    }
    protected string ReadString(byte[] info, ref int index)
    {
        int length = ReadInt(info, ref index);
        string result = Encoding.UTF8.GetString(info, index, length);
        index += length;
        return result;
    }
    protected T ReadData<T>(byte[] info, ref int index) where T : BaseData, new()
    {
        T result = new T();
        index += result.Reading(info, index);
        return result;
    }
    #endregion

}
