function onConnectionTabClicked() {
    getConnectionStatus();
    getThermalSimulationStatus();
    
    document.getElementById('connectionTabBtn').classList.add("selected");
    document.getElementById('gaugeTabBtn').classList.remove("selected");
    document.getElementById('taxiTabBtn').classList.remove("selected");
    document.getElementById('settingsTabBtn').classList.remove("selected");

    document.getElementById('connectionTabContent').style.display = "flex";
    document.getElementById('gaugeTabContent').style.display = "none";
    document.getElementById('taxiTabContent').style.display = "none";
    document.getElementById('settingsTabContent').style.display = "none";

}

function onOpenConnectionClicked() {
    fetch('https://localhost:7187/api/sim-connection', 
        { 
            method: 'PUT',  
            headers: { 
                'Content-type': 'application/json'
            } 
        }).then(function(response) {
            return response.status == 200;
        }).then(function(isConnected) {
            updateConnectionStatus(isConnected);
            if(isConnected) {
                console.log('Connected successfully!');
            } else {
                console.log('Unable to connect!');
            }
        }).catch(function(err) {
            updateConnectionStatus(false);
            console.log('Fetch Error :-S', err);
        });
}

function onCloseConnectionClicked() {
    fetch('https://localhost:7187/api/sim-connection', 
        { 
            method: 'DELETE',  
            headers: { 
                'Content-type': 'application/json'
            } 
        }).then(function(response) {
            return response.status == 200;
        }).then(function(isDisconnected) {
            updateConnectionStatus(!isDisconnected);
            if(isDisconnected) {
                console.log('Disconnected successfully!');
            } else {
                console.log('Unable to disconnect!');
            }
        }).catch(function(err) {
            updateConnectionStatus(false);
            console.log('Fetch Error :-S', err);
        });
}

function onThermalSimOnClicked() {
    fetch('https://localhost:7187/api/thermals', 
        { 
            method: 'PUT',  
            headers: { 
                'Content-type': 'application/json'
            } 
        }).then(function(response) {
            return response.status == 200;
        }).then(function(isConnected) {
            updateThermalSimulationStatus(isConnected);
            if(isConnected) {
                console.log('Thermal simulation is ENABLED');
            } else {
                console.log('Unable to turn on thermal simulation');
            }
        }).catch(function(err) {
            updateThermalSimulationStatus(false);
            console.log('Fetch Error :-S', err);
        });
}

function onThermalSimOffClicked() {
    fetch('https://localhost:7187/api/thermals', 
        { 
            method: 'DELETE',  
            headers: { 
                'Content-type': 'application/json'
            } 
        }).then(function(response) {
            return response.status == 200;
        }).then(function(isDisconnected) {
            updateThermalSimulationStatus(!isDisconnected);
            if(isDisconnected) {
                console.log('Thermal simulation is DISABLED');
            } else {
                console.log('Unable to turn off thermal simulation');
            }
        }).catch(function(err) {
            updateThermalSimulationStatus(false);
            console.log('Fetch Error :-S', err);
        });
}

function getConnectionStatus()
{
    fetch('https://localhost:7187/api/sim-connection/connected').then(function(response) {
        return response.json();
      }).then(function(isConnected) {
        updateConnectionStatus(isConnected);
        console.log(isConnected);
      }).catch(function(err) {
        updateConnectionStatus(false);
        console.log('Fetch Error :-S', err);
      });
}

function getThermalSimulationStatus()
{
    fetch('https://localhost:7187/api/thermals/running').then(function(response) {
        return response.json();
      }).then(function(isConnected) {
        updateThermalSimulationStatus(isConnected);
        console.log(isConnected);
      }).catch(function(err) {
        updateThermalSimulationStatus(false);
        console.log('Fetch Error :-S', err);
      });
}

function updateConnectionStatus(status) {
    if(status) {
        document.getElementById('openConnectionBtn').disabled = true;
        document.getElementById('closeConnectionBtn').disabled = false;
        document.getElementById('thermal-simulator-controls').style.display = "flex";
    } else {
        document.getElementById('openConnectionBtn').disabled = false;
        document.getElementById('closeConnectionBtn').disabled = true;
        document.getElementById('thermal-simulator-controls').style.display = "none";
    }
}

function updateThermalSimulationStatus(status) {
    if(status) {
        document.getElementById('thermalsOnBtn').disabled = true;
        document.getElementById('thermalsOffBtn').disabled = false;
    } else {
        document.getElementById('thermalsOnBtn').disabled = false;
        document.getElementById('thermalsOffBtn').disabled = true;
    }
}

onConnectionTabClicked();