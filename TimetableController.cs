using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing;
using System.Collections.Generic;

namespace ProjectTimetable
{
    class TimetableController
    {

        private Dictionary<TimeSpan, Rectangle> blocks = new Dictionary<TimeSpan, Rectangle>();

        private void CalculateBlocks()
        {

        }

        private Ical.Net.Calendar GetCalendarByLink(string link)
        {
            WebRequest wr = HttpWebRequest.Create(link);
            using (WebResponse response = wr.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                return Ical.Net.Calendar.Load(stream);
            }
        }

        private List<Event> GetEvents(Ical.Net.Calendar calendar)
        {
            List<Event> events = new List<Event>();

            foreach (var calendarEvent in calendar.Events)
            {
                Event ev = new Event();
                ev.StartTime = calendarEvent.DtStart.AsUtc.ToLocalTime();
                ev.EndTime = calendarEvent.DtEnd.AsUtc.ToLocalTime();
                var summary = calendarEvent.Summary;
                var point = summary.LastIndexOf(", ");
                var subject = summary.Substring(0, point);
                var type = summary.Substring(point + 2);
                ev.Subject = subject;
                ev.Type = type;
                events.Add(ev);
            }

            return events;
        }

        private OneWeekEvents GetOneWeekEvents(List<Event> events, DateTime day)
        {
            DateTime startOfThisWeek = day.StartOfWeek(DayOfWeek.Monday);
            DateTime endOfThisWeek = startOfThisWeek.AddDays(5);
            var res = (from ev in events
                     where ev.StartTime >= startOfThisWeek && ev.StartTime < endOfThisWeek
                     select ev).ToList<Event>();

            return new OneWeekEvents(res);
        }

        /// <summary>
        /// Render and save the timetable.
        /// </summary>
        /// <param name="link">The ics file subscription link</param>
        /// <param name="style">Style of the timetable</param>
        /// <param name="day">This day decide which week of the timetable is going to be rendered</param>
        /// <param name="fileName">The name of output file</param>
        public void RenderTimetable(string link, TimetableStyle style, DateTime day, string fileName)
        {
            var owe = GetOneWeekEvents(GetEvents(GetCalendarByLink(link)), day);
            // System.Console.WriteLine(owe.EarliestHour);
            // System.Console.WriteLine(owe.LatestHour);
            var renderer = new TimetableRenderer(3000, 2000, 200, 200, owe);
            renderer.Render(fileName);
        }
    }
}