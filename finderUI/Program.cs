namespace finderUI;

class Program
{
    static void Main(string[] args)
    {

        Console.WriteLine("Hello, World!");
        Console.WriteLine(Environment.CurrentDirectory);
        Console.WriteLine(Environment.OSVersion.VersionString);
        Console.WriteLine("Namespace: {0}", typeof(Program).Namespace);
    }
}
