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
    public partial class ChangePassword : BaseForm
    {
        private userstruct User;
        Form previous;
        bool admin = false;

        public ChangePassword(userstruct currentUser,Form form)
        {
            InitializeComponent();
            previous = form;
            User = currentUser;
            
        }

        public ChangePassword(userstruct currentUser, Form form, bool administrator)
        {
            InitializeComponent();
            previous = form;
            User = currentUser;
            admin = administrator;
            if (admin)
            {
                textBox1.Hide();
                label1.Text = "Changing the password for user " + User.username;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var getUser = from usr in db.users
                          where usr.username == User.username
                          select usr;
            var currentUser = getUser.First(x => true);

            if(textBox2.Text!=textBox3.Text)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (currentUser.password == Hasher.GetHash(textBox1.Text).ToString()||admin)
            {
                currentUser.password = Hasher.GetHash(textBox2.Text).ToString();
                db.SubmitChanges();
                MessageBox.Show("Password changed successfully");
                this.Close();
            }
            else
            {
                MessageBox.Show("Current password incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //label1.Text = "";
            //label2.Text = "";
            //label3.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void ChangePassword_FormClosed(object sender, FormClosedEventArgs e)
        {
            previous.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }
    }
}
