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
        List<Mat> spacesPattern, spacesActual;

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
            
            img = new Mat();
            img1 = new Mat();
            img = CvInvoke.Imread("E:/parkFree.png", Emgu.CV.CvEnum.LoadImageType.Grayscale);
            ibOriginal.Image = img;
            img1 = CvInvoke.Imread("E:/park.png", Emgu.CV.CvEnum.LoadImageType.Grayscale);

            spacesPattern = new List<Mat>();
            spacesActual = new List<Mat>();

            for(int i=0; i<4; i++)
            {
                spacesPattern.Add(new Mat(img, new Rectangle(0 + 160 * i, 0, 160, 160)));
                spacesActual.Add(new Mat(img1, new Rectangle(0 + 160 * i, 0, 160, 160)));
                spacesPattern.Add(new Mat(img, new Rectangle(0 + 160 * i, 320, 160, 160)));
                spacesActual.Add(new Mat(img1, new Rectangle(0 + 160 * i, 320, 160, 160)));
            }

            label1.Text = "";

            var freeSpaces = spacesPattern.Count;
            var takenSpaces = 0;

            for(int i=0; i<spacesPattern.Count; i++)
            {
                comboBox2.Items.Add(i);
                comboBox3.Items.Add(i);
                if(!spacesPattern[i].Equals(spacesActual[i]))
                {
                    takenSpaces++;
                }
            }
            //var adsff = spacesPattern[3] - spacesActual[3];
            var asdf = new Mat();

            CvInvoke.AbsDiff(spacesPattern[3], spacesActual[3], asdf);

            imageBox1.Image = asdf;

            label1.Text = $"Wolne miejsca: {freeSpaces-takenSpaces}; Zajęte miejsca: {takenSpaces};";

            ibProcessed.Image = spacesPattern[0];
            ibProcessedTaken.Image = spacesActual[0];
        }
    }
}
