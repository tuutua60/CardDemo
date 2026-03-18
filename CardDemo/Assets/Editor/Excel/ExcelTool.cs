using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExcelTool
{
    /// <summary>
    /// excel文件存放的路径
    /// </summary>
    public static string EXCEL_PATH = Application.dataPath + "/ArtRes/Excel/";

    /// <summary>
    /// 数据结构类脚本存储位置路径
    /// </summary>
    public static string DATA_CLASS_PATH = Application.dataPath + "/Scripts/Z_ProjectBase/Excel/ExcelData/DataClass/";

    /// <summary>
    /// 容器类脚本存储位置路径
    /// </summary>
    public static string DATA_CONTAINER_PATH = Application.dataPath + "/Scripts/Z_ProjectBase/Excel/ExcelData/Container/";

    /// <summary>
    /// 真正内容开始的行号
    /// </summary>
    public static int BEGIN_INDEX = 4;

    [MenuItem("Tools/GenerateExcelData")]
    private static void GenerateExcelInfo()
    {
        try
        {
            //检查Excel路径是否存在
            if (!Directory.Exists(EXCEL_PATH))
            {
                Debug.LogError($"Excel path does not exist: {EXCEL_PATH}");
                return;
            }

            //记在指定路径中的所有Excel文件 用于生成对应的3个文件
            DirectoryInfo dInfo = Directory.CreateDirectory(EXCEL_PATH);
            //得到指定路径中的所有文件信息 相当于就是得到所有的Excel表
            FileInfo[] files = dInfo.GetFiles();
            //数据表容器
            DataTableCollection tableConllection;
            for (int i = 0; i < files.Length; i++)
            {
                //如果不是excel文件就不要处理了
                if (files[i].Extension != ".xlsx" &&
                    files[i].Extension != ".xls")
                    continue;

                try
                {
                    //打开一个Excel文件得到其中的所有表的数据
                    using (FileStream fs = files[i].Open(FileMode.Open, FileAccess.Read))
                    {
                        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                        tableConllection = excelReader.AsDataSet().Tables;
                        fs.Close();
                    }

                    //遍历文件中的所有表的信息
                    foreach (DataTable table in tableConllection)
                    {
                        try
                        {
                            //生成数据结构类
                            GenerateExcelDataClass(table);
                            //生成容器类
                            GenerateExcelContainer(table);
                            //生成2进制数据
                            GenerateExcelBinary(table);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error processing table {table.TableName}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error processing file {files[i].Name}: {ex.Message}");
                }
            }

            // 生成技能和Buff的二进制配置
            GenerateSkillConfigBinary();
            GenerateBuffConfigBinary();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in GenerateExcelInfo: {ex.Message}");
        }
        finally
        {
            //刷新Project窗口
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// 生成技能二进制配置文件
    /// </summary>
    private static void GenerateSkillConfigBinary()
    {
        // 自动生成容器脚本
        GenerateConfigContainer("SkillConfig", "int");

        // 查找项目中所有的SkillConfig ScriptableObject
        string[] guids = AssetDatabase.FindAssets("t:SkillConfig", new[] { "Assets/Resources/Config/SkillConfig" });
        if (guids.Length == 0)
        {
            Debug.Log("No SkillConfig assets found in Assets/Resources/Config/SkillConfig/");
            return;
        }

        // 确保二进制数据目录存在
        if (!Directory.Exists(BinaryDataMgr.DATA_BINARY_PATH))
            Directory.CreateDirectory(BinaryDataMgr.DATA_BINARY_PATH);

        // 创建二进制文件写入器
        using (FileStream fs = new FileStream(Path.Combine(BinaryDataMgr.DATA_BINARY_PATH, "SkillConfig.tuu"), FileMode.Create, FileAccess.Write))
        {
            // 写入配置数量
            fs.Write(BitConverter.GetBytes(guids.Length), 0, 4);
            // 写入主键名
            string keyName = "skillID";
            byte[] keyNameBytes = Encoding.UTF8.GetBytes(keyName);
            fs.Write(BitConverter.GetBytes(keyNameBytes.Length), 0, 4);
            fs.Write(keyNameBytes, 0, keyNameBytes.Length);

            // 遍历所有SkillConfig并写入数据
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SkillConfig config = AssetDatabase.LoadAssetAtPath<SkillConfig>(path);
                if (config == null) continue;

                FieldInfo[] fields = typeof(SkillConfig).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    // 根据字段类型写入二进制数据
                    if (field.FieldType == typeof(int))
                    {
                        fs.Write(BitConverter.GetBytes((int)field.GetValue(config)), 0, 4);
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        string val = (string)field.GetValue(config) ?? "";
                        byte[] bytes = Encoding.UTF8.GetBytes(val);
                        fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        fs.Write(BitConverter.GetBytes((bool)field.GetValue(config)), 0, 1);
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        fs.Write(BitConverter.GetBytes((float)field.GetValue(config)), 0, 4);
                    }
                    else if (field.FieldType.IsEnum)
                    {
                        fs.Write(BitConverter.GetBytes((int)field.GetValue(config)), 0, 4);
                    }
                    else if (field.FieldType == typeof(int[]))
                    {
                        int[] array = (int[])field.GetValue(config) ?? new int[0];
                        fs.Write(BitConverter.GetBytes(array.Length), 0, 4);
                        foreach (int item in array)
                        {
                            fs.Write(BitConverter.GetBytes(item), 0, 4);
                        }
                    }
                    // Sprite等Unity对象类型不进行序列化
                }
            }
        }
        Debug.Log("SkillConfig binary data generated successfully.");
    }

    /// <summary>
    /// 生成Buff二进制配置文件
    /// </summary>
    private static void GenerateBuffConfigBinary()
    {
        // 自动生成容器脚本
        GenerateConfigContainer("BuffConfig", "int");

        // 查找项目中所有的BuffConfig ScriptableObject
        string[] guids = AssetDatabase.FindAssets("t:BuffConfig", new[] { "Assets/Resources/Config/BuffConfig" });
        if (guids.Length == 0)
        {
            Debug.Log("No BuffConfig assets found in Assets/Resources/Config/BuffConfig/");
            return;
        }

        // 确保二进制数据目录存在
        if (!Directory.Exists(BinaryDataMgr.DATA_BINARY_PATH))
            Directory.CreateDirectory(BinaryDataMgr.DATA_BINARY_PATH);

        // 创建二进制文件写入器
        using (FileStream fs = new FileStream(Path.Combine(BinaryDataMgr.DATA_BINARY_PATH, "BuffConfig.tuu"), FileMode.Create, FileAccess.Write))
        {
            // 写入配置数量
            fs.Write(BitConverter.GetBytes(guids.Length), 0, 4);
            // 写入主键名
            string keyName = "buffID";
            byte[] keyNameBytes = Encoding.UTF8.GetBytes(keyName);
            fs.Write(BitConverter.GetBytes(keyNameBytes.Length), 0, 4);
            fs.Write(keyNameBytes, 0, keyNameBytes.Length);

            // 遍历所有BuffConfig并写入数据
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BuffConfig config = AssetDatabase.LoadAssetAtPath<BuffConfig>(path);
                if (config == null) continue;

                FieldInfo[] fields = typeof(BuffConfig).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    // 根据字段类型写入二进制数据
                    if (field.FieldType == typeof(int))
                    {
                        fs.Write(BitConverter.GetBytes((int)field.GetValue(config)), 0, 4);
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        string val = (string)field.GetValue(config) ?? "";
                        byte[] bytes = Encoding.UTF8.GetBytes(val);
                        fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        fs.Write(BitConverter.GetBytes((bool)field.GetValue(config)), 0, 1);
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        fs.Write(BitConverter.GetBytes((float)field.GetValue(config)), 0, 4);
                    }
                    else if (field.FieldType.IsEnum)
                    {
                        fs.Write(BitConverter.GetBytes((int)field.GetValue(config)), 0, 4);
                    }
                    // Sprite等Unity对象类型不进行序列化
                }
            }
        }
        Debug.Log("BuffConfig binary data generated successfully.");
    }

    /// <summary>
    /// 为ScriptableObject配置生成数据容器类
    /// </summary>
    /// <param name="configName">配置名 (例如 "SkillConfig")</param>
    /// <param name="keyType">主键类型 (例如 "int")</param>
    private static void GenerateConfigContainer(string configName, string keyType)
    {
        //没有路径创建路径
        if (!Directory.Exists(DATA_CONTAINER_PATH))
            Directory.CreateDirectory(DATA_CONTAINER_PATH);

        string containerName = configName + "Container";
        string str = "using System.Collections.Generic;\n\n";
        str += $"public class {containerName}\n{{\n";
        str += $"    public Dictionary<{keyType}, {configName}> dataDic = new Dictionary<{keyType}, {configName}>();\n";
        str += "}";

        File.WriteAllText(DATA_CONTAINER_PATH + containerName + ".cs", str);
        Debug.Log($"Generated container script: {containerName}.cs");
    }

    /// <summary>
    /// 生成Excel表对应的数据结构类
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelDataClass(DataTable table)
    {
        try
        {
            //字段名行
            DataRow rowName = GetVariableNameRow(table);
            //字段类型行
            DataRow rowType = GetVariableTypeRow(table);

            //判断路径是否存在 没有的话 就创建文件夹
            if (!Directory.Exists(DATA_CLASS_PATH))
                Directory.CreateDirectory(DATA_CLASS_PATH);

            //如果我们要生成对应的数据结构类脚本 其实就是通过代码进行字符串拼接 然后存进文件就行了
            string str = "public class " + table.TableName + "\n{\n";

            //变量进行字符串拼接
            for (int i = 0; i < table.Columns.Count; i++)
            {
                str += "    public " + rowType[i].ToString() + " " + rowName[i].ToString() + ";\n";
            }

            str += "}";

            //把拼接好的字符串存到指定文件中去
            File.WriteAllText(DATA_CLASS_PATH + table.TableName + ".cs", str);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error generating data class for {table.TableName}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 生成Excel表对应的数据容器类
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelContainer(DataTable table)
    {
        try
        {
            //得到主键索引
            int keyIndex = GetKeyIndex(table);
            //得到字段类型行
            DataRow rowType = GetVariableTypeRow(table);
            //没有路径创建路径
            if (!Directory.Exists(DATA_CONTAINER_PATH))
                Directory.CreateDirectory(DATA_CONTAINER_PATH);

            string str = "using System.Collections.Generic;\n";

            str += "public class " + table.TableName + "Container" + "\n{\n";

            str += "    ";
            str += "public Dictionary<" + rowType[keyIndex].ToString() + ", " + table.TableName + ">";
            str += "dataDic = new " + "Dictionary<" + rowType[keyIndex].ToString() + ", " + table.TableName + ">();\n";

            str += "}";

            File.WriteAllText(DATA_CONTAINER_PATH + table.TableName + "Container.cs", str);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error generating container for {table.TableName}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 生成excel2进制数据
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelBinary(DataTable table)
    {
        try
        {
            //没有路径创建路径
            if (!Directory.Exists(BinaryDataMgr.DATA_BINARY_PATH))
                Directory.CreateDirectory(BinaryDataMgr.DATA_BINARY_PATH);

            //创建一个2进制文件进行写入
            using (FileStream fs = new FileStream(BinaryDataMgr.DATA_BINARY_PATH + table.TableName + ".tuu", FileMode.OpenOrCreate, FileAccess.Write))
            {
                //存储具体的excel对应的2进制信息
                //1.先要存储我们需要写多少行的数据 方便我们读取
                //-4的原因是因为 前面4行是配置规则 并不是我们需要记录的数据内容
                fs.Write(BitConverter.GetBytes(table.Rows.Count - 4), 0, 4);
                //2.存储主键的变量名
                string keyName = GetVariableNameRow(table)[GetKeyIndex(table)].ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(keyName);
                //存储字符串字节数组的长度
                fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                //存储字符串字节数组
                fs.Write(bytes, 0, bytes.Length);

                //遍历所有内容的行 进行2进制的写入
                DataRow row;
                //得到类型行 根据类型来决定应该如何写入数据
                DataRow rowType = GetVariableTypeRow(table);
                for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
                {
                    //得到一行的数据
                    row = table.Rows[i];
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        try
                        {
                            switch (rowType[j].ToString())
                            {
                                case "int":
                                    fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, 4);
                                    break;
                                case "float":
                                    fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                                    break;
                                case "bool":
                                    fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, 1);
                                    break;
                                case "string":
                                    bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                                    //写入字符串字节数组的长度
                                    fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                                    //写入字符串字节数组
                                    fs.Write(bytes, 0, bytes.Length);
                                    break;
                                default:
                                    Debug.LogError($"Unsupported data type: {rowType[j]} in table {table.TableName}, column {j}");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error processing cell [{i},{j}] in table {table.TableName}: {ex.Message}");
                        }
                    }
                }

                fs.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error generating binary for {table.TableName}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 获取变量名所在行
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetVariableNameRow(DataTable table)
    {
        return table.Rows[0];
    }

    /// <summary>
    /// 获取变量类型所在行
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetVariableTypeRow(DataTable table)
    {
        return table.Rows[1];
    }

    /// <summary>
    /// 获取主键索引
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static int GetKeyIndex(DataTable table)
    {
        DataRow row = table.Rows[2];
        for (int i = 0; i < table.Columns.Count; i++)
        {
            if (row[i].ToString() == "key")
                return i;
        }

        Debug.LogError($"No key found in table {table.TableName}. Using first column as default.");
        return 0;
    }
}