using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace lab_2_mdi
{
    [Serializable()]
    public abstract class Figure
    {
        [NonSerialized] public bool isSelected;

        protected Point p1;
        protected Point p2;
        protected Rectangle localRectangle;
        protected Point localP1, localP2;

        protected Color penColor;
        protected float penWidth;

        public Rectangle rectangle;
        public bool isFilled;
        
        public Figure(Point p1, Point p2, Color penColor, float penWidth)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.penColor = penColor;
            this.penWidth = penWidth;
            setRectangle();
            isSelected = false;
        }

        public Figure(Point p1, Point p2, Color penColor)
            : this(p1, p2, penColor, 1F) { }

        public Figure()
            : this(new Point(), new Point(), Color.Black) { }


        public virtual void setPoint1(Point p)
        {
            p1 = p;
            setRectangle();
        }

        public virtual void setPoint2(Point p)
        {
            p2 = p;
            setRectangle();
        }

        public abstract void Draw(Graphics g, Size scrollPosition);
        public abstract void Draw(Graphics g, Color penColor, Size scrollPosition);
        public abstract void DrawSolid(Graphics g, Size scrollPosition);
        public abstract void DrawDash(Graphics g, Size scrollPosition);
        public abstract void Hide(Graphics g, Size scrollPosition);



        public virtual void MouseMove( Point offset )
        {
            rectangle.Location = Point.Add(rectangle.Location, (Size)offset);
        }

        Point loc = new Point();
        Size size = new Size();
        public void setRectangle()
        {
            loc.X = Math.Min(p1.X, p2.X);
            loc.Y = Math.Min(p1.Y, p2.Y);
            size.Width = Math.Abs(p1.X - p2.X);
            size.Height = Math.Abs(p1.Y - p2.Y);
            rectangle.Location = loc;
            rectangle.Size = size;
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable()]
    public class Rect : Figure
    {       
        Color solidBrushColor = Color.White;

        public Rect(Point p1, Point p2, Color penColor, float penWidth, Color solidBrushColor)
            : base (p1, p2, penColor, penWidth)
        {
            this.solidBrushColor = solidBrushColor;
        }

        public Rect(Point p1, Point p2, Color penColor, float penWidth, Color solidBrushColor, bool isFilled)
            : this(p1, p2, penColor, penWidth, solidBrushColor)
        {
            this.isFilled = isFilled;
        }

        public override void Draw(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawRectangle(penSolid, localRectangle);
        }
        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawRectangle(penSolid, localRectangle);
        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            Brush solidBrush = new SolidBrush(solidBrushColor);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.FillRectangle(solidBrush, localRectangle);
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {
            Pen penDashed = new Pen(penColor, penWidth);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawRectangle(penDashed, localRectangle);
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable()]
    public class _Ellipse : Figure
    {
        Color solidBrushColor = Color.White;

        public _Ellipse(Point p1, Point p2, Color penColor, float penWidth, Color solidBrushColor)
            : base (p1, p2, penColor, penWidth)
        {
            this.solidBrushColor = solidBrushColor;
        }
        public _Ellipse(Point p1, Point p2, Color penColor, float penWidth, Color solidBrushColor, bool isFilled)
            : this(p1, p2, penColor, penWidth, solidBrushColor)
        {
            this.isFilled = isFilled;
        }
        public override void Draw(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawEllipse(penSolid, localRectangle);
        }
        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawEllipse(penSolid, localRectangle);
        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            Brush solidBrush = new SolidBrush(solidBrushColor);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.FillEllipse(solidBrush, localRectangle);
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {
            Pen penDashed = new Pen(penColor, penWidth);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawEllipse(penDashed, localRectangle);
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable()]
    public class _Line : Figure
    {
        public _Line(Point p1, Point p2, Color penColor, float penWidth)
            : base(p1, p2, penColor, penWidth) { }
        public override void Draw(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localP1 = Point.Add(p1, scrollPosition);
            localP2 = Point.Add(p2, scrollPosition);
            g.DrawLine(penSolid, localP1, localP2);

            // test
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawRectangle(Pens.Red, localRectangle);
            //
        }
        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localP1 = Point.Add(p1, scrollPosition);
            localP2 = Point.Add(p2, scrollPosition);
            g.DrawLine(penSolid, localP1, localP2);

        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            // no solid line
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {
            Pen penDashed = new Pen(penColor, penWidth);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localP1 = Point.Add(p1, scrollPosition);
            localP2 = Point.Add(p2, scrollPosition);
            g.DrawLine(penDashed, localP1, localP2);
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            throw new NotImplementedException();
        }
        public override void MouseMove(Point offset)
        {
            base.MouseMove(offset);
            p1 = Point.Add(p1, (Size)offset);
            p2 = Point.Add(p2, (Size)offset);
        }
    }

    [Serializable()]
    class _Curve : Figure
    {
        public List<Point> curvePointList;
        List<Point> localCurvePointList;

        public _Curve(Point p1, Point p2, Color penColor, float penWidth)
            : base(p1, p2, penColor, penWidth)
        {
            curvePointList = new List<Point>();
            curvePointList.Add(p1);
            curvePointList.Add(p2);
            localCurvePointList = new List<Point>();      
        }

        public override void setPoint2(Point p)
        {
            base.setPoint2(p);
            curvePointList.Add(p);
        }

        public override void Draw(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localCurvePointList.Clear();
            foreach (var item in curvePointList)
            {
                localCurvePointList.Add(Point.Add(item, scrollPosition));
            }
            g.DrawCurve(penSolid, localCurvePointList.ToArray());
            // test
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawRectangle(Pens.Red, localRectangle);
            //
        }
        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localCurvePointList.Clear();
            foreach (var item in curvePointList)
            {
                localCurvePointList.Add(Point.Add(item, scrollPosition));
            }
            g.DrawCurve(penSolid, localCurvePointList.ToArray());
        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            // no solid
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {
            Pen penDashed = new Pen(penColor, penWidth);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localCurvePointList.Clear();
            foreach (var item in curvePointList)
            {
                localCurvePointList.Add(Point.Add(item, scrollPosition));
            }
            g.DrawCurve(penDashed, localCurvePointList.ToArray());
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            
        }
        public override void MouseMove(Point offset)
        {
            base.MouseMove(offset);
            for (int i = 0; i < curvePointList.Count; i++)
            {
                curvePointList[i] = Point.Add(curvePointList[i], (Size)offset);
            }
        }

        //public override object Clone()
        //{
        //    _Curve newCurve = (_Curve)MemberwiseClone();
        //    newCurve.curvePointList.Clear();
        //    foreach (var item in curvePointList)
        //    {
        //        newCurve.curvePointList.Add(item);
        //    }
        //    return newCurve;
        //}
    }

    [Serializable()]
    class _Text : Figure
    {
        public string text;
        public Font font;

        public _Text(Point p1, Point p2, Color penColor)
            : this(p1, p2, penColor, new Font(FontFamily.GenericSansSerif, 8F)) { }

        public _Text(Point p1, Point p2, Color penColor, Font font)
            : base(p1, p2, penColor)
        {
            this.font = font;
        }

        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            
        }
        public override void Draw(Graphics g, Size scrollPosition)
        {
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            Brush solidBrush = new SolidBrush(penColor);
            g.DrawString(text, font, solidBrush, localRectangle);
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {
            Pen penDashed = new Pen(Color.Black, 1F);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            Brush solidBrush = new SolidBrush(penColor);
            g.DrawRectangle(penDashed, localRectangle); // Draw rectangle Black for text position
            g.DrawString(text, font, solidBrush, localRectangle);


        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            throw new NotImplementedException();
        }
    }
}
