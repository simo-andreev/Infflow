using System;
using Emotion.Primitives;

namespace Infflow
{
    /// <summary>
    /// @see https://developer.android.com/reference/android/graphics/PorterDuff.Mode
    /// </summary>
    public static class ColorBlenderer
    {
        public static Color Darken(this Color source, Color destination)
        {
            float alphaSrc = source.A / 255.0f;
            float alphaDest = destination.A / 255.0f;
            float alpha = alphaSrc + alphaDest - alphaSrc * alphaDest;

            var col1 = new Color(source, (byte) ((1 - alphaDest) * 255));
            var col2 = new Color(destination, (byte) ((1 - alphaSrc) * 255));
            var col3 = new Color(
                Math.Min(source.R, destination.R),
                Math.Min(source.G, destination.G),
                Math.Min(source.B, destination.B),
                (byte) alpha * 255
            );

            return col1 + col2 + col3;
        }

        public static Color Lighten(this Color source, Color destination)
        {
            float alphaSrc = source.A / 255.0f;
            float alphaDest = destination.A / 255.0f;
            float alpha = alphaSrc + alphaDest - alphaSrc * alphaDest;

            var col1 = new Color(source, (byte) ((1 - alphaDest) * 255));
            var col2 = new Color(destination, (byte) ((1 - alphaSrc) * 255));
            var col3 = new Color(
                Math.Max(source.R, destination.R),
                Math.Max(source.G, destination.G),
                Math.Max(source.B, destination.B),
                (byte) alpha * 255
            );

            return col1 + col2 + col3;
        }

        public static Color Overlay(this Color destination, Color source)
        {
            var paintA = source.A / 255.0;
            
            var resultR = Math.Min(source.R * paintA + destination.R * (1 - paintA), 255);
            var resultG = Math.Min(source.G * paintA + destination.G * (1 - paintA), 255);
            var resultB = Math.Min(source.B * paintA + destination.B * (1 - paintA), 255);

            return new Color((byte) resultR, (byte) resultG, (byte) resultB);
        }
    }
}