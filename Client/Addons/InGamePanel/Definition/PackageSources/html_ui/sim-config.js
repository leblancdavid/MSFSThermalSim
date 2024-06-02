class ValueRange {
    constructor(min, max) {
        this.min = min;
        this.max = max;
    }
}
class SimConfig {
    constructor(numberOfThermals = new ValueRange(5, 10),
        samplingSpeedSeconds = new ValueRange(30, 60),
        durationMinutes = new ValueRange(5, 30),
        altitudeFromGround = new ValueRange(100, 500),
        spawnDistance = new ValueRange(0.01, 0.03),
        relativeSpawnAltitude = new ValueRange(-1000, 0),
        radius = new ValueRange(1000, 2000),
        height = new ValueRange(3000, 10000),
        coreLiftRate = new ValueRange(10, 20),
        coreRadiusPercent = new ValueRange(0.8, 0.85),
        coreTurbulencePercent = new ValueRange(0.0, 2.0),
        sinkRatePercent = new ValueRange(-1.5, -0.5),
        sinkTurbulencePercent = new ValueRange(0, 2.0),
        sinkTransitionRadiusPercent = new ValueRange(0.05, 0.1),
        liftShapeFactor = new ValueRange(0.0, 1.0),
        replaceDistance = 25000,
        framesBetweenTurbulence = new ValueRange(60, 120),
        turbulenceDuration = new ValueRange(60, 240),
        turbulenceStrengthPercent = new ValueRange(1.0, 1.5),
    ) {
        this.numberOfThermals = numberOfThermals;
        this.samplingSpeedSeconds = samplingSpeedSeconds;
        this.durationMinutes = durationMinutes;
        this.altitudeFromGround = altitudeFromGround;
        this.spawnDistance = spawnDistance;
        this.relativeSpawnAltitude = relativeSpawnAltitude;
        this.radius = radius;
        this.height = height;
        this.coreLiftRate = coreLiftRate;
        this.coreRadiusPercent = coreRadiusPercent;
        this.coreTurbulencePercent = coreTurbulencePercent;
        this.sinkRatePercent = sinkRatePercent;
        this.sinkTurbulencePercent = sinkTurbulencePercent;
        this.sinkTransitionRadiusPercent = sinkTransitionRadiusPercent;
        this.liftShapeFactor = liftShapeFactor;
        this.replaceDistance = replaceDistance;
        this.framesBetweenTurbulence = framesBetweenTurbulence;
        this.turbulenceDuration = turbulenceDuration;
        this.turbulenceStrengthPercent = turbulenceStrengthPercent;
    }
}

var config = new SimConfig();

function refreshConfiguration() {
    fetch('http://localhost:17188/api/thermals/configuration').then(function(response) {
        if(response.status == 200)
            return response.json();
        return null;
      }).then(function(configValues) {
        if(configValues != null) {
            this.config = configValues;
            refreshInputView();
            console.log(configValues);
        }
      }).catch(function(err) {
        console.log('Fetch Error :-S', err);
      });
}

function refreshInputView() {
    document.getElementById('configMinNumThermals').innerHTML = config.numberOfThermals.min;
    document.getElementById('configMaxNumThermals').innerHTML = config.numberOfThermals.max;
    document.getElementById('configMinRadius').innerHTML = config.radius.min;
    document.getElementById('configMaxRadius').innerHTML = config.radius.max;
    document.getElementById('configMinHeight').innerHTML = config.height.min;
    document.getElementById('configMaxHeight').innerHTML = config.height.max;
    document.getElementById('configMinCoreLiftRate').innerHTML = config.coreLiftRate.min;
    document.getElementById('configMaxCoreLiftRate').innerHTML = config.coreLiftRate.max;
    document.getElementById('configMinSinkRate').innerHTML = config.sinkRatePercent.min;
    document.getElementById('configMaxSinkRate').innerHTML = config.sinkRatePercent.max;
    document.getElementById('configMinDuration').innerHTML = config.durationMinutes.min;
    document.getElementById('configMaxDuration').innerHTML = config.durationMinutes.max;

}

function sendUpdatedConfig() {
    fetch('http://localhost:17188/api/thermals/configuration', 
        { 
            method: 'PUT',  
            headers: { 
                'Content-type': 'application/json'
            },
            body: JSON.stringify(this.config)
        }).then(function(response) {
            if(response.status == 200)
                return response.json();
            return null;
        }).then(function(configValues) {
            if(configValues != null) {
                this.config = configValues;
                refreshInputView();
                console.log(configValues);
            }
          }).catch(function(err) {
            updateThermalSimulationStatus(false);
            console.log('Fetch Error :-S', err);
        });
}

refreshConfiguration();