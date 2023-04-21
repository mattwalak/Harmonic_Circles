DEBUG = (location.hostname === "localhost" || location.hostname === "127.0.0.1");
host = "ws://18.215.177.114:42742";

if(DEBUG){
  console.log("In debug");
  host = "ws://localhost:42742";
}

function processServerMessage(msgObj){
  console.log(msgObj);
  switch(msgObj.command){
    case "ConnectionSuccess":
      console.log("Server:ConnectionSuccess");
      onNetwork_ConnectionSuccess(msgObj);
      break;
    case "ConnectionFailed":
      console.log("Server:ConnectionFailed");
      onNetwork_ConnectionFailed(msgObj);
      break;
    case "SendCircleButtonUpdateFromGame":
      console.log("Server:SendCircleButtonUpdateFromGame");
      onNetwork_SendCircleButtonUpdateFromGame(msgObj);
      break;
    case "SceneChange":
      console.log("Server:SceneChange");
      onNetwork_SceneChange(msgObj);
      break;
    case "Player2SentKeyChange":
      console.log("Server:Player2SentKeyChange");
      if(playerNum == 1){
        // player 1 response to key change
      }
    default:
      console.log(`Designer: Unknown command = ${msg}`);
  }
};

console.log("Making a new connection!!!");
let socket = new WebSocket(host);


function alert(msg){
  console.log(msg);
}

socket.onopen = function(e) {
  alert("[open] Connection established");
};

socket.onmessage = function(event) {
  alert(`[message] Data received from server: ${event.data}`);

  try{
    msgObj = JSON.parse(event.data.toString());
    if(msgObj.source === "Server"){
      processServerMessage(msgObj);
    }else{
      console.log("Unknown source");
    }
  }catch(e){
    console.log("I don't understand this message");
    console.error(e);
  }
};

socket.onclose = function(event) {
  if (event.wasClean) {
    alert(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
  } else {
    // e.g. server process killed or network down
    // event.code is usually 1006 in this case
    alert('[close] Connection died');
  }
};

socket.onerror = function(error) {
  alert(`[error] ${error.message}`);
  console.log("EEEEEERRRR", error)
};

// ---------------------------------- SEND MESSAGES -------------------------------------------

// state = 1 means clicked, state = -1 means released, state = 2 means just activated
function networkSend_CircleButtonClick(id, state){
  msg = {
    source: "Player",
    command: "CircleButtonClick",
    circleButtonID: id,
    circleButtonState: state
  };

  socket.send(JSON.stringify(msg));
}

function networkSend_KeyChange(key){
  msg = {
    source: "Player",
    command: "SendKeyChange",
    newKey: key
  }

  socket.send(JSON.stringify(msg));
}

// clickUpdate: 1 = click started, 0 = no change, -1 = click ended
function networkSend_TouchPositionData(touchState, posX, posY){
  msg = {
    source: "Player",
    command: "SendTouchPositionData",
    touchState: touchState,
    touchPosX: posX,
    touchPosY: posY
  };

  socket.send(JSON.stringify(msg));
}

function networkSend_requestPlayerSpot(){
  msg = {
    source: "Player",
    command: "RequestPlayerSpot"
  };

  socket.send(JSON.stringify(msg));
}