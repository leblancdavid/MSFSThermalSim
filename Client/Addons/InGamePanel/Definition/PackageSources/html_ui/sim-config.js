class ValueRange {
    constructor(min, max) {
        this.min = min;
        this.max = max;
    }
}
class SimConfig {
    constructor(numberThermals = new ValueRange(5, 10),
        samplingSpeedSeconds = new ValueRange(30, 60),
        durationMinutes = new ValueRange(5, 30),
        altitudeFromGround = new ValueRange(100, 500),
        spawnDistance = new ValueRange(0.01, 0.03),
        relativeSpawnAltitude = new ValueRange(-1000, 0),
        radius = new ValueRange(1000, 2000),
        height = new ValueRange(3000, 10000),
        coreLiftRate = new ValueRange(100, 500),
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
        this.numberThermals = numberThermals;
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