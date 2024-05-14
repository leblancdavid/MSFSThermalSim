function onTaxiingTabClicked() {
    
    document.getElementById('connectionTabBtn').classList.remove("selected");
    document.getElementById('taxiTabBtn').classList.add("selected");
    document.getElementById('gaugeTabBtn').classList.remove("selected");
    document.getElementById('settingsTabBtn').classList.remove("selected");

    document.getElementById('connectionTabContent').style.display = "none";
    document.getElementById('taxiTabContent').style.display = "flex";
    document.getElementById('gaugeTabContent').style.display = "none";
    document.getElementById('settingsTabContent').style.display = "none";

    getTaxiingStatus();
    getTaxiingSpeed();

    document.getElementById("taxiingSpeedInput").addEventListener('change', setTaxiingSpeed);
}

function getTaxiingStatus()
{
    fetch('https://localhost:7187/api/taxiing/running').then(function(response) {
        return response.json();
      }).then(function(isTaxiing) {
        updateTaxiingStatus(isTaxiing);
        console.log(isTaxiing);
      }).catch(function(err) {
        updateTaxiingStatus(false);
        console.log('Fetch Error :-S', err);
      });
}

function updateTaxiingStatus(isTaxiing) {
    if(isTaxiing) {
        document.getElementById('taxiingBtn').classList.remove("turned-off");
    } else {
        document.getElementById('taxiingBtn').classList.add("turned-off");
    }
}

function getTaxiingSpeed()
{
    fetch('https://localhost:7187/api/taxiing/speed').then(function(response) {
        return response.json();
      }).then(function(taxiingSpeed) {
        document.getElementById('taxiingSpeedInput').value = taxiingSpeed;
        console.log(taxiingSpeed);
      }).catch(function(err) {
        console.log('Fetch Error :-S', err);
      });
}

function setTaxiingSpeed() {
    fetch('https://localhost:7187/api/taxiing/speed/' + this.value, 
        { 
            method: 'PUT',  
            headers: { 
                'Content-type': 'application/json'
            } 
        }).then(function(response) {
            return response.status == 200;
        }).then(function(speed) {
            console.log('Taxiing speed updated successfully!');
        }).catch(function(err) {
            console.log('Fetch Error :-S', err);
        });
}

function onTaxiingClicked() {
    if(document.getElementById('taxiingBtn').classList.contains("turned-off")) {
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