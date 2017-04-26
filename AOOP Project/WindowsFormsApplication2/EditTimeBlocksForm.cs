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
    public partial class EditTimeBlocksForm : BaseForm
    {
        BaseForm prev;
        List<int> timeblocks = new List<int>();
        string courseName;

        public EditTimeBlocksForm(BaseForm prevWindow, userstruct inUser, string courseNameIn)
        {
            InitializeComponent();
            prev = prevWindow;
            courseName = courseNameIn;

            foreach (var c in TimeCheck.GetTimes(courseNameIn))
            {
                timeblocks.Add(c);
                listBox1.Items.Add(TimeShowFormat(c));
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox2_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void thursdayCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void wednesdayCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tuesdayCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void mondayCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void fridayCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                linqtoregdbDataContext db = new linqtoregdbDataContext();

                var queryNewCourseID = (from crs in db.courses
                                        where crs.courseName == courseName
                                        select crs.courseID).FirstOrDefault();

                var queryTBToRemove = (from tbs in db.course_timeblocks
                                       where tbs.timeblock == timeblocks[listBox1.SelectedIndex] && tbs.courseID == queryNewCourseID
                                       select tbs).FirstOrDefault();
                db.course_timeblocks.DeleteOnSubmit(queryTBToRemove);
                var selected = listBox1.SelectedIndex;
                timeblocks.RemoveAt(selected);
                listBox1.Items.RemoveAt(selected);
                db.SubmitChanges();
            }
            else
            {
                MessageBox.Show("No course selected.");
            }

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
                        MessageBox.Show("Improper input format! Make sure all times are in military time, and that classes begin and end on half hours.");
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
                    MessageBox.Show("Improper input format! Make sure all times are in military time, and that classes begin and end on half hours.");
                }
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void EditTimeBlocksForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            prev.Show();
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

        private string TimeShowFormat(int timeIn)
        {
            string outFormat = "";
            int dateIn = timeIn / 1000;
            int startTimeIn = (timeIn / 10) % 100;
            int lengthIn = timeIn % 10;

            if (dateIn >= 16)
            {
                outFormat += 'F';
                dateIn -= 16;
            }
            if (dateIn >= 8)
            {
                outFormat = "R" + outFormat;
                dateIn -= 8;
            }
            if (dateIn >= 4)
            {
                outFormat = "W" + outFormat;
                dateIn -= 4;
            }
            if (dateIn >= 2)
            {
                outFormat = "T" + outFormat;
                dateIn -= 2;
            }
            if (dateIn == 1)
            {
                outFormat = "M" + outFormat;
            }

            int startTime = 0;
            bool halfHourStart = false;
            if (startTimeIn % 2 == 1)
            {
                startTime = 30;
                startTimeIn--;
                halfHourStart = true;
            }
            startTime += (startTimeIn / 2) * 100;

            int endTime = 0;
            if ((halfHourStart && (lengthIn % 2 == 0)) || (!halfHourStart && (lengthIn % 2 == 1)))
            {
                endTime = 30;
                lengthIn++;
            }
            endTime += (startTime/100)*100 + (lengthIn / 2) * 100;

            outFormat += ", " + startTime.ToString() + " - " + endTime.ToString() + ".";
            
            return outFormat;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryNewCourseID = (from crs in db.courses
                                    where crs.courseName == courseName
                                    select crs.courseID).FirstOrDefault();

            int numTimeBlocks = listBox1.Items.Count;
            for (int i = 0; i < numTimeBlocks; i++)
            {
                course_timeblock crstb = new course_timeblock
                {
                    courseID = queryNewCourseID,
                    timeblock = timeblocks[0]
                };
                if (((from tbs in db.course_timeblocks
                       where tbs.timeblock == timeblocks[0] && tbs.courseID == queryNewCourseID
                       select tbs).FirstOrDefault() == null))
                {
                    db.course_timeblocks.InsertOnSubmit(crstb);
                }
                timeblocks.RemoveAt(0);
                listBox1.Items.RemoveAt(0);
            }
            try
            {
                db.SubmitChanges();
                MessageBox.Show("Times updated successfully!");
            }
            catch (Exception)
            {
                MessageBox.Show("Database error while updating times.");
            }
            this.Close();
        }

    }
}
