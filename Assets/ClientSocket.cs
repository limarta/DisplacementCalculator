using System;
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
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
#endif

public class ClientSocket : MonoBehaviour
{
    private StreamWriter writer;
    private bool threadStarted = false;
    private int retryState = -1;
    private string exceptionMsg = "No problem";
    private TMP_Text textComp;
    [SerializeField] private string IP;
    [SerializeField] private string port;
    [SerializeField] private bool retryConnection;

    public StreamWriter Writer => writer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("(IP, port) = (" + IP + ", " + port + ")");
        textComp = gameObject.GetComponent<TMP_Text>();
        Thread t = new Thread(new ThreadStart(ConnectToServer));
        t.Start();
    }


    private async void ConnectToServer()
    {
#if UNITY_EDITOR
        Debug.Log("In Unity Editor!");
#endif
#if !UNITY_EDITOR
        StreamSocket clientSocket = new StreamSocket();
        threadStarted = true;

        try {
            if(retryConnection) {
                retryState = 1;
                var cts = new CancellationTokenSource();
                cts.CancelAfter(5000);
                
                var connectAsync = clientSocket.ConnectAsync(new HostName(IP), port);
                var connectTask = connectAsync.AsTask(cts.Token);
                await connectTask;
                writer = new StreamWriter(clientSocket.OutputStream.AsStreamForWrite());
            }
            else {
                retryState = 0;
                await clientSocket.ConnectAsync(new HostName(IP), port);
                writer = new StreamWriter(clientSocket.OutputStream.AsStreamForWrite());
            }
            
            if(writer == null) {
                throw new NullReferenceException("Writer not established despite connecting");
            }
            writer.AutoFlush = true;
            while (true) {}
        }
        catch(Exception exception) {
            exceptionMsg = "Error thrown: " + exception.Message + ". Now retrying.";
            Debug.Log("Error thrown: " + exception.Message + ". Now retrying.");
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if(writer != null)
        {
            textComp.text = "Writer established\nThreadStarted=" + threadStarted + "\nretryState=" + retryState + "\n" + exceptionMsg;
            Debug.Log("Writer established");
        }
        else
        {
            textComp.text = "Writer NOT established\nThreadStarted=" + threadStarted + "\nretryState=" + retryState + "\n" + exceptionMsg;
            Debug.Log("Writer not established");
        }


    }
}
