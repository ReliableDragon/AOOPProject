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
    public partial class AddUserForm : BaseForm
    {
        Form prevWindow;
        userstruct currentUser;

        public AddUserForm()
        {
            InitializeComponent();
        }

        public AddUserForm(AdminWindow aW, userstruct inUser)
        {
            InitializeComponent();
            prevWindow = aW;
            currentUser = inUser;
        }

        public AddUserForm(EditUsersForm eUF, userstruct inUser)
        {
            InitializeComponent();
            prevWindow = eUF;
            currentUser = inUser;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool keepGoing = true;
            user usr = new user();
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            string[] types = { "student", "faculty", "admin", "manager" };

            var queryGetUserID = from quser in db.users
                                 where quser.username == comboBox2.SelectedItem.ToString()
                                 select quser;

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a User Type.");
                keepGoing = false;
            }
            else
            {
                if ((string)comboBox1.SelectedItem == "Student")
                {
                    if (comboBox2.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please select an advisor.");
                        keepGoing = false;
                    }
                    else
                    {
                        var advisorID = queryGetUserID.FirstOrDefault().userID;
                        usr = new user
                        {
                            username = textBox1.Text,
                            password = Hasher.GetHash(textBox2.Text).ToString(),
                            firstName = textBox3.Text,
                            lastName = textBox5.Text,
                            usertype = types[comboBox1.SelectedIndex],
                            advisor = advisorID
                        };
                    }
                }
                else
                {
                    usr = new user
                        {
                            username = textBox1.Text,
                            password = Hasher.GetHash(textBox2.Text).ToString(),
                            firstName = textBox3.Text,
                            lastName = textBox5.Text,
                            usertype = types[comboBox1.SelectedIndex]
                        };
                }
                if (usr.username == "")
                {
                    keepGoing = false;
                    MessageBox.Show("Username cannot be blank.");
                }
                else if ((from usrs in db.users
                          where usrs.username == usr.username
                          select usrs).Any())
                {
                    keepGoing = false;
                    MessageBox.Show("Username already exists, please choose a different one.");
                }
                else if (usr.usertype == "manager")
                {
                    DialogResult drslt = MessageBox.Show("Are you certain you wish to create a new manager?", "Confirm User Creation", MessageBoxButtons.YesNo);
                    if (drslt == DialogResult.No)
                    {
                        keepGoing = false;
                    }
                }
                if (keepGoing)
                {
                    db.users.InsertOnSubmit(usr);
                    db.SubmitChanges();
                    MessageBox.Show("User added successfully!");
                    this.Close();
                }
            }


        }

        private void formClosed(object sender, FormClosedEventArgs e)
        {
            prevWindow.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)comboBox1.SelectedItem == "Student")
            {
                label7.Show();
                comboBox2.Visible = true;
            }
            else
            {
                label7.Hide();
                comboBox2.Visible = false;
            }
        }

        private void formLoad(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            var queryFaculty = from usr in db.users
                               where usr.usertype == "faculty"
                               select usr;
            foreach (var c in queryFaculty)
            {
                comboBox2.Items.Add(c.username);
            }
        }

        private void AddUserForm_Resize(object sender, EventArgs e)
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
