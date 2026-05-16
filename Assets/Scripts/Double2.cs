namespace UnityEngine
{

    public struct Double2
    {

        public double x, y;

        public Double2(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public double magnitude
        {
            get
            {
                return System.Math.Sqrt(x * x + y * y);
            }
        }

        public double sqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }

        public static double Distance(Double2 a, Double2 b)
        {
            return (a - b).magnitude;
        }

        public static Double2 operator +(Double2 left, Double2 right)
        {
            return new Double2(left.x + right.x, left.y + right.y);
        }
        public static Double2 operator +(Double2 left, Vector2 right)
        {
            return new Double2(left.x + right.x, left.y + right.y);
        }
        public static Double2 operator +(Vector2 left, Double2 right)
        {
            return new Double2(left.x + right.x, left.y + right.y);
        }
        public static Double2 operator +(Double2 left, Vector3 right)
        {
            return new Double2(left.x + right.x, left.y + right.y);
        }
        public static Double2 operator +(Vector3 left, Double2 right)
        {
            return new Double2(left.x + right.x, left.y + right.y);
        }

        public static Double2 operator -(Double2 value)
        {
            return new Double2(-value.x,-value.y);
        }

        public static Double2 operator -(Double2 left, Double2 right)
        {
            return new Double2(left.x - right.x, left.y - right.y);
        }
        public static Double2 operator -(Double2 left, Vector2 right)
        {
            return new Double2(left.x - right.x, left.y - right.y);
        }
        public static Double2 operator -(Vector2 left, Double2 right)
        {
            return new Double2(left.x - right.x, left.y - right.y);
        }
        public static Double2 operator -(Double2 left, Vector3 right)
        {
            return new Double2(left.x - right.x, left.y - right.y);
        }
        public static Double2 operator -(Vector3 left, Double2 right)
        {
            return new Double2(left.x - right.x, left.y - right.y);
        }
        
        public static Double2 operator *(double d, Double2 a)
        {
            return new Double2(d * a.x, d * a.y);
        }
        public static Double2 operator *(Double2 a, double d)
        {
            return new Double2(d * a.x, d * a.y);
        }

        public static Double2 operator /(Double2 a, double d)
        {
            return new Double2(a.x / d, a.y / d);
        }

        public static Double2 operator %(Double2 left, Double2 right)
        {
            return new Double2(left.x % right.x, left.y % right.y);
        }
        public static Double2 operator %(Double2 left, Vector2 right)
        {
            return new Double2(left.x % right.x, left.y % right.y);
        }
        public static Double2 operator %(Vector2 left, Double2 right)
        {
            return new Double2(left.x % right.x, left.y % right.y);
        }
        public static Double2 operator %(Double2 left, Vector3 right)
        {
            return new Double2(left.x % right.x, left.y % right.y);
        }
        public static Double2 operator %(Vector3 left, Double2 right)
        {
            return new Double2(left.x % right.x, left.y % right.y);
        }

        public static bool operator ==(Double2 left, Double2 right)
        {
            return left.x == right.x && left.y == right.y;
        }
        public static bool operator !=(Double2 left, Double2 right)
        {
            return left.x != right.x || left.y != right.y;
        }
        public override bool Equals(object other)
        {
            return other.Equals(this);
        }

        public override string ToString()
        {
            return "X: " + x.ToString("0") + "\n" + "Y: " + y.ToString("0");
        }
        public override int GetHashCode()
        {
            return new Vector2(x.GetHashCode(),y.GetHashCode()).GetHashCode();
        }

        public static implicit operator Double2(Vector3 v)
        {
            return new Double2(v.x, v.y);
        }
        public static implicit operator Double2(Vector2 v)
        {
            return new Double2(v.x, v.y);
        }
        public static implicit operator Vector3(Double2 v)
        {
            return new Vector3((float)v.x, (float)v.y);
        }
        public static implicit operator Vector2(Double2 v)
        {
            return new Vector2((float)v.x, (float)v.y);
        }
        public static implicit operator Double2(Vector4 v)
        {
            return new Double2(v.x, v.y);
        }
        public static implicit operator Vector4(Double2 v)
        {
            return new Vector3((float)v.x, (float)v.y);
        }

    }

}
