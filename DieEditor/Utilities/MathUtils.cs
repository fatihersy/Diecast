using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieEditor.Utilities
{
    public static class MathUtils
    {
        public static float Epsilon => 0.00001f;

        public static bool IsTheSameAs(this float lhs, float rhs)
        {
            return Math.Abs(lhs - rhs) < Epsilon;
        }
		public static bool IsTheSameAs(this float? lhs, float? rhs)
		{
            if (!lhs.HasValue || !rhs.HasValue) { 
                return false; 
            }
			return Math.Abs(lhs.Value - rhs.Value) < Epsilon;
		}

	}
}
