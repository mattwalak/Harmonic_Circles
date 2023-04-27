    let polarReferenceDir;
	
	let sourcePos = 0;
	let normSourcePos = 0;
    let sourceMagnitude = 1;
    let sourceDegree = 0;

    let sourceScale;

    let isMagnitudeInterping = false;
    let magnitudeInterpGoal = -1;
    let magnitudeInterpVelocity = 10; // px/sec
    let isDegreeInterping = false;
    let degreeInterpGoal = -1;
    let degreeInterpVelocity = 10; // deg/sec

	let MAIN_RING_DIAMETER = 0;
	let SMALLER_RING_DIAMETER = 0;

    function setup(){
		createCanvas(windowWidth, windowHeight);
		background(0);

        sourceScale = width/10;
        angleMode(DEGREES);
		sourcePos = createVector(width/2, height/2);
		normSourcePos = createVector(0, 0);
		polarReferenceDir = createVector(0, -1);

		sourceDegree = 75;
		sourceMagnitude = 0.4;
		calculateAndSetCartesianFromPolar();

		MAIN_RING_DIAMETER = width * 5/6;
		SMALLER_RING_DIAMETER = width * 1/2;
    }

    function draw(){
		background(0);
        fill(255);

		circle(width/2, height/2, MAIN_RING_DIAMETER);

        fill(0, 255, 0);
        circle(sourcePos.x, sourcePos.y, sourceScale);
    }

    function calculateAndSetPolarFromCartesian(){
		sourceMagnitude = normSourcePos.mag();
		sourceDegree = normSourcePos.angleBetween(polarReferenceDir);
		if(sourceDegree < 0){
			sourceDegree = 360 + sourceDegree;
		}
    }

    function calculateAndSetCartesianFromPolar(){
		console.log("here");
        let p5angle = sourceDegree;
		if(p5angle > 180){
			p5angle = p5angle - 360;
		}

		normSourcePos = polarReferenceDir.copy();
		normSourcePos.rotate(-p5angle) 
		normSourcePos.setMag(sourceMagnitude);
		sourcePos = normSourcePos.copy();
		
		sourcePos.x = sourcePos.x * (MAIN_RING_DIAMETER/2);
		sourcePos.y = sourcePos.y * (MAIN_RING_DIAMETER/2);

		sourcePos.x = sourcePos.x + (width/2);
		sourcePos.y = sourcePos.y + (height/2);
    }

    function sendTouchPosition(){
        var normX = sourcePos.x - (width/2);
        normX = normX / (MAIN_RING_DIAMETER/2);
        var normY = sourcePos.y - (height/2);
        normY = normY / (MAIN_RING_DIAMETER/2);
        // networkSend_TouchPositionData(1, normX, -normY, 1);
    }

    function generalTouchFunction(){
        normSourcePos.x = (mouseX - (width/2)) / (MAIN_RING_DIAMETER / 2);
        normSourcePos.y = (mouseY - (height/2)) / (MAIN_RING_DIAMETER / 2);

        let dist = Math.sqrt(Math.pow(normSourcePos.x, 2) + Math.pow(normSourcePos.y, 2));
        if(dist > 1){
            normSourcePos.x = normSourcePos.x * 1 / dist;
            normSourcePos.y = normSourcePos.y * 1 / dist;
        }

        sourcePos.x = (width / 2) + (normSourcePos.x * (MAIN_RING_DIAMETER / 2));
        sourcePos.y = (height / 2) + (normSourcePos.y * (MAIN_RING_DIAMETER / 2));

		calculateAndSetPolarFromCartesian();
        sendTouchPosition();
    }

    function touchStarted(){
        generalTouchFunction();
    }

    function touchMoved(){
        generalTouchFunction();
    }

    function touchEnded(){
        // Activate interps to lock in a important spots in the circle
		

        // this.generalTouchFunction();
    }