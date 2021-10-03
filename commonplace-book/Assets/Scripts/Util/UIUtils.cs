using UnityEngine;
using System;
using System.Text;

public static class UIUtils {

    public static void AttachAndCenter(GameObject parent, GameObject child) {
        RectTransform parentTransform = parent.GetComponent<RectTransform>();
        RectTransform childTransform = child.GetComponent<RectTransform>();
        childTransform.SetParent(parentTransform);
        childTransform.anchorMin = new Vector2(0.5f, 0.5f);
        childTransform.anchorMax = childTransform.anchorMin;
        childTransform.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
        childTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    /// <returns>The input string with ascii characters for glyphs replacing $A, $B etc</returns>
    public static string GlyphifyString(string input) {
        if (input == null) return null;
        if (!input.Contains("$")) return input;
        StringBuilder output = new StringBuilder();
        for (int i = 0; i < input.Length; i += 1) {
            if (input[i] == '$') {
                var @char = input[i + 1];
                int ascii = @char + 232 - 'A'; // glyph $A starts at 232
                if (@char == 'I') ascii = 208;
                else if (@char == 'J') ascii = 209;
                else if (@char == 'K') ascii = 210;
                else if (@char == 'L') ascii = 211;
                else if (@char == 'M') ascii = 212;
                else if (@char == 'N') ascii = 213;
                else if (@char == 'O') ascii = 224;
                else if (@char == 'P') ascii = 225;
                else if (@char == 'Q') ascii = 226;
                else if (@char == 'R') ascii = 227;
                else if (@char == 'S') ascii = 228;
                else if (@char == 'T') ascii = 229;
                else if (@char == 'U') ascii = 230;
                else if (@char == 'V') ascii = 214;
                else if (@char == 'W') ascii = 215;
                else if (@char == 'X') ascii = 216;
                else if (@char == 'Y') ascii = 217;
                else if (@char == 'Z') ascii = 218;
                output.Append((char)ascii);
                i += 1;
            } else {
                output.Append(input[i]);
            }
        }
        return output.ToString();
    }

    public static DateTime TimestampToDateTime(long timestamp) {
        var offset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return offset.DateTime;
    }

    public static TimeSpan SecondsToTimeSpan(long seconds) {
        return new TimeSpan(0, 0, (int)seconds);
    }

    public static long CurrentTimestamp() {
        var offset = DateTimeOffset.UtcNow;
        return offset.ToUnixTimeSeconds();
    }

    public static string FormatDateTime(DateTime time) {
        return time.ToString("yyyy'-'MM'-'dd");
    }

    public static string FormatTimeSpan(TimeSpan time) {
        return $"{time:g}";
    }
}
