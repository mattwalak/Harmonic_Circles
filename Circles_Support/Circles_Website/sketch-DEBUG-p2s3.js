    let polarReferenceDir;
	
	let sourcePos = 0;
	let normSourcePos = 0;
    let sourceMagnitude = 1;
    let sourceDegree = 0;

    let sourceScale;

    let isMagnitudeInterping = false;
    let magnitudeInterpGoal = -1;
    let magnitudeInterpVelocity = 1;
	let magnitudeInterpVelocityStartValue = 1;
	let magnitudeInterpAcceleration = 10;
	let magnitudeInterpDirection = -1; // Negative is decreasing, positive is increasing

    let isDegreeInterping = false;
    let degreeInterpGoal = -1;
    let degreeInterpVelocity = 20;
	let degreeInterpVelocityStartValue = 20; // deg/sec
	let degreeInterpAcceleration = 1000;
	let degreeInterpDirection = -1; // Negative is decreasing, positive is increasing

	let MAIN_RING_DIAMETER = 0;
	let SMALLER_RING_DIAMETER = 0;
	let markerDotSize;
	let centerMarginFrac = 0.25; // NOTE: Radius, not diameter

    function setup(){
		createCanvas(windowWidth, windowHeight);
		background(0);

        sourceScale = width/10;
        angleMode(DEGREES);
		sourcePos = createVector(width/2, (height/2) - (MAIN_RING_DIAMETER/2));
		normSourcePos = createVector(0, 1);
		polarReferenceDir = createVector(0, -1);
		

		MAIN_RING_DIAMETER = width * 5/6;
		SMALLER_RING_DIAMETER = width * 1/2;
		markerDotSize = width / 20;

		sourceDegree = 0;
		sourceMagnitude = 1;
		calculateAndSetCartesianFromPolar();
    }

    function draw(){
		// Update snap stuff
		let coordinateUpdateScheduled = (isDegreeInterping || isMagnitudeInterping);

		if(isDegreeInterping){
			let change = (deltaTime/1000) * degreeInterpVelocity;
			sourceDegree = sourceDegree + (change * degreeInterpDirection);
			if(degreeInterpDirection < 0){
				if(sourceDegree < degreeInterpGoal){
					sourceDegree = degreeInterpGoal;
					isDegreeInterping = false;
				}
			}else{
				if(sourceDegree > degreeInterpGoal){
					sourceDegree = degreeInterpGoal;
					isDegreeInterping = false;
				}
			}

			degreeInterpVelocity = degreeInterpVelocity + ((deltaTime/1000) * degreeInterpAcceleration);
		}

		if(isMagnitudeInterping){
			let change = (deltaTime/1000) * magnitudeInterpVelocity;
			sourceMagnitude = sourceMagnitude + (change * magnitudeInterpDirection);
			if(magnitudeInterpDirection < 0){
				if(sourceMagnitude < magnitudeInterpGoal){
					sourceMagnitude = magnitudeInterpGoal;
					isMagnitudeInterping = false;
				}
			}else{
				if(sourceMagnitude > magnitudeInterpGoal){
					sourceMagnitude = magnitudeInterpGoal;
					isMagnitudeInterping = false;
				}
			}

			magnitudeInterpVelocity = magnitudeInterpVelocity + ((deltaTime/1000) * magnitudeInterpAcceleration);
		}

		if(coordinateUpdateScheduled){
			calculateAndSetCartesianFromPolar();
		}

		background(0);
        fill(255);

		circle(width/2, height/2, MAIN_RING_DIAMETER);

		fill(0);
		let centerDiameter = centerMarginFrac * MAIN_RING_DIAMETER;
		circle(width/2, height/2, centerDiameter);

		fill(160, 160, 160);
		circle((width/2) + (centerDiameter / 2), height/2, markerDotSize);
		circle((width/2) - (centerDiameter / 2), height/2, markerDotSize);
		circle(width/2, (height/2) + (centerDiameter / 2), markerDotSize);
		circle(width/2, (height/2) - (centerDiameter / 2), markerDotSize);

		circle((width/2) + (MAIN_RING_DIAMETER/2), height/2, markerDotSize);
		circle((width/2) - (MAIN_RING_DIAMETER/2), height/2, markerDotSize);
		circle(width/2, (height/2) + (MAIN_RING_DIAMETER/2), markerDotSize);
		circle(width/2, (height/2) - (MAIN_RING_DIAMETER/2), markerDotSize);

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

		console.log(""+normSourcePos);
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
            normSourcePos.setMag(1);
        }else if(dist < centerMarginFrac){
			normSourcePos.setMag(centerMarginFrac);
		}

        sourcePos.x = (width / 2) + (normSourcePos.x * (MAIN_RING_DIAMETER / 2));
        sourcePos.y = (height / 2) + (normSourcePos.y * (MAIN_RING_DIAMETER / 2));

        sendTouchPosition();
    }

    function touchStarted(){
        generalTouchFunction();
    }

    function touchMoved(){
        generalTouchFunction();
    }

    function touchEnded(){
        // Activate interps to lock in on important spots in the circle
		calculateAndSetPolarFromCartesian();

		// Degree interp
		let backGoal = Math.floor(sourceDegree / 90);
		let frontGoal = backGoal + 1; // No mod here cause we use 360 instead of 0 for front goal
		backGoal = backGoal * 90;
		frontGoal = frontGoal * 90;

		let distToBackGoal = Math.abs(sourceDegree - backGoal);
		let distToFrontGoal = Math.abs(frontGoal - sourceDegree);

		if(distToBackGoal < distToFrontGoal){
			degreeInterpGoal = backGoal;
			degreeInterpDirection = -1;
		}else{
			degreeInterpGoal = frontGoal;
			degreeInterpDirection = 1;
		}

		degreeInterpVelocity = degreeInterpVelocityStartValue;
		isDegreeInterping = true;

		// Magnitude interp
		if(sourceMagnitude > 1 - ((1 - centerMarginFrac)/2)){
			magnitudeInterpGoal = 1;
			magnitudeInterpDirection = 1;
		}else{
			magnitudeInterpGoal = centerMarginFrac;
			magnitudeInterpDirection = -1;
		}

		magnitudeInterpVelocity = magnitudeInterpVelocityStartValue;
		isMagnitudeInterping = true;
    }