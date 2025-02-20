namespace AdvantechDemo;

class Program
{
    static void Main()
    {
        Device4761 device = new Device4761();
        device.Start();

        Console.WriteLine("Press any key to stop the device.");
        Console.ReadKey();

        Console.WriteLine("Stopping.");
        device.Stop();
    }
}