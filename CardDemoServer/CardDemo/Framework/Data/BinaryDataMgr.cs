using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
// 引入 UnityWebRequest 的命名空间
using UnityEngine.Networking;

/// <summary>
/// 2进制数据管理器
/// </summary>
public class BinaryDataMgr
{
    /// <summary>
    /// 2进制数据存储位置路径
    /// </summary>
    public static string DATA_BINARY_PATH = @"D:\vs xiang mu\CardDemo\CardDemo\Framework\Binary\";

    /// <summary>
    /// 用于存储所有Excel表数据的容器
    /// </summary>
    private Dictionary<string, object> tableDic = new Dictionary<string, object>();

    /// <summary>
    /// 数据存储的位置
    /// </summary>
    //private static string SAVE_PATH = Application.persistentDataPath + "/Data/";

    private static BinaryDataMgr instance = new BinaryDataMgr();
    public static BinaryDataMgr Instance => instance;

    private BinaryDataMgr()
    {
        //Initialize()也可以写在这里
        //Initialize();
    }

    /// <summary>
    /// 游戏需要一次Initialize才能读表
    /// </summary>
    public void Initialize()
    {
        // 每张表需要注册 类似这种格式
        //LoadTable<T_TestExcelContainer, T_TestExcel>();
        LoadTable<T_HeroDataContainer, T_HeroData>();
        LoadTable<BuffConfigContainer, BuffConfig>();
        LoadTable<SkillConfigContainer, SkillConfig>();
    }

    /// <summary>
    /// 加载Excel表的2进制数据到内存中 
    /// </summary>
    /// <typeparam name="T">容器类名</typeparam>
    /// <typeparam name="K">数据结构类类名</typeparam>
    public void LoadTable<T, K>()
    {
        // 使用 Path.Combine 来构建路径，更安全
        string filePath = Path.Combine(DATA_BINARY_PATH, typeof(K).Name + ".tuu");
        byte[] bytes = null;

        // 根据不同平台使用不同加载方式
#if UNITY_ANDROID && !UNITY_EDITOR
        // --- 安卓平台加载逻辑 ---
        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
        {
            // UnityWebRequest 是异步的，但我们在初始化阶段需要同步加载，
            // 所以这里使用一个简单的循环来等待它完成。
            var asyncOp = www.SendWebRequest();
            while (!asyncOp.isDone)
            {
                // 等待
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                bytes = www.downloadHandler.data;
            }
            else
            {
                Debug.LogError($"[Android] Failed to load binary file: {www.error} at path {filePath}");
                return;
            }
        }
#else
        // --- 编辑器及其他平台加载逻辑 ---
        if (!File.Exists(filePath))
        {
            Debug.LogError($"[Editor/PC] Binary file not found: {filePath}");
            return;
        }
        bytes = File.ReadAllBytes(filePath);
#endif

        if (bytes == null)
        {
            Debug.LogError($"Failed to read bytes for table {typeof(K).Name}");
            return;
        }

        try
        {
            // --- 后续的字节解析逻辑保持不变 ---
            int index = 0;

            //读取多少行数据
            int count = BitConverter.ToInt32(bytes, index);
            index += 4;

            //读取主键的名字
            int keyNameLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
            index += keyNameLength;

            //创建容器类对象
            Type contaninerType = typeof(T);
            object contaninerObj = Activator.CreateInstance(contaninerType);
            //得到数据结构类的Type
            Type classType = typeof(K);
            //通过反射 得到数据结构类 所有字段的信息
            FieldInfo[] infos = classType.GetFields();

            //读取每一行的信息
            for (int i = 0; i < count; i++)
            {
                //实例化一个数据结构类 对象
                object dataObj = Activator.CreateInstance(classType);
                foreach (FieldInfo info in infos)
                {
                    try
                    {
                        if (info.FieldType == typeof(int))
                        {
                            //相当于就是把2进制数据转为int 然后赋值给了对应的字段
                            info.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                            index += 4;
                        }
                        else if (info.FieldType == typeof(float))
                        {
                            info.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                            index += 4;
                        }
                        else if (info.FieldType == typeof(bool))
                        {
                            info.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                            index += 1;
                        }
                        else if (info.FieldType == typeof(string))
                        {
                            //读取字符串字节数组的长度
                            int length = BitConverter.ToInt32(bytes, index);
                            index += 4;
                            info.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, length));
                            index += length;
                        }
                        else if (info.FieldType.IsEnum)
                        {
                            // 将4字节的二进制数据转为int 然后赋值给对应的枚举字段
                            info.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                            index += 4;
                        }
                        else if (info.FieldType == typeof(int[]))
                        {
                            int arrayLength = BitConverter.ToInt32(bytes, index);
                            index += 4;
                            int[] array = new int[arrayLength];
                            for (int j = 0; j < arrayLength; j++)
                            {
                                array[j] = BitConverter.ToInt32(bytes, index);
                                index += 4;
                            }
                            info.SetValue(dataObj, array);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error setting field {info.Name}: {ex.Message}");
                        // 继续处理下一个字段
                        continue;
                    }
                }

                //读取完一行的数据了 应该把这个数据添加到容器对象中
                //得到容器对象中的 字典对象
                object dicObject = contaninerType.GetField("dataDic").GetValue(contaninerObj);
                //通过字典对象得到其中的 Add方法
                MethodInfo mInfo = dicObject.GetType().GetMethod("Add");
                //得到数据结构类对象中 指定主键字段的值
                object keyValue = classType.GetField(keyName).GetValue(dataObj);
                mInfo.Invoke(dicObject, new object[] { keyValue, dataObj });
            }

            //把读取完的表记录下来
            tableDic.Add(typeof(T).Name, contaninerObj);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing table {typeof(K).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// 得到一张表的信息
    /// </summary>
    /// <typeparam name="T">容器类名</typeparam>
    /// <returns></returns>
    public T GetTable<T>() where T : class
    {
        string tableName = typeof(T).Name;
        if (tableDic.ContainsKey(tableName))
            return tableDic[tableName] as T;

        Debug.LogError($"Table {tableName} not found in loaded tables");
        return null;
    }

    /// <summary>
    /// 存储类对象数据
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fileName"></param>
    public void SaveData(object obj, string fileName)
    {
#if CLIENT_LOGIC
        try
        {
            //先判断路径文件夹有没有
            if (!Directory.Exists(SAVE_PATH))
                Directory.CreateDirectory(SAVE_PATH);

            using (FileStream fs = new FileStream(SAVE_PATH + fileName + ".tuu", FileMode.OpenOrCreate, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving data to {fileName}: {ex.Message}");
        }
#endif
    }

    /// <summary>
    /// 读取2进制数据转换成对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public T LoadData<T>(string fileName) where T : class
    {
#if CLIENT_LOGIC
        string filePath = SAVE_PATH + fileName + ".tuu";

        //如果不存在这个文件 就直接返回泛型对象的默认值
        if (!File.Exists(filePath))
            return default(T);

        try
        {
            T obj;
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                obj = bf.Deserialize(fs) as T;
            }

            return obj;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading data from {fileName}: {ex.Message}");
            return default(T);
        }
#else
        return default(T);
#endif
    }
}