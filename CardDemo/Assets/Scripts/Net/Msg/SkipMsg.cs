using System;
using System.Collections.Generic;
using System.Text;
	public class SkipMsg : BaseMsg
	{
		public bool isPlayerWin;
		public int roundCount;
		public List<int> playerNowHp;
		public List<int> playerNowRage;
		public List<int> enemyNowHp;
		public List<int> enemyNowRage;
		public override int GetBytesNum()
		{
			int num = 8;
			num += 1;
			num += 4;
			num += 2;
			for (int i = 0; i < playerNowHp.Count; ++i)
				num += 4;
			num += 2;
			for (int i = 0; i < playerNowRage.Count; ++i)
				num += 4;
			num += 2;
			for (int i = 0; i < enemyNowHp.Count; ++i)
				num += 4;
			num += 2;
			for (int i = 0; i < enemyNowRage.Count; ++i)
				num += 4;
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes, GetID(), ref index);
			WriteInt(bytes, bytes.Length - 8, ref index);
			WriteBool(bytes, isPlayerWin, ref index);
			WriteInt(bytes, roundCount, ref index);
			WriteShort(bytes, (short)playerNowHp.Count, ref index);
			for (int i = 0; i < playerNowHp.Count; ++i)
				WriteInt(bytes, playerNowHp[i], ref index);
			WriteShort(bytes, (short)playerNowRage.Count, ref index);
			for (int i = 0; i < playerNowRage.Count; ++i)
				WriteInt(bytes, playerNowRage[i], ref index);
			WriteShort(bytes, (short)enemyNowHp.Count, ref index);
			for (int i = 0; i < enemyNowHp.Count; ++i)
				WriteInt(bytes, enemyNowHp[i], ref index);
			WriteShort(bytes, (short)enemyNowRage.Count, ref index);
			for (int i = 0; i < enemyNowRage.Count; ++i)
				WriteInt(bytes, enemyNowRage[i], ref index);
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			isPlayerWin = ReadBool(bytes, ref index);
			roundCount = ReadInt(bytes, ref index);
			playerNowHp = new List<int>();
			short playerNowHpCount = ReadShort(bytes, ref index);
			for (int i = 0; i < playerNowHpCount; ++i)
				playerNowHp.Add(ReadInt(bytes, ref index));
			playerNowRage = new List<int>();
			short playerNowRageCount = ReadShort(bytes, ref index);
			for (int i = 0; i < playerNowRageCount; ++i)
				playerNowRage.Add(ReadInt(bytes, ref index));
			enemyNowHp = new List<int>();
			short enemyNowHpCount = ReadShort(bytes, ref index);
			for (int i = 0; i < enemyNowHpCount; ++i)
				enemyNowHp.Add(ReadInt(bytes, ref index));
			enemyNowRage = new List<int>();
			short enemyNowRageCount = ReadShort(bytes, ref index);
			for (int i = 0; i < enemyNowRageCount; ++i)
				enemyNowRage.Add(ReadInt(bytes, ref index));
			return index - beginIndex;
		}
		public override int GetID()
		{
			return 2003;
		}
	}
