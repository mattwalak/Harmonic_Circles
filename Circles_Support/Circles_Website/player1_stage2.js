function Player1_Stage2(){
    let sourcePos = createVector(width/2, height/2);
    let sourceScale;
    let sourceIsMobile = true;

    this.setup = function(){
        sourceScale = width/10;
    }

    this.draw = function(){

        fill(255);
		circle(width/2, height/2, MAIN_RING_DIAMETER);
    }

    this.unlockSource = function(){
        sourceIsMobile = true;
    }

    this.touchStarted = function(){

    }

    this.touchMoved = function(){

    }

    this.touchEnded = function(){

    }
}