using System;
using System.Collections.Generic;
using System.Text;

namespace jenv.command
{
    /// <summary>
    /// 命令接口，封装命令行为，统一接口规范。
    /// <para>author: Aaron ma</para>
    /// </summary>
    public interface ICommand
    {
        void Execute(string param);
    }
}
