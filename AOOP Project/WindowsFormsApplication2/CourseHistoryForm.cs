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
    public partial class history : BaseForm
    {
        private Form home;
        private userstruct currentUser;
        bool readOnly;

        public history(Form input, userstruct inUser, bool readOnlyIn)
        {
            InitializeComponent();
            home = input;
            currentUser = inUser;
            readOnly = readOnlyIn;
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {
         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            home.Show();
            this.Close();
        }

        private void UpdateGrid()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.RowsDefaultCellStyle.BackColor = Color.White;

            if (comboBox1.SelectedItem.ToString() != "Next Semester")
                button2.Hide();
            else if (!readOnly)
                button2.Show();
            
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            if (comboBox1.Text == "Next Semester")
            {
                bool areConflicts = false; ;
                var queryCourseHistory = from uc in db.user_courses
                                         where uc.username == currentUser.username && uc.semester == "S15"
                                         select uc;

                var queryUserCourses = from uc in db.user_courses
                                       join crs in db.courses on uc.courseID equals crs.courseID
                                       where uc.username == currentUser.username && uc.semester == "S15"
                                       select crs;
                foreach (var c in queryCourseHistory)
                {
                    dataGridView1.Rows.Add(c.courseName, c.semester, (decimal)c.courseCredit, c.grade);
                }
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (conflicts(dataGridView1.Rows[i].Cells[0].Value.ToString()))
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    areConflicts = true;
                }
            }
            if (areConflicts)
            {
                MessageBox.Show("Warning - Times for the classes highlighed in red overlap with another class.", "Time Conflicts", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            }
            else if (comboBox1.Text == "Current Semester")
            {
                var queryCourseHistory = from uc in db.user_courses
                                         where uc.username == currentUser.username && uc.semester == "F14"
                                         select uc;

                var queryUserCourses = from uc in db.user_courses
                                       join crs in db.courses on uc.courseID equals crs.courseID
                                       where uc.username == currentUser.username && uc.semester == "F14"
                                       select crs;
                foreach (var c in queryCourseHistory)
                {
                    dataGridView1.Rows.Add(c.courseName, c.semester, (decimal)c.courseCredit, c.grade);
                }
            }
            else
            {
                var queryCourseHistory = from uc in db.user_courses
                                         where uc.username == currentUser.username && uc.semester != "F14" && uc.semester != "S15"
                                         select uc;

                var queryUserCourses = from uc in db.user_courses
                                       join crs in db.courses on uc.courseID equals crs.courseID
                                       where uc.username == currentUser.username && uc.semester != "F14" && uc.semester != "S15"
                                       select crs;
                double counter = 0.0;
                double sum = 0.0;

                foreach (var c in queryCourseHistory)
                {
                    dataGridView1.Rows.Add(c.courseName, c.semester, (decimal)c.courseCredit, c.grade);
                    if (c.grade != "N" && c.grade != "S")
                    {
                        sum += credit(c.grade) * (double)c.courseCredit;
                        counter += (double)c.courseCredit;
                    }
                }
                dataGridView1.Rows.Add("", "", "", "Total GPA:", calc_GPA(sum, counter));
            }

        }

        private void history_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = "Current Semester";
            if(!readOnly)
                button2.Show();
            UpdateGrid();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGrid();
        }

        public double credit(string grade)
        {
            double x = 0.0;
            if (grade.Equals("A"))
            {
                x = 4.0;
            }
            else if (grade.Equals("B-"))
            {
                x = 3.7;
            }
            else if (grade.Equals("B+"))
            {
                x = 3.3;
            }
            else if (grade.Equals("B"))
            {
                x = 3.0;
            }
            else if (grade.Equals("B-"))
            {
                x = 2.7;
            }
            else if (grade.Equals("C+"))
            {
                x = 2.3;
            }
            else if (grade.Equals("C"))
            {
                x = 2.0;
            }
            else if (grade.Equals("C-"))
            {
                x = 1.7;
            }
            else if (grade.Equals("D+"))
            {
                x = 1.3;
            }
            else if (grade.Equals("D "))
            {
                x = 1.0;
            }
            else if (grade.Equals("D-"))
            {
                x = 0.7;
            }
            else if (grade.Equals("F"))
            {
                x = 0.0;
            }
            else if (grade.Equals("WF"))
            {
                x = 0.0;
            }
            else if (grade.Equals("S"))
            {
                x = 0.0;
            }
            else if (grade.Equals("U"))
            {
                x = 0.0;
            }
            else
            {
                //x = Convert.ToDouble(grade);
                x = 0.0;
            }
            return x;
        }

        public double calc_GPA(double total, double num)
        {
            double solution = total / num;

            return solution;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this class?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();

                    var queryCourseID = from crs in db.user_courses
                                        where crs.courseName == dataGridView1.SelectedRows[0].Cells[0].Value.ToString() && crs.semester == "S15"
                                        select crs;

                    foreach (var c in queryCourseID)
                    {
                        db.user_courses.DeleteOnSubmit(c);
                        var addedClass = (from crs in db.courses
                                          where crs.courseName == c.courseName
                                          select crs).FirstOrDefault();
                        addedClass.numSeats += 1;
                        db.SubmitChanges();
                    }
                    db.SubmitChanges();

                    UpdateGrid();

                    MessageBox.Show("Course Removed Successfully!");
                }
            }
            else
            {
                MessageBox.Show("No course selected!");
            }
        }

        bool conflicts(string courseName)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryOldCourseTimes = from usrcs in db.user_courses
                                      join crstb in db.course_timeblocks
                                      on usrcs.courseID equals crstb.courseID
                                      where usrcs.username == currentUser.username && usrcs.semester == "S15" && usrcs.courseName != courseName
                                      select crstb.timeblock;

            List<int> timesForOldClass = new List<int>();
            foreach (int cID in queryOldCourseTimes)
            {
                timesForOldClass.Add(cID);
            }

            List<int> timesForNewClass = new List<int>();

            var queryNewCourseTimes = from crs in db.courses
                                      join crstb in db.course_timeblocks
                                      on crs.courseID equals crstb.courseID
                                      where crs.courseName == courseName
                                      select crstb.timeblock;

            foreach (var qtb in queryNewCourseTimes)
            {
                timesForNewClass.Add(qtb);
            }

            List<dayAndTime> oldClassTimes = convertTime(timesForOldClass);
            List<dayAndTime> newClassTimes = convertTime(timesForNewClass);


            var addedClass = (from crs in db.courses
                              where crs.courseName == courseName
                              select crs).FirstOrDefault();


            var queryOldCourses = from usrcs in db.user_courses
                                  where usrcs.username == currentUser.username && usrcs.semester == "S15"
                                  select usrcs;

            List<user_course> oldCourses = new List<user_course>();

            foreach (var c in queryOldCourses)
            {
                oldCourses.Add(c);
            }

            if (overlaps(newClassTimes, oldClassTimes))
            {
                return true;
            }
            else return false;
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

        private void history_FormClosed(object sender, FormClosedEventArgs e)
        {
            home.Show();
        }

        private void history_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }



    }

}
