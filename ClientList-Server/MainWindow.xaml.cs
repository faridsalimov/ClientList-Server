using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientList_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket Socket { get; set; }
        public List<string> clients = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            var ipAddress = IPAddress.Parse("192.168.0.106");
            var port = 22003;

            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
            Socket.Bind(endPoint);
            Socket.Listen(10);

            Task.Run(() =>
            {
                ListenClients();
            });
        }

        private void ListenClients()
        {
            while (true)
            {
                Socket clientSocket = Socket.Accept();
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(clientSocket);
            }
        }

        private void HandleClient(object clientObj)
        {
            Socket clientSocket = (Socket)clientObj;
            byte[] buffer = new byte[1024];
            int bytesRead = clientSocket.Receive(buffer);
            string name = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            Application.Current.Dispatcher.Invoke(() =>
            {
                clients.Add(name);
                UpdateListView();
            });

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        private void UpdateListView()
        {
            ClientsListView.Items.Clear();
            foreach (string name in clients)
            {
                ClientsListView.Items.Add(name);
            }
        }
    }
}
