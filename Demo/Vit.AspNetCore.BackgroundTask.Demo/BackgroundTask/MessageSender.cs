using System;

namespace Main.BackgroundTask
{
    public class MessageSender
    {

        public void SendMessage()
        {
            Console.WriteLine("BackgroundTask.MessageSender.SendMessage, "+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

    }
}
