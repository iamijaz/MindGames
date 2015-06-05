using System;
using Quartz;

namespace QuartzSpike
{
    internal class HelloJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Greetings from HelloJob!");
        }
    }
}

