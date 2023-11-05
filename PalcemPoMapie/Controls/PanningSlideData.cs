using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalcemPoMapie.Controls
{
    internal class PanningSlideData
    {
        private bool _isPanning;
        private bool _isSliding;

        private DateTimeOffset _panningStartTimestamp;
        private Point _panningStartPoint;

        private DateTimeOffset _panningEndTimestamp;
        private Point _panningEndPoint;

        private TimeSpan _panningTime;
        private Vector _panningVector;
        private double _panningVelcotiy;

        private DateTimeOffset _slideEndTimestamp;
        private double _slideDistance;
        private Vector _slideNormalizedVector;
        private double _previousSlideDistance;

        public double Deceleration { get; set; }

        public Point PanningStartPoint => _panningStartPoint;

        public Point PanningEndPoint => _panningEndPoint;

        public DateTimeOffset PanningStartTimestamp => _panningStartTimestamp;

        public DateTimeOffset PanningEndTimestamp => _panningEndTimestamp;

        public bool IsSliding => _isSliding;

        public bool IsPanning => _isPanning;

        public void MarkPanningStart(Point point)
        {
            _panningStartTimestamp = DateTimeOffset.Now;
            _isPanning = true;
            _panningStartPoint = point;
        }

        public void MarkPanningEnd(Point point)
        {
            _panningEndTimestamp = DateTimeOffset.Now;
            _isPanning = false;
            _isSliding = true;
            _panningEndPoint = point;

            _panningTime = _panningEndTimestamp - _panningStartTimestamp; // t
            _panningVector = _panningEndPoint - _panningStartPoint; // S
            _panningVelcotiy = _panningVector.Length / _panningTime.TotalMilliseconds; // v0 [px / ms]

            TimeSpan decelerateTime = TimeSpan.FromMilliseconds(_panningVelcotiy / Deceleration); //t'
            _slideDistance = CalculateDistance(decelerateTime, _panningVelcotiy, Deceleration);
            _slideEndTimestamp = _panningEndTimestamp + decelerateTime;
            _slideNormalizedVector = _panningVector.Normalize();
        }

        public Vector GetSlidingDeltaVector() => GetSlidingDeltaVector(DateTimeOffset.Now);

        public Vector GetSlidingDeltaVector(DateTimeOffset currentTimestamp)
        {
            if (!_isSliding)
            {
                return default;
            }

            Vector result;

            if (currentTimestamp > _slideEndTimestamp)
            {
                result = _slideNormalizedVector.Negate() * (_slideDistance - _previousSlideDistance);
                _previousSlideDistance = 0;
                _isSliding = false;
            }
            else
            {
                TimeSpan slideTime = currentTimestamp - _panningEndTimestamp;
                double distance = CalculateDistance(slideTime, _panningVelcotiy, Deceleration);
                result = _slideNormalizedVector.Negate() * (distance - _previousSlideDistance);
                _previousSlideDistance = distance;
            }

            return result;
        }

        // px
        private static double CalculateDistance(TimeSpan time, double startVelocity, double deceleration)
            => startVelocity * time.TotalMilliseconds - deceleration * time.TotalMilliseconds * time.TotalMilliseconds / 2;
    }
}
