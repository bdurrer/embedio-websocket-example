# EmbedIO Websocket Example

This example project exchanges messages with the EmbedIO server using WebSockets.
You can modify the amount of parallel WebSockets in the `script.js`

The server waits a short time before answering to simulate real-world processing.
Over time, this results in overlapping incoming and outgoing messages.