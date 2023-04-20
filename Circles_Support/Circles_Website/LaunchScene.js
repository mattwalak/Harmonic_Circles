let skyAspect = 1.77; // Placeholder until we get the network communication going

let skyBoxMargin = 20;

var calculatedSkyBoxWidth;
var calculatedSkyBoxHeight;
var skyBoxUpperLeftCorner;
var validLaunchUpperCorner;
var validLaunchLowerCorner;

var isInClick = false;
var clickStartTime;
var click_x, click_y;
var TIME_TO_MAX_CLICK = 5000;
var MAX_CIRCLE_RADIUS = 250;

var MIN_ANIM_SIZE = 100;
var MAX_ANIM_SIZE = 300;
var ANIM_TIME = 750; // At this time, the firework image is completely offscreen
var activeAnimData = []; // ALWAYS ordered with oldest first. New anims get added to end
var fireworkImg;
// Format: {startTime: float, scale: float [0, 1], xLine: float, normX: float, normY: float}

var backTextButton_2;

function LaunchScene(){
	this.setup = function(){
		fireworkImg = loadImage('assets/graphics/FireworkBody.png');

		backTextButton_2 = new Button({
	    x: width / 2, y: (11 * height / 12),
	    width: 200,   height: 30,
	    align_x: 0,   align_y: 0,
	    content: 'Back',
	    on_release() {
	      navBackClicked_2(-1);
	    }
	  });
	  backTextButton_2.style("default", {
	    text_size: 16, text_font: font});

	}

	this.show = function(){
		calculatedSkyBoxWidth = width - 2*skyBoxMargin;
		print(calculatedSkyBoxWidth);
		if(calculatedSkyBoxWidth > 400){
			calculatedSkyBoxWidth = 400;
		}
		calculatedSkyBoxHeight = calculatedSkyBoxWidth / skyAspect;
		skyBoxUpperLeftCorner = [(width/2) - (calculatedSkyBoxWidth/2), (height/2) - (calculatedSkyBoxHeight/2)];

		if(fwk_type == 0){
			validLaunchUpperCorner = [skyBoxUpperLeftCorner[0], 
																skyBoxUpperLeftCorner[1] + (calculatedSkyBoxHeight/3)*0];
		}else if(fwk_type == 1){
			validLaunchUpperCorner = [skyBoxUpperLeftCorner[0], 
																skyBoxUpperLeftCorner[1] + (calculatedSkyBoxHeight/3)*1];
		}else if(fwk_type == 2){
			validLaunchUpperCorner = [skyBoxUpperLeftCorner[0], 
																skyBoxUpperLeftCorner[1] + (calculatedSkyBoxHeight/3)*2];
		}

		validLaunchLowerCorner = [validLaunchUpperCorner[0] + calculatedSkyBoxWidth,
															validLaunchUpperCorner[1] + calculatedSkyBoxHeight/3];

		isInClick = false;
	}

	this.hide = function(){

	}

	this.draw = function(){
		// print("active anims = " + activeAnimData.length);
		background(0);

		backTextButton_2.draw();

		// Page title
		fill(255);
		textFont(font);
		textSize(16);
		textAlign(CENTER, CENTER);
		var title = "";
		if(fwk_type == 0){
			title = "Light";
		}else if(fwk_type == 1){
			title = "Medium";
		}else if(fwk_type == 2){
			title = "Heavy";
		}
		text(title, width/2, 20);

		textAlign(CENTER, CENTER);
		textSize(36);
		text("Launch", width/2, height/8);


		textSize(16);
		text("Tap in the blue area to launch a firework! \n Hold down longer for larger fireworks",
			width/2, 3 * height / 16);


		// Draw skyBox
		fill(127);
		rect(skyBoxUpperLeftCorner[0], skyBoxUpperLeftCorner[1], 
			calculatedSkyBoxWidth, calculatedSkyBoxHeight);
		// Draw valid launch area
		fill(102, 255, 255);
		rect(validLaunchUpperCorner[0], validLaunchUpperCorner[1],
			calculatedSkyBoxWidth, calculatedSkyBoxHeight/3);

		// Draw circle if in click
		if(isInClick){
			var t = (millis() - clickStartTime) / TIME_TO_MAX_CLICK;
			t = constrain(t, 0, 1);
			var circleRadius = MAX_CIRCLE_RADIUS * t;

			fill(255, 255, 102, 190);
			circle(click_x, click_y, circleRadius*2);

			fill(0);
			circle(click_x, click_y, 8);
		}

		// Handle active animations

		// first prune the animation data
		var currentTime = millis();
		while(activeAnimData.length > 0 && millis() >= (activeAnimData[0].startTime + ANIM_TIME)){
			sendFirework(fwk_type, fwk_shape, fwk_hue, activeAnimData[0].scale, 
				activeAnimData[0].normX, activeAnimData[0].normY);
			activeAnimData.shift();
		}

		for(var i = 0; i < activeAnimData.length; i++){
			var t = (millis() - activeAnimData[i].startTime) / ANIM_TIME;
			t = constrain(t, 0, 1);

			var scale = lerp(MIN_ANIM_SIZE, MAX_ANIM_SIZE, activeAnimData[i].scale);
			var top_y = -scale;

			var qY = lerp(height, -scale, t);
			image(fireworkImg, activeAnimData[i].x_line, qY, scale, scale);
		}
	}

	this.mousePressedDelegate = function(){
		if(mouseX > validLaunchUpperCorner[0] && mouseY > validLaunchUpperCorner[1]){
			if(mouseX < validLaunchLowerCorner[0] && mouseY < validLaunchLowerCorner[1]){
				isInClick = true;
				clickStartTime = millis();
				click_x = mouseX;
				click_y = mouseY;
			}
		}
	}

	this.mouseReleasedDelegate = function(){
		if(isInClick){
			// New launch firework animation!
			var t = (millis() - clickStartTime) / TIME_TO_MAX_CLICK;
			t = constrain(t, 0, 1);
			var actualScale = lerp(MIN_ANIM_SIZE, MAX_ANIM_SIZE, t);
			var x = lerp(0, width - actualScale, random());
			var animData = {startTime: millis(), scale: t, x_line: (click_x - actualScale/2)};
			animData.normX = (click_x - skyBoxUpperLeftCorner[0]) / calculatedSkyBoxWidth;
			animData.normY = (click_y - skyBoxUpperLeftCorner[1]) / calculatedSkyBoxHeight;
			activeAnimData.push(animData);
		}

		isInClick = false;
	}
}

function navBackClicked_2(ID){
	navigateToScene(1);
}