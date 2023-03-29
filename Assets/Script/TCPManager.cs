using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using System.IO;
using TMPro;
using System.Threading;

public class TCPManager : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawnTarget;
    public TextMeshProUGUI details;
    public TMP_InputField msg;

    public string serverIP = "127.0.0.1";
    public int port = 9999;

    TcpClient client;    
    byte[] receivedBuffer;
    StreamReader reader;
    bool socketReady = false;
    NetworkStream stream;

    public static TCPManager instance;
    public bool canSend = false;
    public Queue<byte[]> packetQueue = new Queue<byte[]>();


    void Awake()
    {
        var obj = FindObjectsOfType<TCPManager>();

        if(obj.Length == 1)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        instance = this;
        
        StartCoroutine("CheckReceive");
        // StartCoroutine("Debugger");
    }

    // Connect To Server
    IEnumerator CheckReceive()
    {
        if (socketReady) yield break;
        try
        {
            client = new TcpClient(serverIP, port);
            client.NoDelay = true;
            

            if (client.Connected)
            {                
                stream = client.GetStream();
                stream.ReadTimeout = 2000;

                Debug.Log("Connect Success");

                if(LoadingAnim.instance != null)
                    LoadingAnim.instance.stopLoading();
                
                socketReady = true;
            }   
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    public void clearBuffer()
    {
        byte[] buffer = new byte[1024];
        while (stream.CanRead && stream.DataAvailable)
        {
            stream.ReadTimeout = 2000;
            // 데이터를 읽고 무시
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            Debug.Log("버퍼 싹 비우기 실행 ");
        }

        canRecv = true;

        System.GC.Collect();
    }
    

    private void Update() {
        if (socketReady)
        {
            // 패킷을 받아 큐에 넣음
            string msg = ReadPacket();
        
            if(msg != null)
            {
                if(msg.Length > 10000)
                {
                    Debug.Log("넘어간다");
                    return;
                }
                
                if (msg != "" || msg != null || !string.IsNullOrEmpty(msg) || !string.IsNullOrWhiteSpace(msg))
                {
                    try{
                        SocketHandler.instance.Handle_Json_Event(msg);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error : " + e);
                    }
                }
            }
        }
    }

    IEnumerator ReadNext()
    {
        if (socketReady && stream.DataAvailable)
        {
            stream.ReadTimeout = 2000;

            try
            {
                var len_byte = new byte[4];
                stream.Read(len_byte, 0, 4);
                
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(len_byte);

                int len_int = BitConverter.ToInt32(len_byte,0);

                stream.ReadTimeout = 2000;
                var data = new byte[len_int];
                stream.Read(data, 0, len_int); // stream에 있던 바이트배열 내려서 새로 선언한 바이트배열에 넣기

                string msg = Encoding.UTF8.GetString(data, 0, len_int); // byte[] to string
                
                if(msg != "")
                {
                    // Debug.Log("Received from the server : '" + msg + "'");

                    SocketHandler.instance.Handle_Json_Event(msg);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Read timed out: {0}", ex.Message);
                // 예외 처리 코드
            }
            
        }

        yield return null;
    }

    public bool canRecv = false;

    IEnumerator clearPacket()
    {
        canRecv = false;
        
        yield return new WaitForSeconds(1f);
    }

    public byte[] GetPacket()
    {
        Debug.Log("GetPacket is call");
        byte[] packet = null;
        lock (packetQueue)
        {
            if (packetQueue.Count > 0)
            {
                packet = packetQueue.Dequeue();
            }
        }

        if(packet != null)
        {
            string msg = Encoding.UTF8.GetString(packet, 0, packet.Length); // byte[] to string
            
            if(msg != "")
            {
                SocketHandler.instance.Handle_Json_Event(msg);
            }
        }
        return packet;
    }

    private string ReadPacket()
    {
        if (stream.DataAvailable)
        {
            try
            {
                stream.ReadTimeout = 2000;
                var len_byte = new byte[4];
                stream.Read(len_byte, 0, 4);
                
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(len_byte);

                int len_int = BitConverter.ToInt32(len_byte,0);

                if(len_int > 300)
                {
                    clearBuffer();
                    Debug.Log("오류나는 패킷은 너굴맨이 처리했다고");
                    return null;
                }

                stream.ReadTimeout = 2000;
                var data = new byte[len_int];
                stream.Read(data, 0, len_int); // stream에 있던 바이트배열 내려서 새로 선언한 바이트배열에 넣기

                return Encoding.UTF8.GetString(data, 0, len_int); // byte[] to string
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
       
        return null;
    }

    public IEnumerator SendMsgRoutine(string message)
    {
       string str = message;

        if(str == "quit")
        {
            CloseSocket();
            yield break;;
        }

        string length = str.Length.ToString();

        byte[] len = Encoding.UTF8.GetBytes(length);
        stream.Write(len,0,len.Length);

        byte[] txt = Encoding.UTF8.GetBytes(str);
        stream.Write(txt,0,txt.Length);

        // Debug.Log("Send : " + Encoding.UTF8.GetString(txt, 0, str.Length).ToString());
    }

    public void SendMsg(string message = "")
    {
        StartCoroutine("SendMsgRoutine",message);
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if(this.client != null)
        {
            this.client.Close();
            this.client = null;
        }

        Debug.Log("Disconnect");
    }
}