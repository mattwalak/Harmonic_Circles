function Player2_Stage3(){
    let sourcePos = createVector(width/2, height/2);
    let normSourcePos = createVector(0, 0);
    let sourceScale;

    this.setup = function(){
        sourceScale = width/10;
    }

    this.draw = function(){
        fill(255);

		circle(width/2, height/2, MAIN_RING_DIAMETER);

        fill(0, 255, 0);
        circle(sourcePos.x, sourcePos.y, sourceScale);
    }

    this.sendTouchPosition = function(){
        networkSend_TouchPositionData(1, normSourcePos.x, -normSourcePos.y, 2);
    }

    this.generalTouchFunction = function(){
        normSourcePos.x = (mouseX - (width/2)) / (MAIN_RING_DIAMETER / 2);
        normSourcePos.y = (mouseY - (height/2)) / (MAIN_RING_DIAMETER / 2);

        let dist = Math.sqrt(Math.pow(normSourcePos.x, 2) + Math.pow(normSourcePos.y, 2));
        if(dist > 1){
            normSourcePos.x = normSourcePos.x * 1 / dist;
            normSourcePos.y = normSourcePos.y * 1 / dist;
        }

        sourcePos.x = (width / 2) + (normSourcePos.x * (MAIN_RING_DIAMETER / 2));
        sourcePos.y = (height / 2) + (normSourcePos.y * (MAIN_RING_DIAMETER / 2));

        this.sendTouchPosition();
    }

    this.touchStarted = function(){
        this.generalTouchFunction();
    }

    this.touchMoved = function(){
        this.generalTouchFunction();
    }

    this.touchEnded = function(){
        this.generalTouchFunction();
    }
}