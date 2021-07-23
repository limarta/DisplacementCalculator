using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using UnityEngine;
using System.Threading;
using UnityEngine.Rendering;

#if !UNITY_EDITOR
using Windows.Networking;
using Windows.Networking.Sockets;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
#endif

public class UwpTest : MonoBehaviour
{
    private StreamWriter writer;
    private int updateCount;
    private TMP_Text m_TextComponent;
    // Start is called before the first frame update
    void Start()
    {
        updateCount = 0;
        Debug.Log("Starting thread");
        Thread t = new Thread(new ThreadStart(ConnectSocket));
        t.Start();
    }

    private async void ConnectSocket()
    {
#if UNITY_EDITOR
        Debug.Log("In Unity Editor!");
        writer = null;
#endif
#if !UNITY_EDITOR
        StreamSocket clientSocket = new StreamSocket();
        string MyLaptop = "18.27.124.131";
        await clientSocket.ConnectAsync(new HostName(MyLaptop), "8080");
        writer = new StreamWriter(clientSocket.OutputStream.AsStreamForWrite());
        writer.AutoFlush = true;
        while (true) {
            
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.transform.position;
        m_TextComponent = gameObject.GetComponent<TMP_Text>();
        m_TextComponent.text = "State of writer: " + writer + "\n Camera pos" + 
                                pos +
                               "\nNumber of updates: " + updateCount;
        if(writer != null)
        {
            writer.WriteLine(pos);
            updateCount += 1;
        }
        else
        {
            Debug.Log("Writer not yet established");
        }
        
    }
}