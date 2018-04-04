using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RS.Lib
{
    /// <summary>
    /// 图片助手类
    /// </summary>
    public class ImageHelper
    {
        #region 辅助方法
        /// <summary>
        /// 根据指定最大宽高度获取图片要合适的宽高度
        /// </summary>
        /// <param name="height"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Size GetSize(int maxWH, Image image)
        {
            int nw, nh;
            if (image.Width <= maxWH && image.Height <= maxWH)
            {
                nw = image.Width;
                nh = image.Height;
            }
            else //进行缩放处理
            {
                if (image.Width >= maxWH) //宽度超过
                {
                    if (image.Width < image.Height)
                    {
                        nh = maxWH;
                        nw = (image.Width * nh) / image.Height;
                    }
                    else
                    {
                        nw = maxWH;
                        nh = (image.Height * nw) / image.Width;
                    }
                }
                else if (image.Height >= maxWH)
                {
                    nh = maxWH;
                    nw = (image.Width * nh) / image.Height;
                }
                else
                {
                    nw = image.Width;
                    nh = image.Height;
                }
            }
            return new Size(nw, nh);
        }

        #endregion

        #region 图片缩放处理

        /// <summary>
        /// 取得指定图片的微缩图
        /// </summary>
        public static Image GetThumbnail(Image image,int MaxWH)
        {
            Image saveImg = null;

            Size size = GetSize(MaxWH, image);
            saveImg = GenerateThumbnail(image, size.Width, size.Height);            
            return saveImg;
        }

        /// <summary>
        /// 对图片进行缩小处理
        /// </summary>
        /// <param name="original"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private static Image GenerateThumbnail(Image original, int w, int h)
        {
            
            System.Drawing.Bitmap tn = new System.Drawing.Bitmap(w, h);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tn);
            
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImage(original, new System.Drawing.Rectangle(0, 0, tn.Width, tn.Height), 0, 0, original.Width, original.Height, System.Drawing.GraphicsUnit.Pixel);
            g.Dispose();
            return tn;
        }


        #endregion

        #region 加水印相关方法

        /// <summary>
        /// 给图片加文字水印
        /// </summary>
        /// <param name="file">待加水印的图片</param>
        public static void AddWaterMarkText(Image img, string MarkText)
        {
            try
            {
                string text = MarkText;

                Font font = new Font("宋体", 20);
                Color color = Color.FromName("#CCCCCC");
                //加文字水印，注意，这里的代码和以下加图片水印的代码不能共存 
                float w = text.Length * (font.Size + 1);
                float h = font.Size + 1;
                float x, y;
                //确定水印位置
                Azimuth azimuth = Azimuth.RightBottom;
                if (azimuth == Azimuth.LeftBottom)
                {
                    x = 10;
                    y = 10;
                }
                switch (azimuth)
                {
                    case Azimuth.LeftBottom:
                        x = 10;
                        y = img.Height - 10 - h;
                        break;
                    case Azimuth.LeftTop:
                        x = 10;
                        y = 10;
                        break;
                    case Azimuth.RightBottom:
                        x = img.Width - 10 - w;
                        y = img.Height - 10 - h;
                        break;
                    case Azimuth.RightTop:
                        x = img.Width - 10 - w;
                        y = 10;
                        break;
                    default:
                        x = 10;
                        y = 10;
                        break;
                }

                Graphics gh = Graphics.FromImage(img);
                gh.DrawImage(img, 0, 0, img.Width, img.Height);
                Brush b = new SolidBrush(color);
                gh.DrawString(text, font, b, x, y);
                gh.Dispose();
            }
            catch
            { }
        }

        /// <summary>
        /// 给图片加图片水印
        /// </summary>
        /// <param name="file">待加水印的图片</param>
        public static void AddWaterMarkImage(Image img, Image MarkImg)
        {
            try
            {
                System.Drawing.Image copyImage = MarkImg;
                int w = copyImage.Width;
                int h = copyImage.Height;
                int x, y;
                Azimuth azimuth = Azimuth.RightBottom;
                if (azimuth == Azimuth.LeftBottom)
                {
                    x = 10;
                    y = 10;
                }

                switch (azimuth)
                {
                    case Azimuth.LeftBottom:
                        x = 10;
                        y = img.Height - 10 - h;
                        break;
                    case Azimuth.LeftTop:
                        x = 10;
                        y = 10;
                        break;
                    case Azimuth.RightBottom:
                        x = img.Width - 10 - w;
                        y = img.Height - 10 - h;
                        break;
                    case Azimuth.RightTop:
                        x = img.Width - 10 - w;
                        y = 10;
                        break;
                    default:
                        x = 10;
                        y = 10;
                        break;
                }


                //加图片水印 
                Graphics g = Graphics.FromImage(img);
                g.DrawImage(copyImage, new Rectangle(x, y, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                g.Dispose();
            }
            catch
            { }
        }

        #endregion
    }

    public enum Azimuth
    {
        LeftTop,
        LeftBottom,
        RightBottom,
        RightTop
    }
}
