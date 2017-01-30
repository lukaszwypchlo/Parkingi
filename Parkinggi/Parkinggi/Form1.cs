using System;
using System.Windows.Forms;
using Emgu.CV;
using System.Drawing;
using System.Collections.Generic;
using Emgu.CV.UI;
using Emgu.CV.Structure;

namespace Parkinggi
{
    public partial class Form1 : Form
    {
        Mat img, img1;
        Image<Bgr, Byte> imgImg;
        List<ParkingSpace> spaces;
        int xClick, yClick;

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var a = (ComboBox)sender;
            var tempImg = new Mat(img, new Rectangle(spaces[a.SelectedIndex].startX, spaces[a.SelectedIndex].startY, spaces[a.SelectedIndex].width, spaces[a.SelectedIndex].heigh));
            var tempImg1 = new Mat(img1, new Rectangle(spaces[a.SelectedIndex].startX, spaces[a.SelectedIndex].startY, spaces[a.SelectedIndex].width, spaces[a.SelectedIndex].heigh));
            ibProcessed.Image = tempImg;
            ibProcessedTaken.Image = tempImg1;
            imgImg.Draw(new Rectangle(spaces[a.SelectedIndex].startX, spaces[a.SelectedIndex].startY, spaces[a.SelectedIndex].width, spaces[a.SelectedIndex].heigh), new Bgr(Color.Green));
            label1.Text = $"Procentowa zmiana: {String.Format("{0:0.00}", spaces[a.SelectedIndex].perChange)}%";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var a = (ComboBox)sender;
            if(a.SelectedItem.Equals(1))
            {
                ibOriginal.Image = imgImg;
            }
            else
            {
                ibOriginal.Image = img1;
            }
        }

        public Form1()
        {
            InitializeComponent();
            img = new Mat();
            img1 = new Mat();
            spaces = new List<ParkingSpace>();
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
            textBox1.Text = Convert.ToString(xClick = offsetX + horizontalScrollBarValue) + "." + Convert.ToString(yClick = offsetY + verticalScrollBarValue);
        }

        private void Form1_FormClodes(object sender, FormClosedEventArgs e)
        {
        }

        private void btnPouseOrResume_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Add(1);
            comboBox1.Items.Add(2);
            
            img = CvInvoke.Imread("E:/G0010599.JPG", Emgu.CV.CvEnum.LoadImageType.Grayscale);
            imgImg = new Image<Bgr, Byte>("E:/G0010599.JPG");
            ibOriginal.Image = imgImg;
            img1 = CvInvoke.Imread("E:/G0010600.JPG", Emgu.CV.CvEnum.LoadImageType.Grayscale);

            //spacesPattern.Add(new Mat(img, new Rectangle(1530, 1350, 100, 50)));
            //spacesActual.Add(new Mat(img1, new Rectangle(1530, 1350, 100, 50)));
            //spacesPattern.Add(new Mat(img, new Rectangle(1530, 1450, 100, 50)));
            //spacesActual.Add(new Mat(img1, new Rectangle(1530, 1450, 100, 50)));

            label1.Text = "";

            var freeSpaces = 0;
            var takenSpaces = 0;

            //for(int i=0; i<spacesActual.Count; i++)
            //{
            //    var tempMat = new Mat();
            //    CvInvoke.AbsDiff(spacesPattern[i], spacesActual[i], tempMat);
            //    spacesDiff.Add(tempMat);
            //}

            //Matrix<Byte> matrix = new Matrix<Byte>(spacesDiff[0].Rows, spacesDiff[0].Cols, spacesDiff[0].NumberOfChannels);
            //spacesDiff[0].CopyTo(matrix);

            //Matrix<Byte> matrix1 = new Matrix<Byte>(spacesDiff[1].Rows, spacesDiff[1].Cols, spacesDiff[1].NumberOfChannels);
            //spacesDiff[1].CopyTo(matrix1);

            //label1.Text = $"Pierwszy: {perChange(matrix)}; Drugi: {perChange(matrix1)};";

            //ibProcessed.Image = spacesPattern[0];
            //ibProcessedTaken.Image = spacesActual[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            spaces[spaces.Count - 1].startX = xClick;
            spaces[spaces.Count - 1].startY = yClick;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            spaces[spaces.Count - 1].width = xClick - spaces[spaces.Count - 1].startX;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var tempMat = new Mat();
            spaces[spaces.Count - 1].heigh = yClick - spaces[spaces.Count - 1].startY;
            comboBox2.Items.Add(spaces.Count);
            CvInvoke.AbsDiff(new Mat(img, new Rectangle(spaces[spaces.Count - 1].startX, spaces[spaces.Count - 1].startY, spaces[spaces.Count - 1].width, spaces[spaces.Count - 1].heigh)),
                new Mat(img1, new Rectangle(spaces[spaces.Count - 1].startX, spaces[spaces.Count - 1].startY, spaces[spaces.Count - 1].width, spaces[spaces.Count - 1].heigh)), tempMat);
            Matrix<Byte> matrix = new Matrix<Byte>(tempMat.Rows, tempMat.Cols, tempMat.NumberOfChannels);
            tempMat.CopyTo(matrix);
            spaces[spaces.Count - 1].perChange = PerChange(matrix);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            spaces.Add(new ParkingSpace());
        }

        private double PerChange(Matrix<Byte> matrix)
        {
            double same, change;
            same = change = 0.0;

            for(int i=0; i<matrix.Rows; i++)
            {
                for(int j=0; j<matrix.Cols; j++)
                {
                    if(matrix[i,j]>30)
                    {
                        change++;
                    }
                    else
                    {
                        same++;
                    }
                }
            }

            return (change/(change+same))*100;
        }
    }
}
