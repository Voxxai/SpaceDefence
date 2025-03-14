using System;
using SpaceDefence.Collision;
using Microsoft.Xna.Framework;

namespace SpaceDefence
{
    public class CircleCollider : Collider, IEquatable<CircleCollider>
    {
        public float X;
        public float Y;
        public Vector2 Center 
        { 
            get { 
                return new Vector2(X, Y); 
            } 

            set { 
                X = value.X; Y = value.Y; 
            } 
        }
        public float Radius;

        /// <summary>
        /// Creates a new Circle object.
        /// </summary>
        /// <param name="x">The X coordinate of the circle's center</param>
        /// <param name="y">The Y coordinate of the circle's center</param>
        /// <param name="radius">The radius of the circle</param>
        public CircleCollider(float x, float y, float radius)
        {
            this.X = x; 
            this.Y = y; 
            this.Radius = radius;
        }

        /// <summary>
        /// Creates a new Circle object.
        /// </summary>
        /// <param name="center">The coordinates of the circle's center</param>
        /// <param name="radius">The radius of the circle</param>
        public CircleCollider(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }


        /// <summary>
        /// Gets whether or not the provided coordinates lie within the bounds of this Circle.
        /// </summary>
        /// <param name="coordinates">The coordinates to check.</param>
        /// <returns>true if the coordinates are within the circle.</returns>
        public override bool Contains(Vector2 coordinates)
        {
            return (Center - coordinates).Length() < Radius;
        }

        /// <summary>
        /// Gets whether or not the Circle intersects another Circle.
        /// </summary>
        /// <param name="other">The Circle to check for intersection.</param>
        /// <returns>true there is any overlap between the two Circles.</returns>
        public override bool Intersects(CircleCollider other)
        {
            float distanceBetweenCenters = Vector2.Distance(Center, other.Center);
            float sumOfRadii = Radius + other.Radius;
            return distanceBetweenCenters < sumOfRadii;
        }


        /// <summary>
        /// Gets whether or not the Circle intersects the Rectangle.
        /// </summary>
        /// <param name="other">The Rectangle to check for intersection.</param>
        /// <returns>true there is any overlap between the Circle and the Rectangle.</returns>
        public override bool Intersects(RectangleCollider other)
        {
            Rectangle rect = other.shape;
            Vector2 circleCenter = Center;
            float circleRadius = Radius;

            // Check if any corner of the rectangle is inside the circle
            if (IsPointInsideCircle(rect.Location.ToVector2(), circleCenter, circleRadius))
                return true;
            if (IsPointInsideCircle(new Vector2(rect.Right, rect.Top), circleCenter, circleRadius))
                return true;
            if (IsPointInsideCircle(new Vector2(rect.Left, rect.Bottom), circleCenter, circleRadius))
                return true;
            if (IsPointInsideCircle(rect.Location.ToVector2() + rect.Size.ToVector2(), circleCenter, circleRadius))
                return true;

            // Check if any side of the rectangle intersects the circle
            if (IsLineIntersectingCircle(rect.Top, rect.Left, rect.Top, rect.Right, circleCenter, circleRadius))
                return true;
            if (IsLineIntersectingCircle(rect.Bottom, rect.Left, rect.Bottom, rect.Right, circleCenter, circleRadius))
                return true;
            if (IsLineIntersectingCircle(rect.Left, rect.Top, rect.Left, rect.Bottom, circleCenter, circleRadius))
                return true;
            if (IsLineIntersectingCircle(rect.Right, rect.Top, rect.Right, rect.Bottom, circleCenter, circleRadius))
                return true;

            // Check if the center of the circle is inside the rectangle
            if (rect.Contains(Center))
                return true;

            return false;
        }
        /// <summary>
        /// Gets whether or not the Circle intersects the Line
        /// </summary>
        /// <param name="other">The Line to check for intersection</param>
        /// <returns>true there is any overlap between the Circle and the Line.</returns>
        public override bool Intersects(LinePieceCollider other)
        {
            // Implemented in the line code.
            return other.Intersects(this);
        }

        /// <summary>
        /// Get the enclosing Rectangle that surrounds the Circle.
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)(X - Radius), (int)(Y - Radius), (int)(2 * Radius), (int)(2 * Radius));
        }

        public bool Equals(CircleCollider other)
        {
            return other.X == X && other.Y == Y && other.Radius == Radius;
        }

        private bool IsPointInsideCircle(Vector2 point, Vector2 circleCenter, float circleRadius)
        {
            return Vector2.Distance(point, circleCenter) < circleRadius;
        }

        private bool IsLineIntersectingCircle(float x1, float y1, float x2, float y2, Vector2 circleCenter, float circleRadius)
        {
            Vector2 start = new Vector2(x1, y1);
            Vector2 end = new Vector2(x2, y2);
            Vector2 lineDir = end - start;
            Vector2 circleToStart = start - circleCenter;
            float a = Vector2.Dot(lineDir, lineDir);
            float b = 2 * Vector2.Dot(circleToStart, lineDir);
            float c = Vector2.Dot(circleToStart, circleToStart) - circleRadius * circleRadius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                return false;
            }
            else if (discriminant == 0)
            {
                float t = -b / (2 * a);
                return t >= 0 && t <= 1;
            }
            else
            {
                float t1 = (-b - MathF.Sqrt(discriminant)) / (2 * a);
                float t2 = (-b + MathF.Sqrt(discriminant)) / (2 * a);
                return (t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1);
            }
        }
    }
}