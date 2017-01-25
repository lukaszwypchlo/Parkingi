using System;
using System.Windows.Forms;
using Emgu.CV;
using System.Drawing;
using System.Collections.Generic;

namespace Parkinggi
{
    public partial class Form1 : Form
    {
        Mat img, img1;
        List<Mat> spacesActual, spacesPattern, spacesDiff;

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var a = (ComboBox)sender;
            ibProcessedTaken.Image = spacesActual[a.SelectedIndex];
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var a = (ComboBox)sender;
            ibProcessed.Image = spacesPattern[a.SelectedIndex];
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var a = (ComboBox)sender;
            if(a.SelectedItem.Equals(1))
            {
                ibOriginal.Image = img;
            }
            else
            {
                ibOriginal.Image = img1;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            var a = (ComboBox)sender;
            imageBox1.Image = spacesDiff[a.SelectedIndex];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_FormClodes(object sender, FormClosedEventArgs e)
        {
        }

        private void btnPouseOrResume_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Add(1);
            comboBox1.Items.Add(2);

            comboBox2.Items.Add(1);
            comboBox2.Items.Add(2);

            comboBox3.Items.Add(1);
            comboBox3.Items.Add(2);
            comboBox4.Items.Add(1);
            comboBox4.Items.Add(2);

            img = new Mat();
            img1 = new Mat();
            img = CvInvoke.Imread("E:/G0010599.JPG", Emgu.CV.CvEnum.LoadImageType.Grayscale);
            ibOriginal.Image = img;
            img1 = CvInvoke.Imread("E:/G0010600.JPG", Emgu.CV.CvEnum.LoadImageType.Grayscale);

            spacesPattern = new List<Mat>();
            spacesActual = new List<Mat>();
            spacesDiff = new List<Mat>();

            spacesPattern.Add(new Mat(img, new Rectangle(1530, 1350, 100, 50)));
            spacesActual.Add(new Mat(img1, new Rectangle(1530, 1350, 100, 50)));
            spacesPattern.Add(new Mat(img, new Rectangle(1530, 1450, 100, 50)));
            spacesActual.Add(new Mat(img1, new Rectangle(1530, 1450, 100, 50)));

            label1.Text = "";

            var freeSpaces = 0;
            var takenSpaces = 0;

            for(int i=0; i<spacesActual.Count; i++)
            {
                var tempMat = new Mat();
                CvInvoke.AbsDiff(spacesPattern[i], spacesActual[i], tempMat);
                spacesDiff.Add(tempMat);
            }

            Matrix<Byte> matrix = new Matrix<Byte>(spacesDiff[0].Rows, spacesDiff[0].Cols, spacesDiff[0].NumberOfChannels);
            spacesDiff[0].CopyTo(matrix);

            Matrix<Byte> matrix1 = new Matrix<Byte>(spacesDiff[1].Rows, spacesDiff[1].Cols, spacesDiff[1].NumberOfChannels);
            spacesDiff[1].CopyTo(matrix1);

            imageBox1.Image = spacesDiff[0];

            label1.Text = $"Pierwszy: {perChange(matrix)}; Drugi: {perChange(matrix1)};";

            ibProcessed.Image = spacesPattern[0];
            ibProcessedTaken.Image = spacesActual[0];
        }

        private double perChange(Matrix<Byte> matrix)
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
