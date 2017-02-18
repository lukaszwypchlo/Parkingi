using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Parkinggi
{
    class Parking
    {
        List<ParkingSpace> spaces;
        Image<Bgr, Byte> image;
        int imgCounter;
        int free, taken;
        string connstring;
        int spaceCounter;
        string folderPath;

        public Parking()
        {
            spaces = new List<ParkingSpace>();
            imgCounter = 0;
            spaceCounter = 1;
            connstring = "Server=sql11.freemysqlhosting.net;Port=3306;Database=sql11156927;Uid=sql11156927;Pwd=ytlTnSgCBV;";
            folderPath = "";
        }

        public void AddSpace(ParkingSpace space)
        {
            spaces.Add(space);
        }

        public void SetImage(Image<Bgr, Byte> image)
        {
            this.image = image;
        }

        public Image<Bgr, Byte> GetImage()
        {
            return image;
        }

        private double PerChange(Matrix<Byte> matrix)
        {
            double same, change;
            same = change = 0.0;

            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    if (matrix.Data[i,j] > 30)
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

        public void CheckPlases()
        {
            free = taken = 0;
            foreach (var item in spaces)
            {
                item.mat = item.mat1;
                var mat = image.Mat;
                item.mat1 = new Mat(mat, new Rectangle(item.startX, item.startY, item.width, item.heigh));
                var tempMat = new Mat(item.mat.Rows, item.mat.Cols, item.mat.Depth, item.mat.NumberOfChannels);
                CvInvoke.AbsDiff(item.mat, item.mat1, tempMat);
                var tempMatrix = new Matrix<Byte>(tempMat.Rows, tempMat.Cols, tempMat.NumberOfChannels);
                tempMat.CopyTo(tempMatrix);
                item.perChange = PerChange(tempMatrix);

                CvInvoke.PutText(image, item.number.ToString(), new System.Drawing.Point(item.startX + item.width / 2, item.startY + item.heigh / 2), FontFace.HersheySimplex, 1.5, new Bgr(Color.Green).MCvScalar, 3);
                if (item.perChange > 30.0)
                {
                    item.isFree = !item.isFree;
                }

                if (item.isFree)
                {
                    free++;
                }
                else
                {
                    taken++;
                }
                image.Draw(new Rectangle(item.startX, item.startY, item.width, item.heigh), item.isFree ? new Bgr(Color.Green) : new Bgr(Color.Red), 4);
            }
        }
        public void SendToDB(int rowId)
        {            
            //var command = $"UPDATE parking SET free_spaces = {free}, taken_spaces = {taken} WHERE row_id = {rowId}; ";
            //using (var conn = new MySqlConnection(connstring))
            //{
            //    conn.Open();
            //    var comm = new MySqlCommand(command, conn);
            //    comm.ExecuteNonQuery();
            //    conn.Close();
            //}
        }

        public int GetFreeCount()
        {
            return free;
        }

        public int GetTakenCount()
        {
            return taken;
        }

        public List<ParkingSpace> GetSpacesList()
        {
            return spaces;
        }

        public int NextSpaceNumber()
        {
            return spaceCounter;
        }

        public void IncSpaceNumber()
        {
            spaceCounter++;
        }

        public void ChangeSpaceStatus(int spaceNum)
        {
            spaces[spaceNum].isFree = !spaces[spaceNum].isFree;
        }

        public void SetFolderPath(string path)
        {
            folderPath = path;
        }

        public string GetFolderPath()
        {
            return folderPath + "\\";
        }
    }
}
