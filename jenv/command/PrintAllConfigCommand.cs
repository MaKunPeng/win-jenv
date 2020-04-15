using jenv.controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace jenv.command
{
    /// <summary>
    /// 打印所有配置
    /// </summary>
    public class PrintAllConfigCommand : JAbstractCommand
    {
        public PrintAllConfigCommand(JenvShell sender) : base(sender)
        {
        }

        public override void Execute(string msg)
        {
            foreach (KeyValuePair<string, string> kv in Sender.ConfigDict)
            {
                Console.WriteLine(kv.Key + " : " + kv.Value);
            }
        }
    }
}
