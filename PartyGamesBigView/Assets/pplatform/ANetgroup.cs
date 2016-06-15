﻿using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using PPlatform.Helper;
using System.Text;

namespace PPlatform
{
    public abstract class ANetgroup : MonoBehaviour, PPlatform.INetgroup
    {
        public enum SignalingMessageType
        {
            Invalid = 0,
            Connected = 1,
            Closed = 2,
            UserMessage = 3,
            UserJoined = 4,
            UserLeft = 5,
            OpenRoom = 6,
            JoinRoom = 7,
            GameStarted = 8,
            RoomNamesDiscovered = 9
        };


        //private struct RoomInfoMessage
        //{
        //    public string name;
        //    public int id;
        //}
        //private struct UserMessage
        //{
        //    public int id;
        //    public string content;
        //}
        //private struct MsgEventData
        //{
        //    private SignalingMessageType mType;
        //    public SignalingMessageType Type
        //    {
        //        get { return mType; }
        //    }

        //    private object mContent;
        //    public object Content
        //    {
        //        get { return mContent; }
        //    }


        //    public MsgEventData(SignalingMessageType lType, object lContent)
        //    {
        //        mType = lType;
        //        mContent = lContent;
        //    }
        //}


        public enum ConnectionState
        {
            NotConnected,
            Connecting,
            Connected
        }

        public static readonly string MESSAGE_NAME = "SMSG";

        public struct SMessage
        {
            public SignalingMessageType type;
            public string content;
            public int id;

            public SMessage(SignalingMessageType t, string s, int i)
            {
                this.type = t;
                this.content = s;
                this.id = i;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                sb.Append("Type: ");
                sb.Append(type);
                sb.Append(", Content: ");
                sb.Append(content);
                sb.Append(", Id: ");
                sb.Append(id);
                sb.Append("]");
                return sb.ToString();
            }
        }


        private Action<SignalingMessageType, int, string> mEventHandler = null;
        protected Queue<SMessage> mEventQueue = new Queue<SMessage>();
        private ConnectionState mConnectionState = ConnectionState.NotConnected;

        private int mOwnId = -1;
        private bool mRoomOwner = false;

        public bool RoomOwner
        {
            get { return mRoomOwner; }
        }
        private string mRoomName = null;

        public string RoomName
        {
            get { return mRoomName; }
        }


        // Update is called once per frame
        protected virtual void Update()
        {
            HandleEvents();

        }
        public void SendMessageTo(string message, int userid)
        {
            SMessage msg = new SMessage();
            msg.type = SignalingMessageType.UserMessage;
            msg.content = message;
            msg.id = userid;
            //Debug.Log("Snd " + msg);
            SendMessageViaSocketIo(msg);
        }
        public void SendGameStarted()
        {
            Debug.Log("Sending game started");
            SMessage msg = new SMessage();
            msg.type = SignalingMessageType.GameStarted;

            SendMessageViaSocketIo(msg);
        }
        public void SendOpenRoom(string roomName)
        {
            mRoomName = roomName;
            SMessage msg = new SMessage(SignalingMessageType.OpenRoom, RoomName, -1);

            SendMessageViaSocketIo(msg);
        }

        private void HandleEvents()
        {
            lock (mEventQueue)
            {
                while (mEventQueue.Count > 0)
                {
                    SMessage data = mEventQueue.Dequeue();
                    HandleEvent(data);
                }
            }

        }

        private void HandleEvent(SMessage message)
        {

            if (message.type == SignalingMessageType.Connected)
            {
                mConnectionState = ConnectionState.Connected;
                mOwnId = message.id;

            }
            else if (message.type == SignalingMessageType.Closed)
            {
                mConnectionState = ConnectionState.NotConnected;
            }
                
            DeliverEvent(message.type, message.id, message.content);
        }

        private void DeliverEvent(SignalingMessageType type, int relatedConId, string content)
        {
            if (mEventHandler != null)
            {
                mEventHandler(type, relatedConId, content);
            }
        }
        protected void AddEvent(SMessage msg)
        {
            lock (mEventQueue)
                mEventQueue.Enqueue(msg);
        }
        private void OnDestroy()
        {
            Cleanup();
        }


        protected abstract void InitSocket();
        protected abstract void SendMessageViaSocketIo(SMessage message);
        protected abstract void Cleanup();

        //private void Cleanup()
        //{

        //    if (mSocket != null)
        //    {
        //        Debug.Log("OnDestroy: unregister all handlers");
        //        //if the connectiong is buggy e.g. it is missing incomming events then disconnect often causes unity to stall completly
        //        //the library seem to block this call until the server replied which never happens
        //        //the server registered the disconnect already while this keeps blocking forever
        //        //mSocket.Disconnect();
        //        //mSocket.Close();


        //        //close causes the same bug. just unregister handlers for now

        //        mSocket.Off(Socket.EVENT_CONNECT);
        //        mSocket.Off(Socket.EVENT_CONNECT_TIMEOUT);
        //        mSocket.Off(Socket.EVENT_CONNECT_ERROR);
        //        mSocket.Off(Socket.EVENT_DISCONNECT);
        //        mSocket.Off(Socket.EVENT_ERROR);
        //        mSocket.Off(Socket.EVENT_RECONNECT);
        //        mSocket.Off(Socket.EVENT_RECONNECT_ATTEMPT);
        //        mSocket.Off(Socket.EVENT_RECONNECT_ERROR);
        //        mSocket.Off(Socket.EVENT_RECONNECT_FAILED);
        //        mSocket.Off(Socket.EVENT_RECONNECTING);

        //        mSocket.Off(EVENT_ROOM_OPENED);
        //        mSocket.Off(EVENT_ROOM_JOINED);
        //        mSocket.Off(EVENT_USER_MESSAGE);
        //        mSocket.Off(EVENT_USER_JOINED);
        //        mSocket.Off(EVENT_USER_LEFT);
        //        mSocket = null;
        //        //m.Close();


        //        Debug.Log("Cleanup complete");
        //    }

        //}


        public void Open(string name, Action<SignalingMessageType, int, string> lEventHandler)
        {
            mEventHandler = lEventHandler;
            mRoomName = name;
            mRoomOwner = true;

            if (mConnectionState == ConnectionState.NotConnected)
            {
                mConnectionState = ConnectionState.Connecting;
                InitSocket();
            }
        }

        public void Close()
        {
            Cleanup();
        }
    }

}
