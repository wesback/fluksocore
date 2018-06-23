using System;

namespace FluksoCore
{
    
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CoreEngine.GetMessages().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
