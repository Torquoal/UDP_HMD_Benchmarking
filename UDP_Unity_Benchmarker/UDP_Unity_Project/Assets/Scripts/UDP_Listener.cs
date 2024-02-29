using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using System.Threading;

public class UDP_Listener : MonoBehaviour
{
    protected UdpClient Client;
    protected IPEndPoint RemoteIpEndPoint;
    private float lastReceiveTime, timeSinceLastPacket, count, average, total, outliers, outlier_value = 0;
    public static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    protected bool running = true;
    Thread m_ListeningThread;
    public String debug_string;
    private float outlier_threshold = 50;
    

    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("UDP Listener Starting");
        stopwatch.Start();
        Client = new UdpClient(9000);
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 9000);
        Start_Listening();

    }

    void Update()
    {
 
    }

    void Start_Listening()
    {
        running = true;
        stopwatch.Start();
        m_ListeningThread = new Thread(ListenForUDPPackages);
        m_ListeningThread.Priority = System.Threading.ThreadPriority.AboveNormal;
        m_ListeningThread.Start();
        Debug.Log("Starts listening...");
    }

    public void ListenForUDPPackages()
    {
        try
        {
            while (this.running)
            {
                byte[] bytes = Client.Receive(ref RemoteIpEndPoint);
                //OnBytesUpdate(bytes);
                if (lastReceiveTime > 0)
                    timeSinceLastPacket = stopwatch.ElapsedMilliseconds - lastReceiveTime;
                lastReceiveTime = stopwatch.ElapsedMilliseconds;
                //Debug.Log("UDP Listener recieved data. Interval: " + timeSinceLastPacket + " Contents: " + Encoding.Default.GetString(bytes));
                count = count + 1;
                total = total + timeSinceLastPacket;
                average = total / count;

                if (timeSinceLastPacket > outlier_threshold)
                {
                    outliers = outliers + 1;
                    outlier_value = timeSinceLastPacket;
                }

                debug_string = ("Interval: " + timeSinceLastPacket + "ms \nAverage: " + Math.Round(average, 2) + "ms \nOutliers: " + outliers + "\nLast Outlier: " + outlier_value + "ms");
                Debug.Log(debug_string);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("UDP thread listener exception");
            Debug.Log("UDP thread listener exception");
        }
        finally
        {
            Client.Close();
        }
    }

}
