function Slider(y_line, span, thickness){
	this.y_line = y_line;
	this.span = span;
	this.thickness = thickness;

	var value = 0;
	var topLeft = [-1, -1];
	var isDragging = false;

	this.draw = function(){
		if(isDragging){
			value = (mouseX - topLeft[0]) / span;
			if(value > 1) value = 1;
			if(value < 0) value = 0;
		}

		colorMode(HSB);
		fill(value * 360, 100, 100);
		topLeft = [width/2 - span/2, y_line - thickness/2];
		rect(topLeft[0], topLeft[1], span, thickness);
		colorMode(RGB);

		fill(255);
		circle(topLeft[0] + thickness/2 + (value * (span - thickness)), topLeft[1] + thickness/2, thickness);
		
	}

	this.onClickStart = function(){
		if((mouseX > topLeft[0]) && (mouseY > topLeft[1])){
			if((mouseX < topLeft[0] + span) && (mouseY < topLeft[1] + thickness)){
				isDragging = true;
			}
		}
	}

	this.onClickEnd = function(){
		isDragging = false;
	}

	this.getValue = function(){
		return value;
	}


}