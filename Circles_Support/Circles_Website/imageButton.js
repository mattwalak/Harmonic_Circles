function ImageButton(posX, posY, sizeX, sizeY, img, onClickCallback, buttonID){
	this.posX = posX;
	this.posY = posY;
	this.sizeX = sizeX;
	this.sizeY = sizeY;
	this.img = img;
	this.onClickCallback = onClickCallback;
	this.buttonID = buttonID;

	var tintColor = [255, 255, 255];

	this.draw = function(){
		tint(tintColor[0], tintColor[1], tintColor[2]);
		
		image(img, posX, posY, sizeX, sizeY);
	}

	this.checkClick = function(){
		if((mouseX > posX) && (mouseY > posY)){
			if((mouseX < (posX + sizeX)) && (mouseY < (posY + sizeY))){
				onClickCallback(buttonID);
			}
		}
	}

	this.applyTint = function(r, g, b){
		tintColor[0] = r;
		tintColor[1] = g;
		tintColor[2] = b;
	}

}