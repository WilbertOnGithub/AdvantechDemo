namespace AdvantechDemo;

class Program
{
    static async Task Main()
    {
        Device4761 device = new Device4761();
        device.Start();

        Console.WriteLine("Press any key to stop the device.");
        
        while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
        {
            // do something
        }        
        device.Stop();
    }
}