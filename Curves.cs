using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace Utilities
{
    public struct LoadStrings
    {
        /// <summary>
        /// Load all files with extension in folder and return it in lists
        /// </summary>
        /// <param name="dataFolder"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static List<List<string>> FromFolder(string dataFolder, string extension)
        {
            List<List<string>> returnList = new List<List<string>>();

            //Open folder and iterate all txt files
            foreach (string fileName in Directory.GetFiles(dataFolder))
            {
                FileInfo f = new FileInfo(fileName);
                if (f.Extension == ("." + extension))
                {
                    List<string> filedata = new List<string>();
                    string[] fileLines = File.ReadAllLines(fileName);
                    filedata = fileLines.ToList<string>();
                    returnList.Add(filedata);
                }
            }
            return returnList;
        }
    }

    public struct GetFileNames
    {
        /// <summary>
        /// Get all filenames with extension in folder and return it in lists
        /// </summary>
        /// <param name="dataFolder"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static List<string> FromFolder(string dataFolder, string extension)
        {
            List<string> returnList = new List<string>();

            //Open folder and iterate all txt files
            foreach (string fileName in Directory.GetFiles(dataFolder))
            {
                FileInfo f = new FileInfo(fileName);
                if (f.Extension == ("." + extension))
                    returnList.Add(f.Name);
            }
            return returnList;
        }
    }

    public struct Vector2
    {
        public float x, y;

        public Vector2(float ix, float iy)
        {
            x = ix;
            y = iy;
        }
    }

    public interface Interpolator
    {
        void Interpolate(List<Vector2> controlPoints, float interval);
        float GetY(float x);
    }

    public struct Lagrange : Interpolator
    {
        public float _interval, _min, _max;
        public List<float> Y;

        public void Interpolate(List<Vector2> controlPoints, float interval)
        {
            _min = controlPoints.FirstOrDefault().x;
            _max = controlPoints.LastOrDefault().x;
            _interval = interval;
            Y = new List<float>();
            List<List<float>> Operators = new List<List<float>>();
            List<Vector2> result = new List<Vector2>();

            try
            {
                if (controlPoints.Count > 0)
                {
                    //Compute lagrange operator for each x
                    for (float x = _min; x < _max; x = x + interval)
                    {
                        //list of float to hold the Lagrange operators
                        List<float> L = new List<float>();

                        //Init the list with 1's
                        for (int i = 0; i < controlPoints.Count; i++)
                            L.Add(1);

                        for (int i = 0; i < L.Count; i++)
                            for (int k = 0; k < controlPoints.Count; k++)
                                if (i != k)
                                    L[i] *= (float)(x - controlPoints[k].x) / (controlPoints[i].x - controlPoints[k].x);

                        Operators.Add(L);
                    }

                    //Computing the Polynomial P(x) which is y in our curve
                    foreach (List<float> O in Operators)
                    {
                        float y = 0;
                        for (int i = 0; i < controlPoints.Count; i++)
                        {
                            y += O[i] * controlPoints[i].y;
                        }

                        Y.Add(y);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Raise exception
            }
        }

        /// <summary>
        /// Returns either exact Y on the curve or a linear interpolation inbetween points. Min and Max Y values are clamped for X outside the range
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float GetY(float x)
        {
            int minIndex, maxIndex;

            if (x >= _max)
                return Y.LastOrDefault();

            if (x <= _min)
                return Y.FirstOrDefault();

            minIndex = (int)System.Math.Floor((x - _min) / _interval);

            if (minIndex == 0)
                return Y[minIndex];

            //If Value is not exact, perform linear interpolation
            float inBetween = ((x - _min) % _interval) / _interval;
            maxIndex = minIndex + 1;

            return (Y[minIndex] + Y[maxIndex] * inBetween);
        }
    }

    public struct Curve
    {
        //ID of curve
        public int ID;

        //Control points to interpolate
        List<Vector2> _controlPoints;
        Interpolator _algorithm;
        bool _initialised;

        public List<Vector2> controlPoints
        {
            get
            {
                return _controlPoints;
            }

            set
            {
                _controlPoints = value;
            }
        }

        public void Calculate(List<Vector2> icontrolPoints, float interval, string algorithm)
        {
            switch (algorithm)
            {
                case "Lagrange": _algorithm = new Lagrange(); break;
                default: _algorithm = new Lagrange(); break;
            }

            _algorithm.Interpolate(icontrolPoints, interval);
            _initialised = false;
        }

        /// <summary>
        /// Calculate curve value at point according to chosen algorithm
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float GetY(float x)
        {
            if (_initialised)
                return _algorithm.GetY(x);
            else
                return 0f;
        }
    }

    public struct CurveLibrary
    {
        public List<Curve> _curves;

        public void LoadCurves(string dataFolder, string algorithm)
        {
            _curves = new List<Curve>();

            //Iterate each file and make array of Vector2
            List<List<string>> data = new List<List<string>>();
            data = Utilities.LoadStrings.FromFolder(dataFolder, "txt");
            List<string> names = new List<string>();
            names = Utilities.GetFileNames.FromFolder(dataFolder, "txt");

            //Iterate through each curve file
            for (int i=0; i< data.Count; i++)
            {
                List<string> curvestring = new List<string>();
                curvestring = data[i];

                //Empty curves are dumped
                if (curvestring.Count < 1)
                {

                    int name = int.Parse(names[i], CultureInfo.InvariantCulture.NumberFormat);
                    List<Vector2> rawcurve = new List<Vector2>();

                    try
                    {
                        //Get control points
                        foreach (string pointstring in curvestring)
                        {
                            string[] xy = pointstring.Split(' ');
                            float x = 0f;
                            float y = 0f;

                                x = float.Parse(xy[0], CultureInfo.InvariantCulture.NumberFormat);
                                y = float.Parse(xy[1], CultureInfo.InvariantCulture.NumberFormat);
                            rawcurve.Add(new Vector2(x, y));
                        }
                        //Calculate smallest difference for the X values from each curve. Divide by 4 to get the interval
                        float interval = 0f;

                        List<float> differences = new List<float>();
                        for (int j = 0; j < rawcurve.Count - 1; j++)
                            differences.Add(rawcurve[j + 1].x - rawcurve[j].x);

                        interval = differences[0];
                        for (int j = 1; j < differences.Count; j++)
                            if (interval > differences[j])
                                interval = differences[j];

                        interval = interval / 4f;

                        //Only curves where the points are not repeated will be included
                        if (interval > 0f)
                        {
                            Curve tcurve = new Curve();
                            tcurve.ID = name;
                            tcurve.Calculate(rawcurve, interval, algorithm);
                            _curves.Add(tcurve);
                        }
                    }
                    catch
                    {
                        //If data is bad, skip this curve
                    }
                }
            }
        }
    }
}