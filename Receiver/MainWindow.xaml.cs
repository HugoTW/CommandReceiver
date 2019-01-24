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
using System.Threading;


namespace Receiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
     public  partial class MainWindow : Window
    {
        Dictionary<string, string> arguments = new Dictionary<string, string>();
        private const string MutexName = "YOUR_MUTEX_XXXX_SINGLE_INSTANCE_AND_NAMEDPIPE";

        static public MainWindow mainWindow;


        static void myReceive(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("Moz MSG " + i.ToString() + ": " + args[i]);
            }

            mainWindow.ReadCommand(args);
        }



        [STAThread]
        static void Main(string[] args)
        {

            // test if this is the first instance and register receiver, if so.
            if (SingletonController.IamFirst(new SingletonController.ReceiveDelegate(myReceive)))
            {
                // OK, this is the first instance, now run whatever you want ...

                App app = new App();
                mainWindow = new MainWindow();
                app.Run(mainWindow);

            }
            else
            {
                // send command line args to running app, then terminate
                SingletonController.Send(args);
            }

            SingletonController.Cleanup();
        }


        public MainWindow()
        {

            Debug.WriteLine("Moz MainWindow Initialized");
            InitializeComponent();
         
        }

         void ReadCommand(string[] args)
        {

            try
            {
                
                for (int index = 0; index < args.Length; index += 2)
                {
                    string arg = args[index].Replace("-", "");
                    arguments.Add(arg, args[index + 1]);
                    Debug.WriteLine("Moz command :" + arg);

                }

                if (arguments.ContainsKey("color"))
                {
                    Debug.WriteLine("Moz command received:" + arguments["color"]);

                    this.Dispatcher.Invoke(() =>
                    {
                        txtCommandReceived.Content = arguments["color"];
                    });

                    arguments.Clear();
                }

    
            }
            catch (Exception err)
            {
                Debug.WriteLine("Moz - error", err.Message);

                this.Dispatcher.Invoke(() =>
                {
                    txtCommandReceived.Content = err.Message;
                });
                arguments.Clear();
            }

        }

    }


  


}
