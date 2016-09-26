namespace AmeCommon.Model
{
    public class DeviceCaptureInfo
    {
        public string TargetFolder { get; set; }
        public string SourceMask { get; set; }
        public string Command { get; set; }

        public override string ToString()
        {
            switch (Command)
            {
                case "move-directory-content":
                    return $"Przenieś katalogi z {SourceMask} do katalogu {TargetFolder}";
                case "move-directory-content-flat":
                    return $"Przenieś wszystki pliki z {SourceMask} i podkatalogów do katalogu {TargetFolder}";
                case "move-files":
                    return $"Przenieś pliki z {SourceMask} do katalogu {TargetFolder}";
                default:
                    return $"{Command} z {SourceMask} do {TargetFolder}";
            }
        }
    }
}