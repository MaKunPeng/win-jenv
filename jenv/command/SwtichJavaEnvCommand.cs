using jenv.controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace jenv.command
{
    public class SwtichJavaEnvCommand : JAbstractCommand
    {
        private const string JAVA_HOME = "JAVA_HOME";
        private const string DEFAULT_JAVA_PATH = "Oracle\\Java\\javapath";
        private const string JAVA_PATH = "%JAVA_HOME%\\bin";

        public SwtichJavaEnvCommand(JenvShell sender) : base(sender)
        {
        }

        public override void Execute(string param)
        {
            if (param == null || param.Trim().Length == 0)
            {
                Console.WriteLine("请指定要切换的Java环境");
                return;
            }
            string java_home_path = Sender.ConfigDict[param];
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

            if (oldPaths.IndexOf(DEFAULT_JAVA_PATH) != -1)
            {
                oldPaths = RemoveDefaultJavaPath(oldPaths, oldJavaHome);
            }
            // 检查旧的系统变量是否已设置了%JAVA_HOME%/bin，如果没有，则加入
            if (oldPaths.IndexOf(JAVA_PATH) == -1)
            {
                Environment.SetEnvironmentVariable("Path", oldPaths + ";" + JAVA_PATH, EnvironmentVariableTarget.Machine);
            }

            Console.WriteLine("已成功切换至" + param);
        }

        /// <summary>
        /// 移除Jdk安装时默认设置的以及旧有的环境变量
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
                // 排除旧有的java path
                if (path.IndexOf(oldJavaHome + "\\bin") != -1)
                {
                    continue;
                }
                newPathList.Add(path);
            }
            string newPaths = "";
            foreach (string item in newPathList)
            {
                newPaths += (item + ";");
            }
            string result = newPaths.Substring(0, newPaths.Length - 1);
            return result;
        }
    }
}
