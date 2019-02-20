using NetLibrary.EventsArgs;
using NetLibrary.Helpers;
using NetLibrary.Models;
using System.Threading.Tasks;
using NetLibrary.Enums;
using System;

namespace Server.Engine.Classes
{
    public class Connection
    {
        public ConnectionModel User { get; set; }

        /// <summary>
        /// Show that is user started send video or not
        /// </summary>
        public bool IsStartedSendingVideo { get; set; }

        /// <summary>
        /// Event which invoke when client send response
        /// </summary>
        public event ReceiveHandler OnReceivedMessage;

        /// <summary>
        /// Event which invoke when client want to start send video
        /// </summary>
        public event VideoCallHandler OnStartSendingVideo;

        /// <summary>
        /// Event which invoke when client want to stop send video
        /// </summary>
        public event VideoCallHandler OnStopSendingVideo;
        public delegate void VideoCallHandler(Connection sender, VideoCallEventArgs e);

        /// <summary>
        /// Event which invoke when client send video frame
        /// </summary>
        public event ReceiveHandler OnReceivedVideoFrame;
        public delegate void ReceiveHandler(Connection sender, ReceivedPacketEventsArgs e);

        /// <summary>
        /// Event which invoke when client send command
        /// </summary>
        public event ReceivedCommandHandler OnReceivedCommand;
        public delegate void ReceivedCommandHandler(Connection sender, ReceivedCommandEventsArgs e);

        /// <summary>
        /// Event which invoke when client disconnected from server
        /// </summary>
        public event DisconnectHandler OnDisconnected;
        public delegate void DisconnectHandler(Connection sender, ReceivedPacketEventsArgs e);

        public void StartReceiveResponses()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        var responseData = await NetHelper.GetDataAsync(User.TcpSocket);

                        if (responseData.ActionState == ActionStates.Disconnect)
                            OnDisconnected(this, new ReceivedPacketEventsArgs(responseData));

                        if (responseData.ActionState == ActionStates.Message)
                            OnReceivedMessage(this, new ReceivedPacketEventsArgs(responseData));

                        if (responseData.ActionState == ActionStates.Video)
                            OnReceivedVideoFrame(this, new ReceivedPacketEventsArgs(responseData));

                        if (responseData.ActionState == ActionStates.Command)
                            OnReceivedCommand(this, new ReceivedCommandEventsArgs(responseData.ClientInfo, responseData.Command));

                        if (responseData.ActionState == ActionStates.StartVideoCall)
                        {
                            IsStartedSendingVideo = true;
                            OnStartSendingVideo(this, new VideoCallEventArgs(responseData.Conversation.Target.Id));
                        }

                        if (responseData.ActionState == ActionStates.StopVideoCall)
                        {
                            IsStartedSendingVideo = false;
                            OnStartSendingVideo(this, new VideoCallEventArgs(responseData.Conversation.Target.Id));
                        }
                    }
                    catch (Exception)
                    {
                        OnDisconnected(this, new ReceivedPacketEventsArgs(null));
                    }
                }
            });
        }
    }
}
