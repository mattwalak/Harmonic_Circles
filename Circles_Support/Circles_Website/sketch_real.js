let playerConnectionStage, player1stage1, player1stage2, player1stage3, player2stage1, player2stage2, player2stage3;

// -1 = unassigned
// 1 = big circle, 
// 2 = ring of buttons
let playerNum = -1;

// 0 = SETUP (Not in game yet)
// 1 = impulse mode
// 2 = rhytmic impulse mode
// 3 = laser mode
let gameState = 0;

let NUM_CIRCLES = 20;
var NUM_KEYS = 1;
let MAIN_RING_DIAMETER;
let SMALLER_RING_DIAMETER;
let font;

let stage1ProgressionCount = 0; // [0, NUM_CIRCLES]

function preload(){
	font = loadFont('assets/Metropolis-Regular.otf');
}

function setup(){
	createCanvas(windowWidth, windowHeight);
	background(0);

	playerConnectionStage = new PlayerConnectionStage();
	playerConnectionStage.setup();

	MAIN_RING_DIAMETER = width * 5/6;
	SMALLER_RING_DIAMETER = width * 1/2;

	player1stage1 = new Player1_Stage1();
	player1stage2 = new Player1_Stage2();
	player1stage3 = new Player1_Stage3();
	player2stage1 = new Player2_Stage1();
	player2stage2 = new Player2_Stage2();
	player2stage3 = new Player2_Stage3();
}

function draw(){
	background(0);
	if(gameState == 0){
		playerConnectionStage.draw();
	}else{
		if(playerNum == 1){
			if(gameState == 1){
				player1stage1.draw();
			}else if(gameState == 2){
				player1stage2.draw();
			}else if(gameState == 3){
				player1stage3.draw();
			}
		}else if(playerNum == 2){
			if(gameState == 1){
				player2stage1.draw();
			}else if(gameState == 2){
				player2stage2.draw();
			}else if(gameState == 3){
				player2stage3.draw();
			}
		}
	}
}

function nextGameStage(){
	// We use a temp value because as soon as the real gameState increases
	// draw can call functions in the new state. We want to finish all setup
	// to avoid drawing a stage that has not been setup yet.
	tempGameStateIncrease = gameState + 1;

	if(playerNum == 1){
		if(tempGameStateIncrease == 1){
			player1stage1.setup();
		}else if(tempGameStateIncrease == 2){
			player1stage2.setup();
		}else if(tempGameStateIncrease == 3){
			player1stage3.setup();
		}
	}else if(playerNum == 2){
		if(tempGameStateIncrease == 1){
			player2stage1.setup();
		}else if(tempGameStateIncrease == 2){
			player2stage2.setup();
		}else if(tempGameStateIncrease == 3){
			player2stage3.setup();
		}
	}

	// Now that setup is complete, actually increase the variable
	gameState++;
}

function keyButtonCallback(id, stateChange){
	if(stateChange == 1){
		// Send toggle signal
		for(let i = 0; i < keyButtons.length; i++){
			keyButtons[i].otherCircleSelected(id);
		}
		
		// Send network signal
		sendKeyChange(id);
	}
}

// INPUT
function touchStarted(){
	if(gameState == 0){
		playerConnectionStage.touchStarted();
	}else{
		if(playerNum == 1){
			if(gameState == 1){
				player1stage1.touchStarted();
			}else if(gameState == 2){
				player1stage2.touchStarted();
			}else if(gameState == 3){
				player1stage3.touchStarted();
			}
		}else if(playerNum == 2){
			if(gameState == 1){
				player2stage1.touchStarted();
			}else if(gameState == 2){
				player2stage2.touchStarted();
			}else if(gameState == 3){
				player2stage3.touchStarted();
			}
		}
	}

  	return false;
}

function touchMoved(){
	if(gameState == 0){
		playerConnectionStage.touchMoved();
	}else{
		if(playerNum == 1){
			if(gameState == 1){
				player1stage1.touchMoved();
			}else if(gameState == 2){
				player1stage2.touchMoved();
			}else if(gameState == 3){
				player1stage3.touchMoved();
			}
		}else if(playerNum == 2){
			if(gameState == 1){
				player2stage1.touchMoved();
			}else if(gameState == 2){
				player2stage2.touchMoved();
			}else if(gameState == 3){
				player2stage3.touchMoved();
			}
		}
	}

  	return false;
}

function touchEnded(){
	if(gameState == 0){
		playerConnectionStage.touchEnded();
	}else{
		if(playerNum == 1){
			if(gameState == 1){
				player1stage1.touchEnded();
			}else if(gameState == 2){
				player1stage2.touchEnded();
			}else if(gameState == 3){
				player1stage3.touchEnded();
			}
		}else if(playerNum == 2){
			if(gameState == 1){
				player2stage1.touchEnded();
			}else if(gameState == 2){
				player2stage2.touchEnded();
			}else if(gameState == 3){
				player2stage3.touchEnded();
			}
		}
	}

  	return false;
}

// NETWORK CALLBACKS

function onNetwork_ConnectionSuccess(msgObj){
	playerNum = msgObj.playerNum;
	nextGameStage();
}

function onNetwork_ConnectionFailed(msgObj){
	playerConnectionStage.onConnectionFailed();
}

function onNetwork_SendCircleButtonUpdateFromGame(msgObj){
	if(playerNum == 2 && gameState == 1){
		player2stage1.activateCircle(msgObj.circleButtonID);
    }

	stage1ProgressionCount++;
}

function onNetwork_SceneChange(msgObj){
	nextGameStage();
}

function onNetwork_Player2SentKeyChange(msgObj){
	if(playerNum == 1 && gameState == 2){
		player1stage2.unlockSource();
	}
}

function onNetwork_StartOfFocusOnPartial(msgObj){
	if(playerNum == 2 && gameState == 2){
		player2stage2.startOfFocusOnPartial();
	}
}

function onNetwork_EndOfFocusOnPartial(msgObj){
	if(playerNum == 2 && gameState == 2){
		player2stage2.endOfFocusOnPartial();
	}
}

function onNetwork_FocusOnPartial(msgObj){
	if(playerNum == 2 && gameState == 2){
		player2stage2.focusOnPartial(msgObj.circleButtonID);
	}
}