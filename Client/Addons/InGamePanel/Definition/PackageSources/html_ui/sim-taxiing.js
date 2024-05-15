var isTaxiing = false;
var taxiSpeed = 5.0;

function getTaxiingStatus()
{
    fetch('https://localhost:7187/api/taxiing/running').then(function(response) {
        return response.json();
      }).then(function(taxi) {
        updateTaxiingStatus(taxi);
        console.log(taxi);
      }).catch(function(err) {
        updateTaxiingStatus(false);
        console.log('Fetch Error :-S', err);
      });
}

function updateTaxiingStatus(taxi) {
    this.isTaxiing = taxi
    if(this.isTaxiing) {
        document.getElementById('taxiingBtn').classList.add("turned-on");
        document.getElementById('taxiingInputContainer').classList.add("turned-on");
    } else {
        document.getElementById('taxiingBtn').classList.remove("turned-on");
        document.getElementById('taxiingInputContainer').classList.remove("turned-on");
    }
}

function getTaxiingSpeed()
{
    fetch('https://localhost:7187/api/taxiing/speed').then(function(response) {
        return response.json();
      }).then(function(speed) {
        this.taxiSpeed = speed
        document.getElementById('taxiingSpeedInput').innerHTML = speed;
        console.log(speed);
      }).catch(function(err) {
        console.log('Fetch Error :-S', err);
      });
}

function onUpdateTaxiingSpeed() {
    setTaxiingSpeed(this.value);
}
function setTaxiingSpeed(speed) {
    this.taxiSpeed = speed;
    fetch('https://localhost:7187/api/taxiing/speed/' + this.taxiSpeed, 
        { 
            method: 'PUT',  
            headers: { 
                'Content-type': 'application/json'
            } 
        }).then(function(response) {
            return response.json();
        }).then(function(speed) {
            this.taxiSpeed = speed
            document.getElementById('taxiingSpeedInput').innerHTML = speed;
            console.log('Taxiing speed updated successfully!');
        }).catch(function(err) {
            console.log('Fetch Error :-S', err);
        });
}



function onTaxiingClicked() {
    //if we are not connected we want to ignore the button
    if(!document.getElementById('connectionBtn').classList.contains("turned-on")) {
        return;
    }

    if(!document.getElementById('taxiingBtn').classList.contains("turned-on")) {
        fetch('https://localhost:7187/api/taxiing', 
        { 
            method: 'PUT',  
            headers: { 
                'Content-type': 'application/json'
            } 
        }).then(function(response) {
            return response.status == 200;
        }).then(function(isTaxiing) {
            updateTaxiingStatus(true);
            if(isTaxiing) {
                console.log('Taxiing successfully!');
            } else {
                console.log('Unable to taxi!');
            }
        }).catch(function(err) {
            updateTaxiingStatus(false);
            console.log('Fetch Error :-S', err);
        });
    } else {
        fetch('https://localhost:7187/api/taxiing', 
        { 
            method: 'DELETE',  
            headers: { 
                'Content-type': 'application/json'
            } 
        }).then(function(response) {
            return response.status == 200;
        }).then(function(isTaxiing) {
            updateTaxiingStatus(false);
            if(isTaxiing) {
                console.log('Turned off taxiing');
            } else {
                console.log('Unable to taxi!');
            }
        }).catch(function(err) {
            updateTaxiingStatus(false);
            console.log('Fetch Error :-S', err);
        });
    }
}

getTaxiingStatus();
getTaxiingSpeed();
document.getElementById("taxiingSpeedInput").addEventListener('change', onUpdateTaxiingSpeed);