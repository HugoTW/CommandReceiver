using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
using System.Timers;
using System.Runtime.InteropServices;
using System.Threading;

namespace Receiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
     public  partial class MainWindow : Window
    {
        Dictionary<string, string> arguments = new Dictionary<string, string>();

        const int MMF_MAX_SIZE = 1024;  // allocated memory for this memory mapped file (bytes)
        const int MMF_VIEW_SIZE = 1024; // how many bytes of the allocated memory can this process access

        // creates the memory mapped file which allows 'Reading' and 'Writing'
        MemoryMappedFile mmf;
        // creates a stream for this process, which allows it to write data from offset 0 to 1024 (whole memory)
        MemoryMappedViewStream mmvStream;

        // timer for read memory mapped file
        private static System.Timers.Timer aReaderTimer;
        public string perviousCommandMsg;


        public MainWindow()
        {

            InitializeComponent();
            InitializeMappedReaderTimer();

        }

      

        private void InitializeMappedReaderTimer()
        {
            // Create a timer with a 0.5 second interval.
            aReaderTimer = new System.Timers.Timer(500);
            // Hook up the Elapsed event for the timer. 
            aReaderTimer.Elapsed += OnTimedEvent;
            aReaderTimer.AutoReset = true;
            aReaderTimer.Enabled = true;

        }


        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            ReadSharedMemory(this, null);
        }

        private void ReadSharedMemory(object sender, RoutedEventArgs e)
        {
            try
            {
                mmf = MemoryMappedFile.OpenExisting("mmf1");
                mmvStream = mmf.CreateViewStream(0, MMF_VIEW_SIZE); // stream used to read data

                BinaryFormatter formatter = new BinaryFormatter();

                // needed for deserialization
                byte[] buffer = new byte[MMF_VIEW_SIZE];

                String message1;

                // reads every second what's in the shared memory
                if (mmvStream.CanRead)
                {
                    // stores everything into this buffer
                    mmvStream.Read(buffer, 0, MMF_VIEW_SIZE);

                    // deserializes the buffer & prints the message
                    message1 = (String)formatter.Deserialize(new MemoryStream(buffer));

                    if (perviousCommandMsg != message1)
                    {

                        this.Dispatcher.Invoke(() =>
                        {
                            perviousCommandMsg = message1;
                            txtCommandReceived.Content = message1;
                            Debug.WriteLine("command:" + message1);

                        });
                    }
                 
                }
            }
            catch (FileNotFoundException error)
            {
                Debug.WriteLine("err:" + error.Message);
            }

            catch (Exception error)
            {
                Debug.WriteLine("err:" + error.Message);
            }

          

        }

        void ReadCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                txtCommandReceived.Content = args;

                for (int index = 1; index < args.Length; index += 2)
                {
                    string arg = args[index].Replace("-", "");
                    arguments.Add(arg, args[index + 1]);
                }

                if (arguments.ContainsKey("color"))
                {
                    Debug.WriteLine("command received:" + arguments["color"]);
                    txtCommandReceived.Content = arguments["color"];
                }
            }
            catch (Exception err)
            {

                txtCommandReceived.Content = err.Message;

            }


        }

        public void TestFun()
        {
            txtCommandReceived.Content = " IT WORKS";

            //Process[] NewProcessList2 =
            //            Process.GetProcessesByName("WindowsApplication1TE ST");
            //foreach (Process TempProcess in NewProcessList2)
            //{
            //    // BUG :
            //    TempProcess.MainModule.GetType().GetMethod("math_a dd").Invoke(TempProcess.MainModule, new object[] { 2, 3 });
            //}



        }


    }


  


}
