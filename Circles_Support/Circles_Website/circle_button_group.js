function CircleButtonGroup(){
    this.buttonsList = [];

    this.addButtonToGroup = function(buttonToAdd){
        // CircleButtonGroup 
        buttonToAdd.onClickCallback = this.onClickCallbackDelegator;

        this.buttonsList.push(buttonToAdd);
    }

    this.onClickCallbackDelegator = function(id, stateChange){

    }

}