let thermalEventsWs = null;

function onConnectionClicked() {
    if(!document.getElementById('connectionBtn').classList.contains("turned-on")) {
        openConnection();
    } else {
        closeConnection();
    }
}

function onThermalSimOnClicked() {
    //if we are not connected we want to ignore the button
    if(document.getElementById('connectionBtn').classList.contains("turned-on")) {
        if(document.getElementById('thermalsBtn').classList.contains("turned-on")) {
            stopThermalSim();
        } else {
            startThermalSim();
        }
    }
}

function openConnection() {
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

function closeConnection() {
    if(this.thermalEventsWs != null) {
        this.thermalEventsWs.close();
        this.thermalEventsWs = null;
    }

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

function startThermalSim() {
    
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

function stopThermalSim() {
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
        document.getElementById('connectionBtn').classList.add('turned-on');
        document.getElementById('thermalsBtn').disabled = false;
    } else {
        document.getElementById('connectionBtn').classList.remove('turned-on');
        document.getElementById('thermalsBtn').disabled = true;
    }
}

function updateThermalSimulationStatus(status) {
    if(status) {
        document.getElementById('thermalsBtn').classList.add('turned-on');
        intializeThermalDataWebSocket();
    } else {
        document.getElementById('thermalsBtn').classList.remove('turned-on');
    }
}

function intializeThermalDataWebSocket() {
    this.thermalEventsWs = new WebSocket('wss://localhost:7187/ws');
    this.thermalEventsWs.addEventListener("open", (event) => {
        console.log(event);
      });
    this.thermalEventsWs.addEventListener("message", (data) => {
        setThermalIndicator(JSON.parse(data.data));
    });
    this.thermalEventsWs.addEventListener("close", (data) => {
        console.log(data);
        //attempt to re-establish connection
        this.thermalEventsWs.close();
        this.thermalEventsWs = null;
        intializeThermalDataWebSocket();
    });
    this.thermalEventsWs.addEventListener("error", (data) => {
        console.log(data);
    });

}

function setThermalIndicator(eventData) {
    document.getElementById('nearestThermalIndicator').style.rotate = (eventData.RelativeNearestThermal - 90.0) + 'deg';
    document.getElementById('windIndicator').style.rotate = (eventData.WindHeading - eventData.AircraftHeading + 90.0) + 'deg';
    document.getElementById('thermalCompass').style.rotate = -1.0 * eventData.AircraftHeading + 'deg';

    if(eventData.CurrentLift > 0.0) {
        let liftValue = eventData.CurrentLift / 30.0;
        if(liftValue > 1.0) {
            liftValue = 1.0;
        }

        document.getElementById('positiveLiftBar').style.height = liftValue * 100.0 + '%';
        document.getElementById('negativeLiftBar').style.height = '0%';
    } else {
        let liftValue = eventData.CurrentLift / -30.0;
        if(liftValue > 1.0) {
            liftValue = 1.0;
        }

        document.getElementById('negativeLiftBar').style.height = liftValue * 100.0 + '%';
        document.getElementById('positiveLiftBar').style.height = '0%';
    }

    if(eventData.ThermalState == 0) {
        document.getElementById('thermalStatusIndicator').style.fill = 'rgb(96,96,96)'
    } else if(eventData.ThermalState == 1) {
        document.getElementById('thermalStatusIndicator').style.fill = 'rgb(255,100,100)'
    } else if(eventData.ThermalState == 2) {
        document.getElementById('thermalStatusIndicator').style.fill = 'yellow'
    } else if(eventData.ThermalState == 3) {
        document.getElementById('thermalStatusIndicator').style.fill = 'rgb(100,255,100)'
    }
}

getConnectionStatus();
getThermalSimulationStatus();