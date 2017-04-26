using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class FacultyHome : BaseForm
    {
        userstruct currentUser;
        Form prevWindow;
        List<string> usernames = new List<string>();
        bool isAdmin;

        public FacultyHome()
        {
            InitializeComponent();
        }

        public FacultyHome(userstruct userIn, Form sendingWindow, bool admin)
        {
            InitializeComponent();
            currentUser = userIn;
            prevWindow = sendingWindow;
            isAdmin = admin;
        }

        public FacultyHome(userstruct userIn, Form sendingWindow)
        {
            InitializeComponent();
            currentUser = userIn;
            prevWindow = sendingWindow;
            isAdmin = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            history historyWindow = new history(this, new userstruct { type = "student", username = usernames[adviseesBox.SelectedIndex] }, true);
            this.Hide();
            historyWindow.Show();
        }

        private void formClosed(object sender, FormClosedEventArgs e)
        {
            prevWindow.Show();
        }

        private void loadForm(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryTeachingSchedule = from crs in db.courses
                                        join usr in db.users
                                        on crs.instructor equals usr.userID
                                        where usr.username == currentUser.username
                                        select crs;
            foreach (var c in queryTeachingSchedule)
            {
                teachingScheduleBox.Items.Add(c.courseName);
            }

            var queryAdvisees = from usr in db.users
                                join adv in db.users
                                on usr.advisor equals adv.userID
                                where adv.username == currentUser.username
                                select usr;
            foreach (var c in queryAdvisees)
            {
                adviseesBox.Items.Add(c.firstName + " " + c.lastName);
                usernames.Add(c.username);
            }
        }

        private void teachingScheduleBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            label4.Show();
            richTextBox1.Clear();
            richTextBox1.Show();
            label10.Show();
            label11.Show();
            label12.Show();
            label13.Show();
            label14.Show();
            label15.Show();
            richTextBox3.Clear();
            richTextBox3.Show();
            label2.Show();

            var selectedCourse = (from crs in db.courses
                                      where crs.courseName == (string)teachingScheduleBox.SelectedItem
                                      select crs).FirstOrDefault();
            label11.Text = selectedCourse.courseTitle;
            label13.Text = selectedCourse.courseCredit.ToString();
            label15.Text = selectedCourse.numSeats.ToString();

            var queryGetTimeBlocks = from crs in db.courses
                                     join tbs in db.course_timeblocks
                                     on crs.courseID equals tbs.courseID
                                     where crs.courseName == (string)teachingScheduleBox.SelectedItem
                                     select tbs.timeblock;
            
            List<int> timeBlocksToConvert = new List<int>();
            foreach (var timeIterator in queryGetTimeBlocks)
            {
                timeBlocksToConvert.Add(timeIterator);
            }
            List<dayAndTime> timeBlocks = convertTime(timeBlocksToConvert);
            foreach (var timeIterator in timeBlocks)
            {
                richTextBox1.Text += timeIterator.ToString() + "\n";
            }

            var queryStudentsInClass = from usrcrs in db.user_courses
                                       join usrs in db.users
                                       on usrcrs.username equals usrs.username
                                       where usrcrs.courseName == selectedCourse.courseName && usrcrs.semester == "S15"
                                       select usrs;
            foreach (var c in queryStudentsInClass) 
            {
                richTextBox3.Text += c.firstName + " " + c.lastName + "\n";
            }
        }

        class dayAndTime
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

        List<dayAndTime> convertTime(List<int> timeIn)
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

        private void adviseesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            string selectedAdvisee = usernames[adviseesBox.SelectedIndex];
            button1.Show();

        }

        private void FacultyHome_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            prevWindow.Show();
            this.Close();
        }

    }
}
