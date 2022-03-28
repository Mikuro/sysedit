using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    public class ModelObject
    {
        [PropertyType(PropertyType.Visual)]
        public double X { get; set; }
        [PropertyType(PropertyType.Visual)]
        public double Y { get; set; }
        [PropertyType(PropertyType.Id)]
        public string Id { get; set; }

        public Func<double>[] points;
        public double[] points_value;

        public class ObjectPoints
        {
            public Collection<string> pointnames;
            public Collection<int> levels;
            public Collection<PointType> directions;
        }

        static Dictionary<string, ObjectPoints> pointsMap = new Dictionary<string, ObjectPoints>();

        static public int type2idx(PointType type)
        {
            return ((int)type) & ((int)PointType.Point7);
        }
        static public PointType type2dir(PointType type)
        {
            return (type & PointType.Destination);
        }

        public virtual void CalcIntrument(int nLevel, PointType pointType)
        {
            throw new NotImplementedException();
        }

        static public int type2level (PointType type)
        {
            return (((int)type) & ((int)PointType.Level3)) / ((int)PointType.Level1);
        }

        public virtual void UpdateDisplay()
        {
            throw new NotImplementedException();
        }

        private void InitPoints()
        {
            if (!ModelObject.pointsMap.ContainsKey(GetType().Name))
            {
                var objectPoints = new ObjectPoints();
                objectPoints.pointnames = new Collection<string>();
                objectPoints.levels = new Collection<int>();
                objectPoints.directions = new Collection<PointType>();
                int current_point = 0;

                foreach (var propertyInfo in GetType().GetProperties())
                {
                    var pointAttribyte = propertyInfo.GetCustomAttribute<PointTypeAttribute>();
                    if (pointAttribyte == null) continue;

                    //if (type2dst(pointAttribyte.type))
                    if (current_point != type2idx(pointAttribyte.type))
                        throw new Exception("Internal Error"); // points numbering problem

                    objectPoints.directions.Add(type2dir(pointAttribyte.type));
                    objectPoints.levels.Add(type2level(pointAttribyte.type));
                    objectPoints.pointnames.Add(propertyInfo.Name);
                    current_point++;
                }
                ModelObject.pointsMap[GetType().Name] = objectPoints;
            }
        }

        public virtual void OneStep(FlowDiagram fd)
        {
        }

        public int GetPointsCount()
        {
            var pointsMap = ModelObject.pointsMap[GetType().Name];
            return pointsMap.pointnames.Count;
        }

        public ModelObject()
        {
            InitPoints();

            points = new Func<double>[GetPointsCount()];
            points_value = new double[GetPointsCount()];
        }

        public string GetPointName(int idx)
        {
            var pointsMap = ModelObject.pointsMap[GetType().Name];
            return pointsMap.pointnames[idx];
        }

        public int GetPointLevel(int idx)
        {
            var pointsMap = ModelObject.pointsMap[GetType().Name];
            return pointsMap.levels[idx];
        }

        public PointType GetPointDirection(int idx)
        {
            var pointsMap = ModelObject.pointsMap[GetType().Name];
            return pointsMap.directions[idx];
        }
        public Func<double> GetPoint(string name)
        {
            var pointsMap = ModelObject.pointsMap[GetType().Name];
            var idx = pointsMap.pointnames.IndexOf(name);

            return points[idx];
        }

        public void SetPoint(string name, Func<double> value)
        {
            var pointsMap = ModelObject.pointsMap[GetType().Name];
            var idx = pointsMap.pointnames.IndexOf(name);

            points[idx] = value;
        }
    }
}
