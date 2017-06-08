using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace imageeditor
{
    public partial class Form2 : Form
    {
        Graphics g;
        bool isClicked;
        public bool isModified;
        public bool isSaved;

        public List<Figure> listFigure;
        public List<Figure> listFigureCopy; // Copy
        Figure myFigure;

        public string saveFileName;
        public Size pictureSize;

        public BufferedGraphics bufferedGraphics;
        public BufferedGraphicsContext bufferedGraphicsContext;

        public bool isMoving = false;  // Moving
        public Point mouseDownPoint = new Point();

        public bool addSelection = false;

        public Form2(Size pictureSize)
        {
            InitializeComponent();
            listFigure = new List<Figure>();
            listFigureCopy = new List<Figure>(); // Copy

            this.pictureSize = pictureSize;
            this.Size = pictureSize;

            myFigure = new Rect(new Point(0, 0), (Point)pictureSize, Color.White, 1F, Color.White, true);
            listFigure.Add(myFigure);

            this.AutoScrollMinSize = pictureSize;

            saveFileName = null;
            isSaved = false;
            isModified = false;
            isClicked = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            bufferedGraphicsContext = BufferedGraphicsManager.Current;
            bufferedGraphicsContext.MaximumBuffer = SystemInformation.PrimaryMonitorMaximizedWindowSize;
            bufferedGraphics = bufferedGraphicsContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
        }

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            g = CreateGraphics();
            isClicked = true;

            mouseDownPoint = e.Location;

            if ( ((Form1)MdiParent).selectSwitch ) // if Selected
            {
                for (int i = 1; i < listFigure.Count; i++) // if click is on top of any selected figure
                {
                    if (listFigure[i].isSelected)
                    {
                        if (listFigure[i].rectangle.IntersectsWith(new Rectangle(Point.Subtract(e.Location, (Size)AutoScrollPosition), new Size(0,0))))
                        {
                            isMoving = true;
                            break;
                        }
                    }
                }

                Console.WriteLine("is moving = " + isMoving);

                if (isMoving)
                // Cloning
                {
                    listFigureCopy.Clear();
                    foreach (var item in listFigure)
                    {
                        listFigureCopy.Add((Figure)item.Clone());
                    }

                }
                else // not moving, make new selection
                {
                    myFigure = new Rect(Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        Color.Black, 1F, ((Form1)ParentForm).solidBrushColor);

                    // look for all figures if on top of any then continue to add selection
                    for (int i = 1; i < listFigure.Count; i++)
                    {
                        if (listFigure[i].rectangle.IntersectsWith(new Rectangle(Point.Subtract(e.Location, (Size)AutoScrollPosition), new Size(0, 0))))
                        {
                            addSelection = true;
                            break;
                        }
                    }
                    if (addSelection)
                    {
                        addSelection = false;
                    }
                    else
                    {
                        for (int i = 1; i < listFigure.Count; i++) // Deselect everything
                        {
                            listFigure[i].isSelected = false;
                        }
                    }
                }
            }
            else
            {
                if (((Form1)this.MdiParent).selectedFigure == 0)
                {
                    myFigure = new Rect(Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).lineWidth, ((Form1)ParentForm).solidBrushColor);
                }
                else if (((Form1)this.MdiParent).selectedFigure == 1)
                {
                    myFigure = new _Ellipse(Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).lineWidth, ((Form1)ParentForm).solidBrushColor);
                }
                else if (((Form1)this.MdiParent).selectedFigure == 2)
                {
                    myFigure = new _Line(Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).lineWidth);
                }
                else if (((Form1)this.MdiParent).selectedFigure == 3)
                {
                    myFigure = new _Curve(Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).lineWidth);
                }
                else if (((Form1)this.MdiParent).selectedFigure == 4)
                {
                    myFigure = new _Text(Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).drawTextFont);
                }
                myFigure.isFilled = ((Form1)MdiParent).isFilled;
            }
        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if (((Form1)MdiParent).selectSwitch)
            {
                if (isClicked)
                {
                    if (isMoving)
                    {
                        bufferedGraphics.Render();

                        for (int i = 1; i < listFigureCopy.Count; i++)
                        {
                            if (listFigureCopy[i].isSelected)
                            {
                                listFigureCopy[i].MouseMove(Point.Subtract(e.Location, (Size)mouseDownPoint));
                                listFigureCopy[i].DrawDash(g, (Size)AutoScrollPosition);
                            }
                        }

                        mouseDownPoint = e.Location; // moving by one pixel
                    }
                    else
                    {
                        bufferedGraphics.Render();
                        myFigure.setPoint2(Point.Subtract(e.Location, (Size)AutoScrollPosition));
                        myFigure.DrawDash(g, (Size)AutoScrollPosition);
                    }
                }
            }
            else
            {
                if (isClicked)
                {
                    bufferedGraphics.Render();
                    myFigure.setPoint2(Point.Subtract(e.Location, (Size)AutoScrollPosition));
                    myFigure.DrawDash(g, (Size)AutoScrollPosition);
                }
            }
            ((Form1)this.MdiParent).sb_coordinates.Text = e.Location.ToString();
        }

        private void Form2_MouseUp(object sender, MouseEventArgs e)
        {
            if (((Form1)MdiParent).selectSwitch)
            {
                if (isMoving)
                {
                    bool isMoved = true;
                    foreach (var item in listFigureCopy)
                    {
                        // check each item if it is inside Drawing Area
                        if (!isInsideDrawingArea(item))
                        {
                            isMoved = false;
                            break;
                        }
                    }
                    Console.WriteLine("Is moved = {0}", isMoved);
                    if (isMoved)
                    {
                        listFigure.Clear();
                        foreach (var item in listFigureCopy)
                        {
                            listFigure.Add((Figure)item.Clone());
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < listFigure.Count; i++) // Tag each element that is inside a rectangle as Selected
                    {
                        if (listFigure[i].rectangle.IntersectsWith(myFigure.rectangle))
                        {
                            listFigure[i].isSelected = true;
                        }
                        Console.WriteLine(i + " is Selected = " + listFigure[i].isSelected);
                    }
                }

            }
            else
            {
                if (isInsideDrawingArea(myFigure))
                {
                    if ((((Form1)MdiParent).selectedFigure == 4)) // Text
                    {
                        TextInputForm1 textInputForm1 = new TextInputForm1();
                        if (textInputForm1.ShowDialog() == DialogResult.OK)
                        {
                            ((_Text)myFigure).text = textInputForm1.textBox1.Text;
                            listFigure.Add(myFigure);
                            isModified = true;
                        }
                    }
                    else
                    {
                        listFigure.Add(myFigure);
                        isModified = true;
                    }
                }
            }

            isClicked = false;
            isMoving = false;  // set Moving back
            g.Dispose();
            Invalidate();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            if (isModified)
            {
                label1.Text = "Modified";
            }
            else
            {
                label1.Text = "Not modified";
            }
            if (isSaved)
            {
                label2.Text = "Is saved";
            }
            else
            {
                label2.Text = "Is not saved";
            }
            foreach (var item in listFigure)
            {
                if (item.isFilled)
                {
                    item.DrawSolid(bufferedGraphics.Graphics, (Size)AutoScrollPosition);
                }
                if (item.isSelected)
                {
                    item.DrawDash(bufferedGraphics.Graphics, (Size)AutoScrollPosition);
                }
                else
                {
                    item.Draw(bufferedGraphics.Graphics, (Size)AutoScrollPosition);
                }
                
            }
            bufferedGraphics.Render(e.Graphics);
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isModified)
            {
                DialogResult dr = MessageBox.Show("Save modified document?", "Document has been modified", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    if (!((Form1)this.ParentForm).saveToFile(this))
                    {
                        e.Cancel = true;
                    }
                }
                else if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            bufferedGraphics.Dispose();
        }

        private void Form2_Scroll(object sender, ScrollEventArgs e)
        {
            
        }

        bool isInsideDrawingArea(Figure figure)
        {
            if (figure.rectangle.Right > pictureSize.Width | figure.rectangle.Bottom > pictureSize.Height)
            {
                return false;
            }
            return true;
        }

        private void Form2_Activated(object sender, EventArgs e)
        {
            ((Form1)this.MdiParent).sb_pictureSize.Text = pictureSize.ToString();
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ((Form1)MdiParent).toolStripButtonDelete_Click(this, new EventArgs());
            }
        }
    }
}
