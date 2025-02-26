 using System.Collections;
 using Automation.BDaq;

namespace AdvantechDemo;

public class Device4761 : IDisposable
{
    private readonly InstantDiCtrl _digitalInputs;
    private readonly InstantDoCtrl _digitalOutputs;
    
    public Device4761()
    {
        const string deviceDescription = "DemoDevice,BID#0";
        
        _digitalInputs = new InstantDiCtrl();
        _digitalInputs.SelectedDevice = new DeviceInformation(deviceDescription);

        // Usually, binary notation is done with least significant bit on the right. However, to tell 
        // the device that I'm only interested in channel 0 and 1, I have to set the mask to 0b11000000.
        // So it looks like the bit notation for the mask property is done with least significant bit on the left.
        // Verify with Advantech if this is the actual designed behaviour
        
        _digitalInputs.DiPmintPorts[0].Mask = 0b11000000;
        // _digitalInputs.DiPmintPorts[0].Mask = 0b00000011;
        
        _digitalInputs.PatternMatch += DigitalInputsOnPatternMatch;

        _digitalOutputs = new InstantDoCtrl();
        _digitalOutputs.SelectedDevice = new DeviceInformation(deviceDescription);
    }
    
    public void Start()
    {
        ErrorCode errorCode = _digitalInputs.SnapStart();
        if (Failed(errorCode))
        {
            Console.WriteLine("Failed to start the device. Error code: {0}", errorCode);
        }
    }

    public void Stop()
    {
        ErrorCode errorCode = _digitalInputs.SnapStop();
        if (Failed(errorCode))
        {
            Console.WriteLine($"Failed to stop the device. Error code: {errorCode}");
        }
    }
    
    private void DigitalInputsOnPatternMatch(object? sender, DiSnapEventArgs e)
    {
        (bool DropPressed, bool OverridePressed) buttonTuple = (GetBit(e.PortData[0], 0), GetBit(e.PortData[0], 1));
        Console.WriteLine("\nData:\t {0}", e.PortData[0]);
        Console.WriteLine("Binary:\t {0}", Convert.ToString(e.PortData[0], 2).PadLeft(8, '0'));
        switch (buttonTuple)
        {
            case {DropPressed: true, OverridePressed: true}:
                Console.WriteLine("DROP + OVERRIDE buttons pressed.");
                break;
            case {DropPressed: true, OverridePressed: false}:
                Console.WriteLine("Only DROP button pressed.");
                break;
            case {DropPressed: false, OverridePressed: true}:
                Console.WriteLine("Only OVERRIDE button pressed.");
                break;
        }
    }
    
    public void Dispose()
    {
        _digitalInputs.PatternMatch -= DigitalInputsOnPatternMatch;
        _digitalInputs.Dispose();
    }
    
    private static bool GetBit(byte b, int bitNumber)
    {
        return new BitArray([b]).Get(bitNumber);
    }
    
    private bool Failed(ErrorCode err)
    {
        return err is < ErrorCode.Success and >= ErrorCode.ErrorHandleNotValid;
    }    
}