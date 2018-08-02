﻿/***************************************************************
 * Description: 
 *
 * Documents: https://github.com/hiramtan/HiSocket
 * Author: hiramtan@live.com
***************************************************************/

using HiFramework;
using System;
using System.Collections.Generic;

namespace HiSocket
{
    public class TcpConnection : TcpSocket, IConnection
    {
        private IPackage package;
        private readonly IByteArray send = new ByteArray();
        private readonly IByteArray receive = new ByteArray();
        private Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();
        public event Action OnConstruct;
        public event Action<byte[]> OnSend;
        public event Action<byte[]> OnReceive;
        public TcpConnection(IPackage package)
        {
            this.package = package;
            OnSocketReceive += OnSocketReceiveHandler;
            ConstructEvent();
        }

        /// <summary>
        /// To quickly get plugin
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IPlugin this[string name]
        {
            get
            {
                return GetPlugin(name);
            }
            set
            {
                AddPlugin(value);
            }
        }

        public new void Send(byte[] bytes)
        {
            send.Write(bytes);
            SendEvent(bytes);
            package.Pack(send, x => { base.Send(x); });
        }
        void OnSocketReceiveHandler(byte[] bytes)
        {
            receive.Write(bytes);
            package.Unpack(receive, x => { ReceiveEvent(x); });
        }

        public void AddPlugin(IPlugin plugin)
        {
            AssertThat.IsNotNull(plugin);
            plugin.Connection = this;
            plugins.Add(plugin.Name, plugin);
        }

        public IPlugin GetPlugin(string name)
        {
            AssertThat.IsNotNullOrEmpty(name);
            return plugins[name];
        }

        public void RemovePlugin(string name)
        {
            AssertThat.IsNotNullOrEmpty(name);
            plugins.Remove(name);
        }
        void ConstructEvent()
        {
            if (OnConstruct != null)
            {
                OnConstruct();
            }
        }

        void SendEvent(byte[] bytes)
        {
            if (OnSend != null)
            {
                OnSend(bytes);
            }
        }

        void ReceiveEvent(byte[] bytes)
        {
            if (OnReceive != null)
            {
                OnReceive(bytes);
            }
        }
    }
}