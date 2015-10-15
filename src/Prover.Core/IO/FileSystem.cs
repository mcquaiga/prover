using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Core.IO
{
    public interface IFileSystem
    {
        void DeleteFile(string path);
        string[] GetFilesInDir(string dir);
        string[] GetFilesInDirRecurrsive(string dir);
        string[] GetFilesInDirMatching(string dir, string pattern, bool recursive);
        string[] GetFilesInDirMatching(string dir, string pattern);
        string[] GetSubdirectories(string dir);
        string AppBaseDirectory { get; }
        //string RequireTempDir(IEngineSettings settings);
        bool FileExists(string path);
        long GetFileLength(string path);
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        void DeleteDirectory(string path);
        string ReadAllText(string path);
        int ForLineInFile(string path, Func<string, int, bool> operation);
        void WriteAllText(string path, string text);
        Task WriteAllTextAsync(string path, byte[] text);
        void SerializeDataToXMLFile(DataTable data, string path);
        DataTable DeserializeXMLFileToData(string path);
        bool CheckFileHasCopied(string filePath);
        string FindTool(string relativePath);
    }

    public class FileSystem : IFileSystem
    {
        private const int WaitIntervalForCopyMoveComplete = 300000; //5min in ms

        public string[] GetFilesInDir(string dir)
        {
            return Directory.GetFiles(dir);
        }

        public string[] GetFilesInDirRecurrsive(string dir)
        {
            return Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
        }

        public string[] GetFilesInDirMatching(string dir, string pattern, bool recursive)
        {
            return Directory.GetFiles(dir, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public string[] GetFilesInDirMatching(string dir, string pattern)
        {
            return GetFilesInDirMatching(dir, pattern, true);
        }

        public string[] GetSubdirectories(string dir)
        {
            return Directory.GetDirectories(dir);
        }

        public string AppBaseDirectory
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."); }
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public long GetFileLength(string path)
        {
            var fileInfo = new FileInfo(path);
            return fileInfo.Length;
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, recursive: true);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public int ForLineInFile(string path, Func<string, int, bool> operation)
        {
            int count = 0, n = 0;
            string line;
            var reader = new StreamReader(path);
            while ((line = reader.ReadLine()) != null)
                if (operation(line, n++))
                    count++;
            return count;
        }

        public async Task WriteAllTextAsync(string path, byte[] text)
        {
            using (FileStream sourceStream = new FileStream(path,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(text, 0, text.Length);
            }
        }

        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        public void SerializeDataToXMLFile(DataTable data, string path)
        {
            data.WriteXml(path, XmlWriteMode.WriteSchema);
        }

        public DataTable DeserializeXMLFileToData(string path)
        {
            var dt = new DataTable();
            dt.ReadXml(path);
            return dt;
        }

        public bool CheckFileHasCopied(string filePath)
        {

            int timeElapsed = 0, timeTick = 1000;
            while (timeElapsed < WaitIntervalForCopyMoveComplete)
            {
                try
                {
                    if (File.Exists(filePath))
                        using (File.OpenRead(filePath))
                            return true;
                    else
                        return false;

                }
                catch (IOException)
                {
                    Thread.Sleep(timeTick);
                    timeElapsed += timeTick;
                }
            }
            return false;
        }

        public string FindTool(string relativePath)
        {
            var currentBaseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (File.Exists(Path.Combine(currentBaseDir, relativePath)))
                return Path.Combine(currentBaseDir, relativePath);

            while (string.IsNullOrWhiteSpace(currentBaseDir))
            {
                var testPath = Path.Combine(currentBaseDir, "tools", "Util", relativePath);
                if (File.Exists(testPath))
                    return testPath;
                currentBaseDir = Path.GetDirectoryName(currentBaseDir);
            }

            return null;
        }

    }
}
