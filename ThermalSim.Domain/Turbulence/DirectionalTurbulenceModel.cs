using ThermalSim.Domain.Extensions;
using ThermalSim.Domain.Position;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Turbulence
{
    public class DirectionalTurbulenceModel : ITurbulenceModel
    {
        private int _counter;
        private int _resetCount;
        private int _duration;
        private double _maxTurbulence;

        private double[] _kernel = null;

        private Random _random = new Random();
        
        private TurbulenceEffect? _turbulence = null;
        private readonly ITurbulenceKernel _sinkKernel;
        private readonly ITurbulenceKernel _transitionKernel;
        private readonly ITurbulenceKernel _coreKernel;

        public TurbulenceProperties Properties { get; set; }

        public DirectionalTurbulenceModel(TurbulenceProperties properties, 
            ITurbulenceKernel? sinkKernel = null,
            ITurbulenceKernel transitionKernel = null,
            ITurbulenceKernel coreKernel = null)
        {

            Properties = properties;
            if (sinkKernel != null)
            {
                _sinkKernel = sinkKernel;
            }
            else
            {
                _sinkKernel = new CosineTurbulenceKernel(1.25, Math.PI / -4.0, -0.5, -0.5);
            }

            if (transitionKernel != null)
            {
                _transitionKernel = transitionKernel;
            }
            else
            {
                _transitionKernel = new CosineTurbulenceKernel(3.25, Math.PI / -4.0, 2.0, 0.0);
            }

            if (coreKernel != null)
            {
                _coreKernel = coreKernel;
            }
            else
            {
                _coreKernel = new CosineTurbulenceKernel(1.25, Math.PI / -4.0, 0.5, 0.5);
            }
        }

        private void UpdateTurbulenceKernel(int duration, ThermalPositionState positionInThermal)
        {
            if(positionInThermal == ThermalPositionState.InThermalSink)
            {
                _kernel = _sinkKernel.GetTurbulenceKernel(duration);
            }
            else if(positionInThermal == ThermalPositionState.InThermalTransition)
            {
                _kernel = _transitionKernel.GetTurbulenceKernel(duration);
            }
            else if(positionInThermal == ThermalPositionState.InThermalCore)
            {
                _kernel = _coreKernel.GetTurbulenceKernel(duration);
            }
        }

        public TurbulenceEffect? GetTurbulenceEffect(AircraftPositionState position, IThermalModel thermal)
        {
            //If we hit turbulence, this will be not null
            if(_turbulence != null)
            {
                _counter++;
                //If we've hit the number of frames of turbulence, then we reset it
                if(_counter >= _duration)
                {
                    _turbulence = null;
                    return _turbulence;
                }

                var output = new TurbulenceEffect()
                {
                    RotationAccelerationBodyX = _turbulence.Value.RotationAccelerationBodyX * _kernel[_counter],
                    RotationAccelerationBodyY = _turbulence.Value.RotationAccelerationBodyY * _kernel[_counter],
                    RotationAccelerationBodyZ = _turbulence.Value.RotationAccelerationBodyZ * _kernel[_counter],
                };

                return output;
            }

            //This counter makes it so that turbulence is spread apart somewhat randomly
            if(_counter < _resetCount)
            {
                _counter++;
                return null;
            }

            //If we get here, we basically start a new turbulence
            ResetCount();

            
            UpdateTurbulenceKernel(_duration, thermal.GetPositionInThermal(position));

            //For now let's make it simple
            _turbulence = GetBaseTurbulenceEffect(position, thermal);
            return _turbulence;
        }

        private TurbulenceEffect GetBaseTurbulenceEffect(AircraftPositionState position, IThermalModel thermal)
        {
            //Get the relative orientation of the center of the thermal
            var angleToThermalCore = thermal.GetRelativeDirection(position).ToRadians();
            var rollEffect = Math.Sin(angleToThermalCore);
            var elevatorEffect = -1.0 * Math.Cos(angleToThermalCore);
            var yawEffect = rollEffect * _random.NextDouble();

            var effect = new TurbulenceEffect()
            {
                RotationAccelerationBodyX = elevatorEffect * _maxTurbulence * Properties.x_Scaler,
                RotationAccelerationBodyY = yawEffect * _maxTurbulence * Properties.y_Scaler,
                RotationAccelerationBodyZ = rollEffect * _maxTurbulence * Properties.z_Scaler,
            };

            double distance = thermal.CalcDistance(position);

            //Console.WriteLine($"Angle: {angleInDegrees} \td: {distance} \tr: {effect.RotationAccelerationBodyZ} \te: {effect.RotationAccelerationBodyX} \ty: {effect.RotationAccelerationBodyY}");

            return effect;
        }


        private void ResetCount()
        {
            _resetCount = Properties.FramesBetweenTurbulence.GetRandomValue(_random);
            _duration = Properties.TurbulenceDuration.GetRandomValue(_random);
            _maxTurbulence = Properties.TurbulenceStrength.GetRandomValue(_random);
            _counter = 0;
        }
    }
}
