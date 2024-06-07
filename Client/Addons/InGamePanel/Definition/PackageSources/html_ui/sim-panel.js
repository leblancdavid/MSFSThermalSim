var currentPanel = 0;

function nextPanel() {
    currentPanel++;
    if(currentPanel > 2) {
        currentPanel = 0;
    }
    showPanel(currentPanel);
}

function previousPanel() {
    currentPanel--;
    if(currentPanel < 0) {
        currentPanel = 2;
    }
    showPanel(currentPanel);
}

function showPanel(currentPanel) {
    if(currentPanel == 0) {
        document.getElementById('mainPanel').style.display = "flex";
        document.getElementById('configPanel').style.display = "none";
        document.getElementById('turbulencePanel').style.display = "none";
    } else if(currentPanel == 1) {
        document.getElementById('mainPanel').style.display = "none";
        document.getElementById('configPanel').style.display = "flex";
        document.getElementById('turbulencePanel').style.display = "none";
    } else if(currentPanel == 2) {
        document.getElementById('mainPanel').style.display = "none";
        document.getElementById('configPanel').style.display = "none";
        document.getElementById('turbulencePanel').style.display = "flex";
    }

}
















function Binding(b) {
  _this = this
  this.elementBindings = []
  this.value = b.object[b.property]
  this.valueGetter = function(){
      return _this.value;
  }
  this.valueSetter = function(val){
      _this.value = val
      for (var i = 0; i < _this.elementBindings.length; i++) {
          var binding=_this.elementBindings[i]
          binding.element[binding.attribute] = val
      }
  }
  this.addBinding = function(element, attribute, event){
      var binding = {
          element: element,
          attribute: attribute
      }
      if (event){
          element.addEventListener(event, function(event){
              _this.valueSetter(element[attribute]);
          })
          binding.event = event
      }       
      this.elementBindings.push(binding)
      element[attribute] = _this.value
      return _this
  }

  Object.defineProperty(b.object, b.property, {
      get: this.valueGetter,
      set: this.valueSetter
  }); 

  b.object[b.property] = this.value;
}



/*
var obj = {a:123}
var myInputElement1 = document.getElementById("myText1")
var myInputElement2 = document.getElementById("myText2")
var myDOMElement = document.getElementById("myDomElement")

new Binding({
	object: obj,
	property: "a"
})
.addBinding(myInputElement1, "value", "keyup")
.addBinding(myInputElement2, "value", "keyup")
.addBinding(myDOMElement, "innerHTML")

obj.a = 456;
*/

