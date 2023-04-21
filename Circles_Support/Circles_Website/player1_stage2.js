function Player1_Stage2(){
    let sourcePos = createVector(width/2, height/2);
    let sourceScale;
    let sourceIsMobile = true;

    this.setup = function(){
        sourceScale = width/10;
    }

    this.draw = function(){
        if(sourceIsMobile){
            fill(255);
        }else{
            fill(255, 160, 160);
        }
        
		circle(width/2, height/2, MAIN_RING_DIAMETER);

        fill(0, 255, 0);
        circle(sourcePos.x, sourcePos.y, sourceScale);
    }

    this.sendTouchPosition = function(){
        var normX = sourcePos.x - (width/2);
        normX = normX / (MAIN_RING_DIAMETER/2);
        var normY = sourcePos.y - (height/2);
        normY = normY / (MAIN_RING_DIAMETER/2);
        networkSend_TouchPositionData(1, normX, -normY);
    }

    this.unlockSource = function(){
        sourceIsMobile = true;
    }

    this.touchStarted = function(){
        if(sourceIsMobile){
            sourcePos.x = mouseX;
            sourcePos.y = mouseY;
            sourceIsMobile = false;
            this.sendTouchPosition();
        }
    }

    this.touchMoved = function(){

    }

    this.touchEnded = function(){

    }
}