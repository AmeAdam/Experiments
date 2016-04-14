namespace AmeCommon.MediaTasks
{
    public interface IIOCommand
    {
        void Execute(DestinationDirectoryHandler destinationDirectory);
    }
}