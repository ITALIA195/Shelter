using UnityEngine;

namespace NGUI.Internal
{
    public static class NGUIMath
    {
        public static Vector3 ApplyHalfPixelOffset(Vector3 pos)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.XBOX360:
                    pos.x -= 0.5f;
                    pos.y += 0.5f;
                    break;
            }
            return pos;
        }

        public static Vector3 ApplyHalfPixelOffset(Vector3 pos, Vector3 scale)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.XBOX360:
                    if (Mathf.RoundToInt(scale.x) == Mathf.RoundToInt(scale.x * 0.5f) * 2)
                    {
                        pos.x -= 0.5f;
                    }
                    if (Mathf.RoundToInt(scale.y) == Mathf.RoundToInt(scale.y * 0.5f) * 2)
                    {
                        pos.y += 0.5f;
                    }
                    break;
            }
            return pos;
        }

        public static Bounds CalculateAbsoluteWidgetBounds(Transform trans)
        {
            UIWidget[] componentsInChildren = trans.GetComponentsInChildren<UIWidget>();
            if (componentsInChildren.Length == 0)
            {
                return new Bounds(trans.position, Vector3.zero);
            }
            Vector3 rhs = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            int index = 0;
            int length = componentsInChildren.Length;
            while (index < length)
            {
                UIWidget widget = componentsInChildren[index];
                Vector2 relativeSize = widget.relativeSize;
                Vector2 pivotOffset = widget.pivotOffset;
                float num3 = (pivotOffset.x + 0.5f) * relativeSize.x;
                float num4 = (pivotOffset.y - 0.5f) * relativeSize.y;
                relativeSize = relativeSize * 0.5f;
                Transform cachedTransform = widget.cachedTransform;
                Vector3 lhs = cachedTransform.TransformPoint(new Vector3(num3 - relativeSize.x, num4 - relativeSize.y, 0f));
                vector2 = Vector3.Max(lhs, vector2);
                rhs = Vector3.Min(lhs, rhs);
                lhs = cachedTransform.TransformPoint(new Vector3(num3 - relativeSize.x, num4 + relativeSize.y, 0f));
                vector2 = Vector3.Max(lhs, vector2);
                rhs = Vector3.Min(lhs, rhs);
                lhs = cachedTransform.TransformPoint(new Vector3(num3 + relativeSize.x, num4 - relativeSize.y, 0f));
                vector2 = Vector3.Max(lhs, vector2);
                rhs = Vector3.Min(lhs, rhs);
                lhs = cachedTransform.TransformPoint(new Vector3(num3 + relativeSize.x, num4 + relativeSize.y, 0f));
                vector2 = Vector3.Max(lhs, vector2);
                rhs = Vector3.Min(lhs, rhs);
                index++;
            }
            Bounds bounds = new Bounds(rhs, Vector3.zero);
            bounds.Encapsulate(vector2);
            return bounds;
        }

        public static Bounds CalculateRelativeInnerBounds(Transform root, UISprite sprite)
        {
            if (sprite.type != UISprite.Type.Sliced)
            {
                return CalculateRelativeWidgetBounds(root, sprite.cachedTransform);
            }
            Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
            Vector2 relativeSize = sprite.relativeSize;
            Vector2 pivotOffset = sprite.pivotOffset;
            Transform cachedTransform = sprite.cachedTransform;
            float num = (pivotOffset.x + 0.5f) * relativeSize.x;
            float num2 = (pivotOffset.y - 0.5f) * relativeSize.y;
            relativeSize = relativeSize * 0.5f;
            float x = cachedTransform.localScale.x;
            float y = cachedTransform.localScale.y;
            Vector4 border = sprite.border;
            if (x != 0f)
            {
                border.x /= x;
                border.z /= x;
            }
            if (y != 0f)
            {
                border.y /= y;
                border.w /= y;
            }
            float num5 = num - relativeSize.x + border.x;
            float num6 = num + relativeSize.x - border.z;
            float num7 = num2 - relativeSize.y + border.y;
            float num8 = num2 + relativeSize.y - border.w;
            Vector3 position = new Vector3(num5, num7, 0f);
            position = cachedTransform.TransformPoint(position);
            position = worldToLocalMatrix.MultiplyPoint3x4(position);
            Bounds bounds = new Bounds(position, Vector3.zero);
            position = new Vector3(num5, num8, 0f);
            position = cachedTransform.TransformPoint(position);
            position = worldToLocalMatrix.MultiplyPoint3x4(position);
            bounds.Encapsulate(position);
            position = new Vector3(num6, num8, 0f);
            position = cachedTransform.TransformPoint(position);
            position = worldToLocalMatrix.MultiplyPoint3x4(position);
            bounds.Encapsulate(position);
            position = new Vector3(num6, num7, 0f);
            position = cachedTransform.TransformPoint(position);
            position = worldToLocalMatrix.MultiplyPoint3x4(position);
            bounds.Encapsulate(position);
            return bounds;
        }

        public static Bounds CalculateRelativeWidgetBounds(Transform trans)
        {
            return CalculateRelativeWidgetBounds(trans, trans);
        }

        public static Bounds CalculateRelativeWidgetBounds(Transform root, Transform child)
        {
            UIWidget[] componentsInChildren = child.GetComponentsInChildren<UIWidget>();
            if (componentsInChildren.Length == 0)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }
            Vector3 rhs = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
            int index = 0;
            int length = componentsInChildren.Length;
            while (index < length)
            {
                UIWidget widget = componentsInChildren[index];
                Vector2 relativeSize = widget.relativeSize;
                Vector2 pivotOffset = widget.pivotOffset;
                Transform cachedTransform = widget.cachedTransform;
                float num3 = (pivotOffset.x + 0.5f) * relativeSize.x;
                float num4 = (pivotOffset.y - 0.5f) * relativeSize.y;
                relativeSize = relativeSize * 0.5f;
                Vector3 position = new Vector3(num3 - relativeSize.x, num4 - relativeSize.y, 0f);
                position = cachedTransform.TransformPoint(position);
                position = worldToLocalMatrix.MultiplyPoint3x4(position);
                vector2 = Vector3.Max(position, vector2);
                rhs = Vector3.Min(position, rhs);
                position = new Vector3(num3 - relativeSize.x, num4 + relativeSize.y, 0f);
                position = cachedTransform.TransformPoint(position);
                position = worldToLocalMatrix.MultiplyPoint3x4(position);
                vector2 = Vector3.Max(position, vector2);
                rhs = Vector3.Min(position, rhs);
                position = new Vector3(num3 + relativeSize.x, num4 - relativeSize.y, 0f);
                position = cachedTransform.TransformPoint(position);
                position = worldToLocalMatrix.MultiplyPoint3x4(position);
                vector2 = Vector3.Max(position, vector2);
                rhs = Vector3.Min(position, rhs);
                position = new Vector3(num3 + relativeSize.x, num4 + relativeSize.y, 0f);
                position = cachedTransform.TransformPoint(position);
                position = worldToLocalMatrix.MultiplyPoint3x4(position);
                vector2 = Vector3.Max(position, vector2);
                rhs = Vector3.Min(position, rhs);
                index++;
            }
            Bounds bounds = new Bounds(rhs, Vector3.zero);
            bounds.Encapsulate(vector2);
            return bounds;
        }

        public static Vector3[] CalculateWidgetCorners(UIWidget w)
        {
            Vector2 relativeSize = w.relativeSize;
            Vector2 pivotOffset = w.pivotOffset;
            Vector4 relativePadding = w.relativePadding;
            float x = pivotOffset.x * relativeSize.x - relativePadding.x;
            float y = pivotOffset.y * relativeSize.y + relativePadding.y;
            float num3 = x + relativeSize.x + relativePadding.x + relativePadding.z;
            float num4 = y - relativeSize.y - relativePadding.y - relativePadding.w;
            Transform cachedTransform = w.cachedTransform;
            return new[] { cachedTransform.TransformPoint(x, y, 0f), cachedTransform.TransformPoint(x, num4, 0f), cachedTransform.TransformPoint(num3, num4, 0f), cachedTransform.TransformPoint(num3, y, 0f) };
        }

        public static int ColorToInt(Color c)
        {
            int num = 0;
            num |= Mathf.RoundToInt(c.r * 255f) << 24;
            num |= Mathf.RoundToInt(c.g * 255f) << 16;
            num |= Mathf.RoundToInt(c.b * 255f) << 8;
            return num | Mathf.RoundToInt(c.a * 255f);
        }

        public static Vector2 ConstrainRect(Vector2 minRect, Vector2 maxRect, Vector2 minArea, Vector2 maxArea)
        {
            Vector2 zero = Vector2.zero;
            float num = maxRect.x - minRect.x;
            float num2 = maxRect.y - minRect.y;
            float num3 = maxArea.x - minArea.x;
            float num4 = maxArea.y - minArea.y;
            if (num > num3)
            {
                float num5 = num - num3;
                minArea.x -= num5;
                maxArea.x += num5;
            }
            if (num2 > num4)
            {
                float num6 = num2 - num4;
                minArea.y -= num6;
                maxArea.y += num6;
            }
            if (minRect.x < minArea.x)
            {
                zero.x += minArea.x - minRect.x;
            }
            if (maxRect.x > maxArea.x)
            {
                zero.x -= maxRect.x - maxArea.x;
            }
            if (minRect.y < minArea.y)
            {
                zero.y += minArea.y - minRect.y;
            }
            if (maxRect.y > maxArea.y)
            {
                zero.y -= maxRect.y - maxArea.y;
            }
            return zero;
        }

        public static Rect ConvertToPixels(Rect rect, int width, int height, bool round)
        {
            Rect rect2 = rect;
            if (round)
            {
                rect2.xMin = Mathf.RoundToInt(rect.xMin * width);
                rect2.xMax = Mathf.RoundToInt(rect.xMax * width);
                rect2.yMin = Mathf.RoundToInt((1f - rect.yMax) * height);
                rect2.yMax = Mathf.RoundToInt((1f - rect.yMin) * height);
                return rect2;
            }
            rect2.xMin = rect.xMin * width;
            rect2.xMax = rect.xMax * width;
            rect2.yMin = (1f - rect.yMax) * height;
            rect2.yMax = (1f - rect.yMin) * height;
            return rect2;
        }

        public static Rect ConvertToTexCoords(Rect rect, int width, int height)
        {
            Rect rect2 = rect;
            if (width != 0f && height != 0f)
            {
                rect2.xMin = rect.xMin / width;
                rect2.xMax = rect.xMax / width;
                rect2.yMin = 1f - rect.yMax / height;
                rect2.yMax = 1f - rect.yMin / height;
            }
            return rect2;
        }

        /// <summary>
        /// Converts <paramref name="num"/> to a <b>six-digit</b> hexadecimal number
        /// </summary>
        /// <param name="num">The number to convert</param>
        /// <returns>The six-digit hexadecimal number representation of <paramref name="num"/></returns>
        public static string DecimalToHex(int num)
        {
            num &= 16777215;
            return num.ToString("X6");
        }

        private static float DistancePointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            Vector2 vector = b - a;
            float sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude == 0f)
            {
                Vector2 vector2 = point - a;
                return vector2.magnitude;
            }
            float num2 = Vector2.Dot(point - a, b - a) / sqrMagnitude;
            if (num2 < 0f)
            {
                Vector2 vector3 = point - a;
                return vector3.magnitude;
            }
            if (num2 > 1f)
            {
                Vector2 vector4 = point - b;
                return vector4.magnitude;
            }
            Vector2 vector5 = a + num2 * (b - a);
            Vector2 vector6 = point - vector5;
            return vector6.magnitude;
        }

        public static  float DistanceToRectangle(Vector2[] screenPoints, Vector2 mousePos)//unsafe
        {
            bool flag = false;
            int val = 4;
            for (int i = 0; i < 5; i++)
            {
                Vector3 vector = screenPoints[RepeatIndex(i, 4)];
                Vector3 vector2 = screenPoints[RepeatIndex(val, 4)];
                if (vector.y > mousePos.y != vector2.y > mousePos.y && mousePos.x < (vector2.x - vector.x) * (mousePos.y - vector.y) / (vector2.y - vector.y) + vector.x)
                {
                    flag = !flag;
                }
                val = i;
            }
            if (flag)
            {
                return 0f;
            }
            float num3 = -1f;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = screenPoints[j];
                Vector3 b = screenPoints[RepeatIndex(j + 1, 4)];
                float num5 = DistancePointToLineSegment(mousePos, a, b);
                if (num5 < num3 || num3 < 0f)
                {
                    num3 = num5;
                }
            }
            return num3;
        }

        public static float DistanceToRectangle(Vector3[] worldPoints, Vector2 mousePos, Camera cam)
        {
            Vector2[] screenPoints = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                screenPoints[i] = cam.WorldToScreenPoint(worldPoints[i]);
            }
            return DistanceToRectangle(screenPoints, mousePos);
        }

        public static Color HexToColor(uint val)
        {
            return IntToColor((int)val);
        }

        public static int HexToDecimal(char c)
        {
            int value = c;
            
            // Return value if less than 10
            if (value >= '0' && value <= '9') 
                return value - '0';

            // Return HexValue
            if (value >= 'a' && value <= 'f')
                return 10 + value - 'a';
            
            // Return HexValue
            if (value >= 'A' && value <= 'F')
                return 10 + value - 'A';

            // c is not an hexadecimal number
            return 15;
        }

        public static string IntToBinary(int val, int bits)
        {
            string str = string.Empty;
            int num = bits;
            while (num > 0)
            {
                switch (num)
                {
                    case 8:
                    case 16:
                    case 24:
                        str = str + " ";
                        break;
                }
                str = str + ((val & 1 << --num) == 0 ? '0' : '1');
            }
            return str;
        }

        public static Color IntToColor(int val)
        {
            float num = 0.003921569f;
            Color black = Color.black;
            black.r = num * ((val >> 24) & 255);
            black.g = num * ((val >> 16) & 255);
            black.b = num * ((val >> 8) & 255);
            black.a = num * (val & 255);
            return black;
        }

        public static float Lerp(float from, float to, float factor)
        {
            return @from * (1f - factor) + to * factor;
        }

        public static Rect MakePixelPerfect(Rect rect)
        {
            rect.xMin = Mathf.RoundToInt(rect.xMin);
            rect.yMin = Mathf.RoundToInt(rect.yMin);
            rect.xMax = Mathf.RoundToInt(rect.xMax);
            rect.yMax = Mathf.RoundToInt(rect.yMax);
            return rect;
        }

        public static Rect MakePixelPerfect(Rect rect, int width, int height)
        {
            rect = ConvertToPixels(rect, width, height, true);
            rect.xMin = Mathf.RoundToInt(rect.xMin);
            rect.yMin = Mathf.RoundToInt(rect.yMin);
            rect.xMax = Mathf.RoundToInt(rect.xMax);
            rect.yMax = Mathf.RoundToInt(rect.yMax);
            return ConvertToTexCoords(rect, width, height);
        }

        public static int RepeatIndex(int val, int max)
        {
            if (max < 1)
            {
                return 0;
            }
            while (val < 0)
            {
                val += max;
            }
            while (val >= max)
            {
                val -= max;
            }
            return val;
        }

        public static float RotateTowards(float from, float to, float maxAngle)
        {
            float f = WrapAngle(to - from);
            if (Mathf.Abs(f) > maxAngle)
            {
                f = maxAngle * Mathf.Sign(f);
            }
            return @from + f;
        }

        public static Vector2 SpringDampen(ref Vector2 velocity, float strength, float deltaTime)
        {
            if (deltaTime > 1f)
            {
                deltaTime = 1f;
            }
            float num = 1f - strength * 0.001f;
            int num2 = Mathf.RoundToInt(deltaTime * 1000f);
            Vector2 zero = Vector2.zero;
            for (int i = 0; i < num2; i++)
            {
                zero += velocity * 0.06f;
                velocity = velocity * num;
            }
            return zero;
        }

        public static Vector3 SpringDampen(ref Vector3 velocity, float strength, float deltaTime)
        {
            if (deltaTime > 1f)
            {
                deltaTime = 1f;
            }
            float num = 1f - strength * 0.001f;
            int num2 = Mathf.RoundToInt(deltaTime * 1000f);
            Vector3 zero = Vector3.zero;
            for (int i = 0; i < num2; i++)
            {
                zero += velocity * 0.06f;
                velocity = velocity * num;
            }
            return zero;
        }

        public static float SpringLerp(float strength, float deltaTime)
        {
            if (deltaTime > 1f)
            {
                deltaTime = 1f;
            }
            int num = Mathf.RoundToInt(deltaTime * 1000f);
            deltaTime = 0.001f * strength;
            float from = 0f;
            for (int i = 0; i < num; i++)
            {
                from = Mathf.Lerp(from, 1f, deltaTime);
            }
            return from;
        }

        public static float SpringLerp(float from, float to, float strength, float deltaTime)
        {
            if (deltaTime > 1f)
            {
                deltaTime = 1f;
            }
            int num = Mathf.RoundToInt(deltaTime * 1000f);
            deltaTime = 0.001f * strength;
            for (int i = 0; i < num; i++)
            {
                from = Mathf.Lerp(from, to, deltaTime);
            }
            return from;
        }

        public static Quaternion SpringLerp(Quaternion from, Quaternion to, float strength, float deltaTime)
        {
            return Quaternion.Slerp(from, to, SpringLerp(strength, deltaTime));
        }

        public static Vector2 SpringLerp(Vector2 from, Vector2 to, float strength, float deltaTime)
        {
            return Vector2.Lerp(from, to, SpringLerp(strength, deltaTime));
        }

        public static Vector3 SpringLerp(Vector3 from, Vector3 to, float strength, float deltaTime)
        {
            return Vector3.Lerp(from, to, SpringLerp(strength, deltaTime));
        }

        public static float Wrap01(float val)
        {
            return val - Mathf.FloorToInt(val);
        }

        public static float WrapAngle(float angle)
        {
            while (angle > 180f)
            {
                angle -= 360f;
            }
            while (angle < -180f)
            {
                angle += 360f;
            }
            return angle;
        }
    }
}

