using System;
using Deployf.Botf;

namespace BotFFlowers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BotfProgram.StartBot(args, onConfigure: (svc, cfg) => { }, onRun: (app, cfg) => { });
        }
    }
}




