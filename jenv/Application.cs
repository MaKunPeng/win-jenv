using jenv.controller;
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
    /// 
    /// <para>auhor: Aaron Ma</para>
    /// <para>created date: 2020-4-15</para>
    /// </summary>
    public class Application
    {
        static void Main(string[] args)
        {
            // 命令行参数解析
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("请输入命令参数！");
                return;
            }

            JenvShell jShell = null;
            try
            {
                jShell = new JenvShell();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("未找到config.json配置文件，请检查程序目录");
                return;
            }

            jShell.Process(args);
        }
    }
}
