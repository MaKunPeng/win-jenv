using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace jenv
{
    /// <summary>
    /// Windows Shell环境下切换Java版本的工具
    /// </summary>
    public class Program
    {
        private static readonly string JAVA_HOME = "JAVA_HOME";
        private static readonly string DEFAULT_JAVA_PATH = "Oracle\\Java\\javapath";
        private static readonly string JAVA_PATH = "%JAVA_HOME%\\bin";

        static void Main(string[] args)
        {
            // 命令行参数解析
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("请输入命令参数！");
                return;
            }
            String arg0 = args[0];

            // 获取当前程序所在目录
            string currentPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            // 解析json配置文件
            string jsonText = "";
            using (StreamReader fileReader = new StreamReader(currentPath + "\\config.json"))
            {
                while (!fileReader.EndOfStream)
                {
                    jsonText += fileReader.ReadLine();
                }
            }
            Dictionary<string, string> config = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);

            // 动作
            switch (arg0)
            {
                case "list":
                    ListJenvConfig(config);
                    break;
                default:
                    SetJavaEnviroment(arg0, config);
                    break;
            }
        }

        /// <summary>
        /// 列出所有已配置的Java环境
        /// </summary>
        private static void ListJenvConfig(Dictionary<string, string> config)
        {
            foreach (KeyValuePair<string, string> kv in config)
            {
                Console.WriteLine(kv.Key + " : " + kv.Value);
            }
        }

        /// <summary>
        /// 设置Java环境变量
        /// </summary>
        private static void SetJavaEnviroment(string jenvKey, Dictionary<string, string> config)
        {
            string java_home_path = config[jenvKey];
            if (java_home_path == null || java_home_path.Length < 1)
            {
                Console.WriteLine("环境未配置，请检查config.json");
                return;
            }
            // 如果未设置过java_home，则新创建一个，否则便更新已有
            string oldJavaHome = Environment.GetEnvironmentVariable(JAVA_HOME, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(JAVA_HOME, java_home_path, EnvironmentVariableTarget.Machine);

            // 获取系统PATH变量
            string oldPaths = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
            // 移除Jdk安装时默认设置的以及旧有的环境变量
            if (oldPaths.IndexOf(DEFAULT_JAVA_PATH) != -1)
            {
                oldPaths = RemoveDefaultJavaPath(oldPaths, oldJavaHome);
            }
            // 检查旧的系统变量是否已设置了%JAVA_HOME%/bin，如果没有，则加入
            if (oldPaths.IndexOf(JAVA_PATH) == -1)
            {
                Environment.SetEnvironmentVariable("Path", oldPaths + ";" + JAVA_PATH, EnvironmentVariableTarget.Machine);
            }

            Console.WriteLine("已成功切换至" + jenvKey);
        }

        /// <summary>
        /// 移除Jdk默认安装时设置的环境变量
        /// </summary>
        /// <param name="oldPaths"></param>
        /// <returns></returns>
        private static string RemoveDefaultJavaPath(string oldPaths, string oldJavaHome)
        {
            string[] oldPathList = oldPaths.Split(";");
            List<string> newPathList = new List<string>();
            foreach (string path in oldPathList)
            {
                // 排除Jdk安装时默认设置的java path
                if (path.Length == 0 || path.IndexOf(DEFAULT_JAVA_PATH) != -1)
                {
                    continue;
                }

                if (path.IndexOf(oldJavaHome + "\\bin") != -1)
                {
                    continue;
                }
                newPathList.Add(path);
            }
            string newPaths = "";
            foreach(string item in newPathList)
            {
                newPaths += (item + ";");
            }
            string result = newPaths.Substring(0, newPaths.Length - 1);
            return result;
        }
    }
}
