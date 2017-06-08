using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace imageeditor
{
    public partial class Form1 : Form
    {
        public float lineWidth;
        public Color penColor;
        public Color solidBrushColor;
        public Font drawTextFont;
        Size pictureSize;

        BinaryFormatter formatter;
        Stream stream;
        SaveFileDialog saveFileDialog;
        OpenFileDialog openFileDialog;

        public int selectedFigure;
        public bool isFilled;

        public bool selectSwitch;

        public Form1()
        {
            InitializeComponent();
            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;

            saveFileDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();
            saveFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            saveFileDialog.Filter = "bin files|*.bin|all files|*.*";
            openFileDialog.Filter = "bin files|*.bin|all files|*.*";
            formatter = new BinaryFormatter();

            //pictureSize = new Size(800, 600);
            //solidBrushColor = Color.White;
            //penColor = Color.Black;
            //lineWidth = 1F;
            //isFilled = false;
            //selectedFigure = 0; // rectangle

            pictureSize = new Size(400, 300);
            solidBrushColor = Color.LightGreen;
            penColor = Color.DarkGreen;
            lineWidth = 3F;
            isFilled = true;
            selectedFigure = 1;

            drawTextFont = new Font(FontFamily.GenericSansSerif, 8F);

            selectSwitch = false;
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(pictureSize);
            f.MdiParent = this;
            f.Text = "Picture " + this.MdiChildren.Length.ToString();
            f.Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 f;
                if (MdiChildren.Length > 0)
                {
                    f = (Form2)this.ActiveMdiChild;
                    if (f.isModified)
                    {
                        if (MessageBox.Show("do you want to save changes?", "document has been modified", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            saveToFile(f);
                        }
                    }
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        f.saveFileName = openFileDialog.FileName;
                        openFromStream(f);
                        f.pictureSize = f.listFigure[0].rectangle.Size;
                        f.Size = f.pictureSize;
                        f.AutoScrollMinSize = f.pictureSize;
                        f.Text = Path.GetFileName(f.saveFileName);
                        f.isSaved = true;

                        f.bufferedGraphics.Dispose();
                        f.bufferedGraphics = f.bufferedGraphicsContext.Allocate(f.CreateGraphics(), f.DisplayRectangle);
                    }
                }
                else
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        f = new Form2(pictureSize);
                        f.MdiParent = this;
                        f.saveFileName = openFileDialog.FileName;
                        openFromStream(f);

                        f.pictureSize = f.listFigure[0].rectangle.Size;
                        f.Size = f.pictureSize;
                        f.AutoScrollMinSize = f.pictureSize;
                        f.Text = Path.GetFileName(f.saveFileName);
                        f.isSaved = true;
                        f.Show();

                        f.bufferedGraphics.Dispose();
                        f.bufferedGraphics = f.bufferedGraphicsContext.Allocate(f.CreateGraphics(), f.DisplayRectangle);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: {0}", ex.Message);
            }
        }

        public void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 f = (Form2)ActiveMdiChild;
                saveToFile(f);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: {0}", ex.Message);
            }


        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 f = (Form2)this.ActiveMdiChild;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    f.saveFileName = saveFileDialog.FileName;
                    saveToStream(f);
                    f.Text = Path.GetFileName(f.saveFileName);
                    f.isSaved = true;
                    f.isModified = false;
                    f.Invalidate();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: {0}", ex.Message);
            }


        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length > 0)
            {
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
            }
            else
            {
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
            }
        }

        private void lineWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lineWidthDialog d = new lineWidthDialog();
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                lineWidth = d.lineWidth;
            }
            sb_penSize.Text = lineWidth.ToString();
        }

        private void lineColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog d = new ColorDialog();
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                penColor = d.Color;
            }
            statusBar1.Invalidate();
        }

        private void fillColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog d = new ColorDialog();
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                solidBrushColor = d.Color;
            }
            statusBar1.Invalidate();
        }

        private void pictureSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureSizeDialog pictureSizeDialog = new PictureSizeDialog();
            if (pictureSizeDialog.ShowDialog() == DialogResult.OK)
            {
                pictureSize = pictureSizeDialog.pictureSize;
            }
            // sb_pictureSize.Text = pictureSize.ToString();
        }

        void saveToStream(Form2 f)
        {
            stream = new FileStream(f.saveFileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, f.listFigure);
            stream.Close();
        }

        void openFromStream(Form2 f)
        {
            stream = new FileStream(f.saveFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            f.listFigure = (List<Figure>)formatter.Deserialize(stream);
            stream.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public bool saveToFile(Form2 f)
        {
            if (f.isSaved)
            {
                saveToStream(f);
                f.isModified = false;
                f.Invalidate();
                return true;
            }
            else
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    f.saveFileName = saveFileDialog.FileName;
                    saveToStream(f);
                    f.Text = Path.GetFileName(f.saveFileName);
                    f.isSaved = true;
                    f.isModified = false;
                    f.Invalidate();
                    return true;
                }
            }
            return false;
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 0; // rectangle
            rectangleToolStripMenuItem.Checked = true;
            ellipseToolStripMenuItem.Checked = false;
            lineToolStripMenuItem.Checked = false;
            curveToolStripMenuItem.Checked = false;

            toolStripButton8.Checked = true;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;

            text_tool_strip_button.Checked = false;
            textToolStripMenuItem.Checked = false;
        }

        private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 1; // ellipse
            rectangleToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = true;
            lineToolStripMenuItem.Checked = false;
            curveToolStripMenuItem.Checked = false;

            toolStripButton8.Checked = false;
            toolStripButton9.Checked = true;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;

            text_tool_strip_button.Checked = false;
            textToolStripMenuItem.Checked = false;
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 2; // line
            rectangleToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = false;
            lineToolStripMenuItem.Checked = true;
            curveToolStripMenuItem.Checked = false;

            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = true;
            toolStripButton11.Checked = false;

            text_tool_strip_button.Checked = false;
            textToolStripMenuItem.Checked = false;
        }

        private void curveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 3; // curve
            rectangleToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = false;
            lineToolStripMenuItem.Checked = false;
            curveToolStripMenuItem.Checked = true;

            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = true;

            text_tool_strip_button.Checked = false;
            textToolStripMenuItem.Checked = false;
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 4; // Text
            rectangleToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = false;
            lineToolStripMenuItem.Checked = false;
            curveToolStripMenuItem.Checked = false;
            textToolStripMenuItem.Checked = true;

            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;
            text_tool_strip_button.Checked = true;
        }

        private void filledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isFilled)
            {
                toolStripButton12.Checked = false;
                filledToolStripMenuItem.Checked = false;
                isFilled = false;
            }
            else
            {
                toolStripButton12.Checked = true;
                filledToolStripMenuItem.Checked = true;
                isFilled = true;
            }
        }

        private void statusBar1_DrawItem(object sender, StatusBarDrawItemEventArgs sbdevent)
        {

            sb_penSize.Text = lineWidth.ToString();

            if (sbdevent.Panel == sb_penColor)
            {
                sbdevent.Graphics.FillRectangle(new SolidBrush(penColor), sbdevent.Bounds);
            }
            else if (sbdevent.Panel == sb_brushColor)
            {
                sbdevent.Graphics.FillRectangle(new SolidBrush(solidBrushColor), sbdevent.Bounds);
            }
        }

        private void text_tool_strip_button_Click(object sender, EventArgs e)
        {

        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                drawTextFont = fontDialog.Font;
                Console.WriteLine(drawTextFont);

                sb_font.Text = drawTextFont.Name + " " + drawTextFont.Size;
            }

        }

        private void font_dialog_tool_strip_button_Click(object sender, EventArgs e)
        {

        }

        private void selectToolStripButton_Click(object sender, EventArgs e)
        {
            if (selectSwitch)
            {
                selectSwitch = false;
                selectToolStripButton.Checked = false;
            }
            else
            {
                selectSwitch = true;
                selectToolStripButton.Checked = true;
            }
            Console.WriteLine(selectSwitch);
        }

        public void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            List<Figure> toRemoveList = new List<Figure>();
            toRemoveList.Clear();
            for (int i = 1; i < ((Form2)ActiveMdiChild).listFigure.Count; i++)
            {
                if (((Form2)ActiveMdiChild).listFigure[i].isSelected)
                {
                    toRemoveList.Add(((Form2)ActiveMdiChild).listFigure[i]);
                    Console.WriteLine("item {0} is added to remove", i);
                }
            }
            foreach (var item in toRemoveList)
            {
                ((Form2)ActiveMdiChild).listFigure.Remove(item);
            }
            ((Form2)ActiveMdiChild).Invalidate();
        }
        // Copy
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string str1 = "asdf";
            //Clipboard.SetDataObject(str1, true);
            Form2 activeForm = (Form2)ActiveMdiChild;
            List<Figure> listSelObj = new List<Figure>();


            for (int i = 1; i < activeForm.listFigure.Count; i++)
            {
                if (activeForm.listFigure[i].isSelected)
                {
                    listSelObj.Add(activeForm.listFigure[i]);
                }
            }

            
            
            // IDataObject dataObj1 = null;
            DataObject dataObj1 = new DataObject();

            dataObj1.SetData("my_application_format1", listSelObj);
            dataObj1.SetData("my_application_format2", listSelObj);

            Clipboard.SetDataObject(dataObj1, true);
        }

        // Paste
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject obj1 = Clipboard.GetDataObject();
            // Console.WriteLine(obj1.GetDataPresent(typeof(string)));
            //string str1 = (string)obj1.GetData(typeof(string));
            //Console.WriteLine(str1);


        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            copyToolStripMenuItem.Enabled = true;
            cutToolStripMenuItem.Enabled = true;
            pasteToolStripMenuItem.Enabled = true;
            selectAllToolStripMenuItem.Enabled = true;

            if (MdiChildren.Length == 0)
            {
                copyToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
                selectAllToolStripMenuItem.Enabled = false;
            }
            else
            {
                Form2 activeForm = (Form2)ActiveMdiChild;
                bool anySelected = false;
                for (int i = 1; i < activeForm.listFigure.Count; i++)
                {
                    if (activeForm.listFigure[i].isSelected)
                    {
                        anySelected = true;
                        break;
                    }
                }

                if (!anySelected)
                {
                    copyToolStripMenuItem.Enabled = false;
                    cutToolStripMenuItem.Enabled = false;
                    pasteToolStripMenuItem.Enabled = false;
                }
            }
        }


    }
}
