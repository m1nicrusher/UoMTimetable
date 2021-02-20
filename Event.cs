using System;
using System.Linq;
using System.Collections.Generic;

namespace ProjectTimetable
{
    class Event
    {
        // They are local time in order to eliminate bugs should
        // the machine changed its time zone while the app is running.
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
    }

    /// <summary>
    /// This collection of 'Event' describes one week of events
    /// </summary>
    class OneWeekEvents
    {
        public List<Event> Events { get; }

        public OneWeekEvents(List<Event> events)
        {
            this.Events = events;
        }

        public DateTime FirstDayOfWeek
        {
            get => Events.Count == 0 ? new DateTime() : Events.FirstOrDefault().StartTime.StartOfWeek(DayOfWeek.Monday);
        }

        public int EarliestHour
        {
            get
            {
                if (Events.Count == 0)
                    return -1;
                int minHour = Events.FirstOrDefault().StartTime.Hour;
                foreach (var item in Events)
                {
                    int hour = item.StartTime.Hour;
                    if (hour < minHour)
                        minHour = hour;
                }
                return minHour;
            }
        }

        public int LatestHour
        {
            get
            {
                if (Events.Count == 0)
                    return -1;
                int maxHour = Events.FirstOrDefault().EndTime.Hour;
                foreach (var item in Events)
                {
                    int hour = item.EndTime.Hour;
                    if (hour > maxHour)
                        maxHour = hour;
                }
                return maxHour;
            }
        }
    }
}