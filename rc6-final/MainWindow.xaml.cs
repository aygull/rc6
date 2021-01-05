using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;


namespace rc6_final
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int mode = 1;
        byte[] userFile;
        int key_long = 128;
        RC6 rc6;
        Mode encryption_mode;
        string filePathStr;
        private void ChooseFile(object sender, RoutedEventArgs e)
        {
            filePathStr = FilesWork.GetFilePathFromDialog();

            if (filePathStr.Length == 0)
            {
                return;
            }
            FileName.Text = filePathStr;
            

        }

        void DoWork(object sender, DoWorkEventArgs e)
        {

            for (int i = 0; i < 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(10);
            }
        }

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        public ICommand _radioCommand;
        public ICommand RadioCommand
        {
            get
            {
                if (_radioCommand == null)
                    _radioCommand = new RelayCommand((param) => { RadioMethod(param); });

                return _radioCommand;
            }
        }


        public void RadioMethod(object parametr)
        {

            switch (parametr.ToString())
            {
                case "EBC":
                    mode = 1;
                    break;
                case "CBC":
                    mode = 2;
                    break;
                case "CFB":
                    mode = 3;
                    break;
                case "OFB":
                    mode = 4;
                    break;

            }
        } 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Encode(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            Task reading = Task.Run(() =>
            {
                userFile = FilesWork.ReadFullFile(filePathStr);
            }
            );
            reading.Wait();
            pbStatus.Value++;
            worker.RunWorkerAsync();

            if (Key128.IsChecked == true)
            {
                key_long = 128;
            }
            else if (Key192.IsChecked == true)
            {
                key_long = 192;
            }
            else if (Key256.IsChecked == true)
            {
                key_long = 256;
            }
            string key = Key.Text;
            key_long = 128;
            if (key.Length == key_long)
            {
                rc6 = new RC6(key_long, Encoding.UTF8.GetBytes(key));
            }
            else
            {
                rc6 = new RC6(key_long);
            }
            encryption_mode = new Mode(rc6);
            Task k = Task.Run(()=>encryption_mode.EncodeEBC(userFile));
            if (mode == 2)
            {
                k = encryption_mode.EncodeCBC(userFile);
            }
            else if (mode == 3)
            {
                k = encryption_mode.EncodeCFB(userFile);
            }
            else if(mode == 4)
            {
                k = encryption_mode.EncodeOFB(userFile);
            }
            k.Wait();
            string path = System.IO.Path.Combine(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName);

            string newFile = System.IO.Path.Combine(path, System.IO.Path.GetFileNameWithoutExtension(filePathStr) + ".enc");

            FilesWork.WriteInFile(Mode.encrypted_text.ToArray(), newFile);
        }

        private void Decrypt(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            Task reading = Task.Run(() =>
            {
                userFile = FilesWork.ReadFullFile(filePathStr);
            }
            );
            pbStatus.Value++;
            worker.RunWorkerAsync();
            reading.Wait();
            Task decoding = Task.Run(()=>encryption_mode.DecodeEBC(userFile));
            if (mode == 2)
            {
                decoding = encryption_mode.DecodeCBC(userFile);
            }
            else if (mode == 3)
            {
                decoding = encryption_mode.DecodeCFB(userFile);
            }
            else if (mode == 4)
            {
                decoding = encryption_mode.DecodeOFB(userFile);
            }
            decoding.Wait();
            string path = System.IO.Path.Combine(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName);

            string newFile = System.IO.Path.Combine(path, "(rc6)" + System.IO.Path.GetFileNameWithoutExtension(filePathStr) +".dec");

            FilesWork.WriteInFile(Mode.original_text.ToArray(), newFile);
        }
    }
}
