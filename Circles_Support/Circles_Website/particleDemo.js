function ParticleDemo(x, y, size){
	this.x = x;
	this.y = y;
	this.size = size;

	this.draw = function(shape, color){
		// 0 = triangle, 1 = square, 2 = circle
		fill(color);
		if(shape == 0){
			triangle(x - (size/2), y + (size/2), x + (size/2), y + (size/2), x, y - (size/2));
		}else if (shape == 1){
			rect(x - (size/2), y - (size/2), size, size);
		}else{
			circle(x, y, size);
		}
	}


}