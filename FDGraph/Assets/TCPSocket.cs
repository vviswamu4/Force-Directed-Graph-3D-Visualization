using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking;

namespace FDGraph.Assets
{
    public class TCPSocket
    {
        private StreamSocketListener listener;
        //HostName host = new HostName("192.168.253.100");
        int port = 7475;
       
        public void Run()
        {
            try
            {
                listener = new StreamSocketListener();
                listener.Control.QualityOfService = SocketQualityOfService.LowLatency;
                listener.ConnectionReceived += onConnection;
                listener.Control.KeepAlive = true;
                connect(port);
            }
            catch(Exception e)
            {
                Debug.WriteLine("There's an error! " + e);
            }
            
        }

        private async void connect(int port)
        {
            try
            {
                await listener.BindServiceNameAsync(port.ToString());
                //await listener.BindEndpointAsync(host,port.ToString());
                Debug.WriteLine("Listening on port " + port);
            }
            catch (Exception e)
            {
                listener.Dispose();
                Debug.WriteLine(e.ToString());
            }
        }

        private async void onConnection(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Debug.WriteLine("Starting data read");
            DataReader reader = new DataReader(args.Socket.InputStream);
            try
            {
                await listen(reader);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Ending Connection!");
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
                DataReaderLoadOperation loadOp1 = reader.LoadAsync(1);
                await loadOp1;
                DataReaderLoadOperation loadOp = reader.LoadAsync(8);
                await loadOp;
                if (loadOp.Status == Windows.Foundation.AsyncStatus.Completed)
                {
                    await getLength(reader);
                    return;
                }
            }
        }

        private async Task getLength(DataReader reader)
        {
            string msgLength = reader.ReadString(8);
            int length = Int32.Parse(msgLength);
            Debug.WriteLine(length);
            await getData(length, reader);
        }

        private async Task getData(int length, DataReader reader)
        {
            //await saveTime();
            while (true)
            {
                DataReaderLoadOperation loadOp = reader.LoadAsync((uint)length);
                await loadOp;
                if (loadOp.Status == Windows.Foundation.AsyncStatus.Completed)
                {
                    string data = reader.ReadString((uint)length);
                    Debug.WriteLine(data);
                    //return;
                }
                /*try
                {
                    await listen(reader);
                }
                catch(Exception e)
                {
                    Debug.WriteLine("Came here!");
                    listener.Dispose();
                }*/
                
            }
            
        }


    }
}
