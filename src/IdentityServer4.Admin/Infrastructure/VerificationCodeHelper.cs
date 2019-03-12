using System;
using System.Drawing;
using SkiaSharp;

namespace IdentityServer4.Admin.Infrastructure
{
    /// <summary>
    /// 图片验证码
    /// </summary>
    public static class VerificationCodeHelper
    {
        private static volatile Color[] _colors =
        {
            Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan,
            Color.Purple
        };

        /// <summary>  
        /// 将生成的字符串写入图像文件  
        /// </summary>  
        /// <param name="code">验证码字符串</param>
        /// <param name="length">生成位数（默认4位）</param>  
        public static byte[] Create(out string code, int length = 4)
        {
            code = RandomCode(length);

            byte[] imageBytes;
            int imageX = 85;
            int imageY = 32;


            using (SKBitmap image = new SKBitmap(imageX, imageY, SKColorType.Bgra8888, SKAlphaType.Premul))
            {
                using (SKCanvas canvas = new SKCanvas(image))
                {
                    canvas.DrawColor(SKColors.Gray);
                    using (SKPaint drawStyle = CreatePaint())
                    {
                        canvas.DrawText(code,  10, imageY - 10, drawStyle);
                    }

                    using (SKImage img = SKImage.FromBitmap(image))
                    {
                        using (SKData p = img.Encode(SKEncodedImageFormat.Png, 100))
                        {
                            imageBytes = p.ToArray();
                        }
                    }
                }
            }

            return imageBytes;
        }

        private static SKRect MeasureText(string text, SKPaint paint)
        {
            SKRect rect = new SKRect();
            paint.MeasureText(text, ref rect);
            return rect;
        }

        private static SKPaint CreatePaint()
        {
            string font = @"";
            font += @"Arial,";
            font += @"Liberation Serif,";
            font += @"Segoe Script,";
            font += @"Consolas,";
            font += @"Comic Sans MS,";
            font += @"SimSun,";
            font += @"Impact";
            return CreatePaint(SKColors.White, font, SKFontStyleWeight.Bold, SKFontStyleWidth.Expanded,
                SKFontStyleSlant.Upright);
        }

        private static SKPaint CreateNoisyPointPaint()
        {
            string font = @"";
            font += @"Arial,";
            font += @"Liberation Serif,";
            font += @"Segoe Script,";
            font += @"Consolas,";
            font += @"Comic Sans MS,";
            font += @"SimSun,";
            font += @"Impact";
            return CreatePaint(SKColors.Fuchsia, font, SKFontStyleWeight.Bold, SKFontStyleWidth.Expanded,
                SKFontStyleSlant.Upright);
        }

        private static SKPaint CreatePaint(SKColor color, string fontName, SKFontStyleWeight weight,
            SKFontStyleWidth width, SKFontStyleSlant slant)
        {
            SKTypeface font = SKTypeface.FromFamilyName(fontName, weight, width, slant);

            SKPaint paint = new SKPaint
            {
                IsAntialias = true,
                Color = color,
                Typeface = font,
                TextSize = 18
            };


            return paint;
        }

        /// <summary>  
        /// 生成指定长度的随机字符串 
        /// </summary>  
        /// <param name="codeLength">字符串的长度</param>  
        /// <returns>返回随机数字符串</returns>  
        private static string RandomCode(int codeLength)
        {
            //组成字符串的字符集合  0-9数字、大小写字母
            string chars =
                "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,P,P,Q,R,S,T,U,V,W,X,Y,Z";

            string[] charArray = chars.Split(new[] {','});
            string code = "";
            int temp = -1; //记录上次随机数值，尽量避避免生产几个一样的随机数  
            Random rand = new Random();
            //采用一个简单的算法以保证生成随机数的不同  
            for (int i = 1; i < codeLength + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int) DateTime.Now.Ticks)); //初始化随机类  
                }

                int t = rand.Next(61);
                if (temp == t)
                {
                    return RandomCode(codeLength); //如果获取的随机数重复，则递归调用  
                }

                temp = t; //把本次产生的随机数记录起来  
                code += charArray[t]; //随机数的位数加一  
            }

            return code;
        }
    }
}