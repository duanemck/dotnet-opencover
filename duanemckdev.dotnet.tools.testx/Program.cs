using System;
using CommandLine;


namespace duanemckdev.dotnet.tools.testx
{
    public class Program
    {
        static int Main(string[] args)
        {
            try
            {
                return Parser.Default.ParseArguments<Options>(args)
                    .MapResult(options => new TestXRunner(options).Run(), (errors) => 1);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return 1;
            }
        }


    }
}
