using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcadProjectLayerUtils;

namespace AcadProjectExtractData
{
    /*
     *  Singleton pattern 
     */
    class ExtractData
    {
        static int MAX_POINT = 1000;
        static int MAX_SET = 100;
        static double EPSILON = 0.004;
        static int VISTED = 1;
        static int UNVISITED = 0;
        static bool CONNECTED = true;
        static bool UNCONNECTED = false;


        // input
        int numberOfPoint;
        double[] x = new double[MAX_POINT];
        double[] y = new double[MAX_POINT];
        int[] indexs = new int[MAX_POINT];
        bool[,] isLineArray = new bool[MAX_POINT, MAX_POINT];

        // temp data
        int[] visited = new int[MAX_POINT];
        int rsCurrentSetIndex;
        int rsCurrentPointIndex;
        int[] rightIndexs;
        int nLayer;

        // extracted ouput
        double[,] resultX = new double[MAX_SET, MAX_POINT];
        double[,] resultY = new double[MAX_SET, MAX_POINT];
        int[] numberPointOfSet = new int[MAX_SET];
        int numberSet;

        //List of Layer
        public List<LayerUtils> mListLayer;
        public List<PointUtils> mListPoint;
        public PointUtils zeroPoint;
        public PointUtils centerPoint;
        public double MSS;

        //Create an object of ExtractData
        private static ExtractData extractDataInstance = new ExtractData();

        //Make the constructor private so that this class cannot be instantiated
        private ExtractData()
        {
            for (int i = 0; i < MAX_POINT; i++)
            {
                for (int j = 0; j < MAX_POINT; j++)
                {
                    isLineArray[i, j] = UNCONNECTED;
                }
            }

            //Initialize
            mListLayer = new List<LayerUtils>();
            mListPoint = new List<PointUtils>();
            zeroPoint = new PointUtils();
            centerPoint = new PointUtils();
            MSS = 0.0;
        }

        //Get the only object available
        public static ExtractData getInstance()
        {
            return extractDataInstance;
        }

        //METHODs definition
        public void setNumberOfPoint(int nPoints)
        {
            numberOfPoint = nPoints;
        }

        public int[] getIndexs()
        {
            return indexs;
        }

        public List<LayerUtils> getListOfLayer()
        {
            return this.mListLayer;
        }

        public PointUtils getZeroPoint()
        {
            return this.zeroPoint;
        }

        public PointUtils getCenterPoint()
        {
            return this.centerPoint;
        }
        public void setMss(double mss)
        {
            this.MSS = mss;
        }

        public double getMss()
        {
            return this.MSS;
        }

        public double[] getArrayX()
        {
            return x;
        }

        public double[] getArrayY()
        {
            return y;
        }

        public bool[,] getIsLineMatrix()
        {
            return isLineArray;
        }

        public string getStringResult()
        {
            string str = "";

            for (int i = 0; i < numberSet; i++)
            {
                //Create new layer then add points to it
                LayerUtils addLayer = new LayerUtils();

                //Console.Write("Layer {0} : \n", i);
                str += "Layer " + i + " : \n";
                for (int j = 0; j < numberPointOfSet[i]; j++)
                {
                    //Console.Write("x = {0}\ty = {1}\n", resultX[i, j], resultY[i, j]);
                    str += "x = " + resultX[i, j] + "\ty = " + resultY[i, j] + "\n";
                    PointUtils addPoint = new PointUtils();
                    addPoint.setXY(resultX[i, j], resultY[i, j]);
                    addLayer.addPointToLayer(addPoint);
                }
                mListLayer.Add(addLayer);

                //Console.Write("\n");
                str += "\n";
            }

            calculateHighDistance(mListLayer, zeroPoint, getMss());
            for (int i = 0; i < mListLayer.Count; i++)
            {
                str += "\nDistance/High of Layer : " + i;
                str += mListLayer[i].toStringData().ToString();
            }
            return str;
        }

        public void calculateHighDistance(List<LayerUtils> listLayer, PointUtils zeroPoint, double MSS)
        {
            for (int i = 0; i < listLayer.Count; i++)
            {
                for (int j = 0; j < listLayer[i].numberOfPoint(); j++)
                {
                    int centerIndex = listLayer[i].numberOfPoint() / 2;

                    double distance = Math.Round(Math.Abs(listLayer[i].getPointAt(j).getX() - listLayer[i].getPointAt(centerIndex).getX()), 2);
                    double high = Math.Round(Math.Abs(listLayer[i].getPointAt(j).getY() - zeroPoint.getY()), 2) + MSS;

                    listLayer[i].getPointAt(j).setDistance(distance);
                    listLayer[i].getPointAt(j).setHigh(high);
                }
            }
        }

        public string getStringInput()
        {
            string str = "";

            for (int i = 0; i < numberOfPoint; i++)
            {
                str += "\nPoint " + (i + 1) + " : " + x[i] + "  " + y[i];
            }
            str += "\n\n";
            for (int i = 0; i < numberOfPoint; i++)
            {
                for (int j = 0; j < numberOfPoint; j++)
                {
                    if (isLineArray[i, j])
                    {
                        str += "true\t";
                    }
                    else
                    {
                        str += "false\t";
                    }
                }
                str += "\n\n";
            }
            return str;
        }

        public void printOutputResult()
        {
            for (int i = 0; i < numberSet; i++)
            {
                Console.Write("Layer {0} : \n", i);
                for (int j = 0; j < numberPointOfSet[i]; j++)
                {
                    Console.Write("x = {0}\ty = {1}\n", resultX[i, j], resultY[i, j]);
                }
                Console.Write("\n");
            }
        }

        public void processExtractData()
        {
            // extracted ouput
            //double[,] resultX = new double[MAX_SET, MAX_POINT];
            //double[,] resultY = new double[MAX_SET, MAX_POINT];
            //int[] numberPointOfSet = new int[MAX_SET];
            //int numberSet;

            initTempData();

            // Find right index array
            int currentIndex = numberOfPoint - 1;
            double cX = x[indexs[currentIndex]];
            double cY = y[indexs[currentIndex]];
            double nextX = x[indexs[currentIndex - 1]], nextY = y[indexs[currentIndex - 1]];
            rightIndexs[nLayer] = indexs[currentIndex];
            nLayer++;
            while (nextY <= cY)
            {
                currentIndex--;
                rightIndexs[nLayer] = indexs[currentIndex];
                nLayer++;
                cX = x[indexs[currentIndex]];
                cY = y[indexs[currentIndex]];
                nextX = x[indexs[currentIndex - 1]]; nextY = y[indexs[currentIndex - 1]];
            }
            // Next Processing here

        }

        private bool checkAllVisited()
        {
            for (int i = 0; i < numberOfPoint; i++)
            {
                if (visited[i] != VISTED)
                {
                    return false;
                }
            }
            return true;
        }

        private void initTempData()
        {
            rsCurrentSetIndex = 0;
            rsCurrentPointIndex = 0;
            numberSet = 0;
        }

    }
}
