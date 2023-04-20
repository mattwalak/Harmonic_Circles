function SkySelectionBox(x, y, boxWidth_in, boxHeight_in){
	this.x = x;
	this.y = y;
	this.boxWidth = boxWidth_in;
	this.boxHeight = boxHeight_in;

	this.display = function(){
		fill(127, 0, 0);
		rect(x, y, this.boxWidth, this.boxHeight);
	}

	this.click = function(mouseX, mouseY){
		if(mouseX > x && mouseX < (x + this.boxWidth)){
			if(mouseY > y && mouseY < (y + this.boxHeight)){
				click_x = (mouseX - x)/this.boxWidth;
				click_y = (mouseY - y)/this.boxHeight;

				return {
					'x': click_x,
					'y': click_y
				};
			}
		}

		return {
			'clickMissed': true
		};
	}


}