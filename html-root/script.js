// increase to test working with parallel sockets
const socketCount = 1;

const websockets = [];

function createSocket(name) {
    const connection = new WebSocket("ws://localhost:8080/event", "json");
    connection.onopen = function (evt) {
        console.log("***ONOPEN " + name);
    };
    connection.onmessage = function (evt) {
        const json = JSON.parse(evt.data);
        console.log("***ONMESSAGE " + name, json);
    }
    connection.onerror = function (evt) {
        console.error("***ERROR " + name, evt);
    }

    connection.onclose = function (event) {
        console.log(`WebSocket ${name} is closed now.`);
    };
    return connection;
}

// init a few sockets
for (let i = 0; i < socketCount; i++) {
    const sock = createSocket(i);
    websockets.push(sock);
}


function spamText(length) {
    let result           = '';
    const characters       = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++ ) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
}

// spam them

function sendMessages() {
    console.log('sending messages on each socket');
    for (let i = 0; i < websockets.length; i++)
    {
        const sock = websockets[i];
        
        if (sock.readyState === 3) {
            console.log(`Socket ${i} failed, replacing it`);
            websockets[i] = createSocket(i);
            return;
        }
        
        if(sock.readyState !== 1) {
            // the socket is not open
            console.log(`Socket ${i} is not open, skip sending`);
            return;
        }

        sock.send(JSON.stringify({
            type: 'spam',
            data: spamText(1000)
        }));
    }
}

let isSpamming = false;
let intervalHandle;

function startTheSpam() {
    if (isSpamming) {
        clearInterval(intervalHandle);
        isSpamming = false;
    } else {
        isSpamming = true;
        intervalHandle = setInterval(sendMessages, 20);
    }
    
    document.getElementById("spam-button").innerText = isSpamming ? "Stop spam" : "Start spam";
}

