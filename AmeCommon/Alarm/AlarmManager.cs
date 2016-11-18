using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeCommon.Alarm
{
    public interface IAlarmManager
    {
        void SendMessage(Message message);
        List<Message> GetMessegesToSend(string fromNumber);
        void ConfirmMessageSend(Guid messageId);
        List<Message> GetPendingMessages();
    }

    public class AlarmManager : IAlarmManager
    {
        private readonly object sync = new object();

        private List<Message> MessageQueue { get; } = new List<Message>();

        public void SendMessage(Message message)
        {
            lock (sync)
            {
                message.Id = Guid.NewGuid();
                MessageQueue.Add(message);
            }
        }

        public List<Message> GetMessegesToSend(string fromNumber)
        {
            lock (sync)
            {
                return MessageQueue.Where(m => m.FromNumber == fromNumber).ToList();
            }
        }

        public void ConfirmMessageSend(Guid messageId)
        {
            lock (sync)
            {
                MessageQueue.RemoveAll(m => m.Id.Equals(messageId));
            }
        }

        public List<Message> GetPendingMessages()
        {
            lock (sync)
            {
                return new List<Message>(MessageQueue);
            }
        }
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string FromNumber { get; set; }
        public string ToNumber { get; set; }
        public string Content { get; set; }
    }


}
