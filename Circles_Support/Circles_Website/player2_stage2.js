function Player2_Stage2(){
    let circleSelectorButtons = [];
    let keyButtons = [];

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
            circleSelectorButtons[i].disable();
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

		keyButtons[0].isPressedIn = true;
    }

    this.draw = function(){
        for(let i = 0; i < circleSelectorButtons.length; i++){
            circleSelectorButtons[i].draw();
        }

        for(let i = 0; i < keyButtons.length; i++){
            keyButtons[i].draw();
        }
    }

    this.circleButtonCallback = function(id, stateChange){
        console.log("CIRCLE BUTTON - CONFIRMING WE ARE CALLING THE CALLBAck for stAgE 2");
    }

    this.keyButtonCallback = function(id, stateChange){
        if(stateChange == 1){
            // Send toggle signal
            for(let i = 0; i < keyButtons.length; i++){
                if(i != id && keyButtons[i].isPressedIn){
                    keyButtons[i].forceDeselect();
                }
            }
        }

        networkSend_KeyChange(id);
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