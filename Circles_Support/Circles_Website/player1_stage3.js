function Player1_Stage3(){
    let sourcePos = createVector(width/2, height/2);
    let sourceMagnitude = 1;
    let sourceDegree = 0;

    let sourceScale;

    let isMagnitudeInterping = false;
    let magnitudeInterpGoal = -1;
    let magnitudeInterpVelocity = 10; // px/sec
    let isDegreeInterping = false;
    let degreeInterpGoal = -1;
    let degreeInterpVelocity = 10; // deg/sec

    this.setup = function(){
        sourceScale = width/10;
        angleMode(DEGREES);
    }

    this.draw = function(){
        colorMode(HSB, 1);
        background(280/360, 1, 0.4);
        colorMode(RGB, 255);

        fill(255);
		circle(width/2, height/2, MAIN_RING_DIAMETER);

        fill(0, 255, 0);
        circle(sourcePos.x, sourcePos.y, sourceScale);
    }

    this.calculateAndSetRadialFromCartesian = function(){

    }

    this.calculateAndSetCartesianFromRadial = function(){

    }

    this.sendTouchPosition = function(){
        var normX = sourcePos.x - (width/2);
        normX = normX / (MAIN_RING_DIAMETER/2);
        var normY = sourcePos.y - (height/2);
        normY = normY / (MAIN_RING_DIAMETER/2);
        networkSend_TouchPositionData(1, normX, -normY, 1);
    }

    this.generalTouchFunction = function(){
        sourcePos.x = mouseX;
        sourcePos.y = mouseY;
        this.sendTouchPosition();
    }

    this.touchStarted = function(){
        this.generalTouchFunction();
    }

    this.touchMoved = function(){
        this.generalTouchFunction();
    }

    this.touchEnded = function(){
        // Activate interps to lock in a important spots in the circle


        // this.generalTouchFunction();
    }
}