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

    public partial class AddClassForm : BaseForm
    {
        BaseForm prev;
        List<int> timeblocks = new List<int>();

        public AddClassForm(BaseForm prevWindow, userstruct inUser)
        {
            prev = prevWindow;
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            InitializeComponent();
            var queryAllFaculty = from usr in db.users
                                  where usr.usertype == "faculty"
                                  select usr;
            foreach (var c in queryAllFaculty)
            {
                comboBox1.Items.Add(c.username);
            }
        }

        private void AddClassForm_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            maskedTextBox1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            maskedTextBox2.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            int timeblockIn, startTime, endTime;
            if (maskedTextBox1.Text == "" || maskedTextBox2.Text == "")
            {
                MessageBox.Show("Please enter times into both the start and end time boxes.");
            }
            else
            {
                try
                {
                    startTime = Convert.ToInt32(maskedTextBox1.Text);
                    endTime = Convert.ToInt32(maskedTextBox2.Text);
                    if (startTime >= 2400 || endTime >= 2400 || !(startTime % 100 == 30 || startTime % 100 == 0) || !((endTime - startTime) % 100 == 0 || (endTime - startTime) % 100 == 30 || (endTime - startTime) % 100 == 70) || !(fridayCheckBox.Checked || mondayCheckBox.Checked || tuesdayCheckBox.Checked || wednesdayCheckBox.Checked || thursdayCheckBox.Checked) || startTime >= endTime)
                    {
                        MessageBox.Show("Improper input format! Make sure all times are in military time, and that classes begin and end on half hours, and that at least one day is selected.");
                    }
                    else if (endTime - startTime >= 500)
                    {
                        MessageBox.Show("Error. Classes may not go for more than 4 1/2 hours.");
                    }
                    else
                    {
                        timeblockIn = TimeCheck.ConvertToTime(startTime, endTime, mondayCheckBox.Checked, tuesdayCheckBox.Checked, wednesdayCheckBox.Checked, thursdayCheckBox.Checked, fridayCheckBox.Checked);
                        timeblocks.Add(timeblockIn);
                        listBox1.Items.Add(TimeShowFormat());

                        mondayCheckBox.Checked = false;
                        tuesdayCheckBox.Checked = false;
                        wednesdayCheckBox.Checked = false;
                        thursdayCheckBox.Checked = false;
                        fridayCheckBox.Checked = false;
                        maskedTextBox1.Text = "";
                        maskedTextBox2.Text = "";
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Improper input format! Make sure all times are in military time, and that classes begin and end on half hours, and that at least one day is selected.");
                }
            }
        }

        private void AddClassForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            prev.Show();
            this.Hide();
        }

        private string TimeShowFormat()
        {
            string outStr = "";
            if (mondayCheckBox.Checked)
                outStr += "M";
            if (tuesdayCheckBox.Checked)
                outStr += "T";
            if (wednesdayCheckBox.Checked)
                outStr += "W";
            if (thursdayCheckBox.Checked)
                outStr += "R";
            if (fridayCheckBox.Checked)
                outStr += "F";
            outStr += ", " + maskedTextBox1.Text + " - " + maskedTextBox2.Text + ".";
            return outStr;
        }
            
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                var selected = listBox1.SelectedIndex;
                timeblocks.RemoveAt(selected);
                listBox1.Items.RemoveAt(selected);
            }
            else
            {
                MessageBox.Show("Please select a time block to remove!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an instructor!");
            }
            else if (maskedTextBox3.Text == "")
            {
                MessageBox.Show("Please enter a course name.");
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("Please enter a course title.");
            }
            else if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("Please add at least one time block.");
            }
            else if (!maskedTextBox3.MaskCompleted)
            {
                MessageBox.Show("Course name not in acceptable format.");
            }
            else
            {
                string courseNameToAdd = maskedTextBox3.Text;
                if (courseNameToAdd[0] == ' ')
                {
                    courseNameToAdd = courseNameToAdd.Substring(1, courseNameToAdd.Length - 1);
                }
                try
                {
                    var queryFacID = (from usr in db.users
                                      where usr.username == comboBox1.Text
                                      select usr.userID).FirstOrDefault();
                    course addCourse = new course
                    {
                        instructor = queryFacID,
                        courseTitle = textBox2.Text,
                        courseName = courseNameToAdd,
                        numSeats = (int)numericUpDown2.Value,
                        courseCredit = numericUpDown1.Value,
                        semester = "S15"
                    };
                    db.courses.InsertOnSubmit(addCourse);
                    db.SubmitChanges();
                    var queryNewCourseID = (from crs in db.courses
                                            where crs.courseName == courseNameToAdd
                                            select crs.courseID).FirstOrDefault();

                    int numTimeBlocks = listBox1.Items.Count;
                    for (int i = 0; i < numTimeBlocks; i++)
                    {
                        course_timeblock crstb = new course_timeblock
                        {
                            courseID = queryNewCourseID,
                            timeblock = timeblocks[0]
                        };
                        db.course_timeblocks.InsertOnSubmit(crstb);
                        timeblocks.RemoveAt(0);
                        listBox1.Items.RemoveAt(0);
                    }
                    db.SubmitChanges();
                    MessageBox.Show("Course added successfully!");
                    this.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Error: Invalid course name.");
                }
                try
                {
                    foreach (var c in listBox2.Items)
                    {
                        db.prereqs.InsertOnSubmit(new prereq
                        {
                            courseName = courseNameToAdd,
                            prereqName = c.ToString()
                        });
                    }
                    db.SubmitChanges();
                    this.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Error adding prerequisites. Course added successfully. Edit prerequisites in the Edit Courses menu.");
                    this.Close();
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a course to add as a prerequisite!");
            }
            else
            {
                if (!listBox2.Items.Contains(listBox3.SelectedItem.ToString()))
                {
                    listBox2.Items.Add(listBox3.SelectedItem.ToString());
                }
                else
                {
                    MessageBox.Show("Error: Prerequisite is already required.");
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a prerequisite to remove!");
            }
            else
            {
                listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            }
        }

        private void AddClassForm_Load(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            listBox3.Items.AddRange((from crs in db.courses
                                     select crs.courseName).ToArray());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            prev.Show();
            this.Close();
        }
    }
}
