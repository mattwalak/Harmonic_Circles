function Player2_Stage1(){
    let circleSelectorButtons = [];


    this.setup = function(){
        for(let i = 0; i < NUM_CIRCLES; i++){
            let rad = (2 * Math.PI) - (i * (2 * Math.PI / NUM_CIRCLES));
            let xPos = Math.cos(rad) * MAIN_RING_DIAMETER / 2;
            let yPos = Math.sin(rad) * MAIN_RING_DIAMETER / 2;
            circleSelectorButtons.push(
                new CircleButton(
                    (width/2) + xPos, 
                    (height/2) + yPos, 
                    width/15, 
                    i, 
                    this.circleButtonCallback
                )
            );
        }
    }

    this.draw = function(){
        for(let i = 0; i < circleSelectorButtons.length; i++){
            circleSelectorButtons[i].draw();
        }
    }

    this.circleButtonCallback = function(id, stateChange){
        if(stateChange == 1){
            // Send toggle signal
            for(let i = 0; i < circleSelectorButtons.length; i++){
                if(i != id && circleSelectorButtons[i].isPressedIn){
                    circleSelectorButtons[i].forceDeselect();
                }
            }
        }
        
        networkSend_CircleButtonClick(id, stateChange);
    }

    this.activateCircle = function(circleButtonID){
        circleSelectorButtons[circleButtonID].setIsActive(true);
    }

    this.touchStarted = function(){
        for(let i = 0; i < circleSelectorButtons.length; i++){
			circleSelectorButtons[i].checkClick();
		}
    }

    this.touchMoved = function(){

    }

    this.touchEnded = function(){

    }
}