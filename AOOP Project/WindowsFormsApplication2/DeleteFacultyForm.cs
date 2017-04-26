using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication2
{
    public partial class DeleteFacultyForm : BaseForm
    {
        Form prevWindow;
        user deletee;
        List<string> usernames = new List<string>();
        List<bool> facColor = new List<bool>();
        linqtoregdbDataContext db = new linqtoregdbDataContext();

        public DeleteFacultyForm(Form prevWindowIn, user inUser)
        {
            InitializeComponent();
            prevWindow = prevWindowIn;
            deletee = inUser;

            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryAdvisees = from usr in db.users
                                join adv in db.users
                                on usr.advisor equals adv.userID
                                where adv.username == deletee.username
                                select usr;
            foreach (var c in queryAdvisees) 
            {
                listBox2.Items.Add(c.firstName + " " + c.lastName);
                usernames.Add(c.username);
            }
            var queryCoursesAffected = from crs in db.courses
                                       join inst in db.users
                                       on crs.instructor equals inst.userID
                                       where inst.username == deletee.username
                                       select crs;
            foreach (var c in queryCoursesAffected)
            {
                listBox1.Items.Add(c.courseName);
            }

            var queryGetFaculty = from usr in db.users
                                  where usr.usertype == "faculty" && usr.username != deletee.username
                                  select usr;

            foreach (var c in queryGetFaculty)
            {
                listBox4.Items.Add(c.username);
                listBox3.Items.Add(c.username);
            }
            for (int i = 0; i < listBox3.Items.Count; i++)
            {
                facColor.Add(false);
            }
            listBox3.DrawMode = DrawMode.OwnerDrawFixed;
            listBox3.DrawItem += new DrawItemEventHandler(listBox3_DrawItem);
        }

        private void formClosed(object sender, FormClosedEventArgs e)
        {
            prevWindow.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a professor to transfer students to!");
            }
            else
            {
                string replacementProf = listBox4.SelectedItem.ToString();

                var replacementID = (from usr in db.users
                                     where usr.username == replacementProf
                                     select usr.userID).FirstOrDefault();

                List<int> selectedItems = new List<int>();
                foreach (var c in listBox2.SelectedIndices)
                    selectedItems.Add((int)c);

                foreach (var c in selectedItems)
                {
                    var userToChange = (from usr in db.users
                                        where usr.username == usernames[c]
                                        select usr).FirstOrDefault();
                    userToChange.advisor = replacementID;
                    listBox2.Items.RemoveAt((int)c);
                    usernames.RemoveAt(c);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> selectedItems = new List<string>();
            foreach (var c in listBox1.SelectedItems)
                selectedItems.Add((string)c);

            foreach (var c in selectedItems)
            {
                var queryGetTimeBlocksForRemove = from crstb in db.course_timeblocks
                                                  join crs in db.courses
                                                  on crstb.courseID equals crs.courseID
                                                  where crs.courseName == c
                                                  select crstb;
                var queryGetCoursesToRemove = from crs in db.courses
                                              where crs.courseName == c
                                              select crs;
                var queryGetUserCoursesToRemove = from usrcrs in db.user_courses
                                                  where usrcrs.courseName == c
                                                  select usrcrs;

                db.course_timeblocks.DeleteAllOnSubmit(queryGetTimeBlocksForRemove);
                db.courses.DeleteAllOnSubmit(queryGetCoursesToRemove);
                db.user_courses.DeleteAllOnSubmit(queryGetUserCoursesToRemove);
                listBox1.Items.Remove(c);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && listBox3.SelectedIndex != -1)
            {
                var instructorID = (from usr in db.users
                                    where usr.username == listBox3.SelectedItem.ToString()
                                    select usr.userID).FirstOrDefault();

                List<string> selectedItems = new List<string>();
                foreach (var c in listBox1.SelectedItems)
                    selectedItems.Add((string)c);

                foreach (var c in selectedItems)
                {
                    var queryGetCoursesToRemove = from crs in db.courses
                                                  where crs.courseName == c
                                                  select crs;
                    foreach (var d in queryGetCoursesToRemove)
                    {
                        d.instructor = instructorID;
                    }
                    listBox1.Items.Remove(c);
                }
            }
            else
            {
                MessageBox.Show("Please select both a course to transfer, and faculty to transfer it to.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0 || listBox2.Items.Count != 0)
            {
                MessageBox.Show("Not all students and classes are accounted for! Please finish reassignment.");
            }
            else
            {
                DialogResult = DialogResult.OK;
                db.SubmitChanges();
                this.Close();
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox3.Items.Count; i++)
            {
                if (listBox1.SelectedIndex != -1 && FacultyOverlap(listBox3.Items[i].ToString(), (string)listBox1.SelectedItem))
                {
                    facColor[i] = true;
                }
                else
                {
                    facColor[i] = false;
                }
            }
            listBox3.Refresh();
            //listBox3.DrawMode = DrawMode.OwnerDrawFixed;
            //listBox3.DrawItem += new DrawItemEventHandler(listBox3_DrawItem);
        }

        public static bool FacultyOverlap(string facUser, string courseToCheck)
        {
            bool result = false;
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            var queryFacTimes = from usr in db.users
                                join crs in db.courses
                                on usr.userID equals crs.instructor
                                join crstb in db.course_timeblocks
                                on crs.courseID equals crstb.courseID
                                where usr.username == facUser
                                select crstb.timeblock;
            var queryNewCourseTimes = from crs in db.courses
                                      join crstb in db.course_timeblocks
                                      on crs.courseID equals crstb.courseID
                                      where crs.courseName == courseToCheck
                                      select crstb.timeblock;
            //MessageBox.Show(TimeCheck.convertTime(queryNewCourseTimes.ToList()));

            foreach (var c in queryFacTimes)
            {
                if (TimeCheck.overlaps(TimeCheck.convertTime(queryNewCourseTimes.ToList()), TimeCheck.convertTime(queryFacTimes.ToList())))
                {
                    result = true;
                }
            }
            return result;
        }

        private void listBox3_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            Brush myBrush = Brushes.Black; //or whatever...
            if (facColor[e.Index])
                myBrush = Brushes.Red;
            else if (e.Index == listBox3.SelectedIndex)
                myBrush = Brushes.White;
            // Draw the current item text based on the current 
            // Font and the custom brush settings.
            //
            e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                  e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle 
            // around the selected item.
            //
            e.DrawFocusRectangle();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
