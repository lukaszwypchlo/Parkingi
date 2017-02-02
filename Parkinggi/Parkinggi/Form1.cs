using System;
using System.Windows.Forms;
using Emgu.CV;
using System.Drawing;
using System.Collections.Generic;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.IO;

namespace Parkinggi
{
    public partial class Form1 : Form
    {
        Mat img;
        Image<Bgr, Byte> imgImg;
        List<ParkingSpace> spaces;
        int xClick, yClick;
        int zdj;

        public Form1()
        {
            InitializeComponent();
            img = new Mat();
            spaces = new List<ParkingSpace>();
            zdj = 1;
            label1.Text = "";
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
            LoadImage();
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
            spaces[spaces.Count - 1].isFree = checkBox1.Checked;
            spaces[spaces.Count - 1].heigh = yClick - spaces[spaces.Count - 1].startY;
            spaces[spaces.Count - 1].number = spaces.Count;
            spaces[spaces.Count - 1].mat = new Mat(img, new Rectangle(spaces[spaces.Count - 1].startX, spaces[spaces.Count - 1].startY, spaces[spaces.Count - 1].width, spaces[spaces.Count - 1].heigh));
            spaces[spaces.Count - 1].mat1 = new Mat(img, new Rectangle(spaces[spaces.Count - 1].startX, spaces[spaces.Count - 1].startY, spaces[spaces.Count - 1].width, spaces[spaces.Count - 1].heigh));
            imgImg.Draw(new Rectangle(spaces[spaces.Count - 1].startX, spaces[spaces.Count - 1].startY, spaces[spaces.Count - 1].width, spaces[spaces.Count - 1].heigh), new Bgr(Color.Green), 2);
            CvInvoke.PutText(imgImg, spaces[spaces.Count - 1].number.ToString(), new System.Drawing.Point(spaces[spaces.Count - 1].startX + spaces[spaces.Count - 1].width / 2, spaces[spaces.Count - 1].startY + spaces[spaces.Count - 1].heigh / 2), FontFace.HersheySimplex, 1.5, new Bgr(Color.Green).MCvScalar, 3);
            ibOriginal.Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            asdf();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            spaces.Add(new ParkingSpace());
        }

        private double PerChange(Matrix<Byte> matrix)
        {
            double same, change;
            same = change = 0.0;

            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    if (matrix[i, j] > 30)
                    {
                        change++;
                    }
                    else
                    {
                        same++;
                    }
                }
            }

            return (change / (change + same)) * 100;
        }

        private void LoadImage()
        {
            img = CvInvoke.Imread($"E:/Zdj3/{zdj.ToString()}.JPG", Emgu.CV.CvEnum.LoadImageType.Grayscale);
            imgImg = new Image<Bgr, Byte>($"E:/Zdj3/{zdj.ToString()}.JPG");
            ibOriginal.Image = imgImg;
            zdj++;
        }

        private void asdf()
        {
            //while(zdj<206)
            //{
            LoadImage();
            int free, taken;
            free = taken = 0;
            foreach (var item in spaces)
            {
                item.mat = item.mat1;
                item.mat1 = new Mat(img, new Rectangle(item.startX, item.startY, item.width, item.heigh));
                var tempMat = new Mat();
                CvInvoke.AbsDiff(item.mat, item.mat1, tempMat);
                var tempMatrix = new Matrix<Byte>(tempMat.Rows, tempMat.Cols, tempMat.NumberOfChannels);
                tempMat.CopyTo(tempMatrix);
                item.perChange = PerChange(tempMatrix);
                imgImg.Draw(new Rectangle(item.startX, item.startY, item.width, item.heigh), new Bgr(Color.Green), 2);
                CvInvoke.PutText(imgImg, item.number.ToString(), new System.Drawing.Point(item.startX + item.width / 2, item.startY + item.heigh / 2), FontFace.HersheySimplex, 1.5, new Bgr(Color.Green).MCvScalar, 3);
                if(item.perChange > 30.0)
                {
                    item.isFree = !item.isFree;
                }

                if(item.isFree)
                {
                    free++;
                }
                else
                {
                    taken++;
                }
            }
            ibOriginal.Refresh();
            label1.Text = $"Ilość wolnych miejsc: {free}; Ilość zajętych miejsc: {taken};";
            dataGridView1.DataSource = spaces.Select(o=> new { o.number, o.perChange, o.isFree }).ToList();

            var webAddr = "http://tempuri.org/WebService.asmx/callJson";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Headers.Add("SOAPAction", "http://tempuri.org/WebService.asmx/callJson");
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = $"{{\"free\":\"{free}\",\"taken\":\"{taken}\"}}";

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
            //}            
        }
    }
}
