using jenv.command;
using jenv.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace jenv.controller
{
    public class JenvShell
    {
        /// <summary>
        /// 配置字典
        /// </summary>
        public Dictionary<string, string> ConfigDict { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 命令行为字典
        /// </summary>
        private Dictionary<string, ICommand> CommandDict = new Dictionary<string, ICommand>();

        public JenvShell()
        {
            Init();
        }

        public void Process(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return;
            }

            // 分离命令和参数，目前暂不支持选项（如 -l -a --all等)
            string commandStr = args[0];
            string param = "";
            if (args.Length > 1)
            {
                param = args[1];
            }

            ICommand command;
            if (!CommandDict.ContainsKey(commandStr) || (command = CommandDict[commandStr]) == null)
            {
                Console.WriteLine("不支持的命令！");
                return;
            }

            command.Execute(param);
        }

        /// <summary>
        /// 初始化工作
        /// </summary>
        protected void Init()
        {
            InitConfig();
            InitCommand();
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        private void InitConfig()
        {
            // 获取当前程序所在目录
            string currentPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            // 解析json配置文件
            string jsonText = "";
            try
            {
                using (StreamReader fileReader = new StreamReader(currentPath + "\\config.json"))
                {
                    while (!fileReader.EndOfStream)
                    {
                        jsonText += fileReader.ReadLine();
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                throw e;
            }

            ConfigDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
        }

        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitCommand()
        {
            CommandDict.Add(JCommandTypeConst.PRINT_ALL_CONFIG, new PrintAllConfigCommand(this));
            CommandDict.Add(JCommandTypeConst.SWITCH_JAVA_ENV, new SwtichJavaEnvCommand(this));
        }
    }
}
