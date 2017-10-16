#if WINDOWS_UDP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking;
using UnityEngine;

namespace Assets
{
    public class Person
    {
        public string name;
        public string group;
        public float x;
        public float y;
        public float z;

       
    }

    public class Link
    {
        public int from;
        public int to;
        public float weight;
    }

    
    public class TCPSocket
    {
        private StreamSocketListener listener;
        //HostName host = new HostName("192.168.253.100");
        int port = 7475;
        public List<Person> persons = new List<Person>();
        public List<Link> links = new List<Link>();
       
        public void Run()
        {
            try
            {
                UnityEngine.Debug.Log("In here!!");
                listener = new StreamSocketListener();
                listener.Control.QualityOfService = SocketQualityOfService.LowLatency;
                listener.ConnectionReceived += onConnection;
                listener.Control.KeepAlive = true;
                connect(port);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There's an error! " + e);
            }
            
        }

        private async void connect(int port)
        {
            try
            {
                await listener.BindServiceNameAsync(port.ToString());
                //await listener.BindEndpointAsync(host,port.ToString
                UnityEngine.Debug.Log("Listening on port" + port);
                System.Diagnostics.Debug.WriteLine("Listening on port " + port);
            }
            catch (Exception e)
            {
                listener.Dispose();
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        private async void onConnection(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Starting data read");
            DataReader reader = new DataReader(args.Socket.InputStream);
            try
            {
                await listen(reader);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Ending Connection!");
                listener.Dispose();
            }
            
            while (true)
            {

            }
        }

        private async Task listen(DataReader reader)
        {
            //await saveTime();
            while (true)
            {
                DataReaderLoadOperation loadOp = reader.LoadAsync(1);
                await loadOp;
                if (loadOp.Status == Windows.Foundation.AsyncStatus.Completed)
                {
                    string type = getType(reader);
                    System.Diagnostics.Debug.WriteLine("Type     " + type);
                    loadOp = reader.LoadAsync(8);
                    await loadOp;
                    if (loadOp.Status == Windows.Foundation.AsyncStatus.Completed)
                    {
                        await getLength(reader, type);
                        return;
                    }
                    //return;
                }
                /*DataReaderLoadOperation loadOp = reader.LoadAsync(8);
                await loadOp;
                if (loadOp.Status == Windows.Foundation.AsyncStatus.Completed)
                {
                    await getLength(reader);
                    return;
                }*/
            }
        }

        string getType(DataReader reader)
        {
            string type = reader.ReadString(1);
            return type;
        }

        private async Task getLength(DataReader reader, String type)
        {
            string msgLength = reader.ReadString(8);
            int length = Int32.Parse(msgLength);
            System.Diagnostics.Debug.WriteLine("Length   "+length);
            await getData(length, reader, type);
        }

        private async Task getData(int length, DataReader reader, String type)
        {
            //await saveTime();
            while (true)
            {
                DataReaderLoadOperation loadOp = reader.LoadAsync((uint)length);
                await loadOp;
                if (loadOp.Status == Windows.Foundation.AsyncStatus.Completed)
                {
                    string data = reader.ReadString((uint)length);
                    System.Diagnostics.Debug.WriteLine(data);
                    if(type == "n")
                    {
                        String[] names = data.Split(';');
                        
                        for(int i = 0; i < names.Length - 1; i+=2)
                        {
                            Person p = new Person();
                            p.name = names[i];
                            p.group = names[i + 1];
                            p.x = 0;
                            p.y = 0;
                            p.z = 0;
                            persons.Add(p);
                        }
                        //Debug
                        /*
                        foreach (Person p in persons)
                        {
                            Debug.WriteLine(p.name);
                            Debug.WriteLine(p.group);
                            Debug.WriteLine(p.x);
                            Debug.WriteLine(p.y);
                            Debug.WriteLine(p.z);
                        }
                        */
                    }
                    if(type == "l")
                    {
                        String[] linkStr = data.Split(';');
       
                        for (int i = 0; i < linkStr.Length - 2; i += 3)
                        {
                            Link l = new Link();
                            l.from = Int32.Parse(linkStr[i]);
                            l.to = Int32.Parse(linkStr[i + 1]);
                            l.weight = float.Parse(linkStr[i + 2]);
                            links.Add(l);  
                        }
                        //Debug
                        /*
                        foreach(Link l in links)
                        {
                            Debug.WriteLine(l.from);
                            Debug.WriteLine(l.to);
                            Debug.WriteLine(l.weight);
                        }
                        */         
                    }
                    if(type == "p")
                    {
                        
                        String[] posStr = data.Split(';');

                        for (int i = 0; i< persons.Count; i++)
                        {
                            persons[i].x = float.Parse(posStr[i]);
                            persons[i].y = float.Parse(posStr[i + 1]);
                            persons[i].z = float.Parse(posStr[i + 2]);
                        }
                        //Debug
                        
                        foreach(Person p in persons)
                        {
                            System.Diagnostics.Debug.WriteLine(p.name);
                            System.Diagnostics.Debug.WriteLine(p.group);
                            System.Diagnostics.Debug.WriteLine(p.x);
                            System.Diagnostics.Debug.WriteLine(p.y);
                            System.Diagnostics.Debug.WriteLine(p.z);
                        }
                        
                        
                    }
                    //return;
                }
                try
                {
                    await listen(reader);
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Came here!");
                    listener.Dispose();
                }
                
            }
            
        }


    }
}
#endif