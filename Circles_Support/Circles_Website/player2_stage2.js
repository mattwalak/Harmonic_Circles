function Player2_Stage2(){
    let circleSelectorButtons = [];
    let keyButtons = [];
    let currentKey = 0;

    let requiredCirclesBuffer = [];
    let activatedCirclesBuffer = [];
    let completedKeysBuffer = [];

    let doneReceivingFocusedPartials = false;

    this.setup = function(){
        // Circle Selector Buttons
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

            circleSelectorButtons[i].inactiveNotSelectedColor = color(160, 255, 160);
            circleSelectorButtons[i].inactiveSelectedColor = color(0, 255, 0);
            circleSelectorButtons[i].setIsEnabled(false);
        }
        
        // Key buttons
        for(let i = 0; i < NUM_KEYS; i++){
			let rad = (2 * Math.PI) - (i * (2 * Math.PI / NUM_KEYS));
			let xPos = Math.cos(rad) * SMALLER_RING_DIAMETER / 2;
			let yPos = Math.sin(rad) * SMALLER_RING_DIAMETER / 2;
			keyButtons.push(
				new CircleButton(
					(width/2) + xPos, 
					(height/2) + yPos, 
					width/15, 
					i, 
					this.keyButtonCallback
				)
			);
            keyButtons[i].inactiveSelectedColor = color(255, 0, 0);
            keyButtons[i].inactiveNotSelectedColor = color(255, 160, 160);
		}

        // Reset buffers
        for(let i = 0; i < NUM_CIRCLES; i++){
            requiredCirclesBuffer.push(false);
            activatedCirclesBuffer.push(false);
        }

        // Key buffer - Resets once per level
        for(let i = 0; i < NUM_KEYS; i++){
            completedKeysBuffer.push(false);
        }

		keyButtons[0].isPressedIn = true;
    }

    this.draw = function(){
        colorMode(HSB, 1);
        background(230/360, 1, 0.4);
        colorMode(RGB, 255);

        for(let i = 0; i < circleSelectorButtons.length; i++){
            circleSelectorButtons[i].draw();
        }

        for(let i = 0; i < keyButtons.length; i++){
            keyButtons[i].draw();
        }
    }

    this.circleButtonCallback = function(id, stateChange){
        // Mark our activatedCircles buffer
        activatedCirclesBuffer[id] = true;

        // Check if we have "passed" this key
        let keyPassed = doneReceivingFocusedPartials;
        if(doneReceivingFocusedPartials){
            for(let i = 0; i < NUM_CIRCLES; i++){
                if(requiredCirclesBuffer[i]){
                    if(!activatedCirclesBuffer[i]){
                        keyPassed = false;
                        break;
                    }
                }
            } 
        }
        
        // Send network signal for sound generation
        // NETWORK - sound for activated key
        networkSend_CircleButtonClick(id, 1);

        if(keyPassed){
            if(!completedKeysBuffer[currentKey]){
                completedKeysBuffer[currentKey] = true;

                // NETWORK  - key passed
                keyButtons[currentKey].setIsActive(true);
                networkSend_KeyComplete(currentKey);
            }
        }
    }

    this.keyButtonCallback = function(id, stateChange){
        if(stateChange == 1){
            // On keyChange functionality
            currentKey = id;

            // Send toggle signal
            for(let i = 0; i < keyButtons.length; i++){
                if(i != id && keyButtons[i].isPressedIn){
                    keyButtons[i].forceDeselect();
                }
            }

            // Reset all the circleButtons
            for(let i = 0; i < circleSelectorButtons.length; i++){
                circleSelectorButtons[i].setIsActive(false);
                circleSelectorButtons[i].setIsEnabled(false);
                circleSelectorButtons[i].setIsPressedIn(false);
            }

            // Reset buffers
            for(var i = 0; i < NUM_CIRCLES; i++){
                requiredCirclesBuffer[i] = false;
                activatedCirclesBuffer[i] = false;
            }

            doneReceivingFocusedPartials = false;
        }



        networkSend_KeyChange(id);
    }

    this.startOfFocusOnPartial = function(){
        doneReceivingFocusedPartials = false;
        for(var i = 0; i < NUM_CIRCLES; i++){
            requiredCirclesBuffer[i] = false;
        }
    }

    this.focusOnPartial = function(circleID){
        requiredCirclesBuffer[circleID] = true;

        circleSelectorButtons[circleID].setIsEnabled(true);
    }

    this.endOfFocusOnPartial = function(){
        doneReceivingFocusedPartials = true;
    }

    this.touchStarted = function(){
        for(let i = 0; i < circleSelectorButtons.length; i++){
			circleSelectorButtons[i].checkClick();
		}

        for(let i = 0; i < keyButtons.length; i++){
			keyButtons[i].checkClick();
		}
    }

    this.touchMoved = function(){

    }

    this.touchEnded = function(){

    }
}