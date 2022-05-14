using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
/// <summary>
/// Summary description for ImageUtility
/// </summary>
public class ImageUtility
{
    static int quality = 30;

    public ImageUtility()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void SaveSlideImage(string ImagePath, string NewPath)
    {
        int Width = 1900;
        int Height = 1424;


        string sImageName = Path.GetFileName(ImagePath);


        Image imgPhoto = resizeImage(Width, Height, ImagePath);

        EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
        // JPEG image codec 
        ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
        EncoderParameters encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = qualityParam;


        imgPhoto.Save(NewPath + "//" + sImageName, jpegCodec, encoderParams);


    }

    public static void SaveThumbnailImage(string ImagePath, string NewPath)
    {
        if (!System.IO.Directory.Exists(NewPath))
        {
            System.IO.Directory.CreateDirectory(NewPath);
        }

        int Width = 100;
        int Height = 75;

        string sImageName = Path.GetFileName(ImagePath);


        Image imgPhoto = resizeImage(Width, Height, ImagePath);

        EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
        // JPEG image codec 
        ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
        EncoderParameters encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = qualityParam;


        imgPhoto.Save(NewPath + "//" + sImageName, jpegCodec, encoderParams);


    }

    public static void SaveThumbnailImageFromTools(string ImagePath, string NewPath)
    {
        if (!System.IO.Directory.Exists(NewPath))
        {
            System.IO.Directory.CreateDirectory(NewPath);
        }

        int Width = 100;
        int Height = 75;

        string sImageName = Path.GetFileName(ImagePath);


        Image imgPhoto = resizeImage(Width, Height, ImagePath);

        EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
        // JPEG image codec 
        ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
        EncoderParameters encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = qualityParam;


        imgPhoto.Save(NewPath + "//" + sImageName, jpegCodec, encoderParams);


    }

    private static Image resizeImage(int newWidth, int newHeight, string stPhotoPath)
    {
        Image imgPhoto = Image.FromFile(stPhotoPath);

        int sourceWidth = imgPhoto.Width;
        int sourceHeight = imgPhoto.Height;

        //Consider vertical pics
        if (sourceWidth < sourceHeight)
        {
            int buff = newWidth;

            newWidth = newHeight;
            newHeight = buff;
        }

        int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
        float nPercent = 0, nPercentW = 0, nPercentH = 0;

        nPercentW = ((float)newWidth / (float)sourceWidth);
        nPercentH = ((float)newHeight / (float)sourceHeight);
        if (nPercentH < nPercentW)
        {
            nPercent = nPercentH;
            destX = System.Convert.ToInt16((newWidth -
                      (sourceWidth * nPercent)) / 2);
        }
        else
        {
            nPercent = nPercentW;
            destY = System.Convert.ToInt16((newHeight -
                      (sourceHeight * nPercent)) / 2);
        }

        int destWidth = (int)(sourceWidth * nPercent);
        int destHeight = (int)(sourceHeight * nPercent);


        Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                      PixelFormat.Format24bppRgb);

        bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                     imgPhoto.VerticalResolution);

        Graphics grPhoto = Graphics.FromImage(bmPhoto);
        grPhoto.Clear(Color.Black);

        grPhoto.InterpolationMode =
            System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        grPhoto.DrawImage(imgPhoto,
            new Rectangle(destX, destY, destWidth, destHeight),
            new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
            GraphicsUnit.Pixel);

        grPhoto.Dispose();
        imgPhoto.Dispose();
        return bmPhoto;
    }

    private static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        // Get image codecs for all image formats 
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

        // Find the correct image codec 
        for (int i = 0; i < codecs.Length; i++)
            if (codecs[i].MimeType == mimeType)
                return codecs[i];

        return null;
    }

    public static void RotateAndSaveImage(string stPhotoPath)
    {
        //create an object that we can use to examine an image file
        using (Image img = Image.FromFile(stPhotoPath))
        {
            //rotate the picture by 90 degrees and re-save the picture as a Jpeg
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            img.Save(stPhotoPath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}