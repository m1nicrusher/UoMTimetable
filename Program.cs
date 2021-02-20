using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Ical.Net;
namespace ProjectTimetable
{
    class Program
    {
        static void Main(string[] args)
        {
            string link = "";
            TimetableController tc = new TimetableController();
            tc.RenderTimetable(
                link,
                new TimetableStyle(),
                new DateTime(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2])),
                "drawingwR.png"
            );
            foreach (var item in args)
            {
                System.Console.WriteLine(item);
            }
        }
    }
}