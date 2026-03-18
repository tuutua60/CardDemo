using System;
using System.Collections.Generic;
using System.Text;
	public class TestMsg : BaseMsg
	{
		public string name;
		public int atk;
		public int lev;
		public override int GetBytesNum()
		{
			int num = 8;
			num += 4 + Encoding.UTF8.GetByteCount(name);
			num += 4;
			num += 4;
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes, GetID(), ref index);
			WriteInt(bytes, bytes.Length - 8, ref index);
			WriteString(bytes, name, ref index);
			WriteInt(bytes, atk, ref index);
			WriteInt(bytes, lev, ref index);
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			name = ReadString(bytes, ref index);
			atk = ReadInt(bytes, ref index);
			lev = ReadInt(bytes, ref index);
			return index - beginIndex;
		}
		public override int GetID()
		{
			return 1001;
		}
	}
