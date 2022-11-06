namespace BicepAzToDotNet
{
    internal interface IFileSystem
    {
        void CreateDirectory(string path);
        bool DirectoryExists(string path);
        bool FileExists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string contents);
    }
}
