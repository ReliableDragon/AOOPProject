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
    public partial class AdminWindow : BaseForm
    {
        private Form login;
        private userstruct User;

        public AdminWindow()
        {
            InitializeComponent();
        }

        public AdminWindow(Form loginPage, userstruct person)
        {
            InitializeComponent();
            login = loginPage;
            User = person;

        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void formClosed(object sender, FormClosedEventArgs e)
        {
            login.Close();
        }

        private void addUser_Click(object sender, EventArgs e)
        {
            AddUserForm aUF = new AddUserForm(this, User);
            aUF.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EditUsersForm eUF = new EditUsersForm(this, User);
            eUF.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddClassForm addForm = new AddClassForm(this, User);
            addForm.Show();
            this.Hide();
        }

        private void AdminWindow_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }

        private void AdminWindow_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            EditClassesForm editForm = new EditClassesForm(this, User);
            editForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FileHandlingForm handleForm = new FileHandlingForm(this, User);
            this.Hide();
            handleForm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            login.Show();
        }

    }

}
