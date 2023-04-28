function Player2_Stage3(){
    let polarReferenceDir = createVector(0, -1);

    let sourcePos = createVector(width/2, height/2);
    let normSourcePos = createVector(0, 0);
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

    let markerDotSize;
    let centerMarginFrac = 0.25; // NOTE: Radius, not diameter

    this.setup = function(){
        sourceScale = width/10;
        angleMode(DEGREES);
        markerDotSize = width / 20;

        sourceDegree = 0;
		sourceMagnitude = 1;
		this.calculateAndSetCartesianFromPolar();
    }

    this.draw = function(){
        colorMode(HSB, 1);
        background(280/360, 1, 0.4);
        colorMode(RGB, 255);

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
			this.calculateAndSetCartesianFromPolar();
            this.sendSourcePosition();
		}
        
        fill(255);
		circle(width/2, height/2, MAIN_RING_DIAMETER);

		colorMode(HSB, 1);
        fill(280/360, 1, 0.4);
		let centerDiameter = centerMarginFrac * MAIN_RING_DIAMETER;
		circle(width/2, height/2, centerDiameter);
        colorMode(RGB, 255);
        
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

    this.calculateAndSetPolarFromCartesian = function(){
        sourceMagnitude = normSourcePos.mag();
		sourceDegree = normSourcePos.angleBetween(polarReferenceDir);
		if(sourceDegree < 0){
			sourceDegree = 360 + sourceDegree;
		}
    }

    this.calculateAndSetCartesianFromPolar = function(){
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

    // Polar coordinates must be calculated!
    this.sendSourcePosition = function(){
        networkSend_TouchPositionData(1, sourceDegree, (sourceMagnitude - centerMarginFrac) / (1 - centerMarginFrac), 2);
    }

    this.generalTouchFunction = function(){
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

        this.calculateAndSetPolarFromCartesian();
        this.sendSourcePosition();
    }

    this.touchStarted = function(){
        this.generalTouchFunction();
    }

    this.touchMoved = function(){
        this.generalTouchFunction();
    }

    this.touchEnded = function(){
        // Activate interps to lock in on important spots in the circle
		this.calculateAndSetPolarFromCartesian();

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
}