﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private readonly SpiralInfo spiralInfo;
        private readonly HashSet<Rectangle> rectangles;
        public HashSet<Rectangle> Rectangles => new HashSet<Rectangle>(rectangles);

        public CircularCloudLayouter(Point center, double radiusStep = 0.0001, double angleStep = 0.1)
        {
            rectangles = new HashSet<Rectangle>();
            spiralInfo = new SpiralInfo(radiusStep, angleStep, center);
        }

        public CircularCloudLayouter(SpiralInfo spiralInfo)
        {
            this.spiralInfo = spiralInfo;
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
                throw new ArgumentException("Width and height should be integer positive numbers");

            var newRectangle = new Rectangle(GetAppropriatePlace(rectangleSize), rectangleSize);
            rectangles.Add(newRectangle);
            return newRectangle;
        }

        public Point GetAppropriatePlace(Size rectangleSize)
        {
            if (rectangles.Count == 0)
                return spiralInfo.Center;

            var currentPosition = spiralInfo.Center;
            var currentAngle = 0.0;
            var currentRadius = 0.0;
            var potentialRectangle = new Rectangle(currentPosition, rectangleSize);
            while (HaveIntersectionWithAnotherRectangle(potentialRectangle) ||
                   IsPlacedInAnotherRectangle(potentialRectangle))
            {
                (currentPosition, currentAngle, currentRadius) = NextStep(
                    currentPosition, currentAngle, currentRadius);
                if (!potentialRectangle.Location.Equals(currentPosition))
                    potentialRectangle = new Rectangle(currentPosition, rectangleSize);
            }
            return currentPosition;
        }

        private ValueTuple<Point, double, double> NextStep(
            Point currentPosition, double currentAngle, double currentRadius)
        {
            var sin = Math.Sin(currentAngle);
            var cos = Math.Cos(currentAngle);
            currentPosition.X = (int)(spiralInfo.Center.X - currentRadius * currentAngle * sin);
            currentPosition.Y = (int)(spiralInfo.Center.Y + currentRadius * currentAngle * cos);
            currentAngle = (currentAngle + spiralInfo.AngleStep) % SpiralInfo.MaxAngle;
            currentRadius += spiralInfo.RadiusStep;

            return (currentPosition, currentAngle, currentRadius);
        }

        private bool HaveIntersectionWithAnotherRectangle(Rectangle rectangle)
        {
            return rectangles.Any(anotherRectangle => RectanglesChecker.HaveIntersection(rectangle, anotherRectangle));
        }

        private bool IsPlacedInAnotherRectangle(Rectangle verifiableRectangle)
        {
            return rectangles.Any(rectangle => RectanglesChecker.IsNestedRectangle(verifiableRectangle, rectangle));
        }
    }
}