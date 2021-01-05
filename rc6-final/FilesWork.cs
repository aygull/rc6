using System;
using System.Text;
using Microsoft.Win32;
using System.IO;

namespace rc6_final
{
    class FilesWork 
    {
        public static string GetFilePathFromDialog(string filter = "All files (*.*)|*.*")
        {
            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var fileDialog = new OpenFileDialog();

            return fileDialog.ShowDialog() == true ? fileDialog.FileName : "";
        }

        public static byte[] ReadFullFile(string filePathStr)
        {
            FileInfo infoFile = new FileInfo(filePathStr);
            byte[] userFile = new byte[infoFile.Length];

            using (BinaryReader strReader = new BinaryReader(File.Open(filePathStr, FileMode.Open), Encoding.UTF8))
            {
                strReader.Read(userFile, 0, userFile.Length);
            }
            return userFile;
        }

        public static void WriteInFile(byte[] text, string path)
        {
          

            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                // запись массива байтов в файл
                fstream.Write(text, 0, text.Length);
            }
        }
    }
}
