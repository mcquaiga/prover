using System;
using System.Data;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IFileSystem
    {
        string AppBaseDirectory { get; }
        void DeleteFile(string path);
        string[] GetFilesInDir(string dir);
        string[] GetFilesInDirRecurrsive(string dir);
        string[] GetFilesInDirMatching(string dir, string pattern, bool recursive);
        string[] GetFilesInDirMatching(string dir, string pattern);

        string[] GetSubdirectories(string dir);

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
}