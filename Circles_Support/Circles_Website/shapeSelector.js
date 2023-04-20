function ShapeSelector(posX, posY, nX, nY, shapeSize, padding, imgArray){
	this.posX = posX;
	this.posY = posY;
	this.nX = nX;
	this.nY = nY;
	this.shapeSize = shapeSize;
	this.padding = padding;
	this.imgArray = imgArray;

	var selectedIndex = 0;
	var hue = 0;

	var upperLeftCorners = []; // Cache these here so we don't have to calculate them every click
	var lowerRightCorners = [];

	this.draw = function(){
		for(var x = 0; x < nX; x++){
			for(var y = 0; y < nY; y++){
				var index = y*nX + x;
				var img = imgArray[index];
				var qX = posX + (x * (shapeSize + padding));
				var qY = posY + (y * (shapeSize + padding));

				colorMode(HSB);
				if(index == selectedIndex){
					tint(hue*360, 100, 100);
				}else{
					tint(hue*360, 0, 100);
				}
				image(img, qX, qY, shapeSize, shapeSize);
				colorMode(RGB);

				upperLeftCorners[index] = [qX, qY];
				lowerRightCorners[index] = [qX + shapeSize, qY + shapeSize];
			}
		}
	}

	this.checkClick = function(){
		for(var x = 0; x < nX; x++){
			for(var y = 0; y < nY; y++){
				var index = y*nX + x;
				if(mouseX > upperLeftCorners[index][0] && mouseY > upperLeftCorners[index][1]){
					if(mouseX < lowerRightCorners[index][0] && mouseY < lowerRightCorners[index][1]){
						selectedIndex = index;
					}
				}
			}
		}
	}

	this.setHue = function(hueIn){
		hue = hueIn;
	}

	this.getSelectedIndex = function(){
		return selectedIndex;
	}
}