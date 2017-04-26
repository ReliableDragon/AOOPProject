using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication2
{
    class TimeCheck
    {
        public class dayAndTime
        {
            public char Day { get; set; }
            public int Length { get; set; }
            public int StartTime { get; set; }

            public override string ToString()
            {
                string st = "";
                switch (Day)
                {
                    case 'M':
                        st = "Monday";
                        break;
                    case 'T':
                        st = "Tuesday";
                        break;
                    case 'W':
                        st = "Wednesday";
                        break;
                    case 'R':
                        st = "Thursday";
                        break;
                    case 'F':
                        st = "Friday";
                        break;
                    default:
                        st = "InvalidDay";
                        break;
                }

                string timeBegin = System.Convert.ToString(StartTime / 2) + ":";
                bool odd = false;
                if (StartTime % 2 == 1)
                {
                    timeBegin += "30";
                    odd = true;
                }
                else timeBegin += "00";

                string timeEnd = System.Convert.ToString((StartTime + Length) / 2) + ":";
                if ((Length % 2 == 1 && odd) || (Length % 2 == 0 && !odd))
                {
                    timeEnd += "00";
                }
                else timeEnd += "30";

                return st + " " + timeBegin + " - " + timeEnd;
            }
        }

        public static int ConvertToTime(int startTime, int endTime, bool onMon, bool onTue, bool onWed, bool onThur, bool onFri)
        {
            //bool startsOdd = false;
            int tb = 0;
            if (onFri)
                tb += 16000;
            if (onThur)
                tb += 8000;
            if (onWed)
                tb += 4000;
            if (onTue)
                tb += 2000;
            if (onMon)
                tb += 1000;

            tb += ((startTime + 20) / 50) * 10;
            tb += (endTime - startTime + 20) / 50;

            //if (startTime % 100 == 30)
            //{
            //    startsOdd = true;
            //    startTime -= 30;
            //    tb += 10;
            //}
            //tb += (startTime * 2) / 10;
            //if (startsOdd)
            //{
            //    startTime += 30;
            //}
            ////Add 30 to get calculation to round up.
            //tb += (((endTime - startTime) + 30) / 100)*2;
            return tb;
        }

        public static List<dayAndTime> convertTime(int timeIn)
        {
            dayAndTime outTime = new dayAndTime();
            List<dayAndTime> outTimes = new List<dayAndTime>();
            
            int dateIn = timeIn / 1000;
            int startTimeIn = (timeIn/ 10) % 100;
            int lengthIn = timeIn % 10;
            outTime.Length = lengthIn;
            outTime.StartTime = startTimeIn;


            if (dateIn >= 16)
            {
                outTime.Day = 'F';
                outTimes.Add(outTime);
                outTime = new dayAndTime();
                outTime.Length = lengthIn;
                outTime.StartTime = startTimeIn;
                dateIn -= 16;
            }
            if (dateIn >= 8)
            {
                outTime.Day = 'R';
                outTimes.Add(outTime);
                outTime = new dayAndTime();
                outTime.Length = lengthIn;
                outTime.StartTime = startTimeIn;
                dateIn -= 8;
            }
            if (dateIn >= 4)
            {
                outTime.Day = 'W';
                outTimes.Add(outTime);
                outTime = new dayAndTime();
                outTime.Length = lengthIn;
                outTime.StartTime = startTimeIn;
                dateIn -= 4;
            }
            if (dateIn >= 2)
            {
                outTime.Day = 'T';
                outTimes.Add(outTime);
                outTime = new dayAndTime();
                outTime.Length = lengthIn;
                outTime.StartTime = startTimeIn;
                dateIn -= 2;
            }
            if (dateIn == 1)
            {
                outTime.Day = 'M';
                outTimes.Add(outTime);
                outTime = new dayAndTime();
                outTime.Length = lengthIn;
                outTime.StartTime = startTimeIn;


            }
            return outTimes;
        }

        public static List<dayAndTime> convertTime(List<int> timeIn)
        {
            List<dayAndTime> outTimes = new List<dayAndTime>();
            dayAndTime outTime = new dayAndTime();

            foreach (int tIn in timeIn)
            {
                int dateIn = tIn / 1000;
                int startTimeIn = (tIn / 10) % 100;
                int lengthIn = tIn % 10;

                outTime.Length = lengthIn;
                outTime.StartTime = startTimeIn;


                if (dateIn >= 16)
                {
                    outTime.Day = 'F';
                    outTimes.Add(outTime);
                    outTime = new dayAndTime();
                    outTime.Length = lengthIn;
                    outTime.StartTime = startTimeIn;
                    dateIn -= 16;
                }
                if (dateIn >= 8)
                {
                    outTime.Day = 'R';
                    outTimes.Add(outTime);
                    outTime = new dayAndTime();
                    outTime.Length = lengthIn;
                    outTime.StartTime = startTimeIn;
                    dateIn -= 8;
                }
                if (dateIn >= 4)
                {
                    outTime.Day = 'W';
                    outTimes.Add(outTime);
                    outTime = new dayAndTime();
                    outTime.Length = lengthIn;
                    outTime.StartTime = startTimeIn;
                    dateIn -= 4;
                }
                if (dateIn >= 2)
                {
                    outTime.Day = 'T';
                    outTimes.Add(outTime);
                    outTime = new dayAndTime();
                    outTime.Length = lengthIn;
                    outTime.StartTime = startTimeIn;
                    dateIn -= 2;
                }
                if (dateIn == 1)
                {
                    outTime.Day = 'M';
                    outTimes.Add(outTime);
                    outTime = new dayAndTime();
                    outTime.Length = lengthIn;
                    outTime.StartTime = startTimeIn;
                }

            }
            return outTimes;
        }

        public static bool overlaps(List<dayAndTime> time1, List<dayAndTime> time2)
        {
            foreach (var i in time1)
            {
                foreach (var j in time2)
                {
                    if (j.Day == i.Day)
                    {
                        //if ((j.StartTime >= i.StartTime && j.StartTime < i.StartTime + i.Length) || (j.StartTime + j.Length > i.StartTime && j.StartTime + j.Length <= i.StartTime + i.Length) || (i.StartTime < j.StartTime + j.Length && i.StartTime + i.Length >= j.StartTime + j.Length))
                        if(i.StartTime <= j.StartTime + j.Length && i.StartTime + i.Length < j.StartTime || j.StartTime <= i.StartTime + i.Length && j.StartTime + j.Length < i.StartTime)
                        {
                            return true;
                        }

                    }
                }
            }
            return false;
        }

        public static List<int> GetTimes(string className)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            var queryGetCourseTimes = from crs in db.courses
                                      join tbs in db.course_timeblocks
                                      on crs.courseID equals tbs.courseID
                                      where crs.courseName == className
                                      select tbs.timeblock;
            return queryGetCourseTimes.ToList();
        }
    }
}
