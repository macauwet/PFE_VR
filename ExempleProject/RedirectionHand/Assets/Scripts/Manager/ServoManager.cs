using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pololu.UsbWrapper;
using Pololu.Usc;
using System;


public class ServoManager : MonoBehaviour
{
    public ushort maxEngage;
    public ushort maxDisengage;
    public ushort qMinPololu;
    public ushort qMaxPololu;
    public ushort deltaQMax;
    private ushort deltaQ = 0;

    public Usc pololu;

    // Start is called before the first frame update
    void Start()
    {
        this.pololu = this.connectToPololuBoard();
    }

    private void OnApplicationQuit() {
        this.disconnectFromPololuBoard();
    }

    public void setVariation(float rate) {
        if (rate >= 1f) {
            rate = 1f;
        } else if (rate <= -1) {
            rate = -1f;
        }

        this.deltaQ = (ushort)((float)this.deltaQMax * rate);
    }

    public void boolEngage(bool value) {
        if (value) this.trySetTarget(0x00, Convert.ToUInt16(this.maxEngage * 4));
        else this.trySetTarget(0x00, Convert.ToUInt16(this.maxDisengage * 4));
    }

    public void updateServo(float rate) {
        this.trySetTarget(0x00, Convert.ToUInt16(((this.maxDisengage - this.maxEngage) * rate + this.maxEngage + this.deltaQ) * 4));
    }

    // ELODIE
    public void updateAccel(float target)
    {
        this.trySetAccel(0x00, Convert.ToUInt16(target));
    }

    public void updateSpeed(float target)
    {
        this.trySetSpeed(0x00, Convert.ToUInt16(target));
    }


    public void trySetAccel(byte channel, ushort target) {
        try 
        {
            this.pololu.setAcceleration(channel, target);
        }
        catch 
        {
            // Debug.Log("Failed writing Accel to pololu.");
        }
    }

    public void trySetSpeed(byte channel, ushort target) {
        try 
        {
            this.pololu.setSpeed(channel, target);
        }
        catch 
        {
            // Debug.Log("Failed writing Accel to pololu.");
        }
    }

    public float[] tryGetPosition() {
        try 
        {
            MaestroVariables variables;
            ServoStatus[] servos;
            short[] stack;
            ushort[] call_stack;
            float target, speed, acceleration, position;
            this.pololu.getVariables(out variables, out stack, out call_stack, out servos);
            // Console.WriteLine(" #  target   speed   accel     pos");
            // for (int i = 0; i < 1; i++)
            // {
            //     Debug.Log("{0,2}{1,8}{2,8}{3,8}{4,8}" + ";"  + i + ";"  + 
            //         servos[i].target + ";"  +  servos[i].speed + ";"  + 
            //         servos[i].acceleration + ";"  +  servos[i].position);
            // }
            target = servos[0].target;
            speed = servos[0].speed;
            acceleration = servos[0].acceleration;
            position = servos[0].position;
            float[] returnMe = new float[4];
            returnMe[0] = target; //, speed, acceleration, position];
            returnMe[1] = speed;
            returnMe[2] = acceleration;
            returnMe[3] = position;
            return returnMe;
        }
        catch 
        {
            // Debug.Log("Failed writing Pos FROM pololu.");
            float[] returnMe = new float[4];
            return returnMe;
        }
    }
    // Write the current target 1/4µs value to the target channel
    public void trySetTarget(byte channel, ushort target) {
        try {
            this.pololu.setTarget(channel, target);
        } catch {
            //Debug.Log("Failed writing to pololu.");
        }
    }
    public Usc connectToPololuBoard() {
        // Get a list of all connected devices of this type.
        List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();

        foreach (DeviceListItem dli in connectedDevices) {
            Usc device = new Usc(dli);    // Connect to the device.
            return device;                // Return the device.
        }

        Debug.Log("Could not find device. Make sure it is plugged in to USB " +
            "and check your Device Manager (Windows) or run lsusb (Linux).");

        return null;
    }

    public void disconnectFromPololuBoard() {
        try {
            this.pololu.Dispose();
        } catch {
            Debug.Log("Could not disconnect from device. Device may be unreachable.");
        }
    }
}
