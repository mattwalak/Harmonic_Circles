let NUM_CIRCLES = 20;

let connectToServerButton;
let font;
let playerNum = -1; // 1 = big circle, 2 = ring of buttons
let impulseModeProgressionCount = 0; // [0, 20]

let mainRingDiameter;
let smallerRingDiameter;
let circleButtons = [];
let keyButtons = [];

let lastClickActionTime = -1;
let lastClickDelayTime = -1;
let maxDelayTimeSec = 1;
let minDelayTimeSec = 0.1;


// 0 = connect to server button
// 1 = awaiting server response
// 2 = connection failed
// 3 = in game
let menuState = 0; 

// 0 = impulse mode
// [1, 2) = rhytmic impulse mode
// 2 = laser mode
let gameState = 0;

function preload(){
	font = loadFont('assets/Metropolis-Regular.otf');
}

function setup(){
	createCanvas(windowWidth, windowHeight);
	background(0);

	mainRingDiameter = width * 5/6;
	smallerRingDiameter = width * 1/2;

	// Connect to server button
	connectToServerButton = new Button({
	    x: width / 2, y: (height / 2),
	    width: 200,   height: 100,
	    align_x: 0,   align_y: 0,
	    content: 'Connect to Server',
	    on_release() {
			requestPlayerSpot();
			menuState = 1;
	    }
	});
	connectToServerButton.style("default", {text_size: 16, text_font: font});

	// TEMP:
	//onConnectionSuccess(2)
}

function draw(){
	background(0);
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
			fill(255);
			circle(width/2, height/2, mainRingDiameter);

			circle_interp = (millis() - lastClickActionTime) / lastClickDelayTime;
  			circle_interp = constrain(circle_interp, 0, 1);
  
  			fill(255, 127, 127);
  			noStroke();
			if(circle_interp > 0 && circle_interp < 1){
				arc(width/2, height/2, 
					mainRingDiameter, mainRingDiameter, 
					-PI/2, -PI/2 + (2*PI*circle_interp));
			}
  			


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

		/*
		textSize(16);
		text("player " + playerNum, width/2, 16);*/
	}
	
}

function circleButtonCallback(id, stateChange){
	if(stateChange == 1){
		// Send toggle signal
		for(let i = 0; i < circleButtons.length; i++){
			circleButtons[i].otherCircleSelected(id);
		}
		
		// Send network signal
		sendCircleButtonClick(id, stateChange);
	}else if(stateChange == -1){
		// Send network signal
		sendCircleButtonClick(id, stateChange);
	}
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

function onConnectionSuccess(playerNumIn){
	playerNum = playerNumIn;
	if(playerNum == 1){
		// Player 1 setup
	}else{
		// Player 2 setup
		for(let i = 0; i < NUM_CIRCLES; i++){
			let rad = (2 * Math.PI) - (i * (2 * Math.PI / NUM_CIRCLES));
			let xPos = Math.cos(rad) * mainRingDiameter / 2;
			let yPos = Math.sin(rad) * mainRingDiameter / 2;
			circleButtons.push(
				new CircleButton(
					(width/2) + xPos, 
					(height/2) + yPos, 
					width/15, 
					i, 
					circleButtonCallback
				)
			);
		}

	}
	menuState = 3;
}

function onConnectionFailed(){
	menuState = 2;
}

function onTransitionToRhythmMode(){
	if(playerNum == 2){
		var NUM_KEYS = 12;
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
}

function calculateAndSendTouchPositionData(clickState){
	var normX = mouseX - (width/2);
	normX = normX / (mainRingDiameter/2);
	var normY = mouseY - (height/2);
	normY = normY / (mainRingDiameter/2);
	sendTouchPositionData(clickState, normX, -normY);
}

function touchStarted(){
	if(playerNum == 1){
		if(millis() > (lastClickActionTime + lastClickDelayTime)){
			lastClickActionTime = millis();
			lastClickDelayTime = 
				lerp(maxDelayTimeSec, 
					minDelayTimeSec, 
					impulseModeProgressionCount / NUM_CIRCLES) * 1000;
					console.log("impulseModeProgressionCount = " + impulseModeProgressionCount);
					console.log("progression frac = " + impulseModeProgressionCount / NUM_CIRCLES);
			calculateAndSendTouchPositionData(1);
		}
	}else if(playerNum == 2){
		for(let i = 0; i < circleButtons.length; i++){
			circleButtons[i].checkClick();
		}

		if(gameState == 1){
			for(let i = 0; i < keyButtons.length; i++){
				keyButtons[i].checkClick();
			}
		}
	}

  	return false;
}

function touchMoved(){
	if(playerNum == 1){
		calculateAndSendTouchPositionData(0);
	}
  	return false;
}

function touchEnded(){
	if(playerNum == 1){
		calculateAndSendTouchPositionData(-1);
	}

  	return false;
}


/*
let typeSelectScene, customizationScene, launchScene;
let currentScene = 0; // typeSelect = 0, customizationScene = 1, launchScene = 2;
let font;

// Return data from typeSelect
let fwk_type = -1; // 0 = high, 1 = mid, 2 = low

// Return data from customizationScene
let fwk_shape = -1; // In order as they appear on the shape select screen 
let fwk_hue = 0; // [0, 1]

// Return data from launchScene
let fwk_scale = 0; // [0, 1]
let fwk_NormXPos = 0; // [0, 1]
let fwk_NormYPos = 0; // [0, 1]


// Return data from Launch Scene

function preload(){
  font = loadFont('assets/Metropolis-Regular.otf');
}

function setup(){
	createCanvas(windowWidth, windowHeight);
	background(0);

	typeSelectScene = new TypeSelectScene();
	typeSelectScene.setup();
	customizationScene = new CustomizationScene();
	customizationScene.setup();
	launchScene = new LaunchScene();
	launchScene.setup();

	typeSelectScene.show();
}

function draw(){
	if(currentScene == 0){
		typeSelectScene.draw();
	}else if(currentScene == 1){
		customizationScene.draw();
	}else if(currentScene == 2){
		launchScene.draw();
	}else{
		print("ERROR - unknown scene #"+currentScene);
	}
}

function mousePressed(){
	if(currentScene == 0){
		typeSelectScene.mousePressedDelegate();
	}else if(currentScene == 1){
		customizationScene.mousePressedDelegate();
	}else if(currentScene == 2){
		launchScene.mousePressedDelegate();
	}else{
		print("ERROR - unknown scene #"+currentScene);
	}
}

function mouseReleased(){
	if(currentScene == 1){
		customizationScene.mouseReleasedDelegate();
	}else if(currentScene == 2){
		launchScene.mouseReleasedDelegate();
	}
}

function windowResized(){
  resizeCanvas(windowWidth, windowHeight);
  aspect = windowWidth/windowHeight;
}

function navigateToScene(targetScene){
	if(currentScene == 0){
		typeSelectScene.hide();
	}else if(currentScene == 1){
		customizationScene.hide();
	}else if(currentScene == 2){
		launchScene.hide();
	}

	if(targetScene == 0){
		typeSelectScene.show();
	}else if(targetScene == 1){
		customizationScene.show();
	}else if(targetScene == 2){
		launchScene.show();
	}
	
	currentScene = targetScene;
}*/

// prevents the mobile browser from processing some default touch events, 
// like swiping left for "back" or scrolling the page.