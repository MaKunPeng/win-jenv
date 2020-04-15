using jenv.controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace jenv.command
{
    public abstract class JAbstractCommand : ICommand
    {
        public JenvShell Sender { get; set; }

        public JAbstractCommand(JenvShell sender)
        {
            this.Sender = sender;
        }

        public abstract void Execute(string param);
    }
}
