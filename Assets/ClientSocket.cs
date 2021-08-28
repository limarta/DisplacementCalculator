using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using System.IO;
using UnityEngine;
using System.Threading;
using UnityEngine.Rendering;
using System.Threading.Tasks;

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
    private int retryState = -1;
    private string exceptionMsg = "No problem";
    private string readMsg = "Nothing yet";
    private TMP_Text textComp;

    private StreamWriter writer;
    private StreamReader reader;
    private ConcurrentQueue<String> writerQueue;
    private ConcurrentQueue<String> readerQueue;
    [SerializeField] private string ip;
    [SerializeField] private string port;
    [SerializeField] private bool retryConnection;

    public string IP => ip; 
    public StreamWriter Writer => writer;
    public StreamReader Reader => reader;
    public ConcurrentQueue<String> WriterQueue => writerQueue;
    public ConcurrentQueue<String> ReaderQueue => readerQueue;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("(ip, port) = (" + ip + ", " + port + ")");
        textComp = gameObject.GetComponent<TMP_Text>();
        writerQueue = new ConcurrentQueue<string>();
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

        try {
            if(retryConnection) {
                retryState = 1;
                var cts = new CancellationTokenSource();
                cts.CancelAfter(5000);
                
                var connectAsync = clientSocket.ConnectAsync(new HostName(ip), port);
                var connectTask = connectAsync.AsTask(cts.Token);
                await connectTask;
                writer = new StreamWriter(clientSocket.OutputStream.AsStreamForWrite());
            }
            else {
                retryState = 0;
                await clientSocket.ConnectAsync(new HostName(ip), port);
                writer = new StreamWriter(clientSocket.OutputStream.AsStreamForWrite());
                reader = new StreamReader(clientSocket.InputStream.AsStreamForRead());
            }
            
            if(writer == null) {
                throw new NullReferenceException("Writer not established despite connected");
            }
            else if(reader == null) {
                throw new NullReferenceException("Reader not established despite connected");
            }
            writer.AutoFlush = true;


            Thread t1 = new Thread(new ThreadStart(Read));
            t1.Start();
            while (true) {

                if(!writerQueue.IsEmpty) {
                    string output;
                    writerQueue.TryDequeue(out output);
                    writer.Write(output);
                }
            }
            t1.Join();
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
        string text = "retryState=" + retryState + "\n" + exceptionMsg;
        if(writer != null && reader != null)
        {
            textComp.text = "Streams established\nRead message: " + readMsg + "\n" + text + DateTimeOffset.Now.ToUnixTimeSeconds();
            Debug.Log("Writer established ");
        }
        else if(writer == null || reader == null)
        {
            if(writer == null)
            {
                text = "Writer not established\n" + text;
                Debug.Log("Writer not established");
            }
            if(reader == null)
            {
                text = "Reader not established\n" + text;
                Debug.Log("Reader not established");
            }
            textComp.text = text + "\n" + DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
    private async void Read()
    {
        while (true)
        {
            char[] size_arr = new char[8];
            await reader.ReadAsync(size_arr, 0, 8);
            readMsg = new string(size_arr);
        } 
    }
  }
