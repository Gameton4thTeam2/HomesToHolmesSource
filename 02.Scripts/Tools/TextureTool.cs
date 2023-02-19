using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTH.Tools
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 텍스쳐 잘라내기 툴
    /// </summary>
    public class TextureTool : MonoBehaviour
    {
        public static Texture2D ResampleAndCrop(Texture2D source, int targetWidth, int targetHeight)
        {
            int sourceWidth = source.width;
            int sourceHeight = source.height;
            float sourceAspect = (float)sourceWidth / sourceHeight; // 가로 세로 비율
            float targetAspect = (float)targetWidth / targetHeight;
            int xOffset = 0;
            int yOffset = 0;
            float factor = 1;
            if (sourceAspect > targetAspect)
            { // crop width
                factor = (float)targetHeight / sourceHeight;
                xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
            }
            else
            { // crop height
                factor = (float)targetWidth / sourceWidth;
                yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
            }
            Color32[] data = source.GetPixels32();
            Color32[] data2 = new Color32[targetWidth * targetHeight];
            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                    // bilinear filtering
                    var c11 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c12 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                    var c21 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c22 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                    var f = new Vector2(Mathf.Repeat(p.x, 1f), Mathf.Repeat(p.y, 1f));
                    data2[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
                }
            }

            var tex = new Texture2D(targetWidth, targetHeight);
            tex.SetPixels32(data2);
            tex.Apply(true);
            return tex;
        }
    }
}