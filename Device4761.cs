using Automation.BDaq;

namespace AdvantechDemo;

public class Device4761 : IDisposable
{
    private readonly InstantDiCtrl _instantDiCtrl;
    
    public Device4761()
    {
        _instantDiCtrl = new InstantDiCtrl();
        _instantDiCtrl.SelectedDevice = new DeviceInformation("DemoDevice,BID#0");
        
        // Only react on changes to the first two ports.
        _instantDiCtrl.DiPmintPorts[0].Mask = 0b11000000;
        
        // Do not react on channels on the second port.
        _instantDiCtrl.DiPmintPorts[1].Mask = 0b00000000;
        
        _instantDiCtrl.PatternMatch += InstantDiCtrlOnPatternMatch;
    }
    
    public void Start()
    {
        _instantDiCtrl.SnapStart();
    }

    public void Stop()
    {
        _instantDiCtrl.SnapStop();
    }
    
    private void InstantDiCtrlOnPatternMatch(object? sender, DiSnapEventArgs e)
    {
        byte portData = e.PortData[0];
        
        Console.WriteLine("Value of channel 0 on port 0: {0}", GetBit(portData, 0));
        Console.WriteLine("Value of channel 1 on port 0: {0}", GetBit(portData, 1));
    }
    
    public void Dispose()
    {
        _instantDiCtrl.PatternMatch -= InstantDiCtrlOnPatternMatch;
        _instantDiCtrl.Dispose();
    }
    
    private static int GetBit(byte b, int bitNumber)
    {
        return (b & (1 << bitNumber)) == 0 ? 0 : 1;
    }
}