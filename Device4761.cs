 using Automation.BDaq;

namespace AdvantechDemo;

public class Device4761 : IDisposable
{
    private readonly InstantDiCtrl _instantDiCtrl;
    
    public Device4761()
    {
        _instantDiCtrl = new InstantDiCtrl();
        _instantDiCtrl.SelectedDevice = new DeviceInformation("DemoDevice,BID#0");
        
        // Only react on changes to the first two channels, represented by the first two bits.
        _instantDiCtrl.DiPmintPorts[0].Mask = 0b11000000;
        
        // Do not react on channels on the second port.
        _instantDiCtrl.DiPmintPorts[1].Mask = 0b00000000;
        
        _instantDiCtrl.PatternMatch += InstantDiCtrlOnPatternMatch;
    }
    
    public void Start()
    {
        ErrorCode errorCode = _instantDiCtrl.SnapStart();
        if (Failed(errorCode))
        {
            Console.WriteLine("Failed to start the device. Error code: {0}", errorCode);
        }
    }

    public void Stop()
    {
        ErrorCode errorCode = _instantDiCtrl.SnapStop();
        if (Failed(errorCode))
        {
            Console.WriteLine("Failed to stop the device. Error code: {0}", errorCode);
        }
    }
    
    private void InstantDiCtrlOnPatternMatch(object? sender, DiSnapEventArgs e)
    {
        (int DropButton, int OverrideButton) buttonTuple = (GetBit(e.PortData[0], 0), GetBit(e.PortData[0], 1));
        Console.WriteLine("\nData:\t {0}", e.PortData[0]);
        Console.WriteLine("Binary:\t {0}", Convert.ToString(e.PortData[0], 2).PadLeft(8, '0'));
        switch (buttonTuple)
        {
            case {DropButton: 1, OverrideButton: 1}:
                Console.WriteLine("DROP + OVERRIDE buttons pressed.");
                break;
            case {DropButton: 1, OverrideButton: 0}:
                Console.WriteLine("Only DROP button pressed.");
                break;
            case {DropButton: 0, OverrideButton: 1}:
                Console.WriteLine("Only OVERRIDE button pressed.");
                break;
        }
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
    
    private bool Failed(ErrorCode err)
    {
        return err is < ErrorCode.Success and >= ErrorCode.ErrorHandleNotValid;
    }    
}