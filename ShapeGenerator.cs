using OpenTK;
using OpenTK.Graphics;
using System.Drawing;
using System.IO;
using System.Text;


namespace SBImageLib
{

    public class ShapeInfo
    {
        public Rectangle Size = new Rectangle(0, 0, 0, 0);
        public int Width => Size.Width; public int Height => Size.Height;
        public Color4 DefaultColor;
        public string Path;
        internal ShapeInfo(Rectangle Size, Color4 Color, string Path)
        {
            this.Size = Size;
            this.DefaultColor = Color;
            this.Path = Path;
        }
        

    }
    public static class Shape
    {
        
        public static ShapeInfo GenerateRectangle(BasicGeneratorParams Params)
        {
            return _generateBasicShape(Params, 1);
        }
        public static ShapeInfo GenerateEllipse(BasicGeneratorParams Params)
        {
            return _generateBasicShape(Params, 0);
        }
        
        private static ShapeInfo? _generateBasicShape(BasicGeneratorParams Params, int type)
        {
            if (Params.IsUnutilized()) return null;

            Rectangle size = Params.Size;
            Color4 c = Params.LineColor;
            Color4 c2 = Params.FillColor;
            int c_int = c.ToArgb();
            int c_int2 = c2.ToArgb();
            int linewidth = Params.LineWidth;
            string typestr = "";
            switch (type)
            {
                case 0:
                    {
                        typestr = "circ";
                        break;
                    }
                case 1:
                    {
                        typestr = "rect";
                        break;
                    }
                default: break;

            }

            string folder = "sb/Generated-Shapes/Basics/";
            string PartialPath = folder
                + "["
                + size.Width.ToString()
                + "x"
                + size.Height.ToString()
                + "]"
                + typestr
                + c_int.ToString()
                + "_"
                + c_int2.ToString()
                + "_"
                + linewidth.ToString()

                + ".png";
            ImageEditor.CreateFolder(folder);
            string FullPath = ImageEditor.GetFullExportPath(PartialPath);
            Rectangle baserect = new Rectangle(0, 0, size.Width + linewidth*2, size.Height + linewidth*2);
            if (!File.Exists(FullPath))
            {

                Bitmap b = new Bitmap(baserect.Width, baserect.Height);
                Rectangle actrect = new Rectangle(linewidth, linewidth, size.Width, size.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    Color cconvert = Color.FromArgb(c_int);
                    Color cconvert2 = Color.FromArgb(c_int2);
                    Pen pen = new Pen(cconvert, linewidth);
                    Brush brush = new SolidBrush(cconvert2);
                    if (type == 0)
                    {
                        g.DrawEllipse(pen, actrect);
                        g.FillEllipse(brush, actrect);
                    }
                    if (type == 1)
                    {
                        g.DrawRectangle(pen, actrect);
                        g.FillRectangle(brush, actrect);
                    }
                    b.Save(FullPath);

                }
            }
            return new ShapeInfo(baserect, c, PartialPath);
        }
        public static ShapeInfo? GenerateLine(ShapeGeneratorParams Params)
        {
            if (Params.IsUnutilized()) return null;
            Rectangle size = Params.Size;
            Color4 c = Params.LineColor;
            int c_int = c.ToArgb();

            CurveType curveType = Params.CurveType;
            int linewidth = Params.LineWidth;
            Vector2[] pts = Params.Points;
            PointF[] pts_F = new PointF[pts.Length];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pts.Length; i++)
            {
                pts_F[i] = new PointF(pts[i].X, pts[i].Y);
                sb.Append(pts[i].X + "-" + pts[i].Y + "--");
            }
            string folder = "sb/Generated-Shapes/Lines/";
            string PartialPath = folder
                + "["
                + size.Width.ToString()
                + "x"
                + size.Height.ToString()
                + "]"
                + c_int.ToString()
                + "_"
                + linewidth.ToString()
                + "_"
                + sb.ToString().GetHashCode().ToString()
                + ".png";
            ImageEditor.CreateFolder(folder);
            string FullPath = ImageEditor.GetFullExportPath(PartialPath);


            if (!File.Exists(FullPath))
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    Color cconvert = Color.FromArgb(c_int);
                    Pen pen = new Pen(cconvert, linewidth);
                    if (pts.Length > 1)
                    {
                        if (curveType == CurveType.None)
                        {
                            g.DrawLines(pen, pts_F);
                            

                        }
                        else
                        {
                            g.DrawCurve(pen, pts_F);
                        }
                    }
                    else
                    {
                        g.DrawLine(pen, pts_F[0], pts_F[1]);
                    }

                    b.Save(FullPath);

                }
            }
            return new ShapeInfo(size, c, PartialPath);

        }
        public static ShapeInfo? GenerateShape(
                    ShapeGeneratorParams Params)
        {
            if (Params.IsUnutilized()) return null;

                   
            Rectangle size = Params.Size;
            Color4 c = Params.LineColor;
            Color4 c2 = Params.FillColor;
            int c_int = c.ToArgb();
            int c_int2 = c2.ToArgb();

            CurveType curveType = Params.CurveType;
            int linewidth  = Params.LineWidth;
            Vector2[] pts = Params.Points;
            PointF[] pts_F = new PointF[pts.Length];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pts.Length; i++)
            {
                pts_F[i] = new PointF(pts[i].X, pts[i].Y);
                sb.Append(pts[i].X + "-" + pts[i].Y + "--");
            }
            string folder = "sb/Generated-Shapes/Shapes/";
            string PartialPath = folder
                + "["
                + size.Width.ToString()
                + "x"
                + size.Height.ToString()
                + "]"
                + c_int.ToString()
                + "_"
                + c_int2.ToString()
                + "_"
                + linewidth.ToString()
                + "_"
                + sb.ToString().GetHashCode().ToString()
                + ".png";
            ImageEditor.CreateFolder(folder);
            string FullPath = ImageEditor.GetFullExportPath(PartialPath);


            if (!File.Exists(FullPath))
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    Color cconvert = Color.FromArgb(c_int);
                    Color cconvert2 = Color.FromArgb(c_int2);
                    Pen pen = new Pen(cconvert, linewidth);
                    Brush brush = new SolidBrush(cconvert2);
                    if (pts.Length > 1)
                    {
                        if (curveType == CurveType.None)
                        {
                            g.DrawPolygon(pen, pts_F);
                            g.FillPolygon(brush, pts_F);
                            
                        }
                        else
                        {
                            g.DrawClosedCurve(pen, pts_F);
                            g.FillClosedCurve(brush, pts_F);
                        }
                    }
                    else
                    {
                        g.DrawLine(pen, pts_F[0], pts_F[1]);
                    }

                    b.Save(FullPath);

                }
            }
            return new ShapeInfo(size, c, PartialPath);
            
        }
    }
    /// <summary>
    /// This class outlines the parameters for the GenerateEllipse and GenerateRectangle class.
    /// </summary>
    public class BasicGeneratorParams
    {
        /// <summary>
        /// Indicates the size of the circle.
        /// </summary>
        public Rectangle Size = Rectangle.Empty;
        /// <summary>
        /// The color of the line.
        /// </summary>
        public Color4 LineColor = Color4.White;
        /// <summary>
        /// The fill color.
        /// </summary>
        public Color4 FillColor = Color4.White;
        /// <summary>
        /// The width of the line.
        /// </summary>
        public int LineWidth = 0;

        internal bool IsUnutilized()
        {
            return (Size.IsEmpty && LineColor==Color4.White && FillColor == Color4.White && LineWidth == 0);
        }
    }
    /// <summary>
    /// This class outlines the parameters for the GenerateShape function. <br/>
    /// Editable Parameters:<br/>
    /// Size (Rectangle) - Size of finished polygon / bezier shape. <br/>
    /// Points (Vector2[]) - Points to draw through.<br/>
    /// LineColor (Color4) - The color of the line.<br/>
    /// FillColor (Color4) - The color of the fill.<br/>
    /// CurveType (CurveType) - The type of curve present when drawing.<br/>
    /// LineWidth (int) - The width of the line.
    /// </summary>
    public class ShapeGeneratorParams
    {
        /// <summary>
        /// Indicates the size of the finished polygon.
        /// </summary>
        public Rectangle Size = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// Acts as the drawing points for the image.
        /// </summary>
        public Vector2[] Points = null;

        /// <summary>
        /// Specifies the color of the line (default: white).
        /// Use FillColor to fill in the shape a certain way.
        /// </summary>
        public Color4 LineColor = Color4.White;

        /// <summary>
        /// Specifies the color of the fill (default: white).
        /// Use LineColor to choose the line color.
        /// </summary>
        public Color4 FillColor = Color4.White;

        /// <summary>
        /// The Curve Type of the drawing.
        /// </summary>
        public CurveType CurveType = CurveType.None;

        /// <summary>
        /// The line width of the drawing (will be the same color as the inside.)
        /// </summary>
        public int LineWidth = 0;
        internal bool IsUnutilized()
        {
            return (Size.IsEmpty && (Points == null || Points.Length < 1)) ;
        }
    }
    public enum CurveType
    {
        None = 0,
        Curve = 1
    }
}
