function Player1_Stage1(){
    let lastClickActionTime = -1;
    let lastClickDelayLength = -1;
    let maxDelayTimeSec = 1;
    let minDelayTimeSec = 0.1;

    this.setup = function(){

    }

    this.draw = function(){
        colorMode(HSB, 1);
        background(180/360, 1, 0.4);
        colorMode(RGB, 255);
        
        fill(255);
		circle(width/2, height/2, MAIN_RING_DIAMETER);
        
		circle_interp = (millis() - lastClickActionTime) / lastClickDelayLength;
  		circle_interp = constrain(circle_interp, 0, 1);
        
  		fill(255, 127, 127);
  		noStroke();
		
        if(circle_interp > 0 && circle_interp < 1){
			arc(width/2, height/2, 
                MAIN_RING_DIAMETER, MAIN_RING_DIAMETER, 
				-PI/2, -PI/2 + (2*PI*circle_interp));
		}
    }

    this.calculateAndSendTouchPositionData = function(clickState){
        var normX = mouseX - (width/2);
        normX = normX / (MAIN_RING_DIAMETER/2);
        var normY = mouseY - (height/2);
        normY = normY / (MAIN_RING_DIAMETER/2);
        networkSend_TouchPositionData(clickState, normX, -normY);
    }

    this.touchStarted = function(){
        if(millis() > (lastClickActionTime + lastClickDelayLength)){
			lastClickActionTime = millis();
			lastClickDelayLength = 
				lerp(maxDelayTimeSec, 
					minDelayTimeSec, 
					stage1ProgressionCount / NUM_CIRCLES) * 1000;
			this.calculateAndSendTouchPositionData(1);
		}
    }

    this.touchMoved = function(){

    }

    this.touchEnded = function(){

    }
}