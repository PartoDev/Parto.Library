using System;
using Parto.Connection.Controller.Abstractions;

namespace Parto.Connection.Client.Test
{
    [ConnectionPath("test")]
    public class TestController : ConnectionController
    {
        [ConnectionPath("1")]
        public bool Test1(int i) => i % 2 == 0;

        [ConnectionPath("error")]
        public void Error() => Console.WriteLine("error");
    }
}