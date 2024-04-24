using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rug.Osc;
using System;
using System.Threading;

public class BCIManager : MonoBehaviour
{
    static OscReceiver receiver;
    static Thread recevingThread;
    public int port = 8000;
    OscAddressManager listener;

    // Start is called before the first frame update
    void Start()
    {
        listener = new OscAddressManager();
        listener.Attach("/met/att", SetPerformanceScore);
        listener.Attach("/met/eng", SetPerformanceScore);
        listener.Attach("/met/exc", SetPerformanceScore);
        listener.Attach("/met/int", SetPerformanceScore);
        listener.Attach("/met/rel", SetPerformanceScore);
        listener.Attach("/met/str", SetPerformanceScore);

        receiver = new OscReceiver(port);
        receiver.Connect();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (recevingThread == null)
        {
            ListenForPacket();
        }
        else if (!recevingThread.IsAlive)
        {
            recevingThread.Join();
            recevingThread = null;
            ListenForPacket();
        }
    }

    void ListenForPacket()
    {
        if (recevingThread == null)
        {
            
            recevingThread = new Thread(new ThreadStart(GetOSCPacket));
            recevingThread.Start();
        } 
        else
        {
            Debug.LogError("Trying to create a new thread when one already exists.");
        }
    }

    void GetOSCPacket()
    {
        try
        {
            while (receiver.State != OscSocketState.Closed)
            {
                // if we are in a state to recieve
                if (receiver.State == OscSocketState.Connected)
                {
                    // get the next message 
                    // this will block until one arrives or the socket is closed
                    OscPacket packet = receiver.Receive();

                    switch (listener.ShouldInvoke(packet))
                    {
                        case OscPacketInvokeAction.Invoke:
                            Console.WriteLine("Received packet");
                            listener.Invoke(packet);
                            break;
                        case OscPacketInvokeAction.DontInvoke:
                            Console.WriteLine("Cannot invoke");
                            Console.WriteLine(packet.ToString());
                            break;
                        case OscPacketInvokeAction.HasError:
                            Console.WriteLine("Error reading osc packet, " + packet.Error);
                            Console.WriteLine(packet.ErrorMessage);
                            break;
                        case OscPacketInvokeAction.Pospone:
                            Console.WriteLine("Posponed bundle");
                            Console.WriteLine(packet.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // if the socket was connected when this happens
            // then tell the user
            if (receiver.State == OscSocketState.Connected)
            {
                Console.WriteLine("Exception in listen loop");
                Console.WriteLine(ex.Message);
            }
        }
    }

    void SetPerformanceScore(OscMessage message)
    {
        switch(message.Address)
        {
            case "/met/att":
                StaticGameManager.MainManager.attentionLevel = (float)message[0];
                break;
            case "/met/eng":
                StaticGameManager.MainManager.engagementLevel = (float)message[0];
                break;
            case "/met/exc":
                StaticGameManager.MainManager.excitementLevel = (float)message[0];
                break;
            case "/met/int":
                StaticGameManager.MainManager.interestLevel = (float)message[0];
                break;
            case "/met/rel":
                StaticGameManager.MainManager.relaxationLevel = (float)message[0];
                break;
            case "/met/str":
                StaticGameManager.MainManager.stressLevel = (float)message[0];
                break;
            default:
                Debug.LogError(message.Address + " is not a performance metric");
                break;
        }
    }

    
    
}

