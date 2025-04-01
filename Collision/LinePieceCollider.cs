using System;
using SpaceDefence.Collision;
using Microsoft.Xna.Framework;

namespace SpaceDefence
{
    public class LinePieceCollider : Collider, IEquatable<LinePieceCollider>
    {
        public Vector2 Start;
        public Vector2 End;

        /// <summary>
        /// The length of the LinePiece, changing the length moves the end vector to adjust the length.
        /// </summary>
        public float Length
        {
            get { return (End - Start).Length(); }
            set { End = Start + GetDirection() * value; }
        }

        /// <summary>
        /// The A component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardA
        {
            get
            {
                return End.Y - Start.Y;
            }
        }

        /// <summary>
        /// The B component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardB
        {
            get
            {
                return Start.X - End.X;
            }
        }

        /// <summary>
        /// The C component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardC
        {
            get
            {
                return (End.X - Start.X) * Start.Y - (End.Y - Start.Y) * Start.X;
            }
        }

        public LinePieceCollider(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public LinePieceCollider(Vector2 start, Vector2 direction, float length)
        {
            Start = start;
            End = start + direction * length;
        }

        /// <summary>
        /// Should return the angle between a given direction and the up vector.
        /// </summary>
        /// <param name="direction">The Vector2 pointing out from (0,0) to calculate the angle to.</param>
        /// <returns> The angle in radians between the the up vector and the direction to the cursor.</returns>
        public static float GetAngle(Vector2 direction)
        {
            direction.Normalize();
            return (float)Math.Atan2(direction.X, -direction.Y);
        }

        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public static Vector2 GetDirection(Vector2 point1, Vector2 point2)
        {
            Vector2 direction = point2 - point1;
            direction.Normalize();
            return direction;
        }

        /// <summary>
        /// Gets whether or not the Line intersects another Line
        /// </summary>
        /// <param name="other">The Line to check for intersection</param>
        /// <returns>true there is any overlap between the Circle and the Line.</returns>
        public override bool Intersects(LinePieceCollider other)
        {
            return LineSegmentsIntersect(Start, End, other.Start, other.End);
        }

        /// <summary>
        /// Gets whether or not the line intersects a Circle.
        /// </summary>
        /// <param name="other">The Circle to check for intersection.</param>
        /// <returns>true there is any overlap between the two Circles.</returns>
        public override bool Intersects(CircleCollider other)
        {
            // Find the closest point on the line segment to the circle's center
            Vector2 closestPoint = NearestPointOnLine(other.Center);

            // Calculate the distance between the closest point and the circle's center
            float distance = Vector2.Distance(other.Center, closestPoint);

            // If the distance is less than the circle's radius, there's an intersection
            return distance <= other.Radius;
        }

        /// <summary>
        /// Gets whether or not the Line intersects the Rectangle.
        /// </summary>
        /// <param name="other">The Rectangle to check for intersection.</param>
        /// <returns>true there is any overlap between the Circle and the Rectangle.</returns>
        public override bool Intersects(RectangleCollider other)
        {
            // Check if any of the rectangle's sides intersect with the line segment
            Vector2 rectTopLeft = other.shape.Location.ToVector2();
            Vector2 rectTopRight = new Vector2(other.shape.Right, other.shape.Top);
            Vector2 rectBottomLeft = new Vector2(other.shape.Left, other.shape.Bottom);
            Vector2 rectBottomRight = other.shape.Location.ToVector2() + other.shape.Size.ToVector2();

            bool intersects = LineSegmentsIntersect(Start, End, rectTopLeft, rectTopRight) ||
                              LineSegmentsIntersect(Start, End, rectTopRight, rectBottomRight) ||
                              LineSegmentsIntersect(Start, End, rectBottomRight, rectBottomLeft) ||
                              LineSegmentsIntersect(Start, End, rectBottomLeft, rectTopLeft);

            // Check if the line segment is completely inside or on the rectangle's bounds
            if (!intersects && (other.Contains(Start) || IsPointOnRectangleEdge(Start, other.shape)) && (other.Contains(End) || IsPointOnRectangleEdge(End, other.shape)))
            {
                intersects = true;
            }

            return intersects;
        }

        private bool IsPointOnRectangleEdge(Vector2 point, Rectangle rect)
        {
            return (point.X == rect.Left || point.X == rect.Right) && (point.Y >= rect.Top && point.Y <= rect.Bottom) ||
                   (point.Y == rect.Top || point.Y == rect.Bottom) && (point.X >= rect.Left && point.X <= rect.Right);
        }

        private bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float det = (p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X);
            if (det == 0)
            {
                return false;
            }

            float t = ((p1.X - p3.X) * (p3.Y - p4.Y) - (p1.Y - p3.Y) * (p3.X - p4.X)) / det;
            float u = -((p1.X - p2.X) * (p1.Y - p3.Y) - (p1.Y - p2.Y) * (p1.X - p3.X)) / det;

            return t > 0 && t < 1 && u > 0 && u < 1;
        }

        /// <summary>
        /// Calculates the intersection point between 2 lines.
        /// </summary>
        /// <param name="Other">The line to intersect with</param>
        /// <returns>A Vector2 with the point of intersection.</returns>
        public Vector2 GetIntersection(LinePieceCollider Other)
        {
            float a1 = StandardA;
            float b1 = StandardB;
            float c1 = StandardC;
            float a2 = Other.StandardA;
            float b2 = Other.StandardB;
            float c2 = Other.StandardC;

            float det = a1 * b2 - a2 * b1;

            if (det == 0)
            {
                return Vector2.Zero;
            }

            float x = (b1 * c2 - b2 * c1) / det;
            float y = (a2 * c1 - a1 * c2) / det;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Finds the nearest point on a line to a given vector, taking into account if the line is .
        /// </summary>
        /// <param name="other">The Vector you want to find the nearest point to.</param>
        /// <returns>The nearest point on the line.</returns>
        public Vector2 NearestPointOnLine(Vector2 point)
        {
            Vector2 lineDirection = GetDirection(); // Ensure this returns a normalized direction
            Vector2 lineStartToPoint = point - Start;

            float projectionLength = Vector2.Dot(lineStartToPoint, lineDirection);

            // Clamp the projection length to the line segment
            projectionLength = Math.Max(0, Math.Min(Length, projectionLength));

            return Start + lineDirection * projectionLength;
        }

        /// <summary>
        /// Returns the enclosing Axis Aligned Bounding Box containing the control points for the line.
        /// As an unbound line has infinite length, the returned bounding box assumes the line to be bound.
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetBoundingBox()
        {
            Point topLeft = new Point((int)Math.Min(Start.X, End.X), (int)Math.Min(Start.Y, End.Y));
            Point size = new Point((int)Math.Max(Start.X, End.X), (int)Math.Max(Start.Y, End.X)) - topLeft;
            return new Rectangle(topLeft, size);
        }

        /// <summary>
        /// Gets whether or not the provided coordinates lie on the line.
        /// </summary>
        /// <param name="coordinates">The coordinates to check.</param>
        /// <returns>true if the coordinates are within the circle.</returns>
        public override bool Contains(Vector2 coordinates)
        {
            // Consider a small tolerance for floating-point comparisons
            float tolerance = 0.001f;

            // Check if the point is within the bounding box of the line segment
            if (coordinates.X < Math.Min(Start.X, End.X) - tolerance ||
                coordinates.X > Math.Max(Start.X, End.X) + tolerance ||
                coordinates.Y < Math.Min(Start.Y, End.Y) - tolerance ||
                coordinates.Y > Math.Max(Start.Y, End.Y) + tolerance)
            {
                return false;
            }

            // Check if the point is on the line
            float crossProduct = (coordinates.Y - Start.Y) * (End.X - Start.X) -
                                 (coordinates.X - Start.X) * (End.Y - Start.Y);

            return Math.Abs(crossProduct) < tolerance;
        }

        public bool Equals(LinePieceCollider other)
        {
            return other.Start == this.Start && other.End == this.End;
        }

        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public static Vector2 GetDirection(Point point1, Point point2)
        {
            return GetDirection(point1.ToVector2(), point2.ToVector2());
        }

        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public Vector2 GetDirection()
        {
            return GetDirection(Start, End);
        }

        /// <summary>
        /// Should return the angle between a given direction and the up vector.
        /// </summary>
        /// <param name="direction">The Vector2 pointing out from (0,0) to calculate the angle to.</param>
        /// <returns> The angle in radians between the the up vector and the direction to the cursor.</returns>
        public float GetAngle()
        {
            return GetAngle(GetDirection());
        }
    }
}