using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using System.Globalization;


public class ContactManager : MonoBehaviour {
    public string arduinoCOM;
    public bool looping = true;
    public bool boardConnected;

    private SerialPort stream;
    private Thread thread;

    float floatReceived;
    public float fsrValue, touchSensingValue;

    private string[] dataSplit;

    public bool IsLooping() {
        lock (this) {
            return this.looping;
        }
    }

    private void Start() {
        //Run communication thread
        this.thread = new Thread(this.threadLoop);
        this.thread.Start();
    }

    public void threadLoop() {
        this.connectToArduino();

        // Looping
        while (this.IsLooping()) {
            
            //Don't know why, but we have to write a character to the arduino serial port to be able to read it afterwise...
            this.writeToArduino("a");

            // Read from Arduino
            string result = this.readFromArduino();

            if (result != null) {
                try{
                    this.fsrValue = float.Parse(result);
                    this.stream.DiscardInBuffer();
                }
                catch (Exception e)
                {
                    Debug.LogError("DataSplit error: " + e.Message);
                    this.fsrValue = 0;
                    // throw;
                }
                               
       //       this.fsrValue = float.Parse(dataSplit[0], CultureInfo.InvariantCulture);
                // this.touchSensingValue = float.Parse(dataSplit[1], CultureInfo.InvariantCulture);
                // this.fsrValue = float.Parse(result);
                this.stream.DiscardInBuffer();
            }
        }

        Debug.Log("stream closing");
        this.stream.Close();
    }

    public void connectToArduino() {
        if (int.Parse(this.arduinoCOM) >= 0) {
            this.stream = new SerialPort("COM" + this.arduinoCOM, 115200);
            this.stream.ReadBufferSize = 8192;
            this.stream.WriteBufferSize = 128;
            this.stream.ReadTimeout = 500;

            this.stream.Parity = Parity.None;
            this.stream.StopBits = StopBits.One;

            try {
                this.stream.Open();
                Debug.Log("Communication port with Arduino is ready.");
                boardConnected = true;
            } catch (Exception e) {
                Debug.LogError(e.Message);
                boardConnected = false;
                throw;
            }

        } else {
            Debug.Log("Unable to open communication port with Arduino.");
        }
    }

    public string readFromArduino(int timeout = 50) {
        try {
            return this.stream.ReadLine();
        } catch (TimeoutException e) {
            Debug.Log("TimeoutError in read: " + e.Message);

            // throw;
            return null;
        } catch (Exception e) {
            Debug.Log("Read error: " + e.Message);
            // throw;
            return null;
        }
    }

    public void writeToArduino(string message) {
        if (this.stream != null) {
            this.stream.WriteLine(message);
            this.stream.BaseStream.Flush();
        }
    }

    public void stopThread() {
        lock (this) {
            this.looping = false;
        }
    }

    private void OnApplicationQuit() {
        this.stopThread();
    }
}
