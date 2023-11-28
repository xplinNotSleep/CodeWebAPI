namespace AgDataHandle.BaseService.Utils
{
    internal class PathLunix
    {
        public static string Combine(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
                return null;
            return Path.Combine(path1, path2).ReplaceAll(new string[] { "\\" }, "/");
        }
    }
}
