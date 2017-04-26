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
    public partial class EditClassesForm : BaseForm
    {
        BaseForm prevWindow;
        userstruct currentUser;
        bool editMode;
        int selectedIndex;
        string currSemester;

        public EditClassesForm(BaseForm inWindow, userstruct inUser)
        {
            prevWindow = inWindow;
            currentUser = inUser;
            InitializeComponent();
            editMode = false;
            selectedIndex = -1;

            linqtoregdbDataContext db = new linqtoregdbDataContext();
            var queryGetInstructors = from usr in db.users
                                      where usr.usertype == "faculty"
                                      select usr;
            foreach (var c in queryGetInstructors)
            {
                comboBox1.Items.Add(c.username);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                MessageBox.Show("No course selected!");
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete " + textBox1.Text + "?", "Confirm Deletion", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                    string selectedCourse = listBox1.SelectedItem.ToString();

                    if ((from prq in db.prereqs
                         where prq.prereqName == selectedCourse
                         select prq).FirstOrDefault() != null)
                    {
                        if (MessageBox.Show("Note - class is a prerequisite, so it cannot be removed from the database. Change semester to prevent signup instead?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            var queryGetCoursesToRemoveEdit = (from crs in db.courses
                                                               where crs.courseName == selectedCourse && crs.semester == maskedTextBox1.Text
                                                               select crs).FirstOrDefault();
                            var queryGetUserCoursesToRemoveEdit = from usrcrs in db.user_courses
                                                                  where usrcrs.courseName == selectedCourse
                                                                  select usrcrs;

                            db.user_courses.DeleteAllOnSubmit(queryGetUserCoursesToRemoveEdit);
                            db.SubmitChanges();
                            queryGetCoursesToRemoveEdit.semester = "F00";
                            db.SubmitChanges();

                            listBox1.SelectedIndex = -1;
                            listBox1.Refresh();
                        }
                        else
                        {
                            MessageBox.Show("Course not removed.");
                        }
                    }
                    else
                    {
                        db.prereqs.DeleteAllOnSubmit((from prq in db.prereqs
                                                      where prq.courseName == selectedCourse
                                                      select prq));
                        db.SubmitChanges();

                        var queryGetTimeBlocksForRemove = from crstb in db.course_timeblocks
                                                          join crs in db.courses
                                                          on crstb.courseID equals crs.courseID
                                                          where crs.courseName == selectedCourse && crs.semester == maskedTextBox1.Text
                                                          select crstb;
                        var queryGetCoursesToRemove = from crs in db.courses
                                                      where crs.courseName == selectedCourse && crs.semester == maskedTextBox1.Text
                                                      select crs;
                        var queryGetUserCoursesToRemove = from usrcrs in db.user_courses
                                                          where usrcrs.courseName == selectedCourse && usrcrs.semester == maskedTextBox1.Text
                                                          select usrcrs;

                        db.user_courses.DeleteAllOnSubmit(queryGetUserCoursesToRemove);
                        db.SubmitChanges();
                        db.course_timeblocks.DeleteAllOnSubmit(queryGetTimeBlocksForRemove);
                        db.SubmitChanges();
                        db.courses.DeleteAllOnSubmit(queryGetCoursesToRemove);
                        db.SubmitChanges();

                        listBox1.SelectedIndex = -1;
                        listBox1.Items.Remove(selectedCourse);
                        listBox1.Refresh();
                    }
                }
            }
        }

        private void EditClassesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            prevWindow.Show();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            listBox5.Items.Clear();
            maskedTextBox1.Text = "";

            selectedIndex = listBox1.SelectedIndex;
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            if (listBox1.SelectedIndex != -1)
            {
                listBox2.Items.AddRange((from prq in db.prereqs
                                         where prq.courseName == listBox1.SelectedItem.ToString()
                                         select prq.prereqName).ToArray());
                listBox3.Items.AddRange((from crs in db.courses
                                         select crs.courseName).ToArray());

                var queryGetCourse = (from crs in db.courses
                                      where crs.courseName == listBox1.SelectedItem.ToString()
                                      select crs).FirstOrDefault();
                textBox1.Text = queryGetCourse.courseTitle;
                textBox3.Text = queryGetCourse.numSeats.ToString();
                textBox4.Text = queryGetCourse.courseCredit.ToString();
                maskedTextBox1.Text = queryGetCourse.semester;
                currSemester = queryGetCourse.semester;
                var queryGetInstructor = (from usr in db.users
                                          where usr.userID == queryGetCourse.instructor
                                          select usr.username).FirstOrDefault();
                textBox2.Text = queryGetInstructor;

                richTextBox1.Text = "";
                foreach (var c in TimeCheck.GetTimes(listBox1.SelectedItem.ToString()))
                {
                    foreach (var d in TimeCheck.convertTime(c))
                    {
                        richTextBox1.Text += d.ToString() + "\n";
                    }
                }
                listBox4.Items.AddRange((from usrcrs in db.user_courses
                                         where usrcrs.courseName == listBox1.SelectedItem.ToString()
                                         select usrcrs.username).ToArray());
                var queryGetUsersNotInClass = from usr in db.users
                                              where usr.usertype == "student"
                                              select usr.username;
                foreach (var c in queryGetUsersNotInClass)
                {
                    if (!listBox4.Items.Contains(c))
                    {
                        listBox5.Items.Add(c);
                    }
                }
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                richTextBox1.Text = "";
                listBox4.Items.Clear();
                listBox5.Items.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddClassForm addForm = new AddClassForm(this, currentUser);
            addForm.Show();
            this.Hide();
        }

        private void EditClassesForm_VisibleChanged(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            listBox1.Items.Clear();
            var queryAllCourses = from crs in db.courses
                                  orderby crs.courseName
                                  select crs.courseName;
            foreach (var c in queryAllCourses)
            {
                listBox1.Items.Add(c);
            }
            listBox1.SelectedIndex = selectedIndex;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                MessageBox.Show("No course selected!");
            else
            {
                if (!editMode)
                {
                    editMode = true;
                    textBox1.ReadOnly = false;
                    maskedTextBox1.ReadOnly = false;
                    textBox2.Hide();
                    comboBox1.Show();
                    textBox3.Hide();
                    numericUpDown1.Show();
                    textBox4.Hide();
                    numericUpDown2.Show();
                    button4.Visible = true;
                    button7.Visible = false;
                    button3.Text = "Save Changes";
                    comboBox1.SelectedItem = textBox2.Text;
                    listBox1.Enabled = false;
                    button1.Hide();
                    button2.Hide();
                    groupBox2.Hide();
                    groupBox1.Show();

                    linqtoregdbDataContext db = new linqtoregdbDataContext();

                    var queryGetCourse = (from crs in db.courses
                                          where crs.courseName == listBox1.SelectedItem.ToString()
                                          select crs).FirstOrDefault();
                    numericUpDown1.Value = queryGetCourse.courseCredit;
                    numericUpDown2.Value = queryGetCourse.numSeats;
                }
                else
                {
                    if (textBox1.Text == "")
                    {
                        MessageBox.Show("Class title cannot be blank.");
                    }
                    else if (maskedTextBox1.Text.Length != 3)
                    {
                        MessageBox.Show("Improper semester.");
                    }
                    else
                    {
                        linqtoregdbDataContext db = new linqtoregdbDataContext();

                        if (maskedTextBox1.Text != currSemester && listBox4.Items.Count > 0)
                        {
                            if (MessageBox.Show("Are you sure you want to change the semester? Student registrations for this class will be removed.", "Confirm Semester Change", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                var numSeatsFreed = (from usrcrs in db.user_courses
                                                   where usrcrs.courseName == listBox1.SelectedItem.ToString()
                                                   select usrcrs).Count();
                                var queryCourseSeatsChanged = (from crs in db.courses
                                                               where crs.courseName == listBox1.SelectedItem.ToString()
                                                               select crs).FirstOrDefault();

                                db.user_courses.DeleteAllOnSubmit((from usrcrs in db.user_courses
                                                                   where usrcrs.courseName == listBox1.SelectedItem.ToString()
                                                                   select usrcrs));
                                queryCourseSeatsChanged.numSeats += numSeatsFreed;
                                numericUpDown2.Value += numSeatsFreed;
                                db.SubmitChanges();
                            }
                            else
                            {
                                maskedTextBox1.Text = "S15";
                            }
                        }
                        editMode = false;
                        textBox1.ReadOnly = true;
                        maskedTextBox1.ReadOnly = true;
                        comboBox1.Hide();
                        textBox2.Show();
                        numericUpDown1.Hide();
                        numericUpDown2.Hide();
                        textBox3.Show();
                        textBox4.Show();
                        button4.Visible = false;
                        button7.Visible = true;
                        button3.Text = "Edit Course";
                        listBox1.Enabled = true;
                        button1.Show();
                        button2.Show();
                        button7.Show();
                        groupBox1.Hide(); 
                        groupBox2.Show();

                        var queryGetInstructorID = (from usr in db.users
                                                    where usr.username == comboBox1.SelectedItem.ToString()
                                                    select usr.userID).FirstOrDefault();
                        var queryCourseToChange = (from crs in db.courses
                                                   where crs.courseName == listBox1.SelectedItem.ToString()
                                                   select crs).FirstOrDefault();

                        queryCourseToChange.numSeats = (int)numericUpDown2.Value;
                        queryCourseToChange.courseCredit = numericUpDown1.Value;
                        queryCourseToChange.courseTitle = textBox1.Text;
                        queryCourseToChange.courseName = listBox1.SelectedItem.ToString();
                        queryCourseToChange.instructor = queryGetInstructorID;
                        queryCourseToChange.semester = maskedTextBox1.Text;

                        db.SubmitChanges();

                        textBox2.Text = comboBox1.Text;
                        textBox3.Text = numericUpDown2.Value.ToString();
                        textBox4.Text = numericUpDown1.Value.ToString();

                        currSemester = maskedTextBox1.Text;
                        listBox1_SelectedIndexChanged(listBox1, e);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            EditTimeBlocksForm editForm = new EditTimeBlocksForm(this, currentUser, listBox1.SelectedItem.ToString());
            button3.PerformClick();
            editForm.Show();
            this.Hide();
        }

        private void EditClassesForm_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a prerequisite to remove!");
            }
            else
            {
                try
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                    db.prereqs.DeleteOnSubmit((from prq in db.prereqs
                                               where prq.courseName == listBox1.SelectedItem.ToString() && prq.prereqName == listBox2.SelectedItem.ToString()
                                               select prq).FirstOrDefault());
                    db.SubmitChanges();
                    listBox2.Items.Clear();
                    listBox2.Items.AddRange((from prq in db.prereqs
                                             where prq.courseName == listBox1.SelectedItem.ToString()
                                             select prq.prereqName).ToArray());
                }
                catch (Exception)
                {
                    MessageBox.Show("Error removing prerequisite!");
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
                try
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                    db.prereqs.InsertOnSubmit(new prereq
                    {
                        courseName = listBox1.SelectedItem.ToString(),
                        prereqName = listBox3.SelectedItem.ToString()
                    });
                    db.SubmitChanges();
                    listBox2.Items.Clear();
                    listBox2.Items.AddRange((from prq in db.prereqs
                                             where prq.courseName == listBox1.SelectedItem.ToString()
                                             select prq.prereqName).ToArray());
                }
                catch (Exception)
                {
                    MessageBox.Show("Error adding prerequisite. Make sure you aren't inserting a duplicate.");
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            prevWindow.Show();
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox5.SelectedIndex != -1)
            {
                if (maskedTextBox1.Text != "S15")
                {
                    MessageBox.Show("Error - Cannot add students to courses not offered in the next semester.");
                }
                else
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                    listBox4.Items.Add(listBox5.SelectedItem.ToString());
                    db.user_courses.InsertOnSubmit(new user_course
                    {
                        courseName = listBox1.SelectedItem.ToString(),
                        username = listBox5.SelectedItem.ToString(),
                        semester = maskedTextBox1.Text,
                        courseCredit = numericUpDown1.Value,
                        grade = "N",
                        courseID = (from crs in db.courses
                                    where crs.courseName == listBox1.SelectedItem.ToString() && crs.semester == maskedTextBox1.Text
                                    select crs.courseID).FirstOrDefault()
                    });
                    (from crs in db.courses
                     where crs.courseName == listBox1.SelectedItem.ToString() && crs.semester == maskedTextBox1.Text
                     select crs).FirstOrDefault().numSeats -= 1;
                    textBox3.Text = (Convert.ToInt32(textBox3.Text) - 1).ToString();
                    listBox5.Items.Remove(listBox5.SelectedItem);
                    db.SubmitChanges();
                }
            }
            else
            {
                MessageBox.Show("Please select a student to add.");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex != -1)
            {
                linqtoregdbDataContext db = new linqtoregdbDataContext();
                db.user_courses.DeleteOnSubmit((from usrcrs in db.user_courses
                                                where usrcrs.courseName == listBox1.SelectedItem.ToString() && usrcrs.username == listBox4.SelectedItem.ToString() && usrcrs.semester == maskedTextBox1.Text
                                                select usrcrs).FirstOrDefault());
                textBox3.Text = (Convert.ToInt32(textBox3.Text) + 1).ToString();
                (from crs in db.courses
                 where crs.courseName == listBox1.SelectedItem.ToString() && crs.semester == maskedTextBox1.Text
                 select crs).FirstOrDefault().numSeats += 1;
                listBox5.Items.Add(listBox4.SelectedItem.ToString());
                listBox4.Items.Remove(listBox4.SelectedItem);
                db.SubmitChanges();

                //listBox4.Items.AddRange((from usrcrs in db.user_courses
                //                         where usrcrs.courseName == listBox1.SelectedItem.ToString()
                //                         select usrcrs.username).ToArray());
                //var queryGetUsersNotInClass = from usr in db.users
                //                              select usr.username;
                //foreach (var c in queryGetUsersNotInClass)
                //{
                //    if (!listBox4.Items.Contains(c))
                //    {
                //        listBox5.Items.Add(c);
                //    }
                //}
            }
            else
            {
                MessageBox.Show("Please select a student to remove.");
            }
        }
    }
}
