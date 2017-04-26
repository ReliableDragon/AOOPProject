using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace WindowsFormsApplication2
{

    public struct userstruct
    {
        public string username { get; set; }
        public string type { get; set; }
    }

    public partial class Classes : BaseForm
    {
        const string nextTerm = "S15";
        private Form prevForm;
        private userstruct User;
        bool isAdmin;

        public Classes()
        {
            InitializeComponent();
        }

        public Classes(Form homePage,userstruct status)
        {
            prevForm = homePage;
            InitializeComponent();
            User = status;
            isAdmin = false;
            comboBox1.Items.Clear();

            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryCoursesOffered = from crs in db.courses
                                      where crs.semester == nextTerm
                                      select crs.courseName;
            
            //string line;
            //System.IO.StreamReader database = new System.IO.StreamReader("courses.in");
            //while (!database.EndOfStream)
            //{
                //line = database.ReadLine();
            foreach (var c in queryCoursesOffered) {
                string thing="";
                for (int i = 0; c[i] != '-'; i++)
                    thing += c[i];
                if (!comboBox1.Items.Contains(thing))
                {
                    comboBox1.Items.Add(thing);
                }
           }
            //database.Close();
        }

        public Classes(Form homePage, userstruct status, bool admin)
        {
            prevForm = homePage;
            InitializeComponent();
            User = status;
            isAdmin = admin;
            comboBox1.Items.Clear();

            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryCoursesOffered = from crs in db.courses
                                      where crs.semester == nextTerm
                                      select crs.courseName;

            //string line;
            //System.IO.StreamReader database = new System.IO.StreamReader("courses.in");
            //while (!database.EndOfStream)
            //{
            //line = database.ReadLine();
            foreach (var c in queryCoursesOffered)
            {
                string thing = "";
                for (int i = 0; c[i] != '-'; i++)
                    thing += c[i];
                if (!comboBox1.Items.Contains(thing))
                {
                    comboBox1.Items.Add(thing);
                }
            }
            //database.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            prevForm.Show();
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label9.Visible = false;
            comboBox3.Visible = false;
            comboBox3.Enabled = false;

            comboBox2.Items.Clear();

            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryCoursesOffered = from crs in db.courses
                                      where crs.courseName.Contains(comboBox1.Text) && crs.semester == nextTerm
                                      select crs.courseTitle;
            
            
            //string line;
            //System.IO.StreamReader database = new System.IO.StreamReader("courses.in");
            foreach (var c in queryCoursesOffered)
            {
                //line = database.ReadLine();
                //string thing = "";
                //for (int i = 0; c[i] != '-'; i++)
                //    thing += c[i];
                //if (comboBox1.Text.Equals(thing))
                //{
                    //var thing="";
                    //for (int i = 11; i<26; i++)
                    //    thing += c[i];
                    if(!comboBox2.Items.Contains(c))
                        comboBox2.Items.Add(c);
                //}
            }
            //database.Close();

            label2.Visible = true;
            comboBox2.Visible = true;
            comboBox2.Enabled = true;
            comboBox2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            //label5.Visible = false;
            label6.Visible = false;
            //label7.Visible = false;
            //label8.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            button3.Visible = false;
            button3.Enabled = false;
            button4.Visible = false;
            button4.Enabled = false;
            richTextBox1.Visible = false;
            richTextBox1.Clear();
            richTextBox1.ForeColor = Color.Black;
          
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label3.Visible = false;
            label4.Visible = false;
            //label5.Visible = false;
            label6.Visible = false;
            //label7.Visible = false;
            //label8.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            button3.Visible = false;
            button3.Enabled = false;
            button4.Visible = false;
            button4.Enabled = false;
            label9.Visible = true;
            comboBox3.Visible = true;
            comboBox3.Enabled = true;
            comboBox3.Items.Clear();
            richTextBox1.Visible = false;
            richTextBox1.Clear();
            richTextBox1.ForeColor = Color.Black;

            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryCoursesOffered = from crs in db.courses
                                      where crs.courseTitle == comboBox2.Text && crs.semester == nextTerm
                                      select crs;
            

            //string line;
            //string section="";
            //System.IO.StreamReader database = new System.IO.StreamReader("courses.in");
            //while (!database.EndOfStream)
            foreach(var c in queryCoursesOffered)
            {
                //int i;
                //line = database.ReadLine();
                //string thing = "";
                //string otherThing = "";
                //for (i = 0; line[i] != '-'; i++)
                //    thing += line[i];
                //for (i++; line[i] != '-'; i++);
                //for (i++; line[i] != ' '; i++)
                //    otherThing += line[i];
                //if (comboBox1.Text.Equals(thing))
                //{
                //    thing = "";
                //    for (i = 11; i < 26; i++)
                //        thing += line[i];
                    //if (comboBox2.Text.ToString() == thing)
                    //{
                        comboBox3.Items.Add(c.courseName.Substring(c.courseName.Length-2));
                        //section = line;
                    //}
                //}
            }
            //database.Close();

            if (comboBox3.Items.Count.Equals(1))
            {
                convertLine();
                comboBox3.Text = Convert.ToString(comboBox3.Items[0]);
            }
        }

        private void convertLine()
        {
            label3.Visible = true;
            label4.Visible = true;
            label6.Visible = true;
            //label7.Visible = true;
            //label8.Visible = false;
            //label5.Visible = false;
            label13.Visible = true;
            label10.Visible = true;
            label11.Visible = true;
            label12.Visible = true;
            label14.Visible = true;
            label15.Visible = true;
            if (User.type != "faculty"&&User.type!="admin" && User.type != "manager")
            {
                button3.Visible = true;
                button3.Enabled = true;
                button4.Visible = true;
                button4.Enabled = true;
            }

            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryCoursesOffered = from crs in db.courses
                                      where crs.courseTitle == comboBox2.Text && crs.courseName.Contains(comboBox3.Text) && crs.semester == nextTerm
                                      select crs;

            var queryCourseInstructor = from crs in db.courses
                                        join usr in db.users on crs.instructor equals usr.userID
                                        where crs.courseTitle == comboBox2.Text && crs.courseName.Contains(comboBox3.Text) && crs.semester == nextTerm
                                        select usr.username;
            
            var c = queryCoursesOffered.First(x => true);
            var instructorName = queryCourseInstructor.First(x => true);
            //string thing = "";
            //for (int i = 0; i < 10; i++)
            //    thing += line[i];
            label11.Text = c.courseName;


            var queryGetPrereqs = from prq in db.prereqs
                                  where prq.courseName == label11.Text
                                  select prq.prereqName;
            if (queryGetPrereqs.Count() > 0)
            {
                richTextBox1.Text += "Required prerequisites: " + queryGetPrereqs.ToArray()[0].ToString();
                for (int i = 1; i < queryGetPrereqs.Count(); i++)
                {
                    richTextBox1.Text += ", " + queryGetPrereqs.ToArray()[i].ToString();
                }
                richTextBox1.Text += ".\n";
            }
            richTextBox1.Text += "Meeting times:\n";
            //thing = "";
            //for (int i = 27; i < 37; i++)
            //    thing += line[i];
            label6.Text = instructorName;
            //thing = "";
            //for (int i = 38; i < 42; i++)
            //    thing += line[i];
            label13.Text = c.courseCredit.ToString();
            //thing = "";
            //for (int i = 43; line[i]!=' '; i++)
            //    thing += line[i];
            if (c.numSeats >= 0)
            {
                label15.Text = c.numSeats.ToString();
            }
            else
            {
                label15.Text = "0";
            }

            var queryGetTimeBlocks = from crs in db.courses
                                     join tbs in db.course_timeblocks
                                     on crs.courseID equals tbs.courseID
                                     where crs.courseTitle == comboBox2.Text && crs.courseName.Contains(comboBox3.Text) && crs.semester == nextTerm
                                     select tbs.timeblock;
            List<int> timeBlocksToConvert = new List<int>();
            foreach (var timeIterator in queryGetTimeBlocks)
            {
                timeBlocksToConvert.Add(timeIterator);
            }
            List<dayAndTime> timeBlocks = convertTime(timeBlocksToConvert);

            var queryTimeConflict = from usrcrs in db.user_courses
                                    join crstb in db.course_timeblocks
                                    on usrcrs.courseID equals crstb.courseID
                                    where usrcrs.username == User.username && usrcrs.semester == nextTerm
                                    select crstb.timeblock;
            timeBlocksToConvert = new List<int>();
            foreach (var timeIterator in queryTimeConflict)
            {
                timeBlocksToConvert.Add(timeIterator);
            }
            List<dayAndTime> timeBlocksConflict = convertTime(timeBlocksToConvert);

            richTextBox1.Show();
            foreach (var timeIterator in timeBlocks) 
            {
                richTextBox1.Text += timeIterator.ToString() + "\n";
                
            }

            var queryOnSchedule = from usrcrs in db.user_courses
                                  where usrcrs.courseName == label11.Text && usrcrs.semester == nextTerm && usrcrs.username == User.username
                                  select usrcrs.courseName;
            string baseCourseName = label11.Text.Substring(0, label11.Text.Length - 2);

            bool hasPrereq = true;
            var queryCoursePrereqs = from prq in db.prereqs
                                     where prq.courseName == label11.Text
                                     select prq.prereqName.Substring(0, 7);
            foreach (var d in queryCoursePrereqs)
            {
                if (!(from usrcrs in db.user_courses
                      where usrcrs.username == User.username && usrcrs.semester != nextTerm
                      select usrcrs.courseName.Substring(0, 7)).Contains(d))
                    hasPrereq = false;
            }

            if (queryOnSchedule.Contains(label11.Text))
            {
                richTextBox1.ForeColor = Color.DarkRed;
                richTextBox1.Text += "Note - You're already registered for this class!\n";
            }
            else if ((from chkSections in db.user_courses
                      where chkSections.courseName.Contains(baseCourseName) && chkSections.username == User.username && chkSections.semester == nextTerm
                      select chkSections).Any())
            {
                richTextBox1.ForeColor = Color.Crimson;
                richTextBox1.Text += "Note - You're already registered for another section of this class.\n";
            }
            else if (overlaps(timeBlocks, timeBlocksConflict))
            {
                richTextBox1.ForeColor = Color.Red;
                richTextBox1.Text += "Warning - This class conflicts with others on your schedule!\n";
            }
            else if (!hasPrereq && User.type == "student")
            {
                richTextBox1.ForeColor = Color.DarkMagenta;
                richTextBox1.Text += "Missing prerequisite. You don't have the required prerequisites to take this course.";
            }

            //for(int j=0;j<((int)line[47]-(int)'0');j++)
            //{
            //    thing = "";
            //    for (int i = 49 + 6 * j; i < 51 + 6 * j; i++)
            //        thing += line[i];
            //    int number = Convert.ToInt32(thing);
            //    thing = "";
            //    if (number >= 16)
            //    {
            //        thing += '1';
            //        number -= 16;
            //    }
            //    else
            //        thing += '0';
            //    if (number >= 8)
            //    {
            //        thing += '1';
            //        number -= 8;
            //    }
            //    else
            //        thing += '0';
            //    if (number >= 4)
            //    {
            //        thing += '1';
            //        number -= 4;
            //    }
            //    else
            //        thing += '0'; 
            //    if (number >= 2)
            //    {
            //        thing += '1';
            //        number -= 2;
            //    }
            //    else
            //        thing += '0'; 
            //    if (number == 1)
            //    {
            //        thing += '1';
            //    }
            //    else
            //        thing += '0';
            //    if (j == 0)
            //        label7.Text = "";
            //    else
            //        label8.Text = "";
            //    if (thing[4] == '1')
            //        if (j == 0)
            //            label7.Text += 'M';
            //        else
            //            label8.Text += "M";
            //    if (thing[3] == '1')
            //        if (j == 0)
            //            label7.Text += 'T';
            //        else
            //            label8.Text += "T";
            //    if (thing[2] == '1')
            //        if (j == 0)
            //            label7.Text += 'W';
            //        else
            //            label8.Text += "W";
            //    if (thing[1] == '1')
            //        if (j == 0)
            //            label7.Text += 'R';
            //        else
            //            label8.Text += "R";
            //    if (thing[0] == '1')
            //        if (j == 0)
            //            label7.Text += 'F';
            //        else
            //            label8.Text += "F";

            //    thing = "";
            //    for (int i = 51 + 6 * j; i < 53 + 6 * j; i++)
            //        thing += line[i];
            //    number = Convert.ToInt32(thing);
            //    int end = (int)line[53 + 6 * j]-(int)'0' + number;
            //    if (j == 0)
            //    {
            //        label7.Text += " ";
            //        label7.Text += number / 2;
            //        label7.Text += ':';
            //        if (number % 2 == 1)
            //            label7.Text += "30-";
            //        else
            //            label7.Text += "00-";

            //        label7.Text += end / 2;
            //        label7.Text += ':';
            //        if (end % 2 == 1)
            //            label7.Text += "30";
            //        else
            //            label7.Text += "00";
            //    }
            //    else
            //    {
            //        label8.Text += " ";
            //        label8.Text += number / 2;
            //        label8.Text += ':';
            //        if (number % 2 == 1)
            //            label8.Text += "30-";
            //        else
            //            label8.Text += "00-";

            //        label8.Text += end / 2;
            //        label8.Text += ':';
            //        if (end % 2 == 1)
            //            label8.Text += "30";
            //        else
            //            label8.Text += "00";
            //        label8.Visible = true;
            //        label5.Visible = true;
            //    }
            //}
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.ForeColor = Color.Black;

            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryCoursesOffered = from crs in db.courses
                                      where crs.courseTitle == comboBox2.Text && crs.courseName.Contains(comboBox3.Text) && crs.semester == nextTerm
                                      select crs;

            //string line;
            
            //System.IO.StreamReader database = new System.IO.StreamReader("courses.in");
            //while (!database.EndOfStream)
            //foreach (var c in queryCoursesOffered)
            //{
                //int i;
                //line = database.ReadLine();
                //string thing = "";
                //string otherThing = "";
                //for (i = 0; line[i] != '-'; i++)
                    //thing += line[i];
                //for (i++; line[i] != '-'; i++) ;
                //for (i++; line[i] != ' '; i++)
                    //otherThing += line[i];
                //if (comboBox1.Text.Equals(thing))
                //{
                    //thing = "";
                    //for (i = 11; i < 26; i++)
                        //thing += line[i];
                    //if (comboBox2.Text.ToString() == thing)
                    //{
                        //if (otherThing == comboBox3.Text.ToString())
                            convertLine();
                    //}
                //}
            //}
            //database.Close();
        }

        class dayAndTime {
            public char Day {get; set;}
            public int Length {get; set;}
            public int StartTime {get; set;}

            public override string  ToString()
{
                string st = "";
 	            switch(Day) {
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
                
                string timeBegin = System.Convert.ToString(StartTime/2)+":";
                bool odd = false;
                if (StartTime%2 == 1) 
                {
                    timeBegin += "30";
                    odd = true;
                }
                else timeBegin += "00";

                string timeEnd = System.Convert.ToString((StartTime+Length)/2)+":";
                if ((Length%2 == 1 && odd) || (Length%2 == 0 && !odd)) 
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

        bool overlaps(List<dayAndTime> time1, List<dayAndTime> time2)
        {
            foreach (var i in time1)
            {
                foreach (var j in time2)
                {
                    if (j.Day == i.Day)
                    {
                        if ((j.StartTime >= i.StartTime && j.StartTime < i.StartTime + i.Length) || (j.StartTime + j.Length > i.StartTime && j.StartTime + j.Length <= i.StartTime + i.Length))
                        {
                            return true;
                        }

                    }
                }
            }
            return false;
        }

        bool conflicts()
        {
            string courseName = label11.Text;
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryOldCourseTimes = from usrcs in db.user_courses
                                   join crstb in db.course_timeblocks
                                   on usrcs.courseID equals crstb.courseID
                                   where usrcs.username == User.username && usrcs.semester == nextTerm
                                   select crstb.timeblock;

           //int courseID = getFirstCourseID.FirstOrDefault();

           //var queryCourseIDs = from uc in db.user_courses
           //            where uc.username == User.username
           //            select uc.courseID;

           //List<int> courseIDs = new List<int>();
           //foreach (var i in queryCourseIDs)
           //{
           //        courseIDs.Add((int)i);
           //}
            
           List<int> timesForOldClass = new List<int>();
           foreach (int cID in queryOldCourseTimes)
           {
                   timesForOldClass.Add(cID);
           }

           List<int> timesForNewClass = new List<int>();

            var queryNewCourseTimes = from crs in db.courses
                                      join crstb in db.course_timeblocks
                                      on crs.courseID equals crstb.courseID
                                      where crs.courseName == courseName && crs.semester == nextTerm
                                      select crstb.timeblock;

            foreach (var qtb in queryNewCourseTimes)
            {
                timesForNewClass.Add(qtb);
            }

            List<dayAndTime> oldClassTimes = convertTime(timesForOldClass);
            List<dayAndTime> newClassTimes = convertTime(timesForNewClass);


            var addedClass = (from crs in db.courses
                              where crs.courseName == courseName && crs.semester == nextTerm
                              select crs).FirstOrDefault();

            if (addedClass.numSeats < 1 && !isAdmin)
            {
                MessageBox.Show("There are no seats available in this class!");
                return true;
            }

            //System.IO.StreamReader classList = new System.IO.StreamReader("classList.in");
            //List<string> everything = new List<string>();
            //while(!classList.EndOfStream)
            //{
            //    everything.Add(classList.ReadLine());
            //}
            //classList.Close();
            //int index = everything.FindIndex(x => x.Contains(User.username));

            var queryOldCourses = from usrcs in db.user_courses
                                  where usrcs.username == User.username && usrcs.semester == nextTerm
                                  select usrcs;
            var queryPrevCourses = from usrcs in db.user_courses
                                  where usrcs.username == User.username && usrcs.semester != nextTerm
                                  select usrcs.courseName.Substring(0, 7);

            List<user_course> oldCourses = new List<user_course>();
            double credits = 0;
            foreach (var c in queryOldCourses) 
            {
                oldCourses.Add(c);
                credits += (double)c.courseCredit;
            }
            
            // UNKNOWN CODE
            //if (oldCourses.Count == 0)
            //    return false;


            //string numClasses = System.Convert.ToString(everything[index][11]) + System.Convert.ToString(everything[index][12]);
            //int classes = System.Convert.ToInt32(numClasses);

            //for (int i = 0; i < classes; i++)
            //{
            //    if (everything[index].Substring(25 + i * 24, 3) == nextTerm)
            //        credits+=Convert.ToDouble(everything[index].Substring(29+i*24,4));
            //}
            string outParam = "";
            var queryCoursePrereqs = from prq in db.prereqs
                                     where prq.courseName == addedClass.courseName
                                     select prq.prereqName.Substring(0, 7);
            foreach (var c in queryCoursePrereqs)
            {
                if (!(from usrcrs in db.user_courses
                      where usrcrs.username == User.username && usrcrs.semester != nextTerm && (usrcrs.grade[0] == 'R' ?  (usrcrs.grade[1] == 'A' || usrcrs.grade[1] == 'B' || usrcrs.grade[1] == 'C' || usrcrs.grade[1] == 'D' || usrcrs.grade[1] == 'S') : (usrcrs.grade[0] == 'A' || usrcrs.grade[0] == 'B' || usrcrs.grade[0] == 'C' || usrcrs.grade[0] == 'D' || usrcrs.grade[0] == 'S'))
                      select usrcrs.courseName.Substring(0, 7)).Contains(c) && !isAdmin)
                {
                    if (c[c.Length - 1] == '-')
                    {
                        outParam = c.Substring(0, c.Length - 1);
                    }
                    else
                    {
                        outParam = c;
                    }
                    MessageBox.Show("You haven't taken " + outParam + ", so you aren't eligible for " + comboBox2.SelectedItem.ToString() + ".");
                    return true;
                }
            }

            if (credits + (double)addedClass.courseCredit >= 5 && !isAdmin)
            {
                MessageBox.Show("You are registered for too many classes to add this one!");
                return true;
            }
            bool taken = false;
            foreach (var c in oldCourses) {
                if (c.courseName == label11.Text)
                    taken = true;
            }
            if (!taken)
            {
                foreach (var c in oldCourses)
                {
                    if (c.courseName.Substring(0, c.courseName.Length - 2) == addedClass.courseName.Substring(0, addedClass.courseName.Length - 2) && !isAdmin)
                    {
                        MessageBox.Show("You're already registered for another section of this course!");
                        return true;
                    }
                }
                if (overlaps(newClassTimes, oldClassTimes))
                {
                    MessageBox.Show("This class overlaps with another class, continuing with registration.");
                }
                if (queryPrevCourses.Contains(label11.Text.Substring(0, 7)))
                {
                    MessageBox.Show("You've taken this class before, continuing with registration.");
                }
                 return false;
            }

            

            //for (int i = 0; i < classes; i++)
            //{
            //    string theClass = "";
            //    int j = 0;

            //    for (; everything[index][14 + i * 24 + j]!='-'; j++)
            //    {
            //        theClass += everything[index][14 + i * 24 + j];
            //    }
            //    theClass+='-';
            //    for (j++; everything[index][14 + i * 24 + j] != '-'; j++)
            //    {
            //        theClass += everything[index][14 + i * 24 + j];
            //    }
                if (taken)
                {
                    //if (everything[index].Substring(25 + i * 24, 3) == nextTerm)
                    //{
                        MessageBox.Show("You are already registered for this class!");
                        return true;
                    //}
                }
            //}
            
            if (overlaps(newClassTimes, oldClassTimes)) 
            {
                MessageBox.Show("This class overlaps with another class, continuing with registration.");
            }
            MessageBox.Show("You have taken this course before, continuing with registration.");
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {


            //var queryCoursesOffered = from crs in db.courses
            //                          where crs.courseTitle == comboBox2.Text
            //                          select crs;
            //string line="";
            //System.IO.StreamReader database = new System.IO.StreamReader("courses.in");
            //while (!database.EndOfStream)
            //{
            //    line=database.ReadLine();
            //    if(line.Contains(label11.Text))
            //        break;
            //}
            //database.Close();

            if (!conflicts())
            {
                linqtoregdbDataContext db = new linqtoregdbDataContext();
                //database = new System.IO.StreamReader("courses.in");
                //int index=0;
                //int iterator = 0;
                //List<string> courses = new List<string>();
                //while (!database.EndOfStream)
                //{
                //    line = database.ReadLine();
                //    courses.Add(line);
                //    if (line.Contains(label11.Text))
                //        index=iterator;
                //    iterator++;
                //}
                //database.Close();
                var addedClass = (from crs in db.courses
                                  where crs.courseName == label11.Text && crs.semester == nextTerm
                                  select crs).FirstOrDefault();

                var queryOldCourses = from usrcs in db.user_courses
                                      where usrcs.username == User.username && usrcs.semester == nextTerm
                                      select usrcs;

                List<user_course> oldCourses = new List<user_course>();
                foreach (var c in queryOldCourses)
                {
                    oldCourses.Add(c);
                }

                //int seats=System.Convert.ToInt32(courses[index].Substring(43, 3))-1;
                //string seatsLeft = System.Convert.ToString(seats);
                //while(seatsLeft.Length<3)
                //    seatsLeft += " ";
                addedClass.numSeats -= 1;

                //courses[index] = courses[index].Substring(0, 43) + seatsLeft + courses[index].Substring(46);

                //System.IO.StreamWriter outDatabase = new System.IO.StreamWriter("courses.in");
                //foreach (string s in courses)
                //    outDatabase.WriteLine(s);
                //outDatabase.Close();
                db.SubmitChanges();

                convertLine(); //WHAT DO???

                //string courseAdded = label11.Text;
                //for (int i = courseAdded.Length; i < 11; i++)
                //{
                //    courseAdded += " ";
                //}
                //courseAdded += nextTerm + " ";
                //courseAdded += label13.Text + " " + "N  " + " ";

                //Now we call the function to add the new course to the file with the old ones.
                //PushChanges("classList.in", courseAdded);
                //linqtoregdbDataContext db = new linqtoregdbDataContext();

                var queryCourseID = from crs in db.courses
                                    where crs.courseName == label11.Text && crs.semester == nextTerm
                                    select crs.courseID;
                int newCourseID = queryCourseID.First(x => true);
                user_course uc = new user_course
                {
                    courseID = newCourseID,
                    username = User.username,
                    courseName = label11.Text,
                    semester = "S15",
                    courseCredit = System.Convert.ToDecimal(label13.Text),
                    grade = "N"
                };
                    db.user_courses.InsertOnSubmit(uc);
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (DuplicateKeyException exc)
                    {

                    }
                    MessageBox.Show("Course Registered Successfully!");
            }
            richTextBox1.Clear();
            richTextBox1.ForeColor = Color.Black;
            comboBox3_SelectedIndexChanged(sender, e);
            
        }

        //private void PushChanges(string fileName, string courseAdded)
        //{
        //    string username = User.username; 
        //    System.IO.StreamReader database;
        //    try
        //    {
        //        database = new System.IO.StreamReader(fileName);
        //    }
        //    catch (System.IO.FileNotFoundException)
        //    {
        //        System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.CreateNew);
        //        fs.Close();
        //        database = new System.IO.StreamReader(fileName);
        //    }
        //    List<string> info = new List<string>();
        //    while (!database.EndOfStream) //Read in input file to memory.
        //    {
        //        info.Add(database.ReadLine());
        //    }
        //    database.Close();
        //    var result = info.FindIndex(x => x.Contains(username)); //Find the line with the student we want.
        //    int coursenums;
        //    bool isEntry;
        //    if (result != -1) //Not found, so user has no entry. 
        //    {
        //        var courseholder = System.Convert.ToString(info[result][11]) + System.Convert.ToString(info[result][12]); //Get the number of courses from the string
        //        coursenums = System.Convert.ToInt16(courseholder);
        //        isEntry = true;
        //    }
        //    else
        //    {
        //        coursenums = 1;
        //        isEntry = false;
        //    }
        //    string newline = username;
        //    for (int i = newline.Length; i < 11; i++) //Fill in space after username
        //    {
        //        newline = newline + " ";
        //    }
        //    newline += System.Convert.ToString(coursenums + 1); //Add one to number of courses.
        //    if (newline.Length != 13) //Add space if one digit number of courses
        //    {
        //        newline += " ";
        //    }
        //    newline += " ";
        //    if (isEntry)
        //    {
        //        newline += info[result].Substring(14, info[result].Length - 14);
        //    }
        //    newline += courseAdded;
        //    if (isEntry)
        //    {
        //        info[result] = newline;
        //    }
        //    else
        //    {
        //        info.Add(newline);
        //    }

        //    System.IO.StreamWriter outDatabase = new System.IO.StreamWriter(fileName);

        //    foreach (var e in info)
        //    {
        //        outDatabase.WriteLine(e);
        //    }
        //    outDatabase.Close();

        //    MessageBox.Show("Course Registered Successfully!");
        //}

        private void button4_Click(object sender, EventArgs e)
        {
            //System.IO.StreamReader database = new System.IO.StreamReader("classList.in");
            //int index = 0;
            //int iterator = 0;
            //string line;
            //List<string> courses = new List<string>();
            //while (!database.EndOfStream)
            //{
            //    line = database.ReadLine();
            //    courses.Add(line);
            //    if (line.Contains(User.username))
            //        index = iterator;
            //    iterator++;
            //}
            //database.Close();

            //int classes = System.Convert.ToInt32(courses[index].Substring(11, 2));
            //line = "";
            //string pre = courses[index].Substring(0, 11);
            //bool change = false;
            //int classesNow = classes;
            //for (int i = 0; i < classes; i++)
            //{
            //    if ((courses[index].Substring(14 + i * 24, 24).Contains(label11.Text) && courses[index].Substring(13 + i * 24, 24).Contains(nextTerm)))
            //    {
            //        classesNow--;
            //        change = true;
            //    }
            //    else
            //    {
            //        line += courses[index].Substring(14 + i * 24, 24);
            //    }
            //}
            //pre += Convert.ToString(classesNow);
            //while (pre.Length < 14)
            //    pre += " ";
            //courses[index] = pre + line;

            linqtoregdbDataContext db = new linqtoregdbDataContext();

            bool isCourseRegistered = (from usrcrs in db.user_courses
                                       where usrcrs.username == User.username && usrcrs.semester == nextTerm && usrcrs.courseName == label11.Text
                                       select usrcrs).Count() != 0;

            if (!isCourseRegistered)
                MessageBox.Show("You are not currently registered for this course!");
            else
            {
                //System.IO.StreamReader database2 = new System.IO.StreamReader("courses.in");
                //int index2=0;
                //int iterator2 = 0;
                //List<string> courses2 = new List<string>();
                //string line2="";
                //while (!database2.EndOfStream)
                //{
                //    line2 = database2.ReadLine();
                //    courses2.Add(line2);
                //    if (line2.Contains(label11.Text))
                //        index2=iterator2;
                //    iterator2++;
                //}
                //database2.Close();

                //int seats=System.Convert.ToInt32(courses2[index2].Substring(43, 3))+1;
                //string seatsLeft = System.Convert.ToString(seats);
                //while(seatsLeft.Length<3)
                //    seatsLeft += " ";

                //courses2[index2] = courses2[index2].Substring(0, 43) + seatsLeft + courses2[index2].Substring(46);

                var addedClass = (from crs in db.courses
                                  where crs.courseName == label11.Text && crs.semester == nextTerm
                                  select crs).FirstOrDefault();
                addedClass.numSeats += 1;
                db.SubmitChanges();

                //System.IO.StreamWriter outDatabase2 = new System.IO.StreamWriter("courses.in");
                //foreach (string f in courses2)
                //    outDatabase2.WriteLine(f);
                //outDatabase2.Close();

                convertLine();

                //System.IO.StreamWriter outDatabase = new System.IO.StreamWriter("classList.in");
                //foreach (var l in courses)
                //{
                //    outDatabase.WriteLine(l);
                //}
                //outDatabase.Close();

                //Remove class from database
                //linqtoregdbDataContext db = new linqtoregdbDataContext();

                var queryCourseID = from crs in db.user_courses
                                    where crs.courseName == label11.Text && crs.semester == nextTerm && crs.username == User.username
                                    select crs;
                var newCourse = queryCourseID.First(x => true);

                //var queryUserCourse = (from usrcrs in db.user_courses
                //                       where usrcrs.courseID == newCourse.courseID && usrcrs.username == User.username
                //                       select usrcrs).FirstOrDefault();

                //user_course uc = new user_course
                //{
                //    user_courseID = newCourseID
                //    //username = User.username,
                //    //courseName = label11.Text,
                //    //semester = "S15",
                //    //courseCredit = System.Convert.ToDecimal(label13.Text),
                //    //grade = "N"
                //};
                //db.user_courses.Attach(uc);
                db.user_courses.DeleteOnSubmit(newCourse);
                db.SubmitChanges();

                richTextBox1.Clear();
                richTextBox1.ForeColor = Color.Black;
                comboBox3_SelectedIndexChanged(sender, e);

                MessageBox.Show("Course Removed Successfully!");
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }

        private void Classes_FormClosed(object sender, FormClosedEventArgs e)
        {
            prevForm.Show();
        }

        public static bool Passes(string inGrade)
        {
            if (inGrade[0] == 'R')
                inGrade = inGrade.Substring(1);
            if (inGrade[0] == 'A' || inGrade[0] == 'B' || inGrade[0] == 'C' || inGrade[0] == 'D' || inGrade[0] == 'S')
                return true;
            else return false;
        }
       
    }
}