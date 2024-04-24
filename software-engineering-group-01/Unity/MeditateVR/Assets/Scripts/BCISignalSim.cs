using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Rug.Osc;
using System.Threading;

public class BCISignalSim : MonoBehaviour
{
    int port = 8000;
    IPAddress address;
    private Thread sendThread;
    
    public float attentionLevel = 0.5f;
    public float engagementLevel = 0.5f;
    public float excitementLevel = 0.5f;
    public float interestLevel = 0.5f;
    public float relaxationLevel = 0.5f;
    public float stressLevel = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        address = IPAddress.Parse("127.0.0.1");
    }

    // Update is called once per frame
    void Update()
    {
        if (sendThread == null)
        {
            
            StartThread();
        }
        else if (!sendThread.IsAlive)
        {
            sendThread.Join();
            sendThread = null;
            StartThread();
        }
    }

    void StartThread()
    {
        sendThread = new Thread(SendSignal);
        sendThread.Start();
    }
    
    void SendSignal()
    {
        using (OscSender sender = new OscSender(address, port))
        {
            sender.Connect();
            sender.Send(new OscMessage("/met/att", attentionLevel));
            Thread.Sleep(1000);
            sender.Send(new OscMessage("/met/eng", engagementLevel));
            Thread.Sleep(1000);
            sender.Send(new OscMessage("/met/exc", excitementLevel));
            Thread.Sleep(1000);
            sender.Send(new OscMessage("/met/int", interestLevel));
            Thread.Sleep(1000);
            sender.Send(new OscMessage("/met/rel", relaxationLevel));
            Thread.Sleep(1000);
            sender.Send(new OscMessage("/met/str", stressLevel));
            Thread.Sleep(1000);

        }
    }
}
