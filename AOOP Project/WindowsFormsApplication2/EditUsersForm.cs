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
    public partial class EditUsersForm : BaseForm
    {
        private BaseForm adminWindow;
        private userstruct User;
        static AutoResetEvent autoEvent = new AutoResetEvent(false);
        bool editMode;
        int selectedUser;

        public EditUsersForm()
        {
            InitializeComponent();
        }

        public EditUsersForm(BaseForm aW, userstruct inUser)
        {
            InitializeComponent();
            adminWindow = aW;
            User = inUser;
            editMode = false;
            selectedUser = -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddUserForm aUF = new AddUserForm(this, User);
            aUF.Show();
            this.Hide();
            formLoad(sender, e);
        }

        private void formClosed(object sender, EventArgs e)
        {
            adminWindow.Show();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedUser = listBox1.SelectedIndex;
            if (listBox1.SelectedIndex != -1)
            {
                linqtoregdbDataContext db = new linqtoregdbDataContext();

                var queryGetUser = (from usr in db.users
                                    where usr.username == listBox1.SelectedItem.ToString()
                                    select usr).FirstOrDefault();
                textBox1.Text = queryGetUser.firstName;
                textBox3.Text = queryGetUser.lastName;
                if (queryGetUser.usertype == "student")
                {
                    textBox4.Show();
                    label4.Show();
                    button4.Show();
                    button6.Show();
                    button7.Hide();
                    textBox4.Text = (from usr in db.users
                                     where usr.userID == queryGetUser.advisor
                                     select usr.username).FirstOrDefault();
                }
                else if (queryGetUser.usertype == "faculty")
                {
                    button7.Show();
                    textBox4.Hide();
                    label4.Hide();
                    button6.Hide();
                    button4.Hide();
                }
                else
                {
                    button7.Hide();
                    textBox4.Hide();
                    label4.Hide();
                    button6.Hide();
                    button4.Hide();
                }
            }
        }

        private void formLoad(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            var queryAllUsers = from usr in db.users
                                orderby usr.username
                                select usr;
            foreach (var c in queryAllUsers)
            {
                listBox1.Items.Add(c.username);
            }
            listBox1.SelectedIndex = selectedUser;

            if (User.type == "admin")
            {
                button1.Hide();
                button2.Hide();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            if (listBox1.SelectedIndex == -1)
                MessageBox.Show("No user selected!");
            else
            {
                bool successfulDelete = true;
                string userBeingDeleted = (string)listBox1.SelectedItem;
                string confirmMessage = "Are you sure you want to delete " + userBeingDeleted + "?";
                DialogResult drslt = MessageBox.Show(confirmMessage, "Confirm Delete?", MessageBoxButtons.YesNo);
                if (drslt == DialogResult.Yes)
                {
                    user usrToDelete = (from usr in db.users
                                        where usr.username == (string)listBox1.SelectedItem
                                        select usr).FirstOrDefault();
                    listBox1.SelectedIndex = -1;
                    if (usrToDelete.usertype == "student")
                    {
                        var queryRegisteredCourses = from usrcrs in db.user_courses
                                                     where usrcrs.username == usrToDelete.username
                                                     select usrcrs;

                        foreach (var c in queryRegisteredCourses)
                        {
                            var queryToReturnSeats = (from crs in db.courses
                                                      where crs.courseName == c.courseName
                                                      select crs).FirstOrDefault();
                            queryToReturnSeats.numSeats += 1;
                        }

                        db.user_courses.DeleteAllOnSubmit(queryRegisteredCourses);
                        db.users.DeleteOnSubmit(usrToDelete);
                        db.SubmitChanges();
                    }
                    else if (usrToDelete.usertype == "faculty")
                    {
                        DeleteFacultyForm deletionForm = new DeleteFacultyForm(this, usrToDelete);
                        if (deletionForm.ShowDialog(this) != DialogResult.OK)
                        {
                            successfulDelete = false;
                        }
                        else
                        {
                            db.users.DeleteOnSubmit(usrToDelete);
                            db.SubmitChanges();
                        }
                    }
                    else
                    {
                        db.users.DeleteOnSubmit(usrToDelete);
                        db.SubmitChanges();
                    }

                    if (successfulDelete)
                    {
                        string successfulDeleteMessage = userBeingDeleted + " has been successfully deleted.";
                        MessageBox.Show(successfulDeleteMessage);
                        formLoad(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Deletion Cancelled.");
                    }
                }
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("No user selected!");
            }
            else
            {
                if (editMode)
                {
                    editMode = false;
                    textBox1.ReadOnly = true;
                    textBox3.ReadOnly = true;
                    button5.Text = "Edit User";
                    linqtoregdbDataContext db = new linqtoregdbDataContext();

                    var queryGetUser = (from usr in db.users
                                        where usr.username == listBox1.SelectedItem.ToString()
                                        select usr).FirstOrDefault();
                    if (queryGetUser.usertype == "student")
                    {
                        comboBox1.Visible = false;
                        textBox4.Visible = true;
                        textBox4.Text = comboBox1.SelectedItem.ToString();
                        queryGetUser.advisor = (from usr in db.users
                                                where usr.username == comboBox1.SelectedItem.ToString()
                                                select usr.userID).FirstOrDefault();
                    }
                    queryGetUser.firstName = textBox1.Text;
                    queryGetUser.lastName = textBox3.Text;

                    db.SubmitChanges();
                    
                }
                else
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                    editMode = true;
                    textBox1.ReadOnly = false;
                    textBox3.ReadOnly = false;
                    button5.Text = "Save Changes";
                    var queryGetUser = (from usr in db.users
                                        where usr.username == listBox1.SelectedItem.ToString()
                                        select usr).FirstOrDefault();

                    if (queryGetUser.usertype == "student")
                    {
                        textBox4.Visible = false;
                        comboBox1.Visible = true;
                        comboBox1.Items.Clear();
                        var queryGetInstructors = from usr in db.users
                                                  where usr.usertype == "faculty"
                                                  select usr.username;

                        foreach (var c in queryGetInstructors)
                        {
                            comboBox1.Items.Add(c);
                        }
                        comboBox1.SelectedItem = textBox4.Text;
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                userstruct userChangePass = new userstruct();
                userChangePass.username = listBox1.SelectedItem.ToString();

                ChangePassword passwordWindow = new ChangePassword(userChangePass, this, true);
                passwordWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Error - No user selected!");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            userstruct userAddClass = new userstruct();
            userAddClass.username = listBox1.SelectedItem.ToString();

            Classes classWindow = new Classes(this, userAddClass, true);
            classWindow.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            userstruct userViewHistory = new userstruct();
            userViewHistory.username = listBox1.SelectedItem.ToString();

            history historyForm = new history(this, userViewHistory, false);
            historyForm.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            userstruct facultyToView = new userstruct();
            facultyToView.username = listBox1.SelectedItem.ToString();

            FacultyHome facultyWindow = new FacultyHome(facultyToView, this, true);
            facultyWindow.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            adminWindow.Show();
            this.Close();
        }

    }

}
