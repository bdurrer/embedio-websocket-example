﻿using System;
using System.Threading;
using EmbedIO.WebSockets;
using System.Threading.Tasks;

namespace EmbedIO_Websocket_Example
{
    class EventSocketModule : WebSocketModule
    {

        public EventSocketModule() :
            base("/event", true)
        {
            AddProtocol("json");
        }

        /// <inheritdoc />
        protected override async Task OnMessageReceivedAsync(IWebSocketContext context, byte[] rxBuffer,
            IWebSocketReceiveResult rxResult)
        {
            string text = Encoding.GetString(rxBuffer);
            JsEvent evt = JsonUtils.DeserializeFromJson<JsEvent>(text);
            Logger.Info("Got message of type {0}", evt.Type);

            if (evt.Type == "spam")
            {
                // wait some time, simulating actual work being done and then respond with a big chunk of text
                Random rnd = new Random();
                await Task.Delay(rnd.Next(50, 150));
                var responseEvent = new JsEvent("spam-back")
                {
                    Data = JsDataRow.GenerateLargeTable()
                };
                await SendTargetedEvent(context, responseEvent).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        protected override async Task OnClientConnectedAsync(IWebSocketContext context)
        {
            Logger.Info("client connected");
            await SendTargetedEvent(context, new JsEvent("connected")).ConfigureAwait(false);

        }

        private Task SendTargetedEvent(IWebSocketContext context, JsEvent jsEvent)
        {
            return SendAsync(context, JsonUtils.SerializeToJson(jsEvent));
        }

        /// <inheritdoc />
        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            Logger.Info("client disconnected");
            return Task.CompletedTask;
        }

        public async Task BroadcastEvent(JsEvent jsEvent)
        {
            var json = JsonUtils.SerializeToJson(jsEvent);
            await BroadcastAsync(json).ConfigureAwait(false);
        }
    }
}
