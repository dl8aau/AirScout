﻿
namespace GMap.NET.WindowsForms.Markers
{
   using System.Drawing;
   using System.Collections.Generic;

#if !PocketPC
   using System.Windows.Forms.Properties;
   using System;
   using System.Runtime.Serialization;
#else
   using GMap.NET.WindowsMobile.Properties;
#endif

   public enum GMarkerGoogleType
   {
      none = 0,
      arrow,
      blue,
      blue_small,
      blue_dot,
      blue_pushpin,
      brown_small,
      gray_small,
      green,
      green_small,
      green_dot,
      green_pushpin,
      green_big_go,
      yellow,
      yellow_small,
      yellow_dot,
      yellow_big_pause,
      yellow_pushpin,
      lightblue,
      lightblue_dot,
      lightblue_pushpin,
      orange,
      orange_small,
      orange_dot,
      pink,
      pink_dot,
      pink_pushpin,
      purple,
      purple_small,
      purple_dot,
      purple_pushpin,
      red,
      red_small,
      red_dot,
      red_pushpin,
      red_big_stop,
      black_small,
        white_small,
        white_dot,      // 2021_05_17 DL2ALF
    }

#if !PocketPC
    [Serializable]
   public class GMarkerGoogle : GMapMarker, ISerializable, IDeserializationCallback
#else
   public class GMarkerGoogle : GMapMarker
#endif
   {
      public float? Bearing;
      Bitmap Bitmap;
      Bitmap BitmapShadow;

      static Bitmap arrowshadow;
      static Bitmap msmarker_shadow;
      static Bitmap shadow_small;
      static Bitmap pushpin_shadow;

      GMarkerGoogleType type;

      public GMarkerGoogle(PointLatLng p, GMarkerGoogleType type) :
          this(p, null, type){}

      public GMarkerGoogle(PointLatLng p, Font font, GMarkerGoogleType type)
         : base(p, font)
      {
         this.type = type;

         if(type != GMarkerGoogleType.none)
         {
            LoadBitmap();
         }
      }

      void LoadBitmap()
      {
         Bitmap = GetIcon(type.ToString());
         Size = new System.Drawing.Size(Bitmap.Width, Bitmap.Height);

         switch(type)
         {
            case GMarkerGoogleType.arrow:
            {
               Offset = new Point(-11, -Size.Height);

               if(arrowshadow == null)
               {
                  arrowshadow = Resources.arrowshadow;
               }
               BitmapShadow = arrowshadow;
            }
            break;

            case GMarkerGoogleType.blue:
            case GMarkerGoogleType.blue_dot:
            case GMarkerGoogleType.green:
            case GMarkerGoogleType.green_dot:
            case GMarkerGoogleType.yellow:
            case GMarkerGoogleType.yellow_dot:
            case GMarkerGoogleType.lightblue:
            case GMarkerGoogleType.lightblue_dot:
            case GMarkerGoogleType.orange:
            case GMarkerGoogleType.orange_dot:
            case GMarkerGoogleType.pink:
            case GMarkerGoogleType.pink_dot:
            case GMarkerGoogleType.purple:
            case GMarkerGoogleType.purple_dot:
            case GMarkerGoogleType.red:
            case GMarkerGoogleType.red_dot:
            case GMarkerGoogleType.white_dot:
            {
               Offset = new Point(-Size.Width / 2 + 1, -Size.Height + 1);

               if(msmarker_shadow == null)
               {
                  msmarker_shadow = Resources.msmarker_shadow;
               }
               BitmapShadow = msmarker_shadow;
            }
            break;

            case GMarkerGoogleType.black_small:
            case GMarkerGoogleType.blue_small:
            case GMarkerGoogleType.brown_small:
            case GMarkerGoogleType.gray_small:
            case GMarkerGoogleType.green_small:
            case GMarkerGoogleType.yellow_small:
            case GMarkerGoogleType.orange_small:
            case GMarkerGoogleType.purple_small:
            case GMarkerGoogleType.red_small:
            case GMarkerGoogleType.white_small:
            {
               Offset = new Point(-Size.Width / 2, -Size.Height + 1);

               if(shadow_small == null)
               {
                  shadow_small = Resources.shadow_small;
               }
               BitmapShadow = shadow_small;
            }
            break;

            case GMarkerGoogleType.green_big_go:
            case GMarkerGoogleType.yellow_big_pause:
            case GMarkerGoogleType.red_big_stop:
            {
               Offset = new Point(-Size.Width / 2, -Size.Height + 1);
               if(msmarker_shadow == null)
               {
                  msmarker_shadow = Resources.msmarker_shadow;
               }
               BitmapShadow = msmarker_shadow;
            }
            break;

            case GMarkerGoogleType.blue_pushpin:
            case GMarkerGoogleType.green_pushpin:
            case GMarkerGoogleType.yellow_pushpin:
            case GMarkerGoogleType.lightblue_pushpin:
            case GMarkerGoogleType.pink_pushpin:
            case GMarkerGoogleType.purple_pushpin:
            case GMarkerGoogleType.red_pushpin:
            {
               Offset = new Point(-9, -Size.Height + 1);

               if(pushpin_shadow == null)
               {
                  pushpin_shadow = Resources.pushpin_shadow;
               }
               BitmapShadow = pushpin_shadow;
            }
            break;
         }
      }

      /// <summary>
      /// marker using manual bitmap, NonSerialized
      /// </summary>
      /// <param name="p"></param>
      /// <param name="Bitmap"></param>
      public GMarkerGoogle(PointLatLng p, Bitmap Bitmap) :
          this(p, null, Bitmap) { }

      public GMarkerGoogle(PointLatLng p, Font font, Bitmap Bitmap)
         : base(p, font)
      {
         this.Bitmap = Bitmap;
         Size = new System.Drawing.Size(Bitmap.Width, Bitmap.Height);
         Offset = new Point(-Size.Width / 2, -Size.Height/2);
      }

      static readonly Dictionary<string, Bitmap> iconCache = new Dictionary<string, Bitmap>();

      internal static Bitmap GetIcon(string name)
      {
         Bitmap ret;
         if(!iconCache.TryGetValue(name, out ret))
         {
            ret = Resources.ResourceManager.GetObject(name, Resources.Culture) as Bitmap;
            iconCache.Add(name, ret);
         }
         return ret;
      }

      static readonly Point[] Arrow = new Point[] { new Point(-7, 7), new Point(0, -22), new Point(7, 7), new Point(0, 2) };

      public override void OnRender(Graphics g)
      {
#if !PocketPC
         //if(!Bearing.HasValue)
         {
            if(BitmapShadow != null)
            {
               g.DrawImage(BitmapShadow, LocalPosition.X, LocalPosition.Y, BitmapShadow.Width, BitmapShadow.Height);
            }
         }

         //if(Bearing.HasValue)
         //{
         //   g.RotateTransform(Bearing.Value - Overlay.Control.Bearing);
         //   g.FillPolygon(Brushes.Red, Arrow);
         //}

         //if(!Bearing.HasValue)
         {
            g.DrawImage(Bitmap, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
         }
#else
         if(BitmapShadow != null)
         {
            DrawImageUnscaled(g, BitmapShadow, LocalPosition.X, LocalPosition.Y);
         }
         DrawImageUnscaled(g, Bitmap, LocalPosition.X, LocalPosition.Y);
#endif
      }

      public override void Dispose()
      {
         if(Bitmap != null)
         {
            if(!iconCache.ContainsValue(Bitmap))
            {
               Bitmap.Dispose();
               Bitmap = null;
            }
         }

         base.Dispose();
      }

#if !PocketPC

      #region ISerializable Members

      void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("type", this.type);
         info.AddValue("Bearing", this.Bearing);

         base.GetObjectData(info, context);
      }

      protected GMarkerGoogle(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
         this.type = Extensions.GetStruct<GMarkerGoogleType>(info, "type", GMarkerGoogleType.none);
         this.Bearing = Extensions.GetStruct<float>(info, "Bearing", null);
      }

      #endregion

      #region IDeserializationCallback Members

      public void OnDeserialization(object sender)
      {
         if(type != GMarkerGoogleType.none)
         {
            LoadBitmap();
         }
      }

      #endregion

#endif
   }
}
