function PlayerConnectionStage(){
    let connectToServerButton;

    // 0 = connect to server button
    // 1 = awaiting server response
    // 2 = connection failed
    let menuState = 0; 

    this.setup = function(){
        // Connect to server button
        connectToServerButton = new Button({
            x: width / 2, y: (height / 2),
            width: 200,   height: 100,
            align_x: 0,   align_y: 0,
            content: 'Connect to Server',
            on_release() {
                networkSend_requestPlayerSpot();
                menuState = 1;
            }
        });
        connectToServerButton.style("default", {text_size: 16, text_font: font});

        // TEMP:
	    //onConnectionSuccess(2)
    }

    this.draw = function(){
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
        }
        /*
        else if(menuState == 3){
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

    /*
    this.onConnectionSuccess = function(playerNumIn){
        playerNum = playerNumIn;
        if(playerNum == 1){
            // Player 1 setup
        }else{
            // Player 2 setup
            
    
        }
        menuState = 3;
    }*/
    
    this.onConnectionFailed = function(){
        menuState = 2;
    }

    this.touchStarted = function(){

    }

    this.touchMoved = function(){

    }

    this.touchEnded = function(){

    }
}