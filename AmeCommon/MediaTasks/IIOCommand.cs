using AmeCommon.MediaTasks.MoveFilesCommands;

namespace AmeCommon.MediaTasks
{
    public interface IIOCommand
    {
        void Execute(DestinationDirectory destinationDirectory);
    }
}