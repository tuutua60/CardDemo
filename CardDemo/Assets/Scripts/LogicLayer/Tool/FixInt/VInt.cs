using System;

[Serializable]
public struct VInt:IComparable<VInt>
{
    private long i;
    //位移计数
    const int FIX_MULTIPLE = 1024;

    public static readonly VInt one = new VInt((long)FIX_MULTIPLE);
    public int Int { get { return (int)i; } }
    public float RawFloat { get { return (float)this.i * 1.0f / FIX_MULTIPLE; } }
    public int RawInt { get { return (int)i / FIX_MULTIPLE; } }

    private VInt(long i)
    {
        this.i = i;
    }
    public VInt(int i)
    {
        this.i = i * FIX_MULTIPLE;
    }
    public VInt(float f)
    {
        this.i = (int)Math.Round((double)(f * 1.0f * FIX_MULTIPLE));
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }
        VInt vInt = (VInt)o;
        return this.i == vInt.i;
    }

    public override int GetHashCode()
    {
        return this.i.GetHashCode();
    }

    public static VInt Min(VInt a, VInt b)
    {
        return new VInt(Math.Min(a.i, b.i));
    }

    public static VInt Max(VInt a, VInt b)
    {
        return new VInt(Math.Max(a.i, b.i));
    }

    public override string ToString()
    {
        return this.RawFloat.ToString();
    }

 

    public int CompareTo(VInt other)
    {
        return i.CompareTo(other.i);
    }

    public static explicit operator VInt(float f)
    {
        return new VInt((int)Math.Round((double)(f * 1.0f * FIX_MULTIPLE)));
    }

    public static implicit operator VInt(int i)
    {

        return new VInt(i);
    }

    public static explicit operator float(VInt ob)
    {
        return (float)ob.i * 1.0f / FIX_MULTIPLE;
    }

    public static explicit operator long(VInt ob)
    {
        return (long)ob.i;
    }

    public static VInt operator +(VInt a, VInt b)
    {
        return new VInt(a.i + b.i);
    }

    public static VInt operator -(VInt a, VInt b)
    {
        return new VInt(a.i - b.i);
    }

    public static VInt operator *(VInt a, VInt b)
    {
        long value = a.i * b.i;
        if (value >= 0)
        {
            value /= FIX_MULTIPLE;
        }
        else
        {
            value = -(-value / FIX_MULTIPLE);
        }
        return new VInt(value);
    }

    public static VInt operator /(VInt a, VInt b)
    {
        return new VInt((a.i * FIX_MULTIPLE / b.i));
    }
    public static bool operator ==(VInt a, VInt b)
    {
        return a.i == b.i;
    }
    public static VInt operator -(VInt a)
    {
        return new VInt(-a.i);
    }

    public static bool operator !=(VInt a, VInt b)
    {
        return a.i != b.i;
    }

    public static bool operator >(VInt a, VInt b)
    {
        return a.i > b.i;
    }
    public static bool operator <(VInt a, VInt b)
    {
        return a.i < b.i;
    }
    public static bool operator >=(VInt a, VInt b)
    {
        return a.i >= b.i;
    }
    public static bool operator <=(VInt a, VInt b)
    {
        return a.i <= b.i;
    }
    public static VInt operator >>(VInt value, int moveCount)
    {
        if (value.i >= 0)
        {
            return new VInt(value.i >> moveCount);
        }
        else
        {
            return new VInt(-(-value.i >> moveCount));
        }

    }
    public static VInt operator <<(VInt value, int moveCount)
    {
        return new VInt(value.i << moveCount);
    }
}
