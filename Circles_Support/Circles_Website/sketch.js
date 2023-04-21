let playerConnectionStage, player1stage1, player1stage2, player2stage1, player2stage2;

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
var NUM_KEYS = 12;
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
	player2stage1 = new Player2_Stage1();
	player2stage2 = new Player2_Stage2();
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

			}
		}else if(playerNum == 2){
			if(gameState == 1){
				player2stage1.draw();
			}else if(gameState == 2){
				player2stage2.draw();
			}else if(gameState == 3){
				
			}
		}
	}


	/*
	if(menuState == 0){
		connectToServerButton.draw();
	}else if(menuState == 1){
		fill(255);
		textSize(16);
		text("...waiting for server response", width/2, height/2);
	}else if(menuState == 2){
		fill(255);
		textSize(16);
		text("connection failed\nreload to try again", width/2, height/2);
	}else if(menuState == 3){
		if(playerNum == 1){
			
  			


		}else if(playerNum == 2){
			for(let i = 0; i < circleButtons.length; i++){
				fill(255);
				circleButtons[i].draw();
			}

			if(gameState == 1){
				for(let i = 0; i < keyButtons.length; i++){
					fill(255);
					keyButtons[i].draw();
				}
			}
		}

	}*/
	
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
			
		}
	}else if(playerNum == 2){
		if(tempGameStateIncrease == 1){
			player2stage1.setup();
		}else if(tempGameStateIncrease == 2){
			player2stage2.setup();
		}else if(tempGameStateIncrease == 3){
			
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

/*
function onTransitionToRhythmMode(){
	if(playerNum == 2){
		
		for(let i = 0; i < NUM_KEYS; i++){
			let rad = (2 * Math.PI) - (i * (2 * Math.PI / NUM_KEYS));
			let xPos = Math.cos(rad) * smallerRingDiameter / 2;
			let yPos = Math.sin(rad) * smallerRingDiameter / 2;
			keyButtons.push(
				new CircleButton(
					(width/2) + xPos, 
					(height/2) + yPos, 
					width/15, 
					i, 
					keyButtonCallback
				)
			);
		}

		keyButtons[0].isPressedIn = true;
	}

	gameState = 1;
}*/

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

			}
		}else if(playerNum == 2){
			if(gameState == 1){
				player2stage1.touchStarted();
			}else if(gameState == 2){
				player2stage2.touchStarted();
			}else if(gameState == 3){
				
			}
		}
	}


	/*
	if(playerNum == 1){
		
	}else if(playerNum == 2){
		

		if(gameState == 1){
			for(let i = 0; i < keyButtons.length; i++){
				keyButtons[i].checkClick();
			}
		}
	}*/

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

			}
		}else if(playerNum == 2){
			if(gameState == 1){
				player2stage1.touchMoved();
			}else if(gameState == 2){
				player2stage2.touchMoved();
			}else if(gameState == 3){
				
			}
		}
	}

	/*
	if(playerNum == 1){
		calculateAndSendTouchPositionData(0);
	}*/
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

			}
		}else if(playerNum == 2){
			if(gameState == 1){
				player2stage1.touchEnded();
			}else if(gameState == 2){
				player2stage2.touchEnded();
			}else if(gameState == 3){
				
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