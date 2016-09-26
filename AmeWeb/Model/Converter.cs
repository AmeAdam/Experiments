namespace AmeWeb.Model
{
    public static class Converter
    {
        private static readonly string[] Sizes = {"B", "KB", "MB", "GB"};

        public static string DisplayFileSize(this long fileSize)
        {
            int order = 0;
            double size = fileSize;
            while (size >= 1024 && ++order < Sizes.Length)
            {
                size = size / 1024;
            }
            return $"{size:0.##} {Sizes[order]}";
        }
    }
}
