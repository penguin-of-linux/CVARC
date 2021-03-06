﻿using System.Runtime.Serialization;
using AIRLab.Mathematics;

namespace RepairTheStarship.Sensors
{
    [DataContract]
    public class MapItem
    {
        [DataMember]
        public string Tag { get; set; }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        public MapItem(string type, Frame3D Location)
        {
            switch (type)
            {
                case "DR": Tag = "RedDetail"; break;
                case "DB": Tag = "BlueDetail"; break;
                case "DG": Tag = "GreenDetail"; break;
                case "VW": Tag = "VerticalWall"; break;
                case "VWR": Tag = "VerticalRedSocket"; break;
                case "VWB": Tag = "VerticalBlueSocket"; break;
                case "VWG": Tag = "VerticalGreenSocket"; break;
                case "HW": Tag = "HorizontalWall"; break;
                case "HWR": Tag = "HorizontalRedSocket"; break;
                case "HWB": Tag = "HorizontalBlueSocket"; break;
                case "HWG": Tag = "HorizontalGreenSocket"; break;
            }
            X = Location.X;
            Y = Location.Y;
        }

        public override string ToString()
        {
            return string.Format("Tag: {0} X:{1} Y:{2}", Tag, X, Y);
        }
    }
}