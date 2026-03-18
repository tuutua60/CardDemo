using System;
using System.Collections.Generic;
using System.Text;
	public class BattleMsg : BaseMsg
	{
		public int seed;
		public List<int> playerDataList;
		public List<int> enemyDataList;
		public override int GetBytesNum()
		{
			int num = 8;
			num += 4;
			num += 2;
			for (int i = 0; i < playerDataList.Count; ++i)
				num += 4;
			num += 2;
			for (int i = 0; i < enemyDataList.Count; ++i)
				num += 4;
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes, GetID(), ref index);
			WriteInt(bytes, bytes.Length - 8, ref index);
			WriteInt(bytes, seed, ref index);
			WriteShort(bytes, (short)playerDataList.Count, ref index);
			for (int i = 0; i < playerDataList.Count; ++i)
				WriteInt(bytes, playerDataList[i], ref index);
			WriteShort(bytes, (short)enemyDataList.Count, ref index);
			for (int i = 0; i < enemyDataList.Count; ++i)
				WriteInt(bytes, enemyDataList[i], ref index);
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			seed = ReadInt(bytes, ref index);
			playerDataList = new List<int>();
			short playerDataListCount = ReadShort(bytes, ref index);
			for (int i = 0; i < playerDataListCount; ++i)
				playerDataList.Add(ReadInt(bytes, ref index));
			enemyDataList = new List<int>();
			short enemyDataListCount = ReadShort(bytes, ref index);
			for (int i = 0; i < enemyDataListCount; ++i)
				enemyDataList.Add(ReadInt(bytes, ref index));
			return index - beginIndex;
		}
		public override int GetID()
		{
			return 2001;
		}
	}
