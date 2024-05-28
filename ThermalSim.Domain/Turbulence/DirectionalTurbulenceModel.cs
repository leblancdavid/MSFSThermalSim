﻿using ThermalSim.Domain.Extensions;
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
                RotationVelocityBodyX = position.RotationVelocityBodyX + _turbulence.Value.RotationVelocityBodyX * _kernel[_counter],
                RotationVelocityBodyY = position.RotationVelocityBodyY + _turbulence.Value.RotationVelocityBodyY * _kernel[_counter],
                RotationVelocityBodyZ = position.RotationVelocityBodyZ + _turbulence.Value.RotationVelocityBodyZ * _kernel[_counter],
            };

            return output;
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
                RotationVelocityBodyX = elevatorEffect * _maxTurbulence * Properties.x_Scaler,
                RotationVelocityBodyY = yawEffect * _maxTurbulence * Properties.y_Scaler,
                RotationVelocityBodyZ = rollEffect * _maxTurbulence * Properties.z_Scaler,
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
