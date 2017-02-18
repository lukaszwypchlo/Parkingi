using System;
using System.Windows.Forms;
using Emgu.CV;
using System.Drawing;
using System.Collections.Generic;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Linq;

namespace Parkinggi
{
    public partial class Form1 : Form
    {
        Mat img;
        Image<Bgr, Byte> imgImg;
        List<Parking> parkings;
        ParkingSpace addingSpace;
        int actualParking;
        int xClick, yClick;
        int startX, startY, width, heigh;
        int zdj;
        int clickCount;
        bool clickFlag;

        public Form1()
        {
            InitializeComponent();
            img = new Mat();
            zdj = 1;
            label1.Text = "";
            clickCount = 0;
            clickFlag = false;
            parkings = new List<Parking>();
            actualParking = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void ibOriginal_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs)e;
            var ib = (ImageBox)sender;
            Point coordinates = me.Location;

            int offsetX = (int)(me.Location.X / ib.ZoomScale);
            int offsetY = (int)(me.Location.Y / ib.ZoomScale);
            int horizontalScrollBarValue = ib.HorizontalScrollBar.Visible ? (int)ib.HorizontalScrollBar.Value : 0;
            int verticalScrollBarValue = ib.VerticalScrollBar.Visible ? (int)ib.VerticalScrollBar.Value : 0;
            xClick = offsetX + horizontalScrollBarValue;
            yClick = offsetY + verticalScrollBarValue;

            if (clickFlag)
            {
                switch (clickCount)
                {
                    case 0:
                        addingSpace = new ParkingSpace();
                        addingSpace.startX = xClick;
                        addingSpace.startY = yClick;
                        clickCount++;
                        break;
                    case 1:
                        var image = parkings[actualParking].GetImage();
                        var mat = image.Mat;
                        addingSpace.width = xClick - addingSpace.startX;
                        addingSpace.isFree = checkBox1.Checked;
                        addingSpace.heigh = yClick - addingSpace.startY;
                        addingSpace.number = parkings[actualParking].NextSpaceNumber();
                        parkings[actualParking].IncSpaceNumber();
                        addingSpace.mat = new Mat(mat, new Rectangle(addingSpace.startX, addingSpace.startY, addingSpace.width, addingSpace.heigh));
                        addingSpace.mat1 = new Mat(mat, new Rectangle(addingSpace.startX, addingSpace.startY, addingSpace.width, addingSpace.heigh));
                        image.Draw(new Rectangle(addingSpace.startX, addingSpace.startY, addingSpace.width, addingSpace.heigh), addingSpace.isFree ? new Bgr(Color.Green) : new Bgr(Color.Red), 4);                        
                        CvInvoke.PutText(image, addingSpace.number.ToString(), new System.Drawing.Point(addingSpace.startX + addingSpace.width / 2, addingSpace.startY + addingSpace.heigh / 2), FontFace.HersheySimplex, 1.5, new Bgr(Color.Green).MCvScalar, 3);
                        ibOriginal.Image = image;
                        ibOriginal.Refresh();
                        parkings[actualParking].SetImage(image);
                        parkings[actualParking].AddSpace(addingSpace);
                        clickCount++;
                        break;
                    default:
                        clickFlag = false;
                        break;
                }
            }
        }

        private void Form1_FormClodes(object sender, FormClosedEventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadImage();
            Proceed();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            actualParking = (Int32)cb.SelectedItem - 1;
            ibOriginal.Image = parkings[actualParking].GetImage();
            ibOriginal.Refresh();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var ofd = (OpenFileDialog)sender;
            parkings[actualParking].SetFolderPath(ofd.FileName.Remove(ofd.FileName.Length - 1 - ofd.SafeFileName.Length));
            imgImg = new Image<Bgr, Byte>($"{parkings[actualParking].GetFolderPath()}{ofd.SafeFileName}");
            parkings[actualParking].SetImage(imgImg);
            Proceed();
        }

        private void dodajParkingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parking = new Parking();
            parkings.Add(parking);
            comboBox1.Items.Add(parkings.Count);
        }

        private void wybierzŚcieżkęToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            clickFlag = true;
            clickCount = 0;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 2)
            {
                parkings[actualParking].ChangeSpaceStatus(e.RowIndex);
                Proceed();
            }
        }

        private void LoadImage()
        {
            imgImg = new Image<Bgr, Byte>($"{parkings[actualParking].GetFolderPath()}{zdj.ToString()}.JPG");
            parkings[actualParking].SetImage(imgImg);
            zdj++;
        }

        private void Proceed()
        {
            parkings[actualParking].CheckPlases();
            ibOriginal.Image = parkings[actualParking].GetImage();
            ibOriginal.Refresh();
            label1.Text = $"Ilość wolnych miejsc: {parkings[actualParking].GetFreeCount()}; Ilość zajętych miejsc: {parkings[actualParking].GetTakenCount()};";
            var dataSource = parkings[actualParking].GetSpacesList();
            dataGridView1.DataSource = dataSource.Select(o=> new { o.number, o.perChange, o.isFree }).ToList();         
        }
    }
}
