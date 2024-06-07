using ThermalSim.Domain.Extensions;
using ThermalSim.Domain.Position;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Turbulence
{
    public class DirectionalTurbulenceModel : ITurbulenceModel
    {
        private int _counter;
        private int _duration;
        private double _maxTurbulence;

        private double[] _kernel = null;

        private Random _random = new Random();
        
        private TurbulenceEffect? _turbulence;
        private readonly ITurbulenceKernel _sinkKernel;
        private readonly ITurbulenceKernel _transitionKernel;
        private readonly ITurbulenceKernel _coreKernel;

        public TurbulenceProperties Properties { get; set; }
        public double SmoothingFactor { get; set; } = 0.5;

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
            _counter++;
            //If we've hit the number of frames of turbulence, then we reset it
            if(_turbulence == null || _counter >= _duration)
            {
                ResetCount();

                UpdateTurbulenceKernel(_duration, thermal.GetPositionInThermal(position));

                _turbulence = GetBaseTurbulenceEffect(position, thermal);
            }

            var output = new TurbulenceEffect()
            {
                RotationAccelerationBodyX = position.RotationAccelerationBodyX * (1.0 - SmoothingFactor) + _turbulence.Value.RotationAccelerationBodyX * _kernel[_counter] * SmoothingFactor,
                RotationAccelerationBodyY = position.RotationAccelerationBodyY * (1.0 - SmoothingFactor) + _turbulence.Value.RotationAccelerationBodyY * _kernel[_counter] * SmoothingFactor,
                RotationAccelerationBodyZ = position.RotationAccelerationBodyZ * (1.0 - SmoothingFactor) + _turbulence.Value.RotationAccelerationBodyZ * _kernel[_counter] * SmoothingFactor,
            };

            return output;
        }

        private TurbulenceEffect GetBaseTurbulenceEffect(AircraftPositionState position, IThermalModel thermal)
        {
            //Get the relative orientation of the center of the thermal
            var angleToThermalCore = thermal.GetRelativeDirection(position).ToRadians();
            var rollEffect = Math.Sin(angleToThermalCore);
            var pitchEffect = -1.0 * Math.Cos(angleToThermalCore);
            var yawEffect = -1.0 * rollEffect * _random.NextDouble();

            var pitchMod = _maxTurbulence;
            pitchMod += pitchMod * Properties.PitchTurbulenceModifier;

            var yawMod = _maxTurbulence;
            yawMod += yawMod * Properties.YawTurbulenceModifier;

            var rollMod = _maxTurbulence;
            rollMod += rollMod * Properties.RollTurbulenceModifier;

            var effect = new TurbulenceEffect()
            {
                RotationAccelerationBodyX = pitchEffect * pitchMod,
                RotationAccelerationBodyY = yawEffect * yawMod,
                RotationAccelerationBodyZ = rollEffect * rollMod,
            };

            return effect;
        }


        private void ResetCount()
        {
            _duration = Properties.TurbulenceDuration.GetRandomValue(_random);
            _maxTurbulence = Properties.TurbulenceStrength.GetRandomValue(_random);
            _counter = 0;
        }
    }
}
