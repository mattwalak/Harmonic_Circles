function CircleButton(posX, posY, diameter, id, onClickCallback){
    this.posX = posX;
    this.posY = posY;
    this.diameter = diameter;
    this.id = id;
    this.onClickCallback = onClickCallback;
    this.isPressedIn = false;
    this.circleHasBeenActivated = false;
    this.enabled = true;

    this.strokeColor = color(255);
    this.disabledColor = color(160);
    this.activeSelectedColor = color(0, 255, 0);
    this.activeNotSelectedColor = color(160, 255, 160);
    this.inactiveSelectedColor = color(255, 0, 0);
    this.inactiveNotSelectedColor = color(255, 160, 160);

    this.draw = function(){
        if(this.enabled){
            if(this.circleHasBeenActivated){
                if(this.isPressedIn){
                    strokeWeight(4);
                    stroke(this.strokeColor);
                    fill(this.activeSelectedColor);
                }else{
                    fill(this.activeNotSelectedColor);
                }
            }else{
                if(this.isPressedIn){
                    strokeWeight(4);
                    stroke(this.strokeColor);
                    fill(this.inactiveSelectedColor);
                }else{
                    fill(this.inactiveNotSelectedColor);
                }
            }
        }else{
            fill(this.disabledColor);
        }

        circle(posX, posY, diameter);
        noStroke();
    }

    this.checkClick = function(){
        var dist = Math.sqrt(Math.pow(mouseX - this.posX, 2) + Math.pow(mouseY - this.posY, 2));
        if (dist < (this.diameter / 2)){
            this.isPressedIn = !this.isPressedIn;
            onClickCallback(this.id, (this.isPressedIn ? 1 : -1));
        }
    }

    this.otherCircleSelected = function(otherId){
        if(id != otherId){
            this.isPressedIn = false;
            onClickCallback(this.id, -1);
        }
    }

    this.wasJustActivated = function(){
        this.circleHasBeenActivated = true;
    }

}