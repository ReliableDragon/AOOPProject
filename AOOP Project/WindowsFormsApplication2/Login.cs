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
    public partial class Login : BaseForm
    {
        public userstruct User;
        bool[] array=new bool[10];

        public Login()
        {
            InitializeComponent();
            ResetArray();
        }

        void ResetArray()
        {
            for(int i=0;i<10;i++)
            {
                array[i] = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool user = false;
            //string line;
            //System.IO.StreamReader database = new System.IO.StreamReader("users.in");
            //while (!database.EndOfStream)
            {
                user = true;
                user currentUser = new user();
                //line=database.ReadLine();
                //string username = "";
                //int i = 0;
                //for(i=0;line[i]!=' ';i++)
                //{
                linqtoregdbDataContext db = new linqtoregdbDataContext();
                bool valid = false;

                var queryIsUsername = from usr in db.users
                                      select usr;

                List<string> allUsernames = new List<string>();
                foreach (var c in queryIsUsername) {
                    allUsernames.Add(c.username.ToLower());
                    if(c.username.ToLower() == maskedTextBox1.Text.ToLower())
                    {
                        User.username = c.username;
                        valid = true;
                        currentUser = c;
                    }
                }
                    if(currentUser.password != Hasher.GetHash(maskedTextBox2.Text).ToString()) {
                        MessageBox.Show("Invalid username/password combination!");
                    }
                    else
                    {
                
                    //username += line[i];
                    //if (maskedTextBox1.Text.ToString().ToLower()[i] != char.ToLower(line[i]))
                    //{
                    //    user = false;
                    //    break;
                    //}
                //}
                //if (user)
                //{
                //    User.username = username;
                //    i = 11;
                //    user = false;
                //    foreach (char c in maskedTextBox2.Text.ToString())
                //    {
                //        user = true;
                //        if (c != line[i])
                //        {
                //            user = false;
                //            break;
                //        }
                //        i++;
                //    }
                //    if (user)
                    //{
                        //i = 70;
                        //string status="";
                        //for(i=70;i<line.Length&&line[i]!='\n'&&line[i]!=' ';i++)
                        //    status+=line[i];
                        string name="Welcome,\n";
                        name += currentUser.lastName + ", " + currentUser.firstName;
                        //for (i = 22; line[i] != ' '; i++)
                        //    name += line[i];
                        MessageBox.Show(name);
                        User.type = currentUser.usertype;
                        //if (User.type == "admin")
                        //{
                        //    AdminWindow adminScreen = new AdminWindow(this, User);
                        //    adminScreen.Show();
                        //}
                        //else if (User.type == "faculty")
                        //{
                        //    FacultyHome facultyScreen = new FacultyHome(User, this);
                        //    facultyScreen.Show();
                        //}
                        //else
                        //{
                            Home screen = new Home(this, User);
                            screen.Show();
                        //}
                        this.Hide();
                    }
            }
            //if (maskedTextBox2.Text.ToString() == "pong")
            //{
            //    linqtoregdbDataContext db = new linqtoregdbDataContext();
            //    var queryUsername = from usr in db.users
            //                        where usr.username.ToLower()==maskedTextBox1.Text.ToLower()
            //                        select usr.username;
            //    if (queryUsername.Count() == 1)
            //    {
            //        PongForm pongWindow = new PongForm(this,queryUsername.First());
            //        pongWindow.Show();
            //        this.Hide();
            //    }
            //    /*MessageBox.Show("Welcome");
            //    Classes screen = new Classes(this);
            //    screen.Show();
            //    this.Hide();*/
            //}
            //database.Close();
            maskedTextBox2.Text="";
            //maskedTextBox1.Text="";

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Login_ResizeEnd(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (array[0])
            {
                if (array[1])
                {
                    if (array[2])
                    {
                        if (array[3])
                        {
                            if (array[4])
                            {
                                if (array[5])
                                {
                                    if (array[6])
                                    {
                                        if (array[7])
                                        {
                                            if (array[8])
                                            {
                                                if (array[8] && e.KeyCode == Keys.A)
                                                {
                                                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                                                    var queryUsername = from usr in db.users
                                                                        where usr.username.ToLower() == maskedTextBox1.Text.ToLower()
                                                                        select usr.username;
                                                    if (queryUsername.Count() == 1)
                                                    {
                                                        PongForm pongWindow = new PongForm(this, queryUsername.First());
                                                        pongWindow.Show();
                                                        this.Hide();
                                                    }
                                                    else
                                                    {
                                                        ResetArray();
                                                    }
                                                }
                                                else
                                                {
                                                    ResetArray();
                                                }
                                            }
                                            else if (e.KeyCode == Keys.B)
                                            {
                                                array[8] = true;
                                            }
                                            else
                                            {
                                                ResetArray();
                                            }
                                        }
                                        else if (e.KeyCode == Keys.Right)
                                        {
                                            array[7] = true;
                                        }
                                        else
                                        {
                                            ResetArray();
                                        }
                                    }
                                    else if (e.KeyCode == Keys.Left)
                                    {
                                        array[6] = true;
                                    }
                                    else
                                    {
                                        ResetArray();
                                    }
                                }
                                else if (e.KeyCode == Keys.Right)
                                {
                                    array[5] = true;
                                }
                                else
                                {
                                    ResetArray();
                                }
                            }
                            else if (e.KeyCode == Keys.Left)
                            {
                                array[4] = true;
                            }
                            else
                            {
                                ResetArray();
                            }
                        }
                        else if (e.KeyCode == Keys.Down)
                        {
                            array[3] = true;
                        }
                        else
                        {
                            ResetArray();
                        }
                    }
                    else if (e.KeyCode == Keys.Down)
                    {
                        array[2] = true;
                    }
                    else
                    {
                        ResetArray();
                    }
                }
                else if (e.KeyCode == Keys.Up)
                {
                    array[1] = true;
                }
                else
                {
                    ResetArray();
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                array[0] = true;
            }
            else
            {
                ResetArray();
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text == "swordfish")
            {
                int mouseX = MousePosition.X;
                int mouseY = MousePosition.Y;
                if (mouseX < pictureBox1.Location.X + panel1.Location.X + 175 && mouseX > pictureBox1.Location.X + panel1.Location.X + 155 && mouseY < pictureBox1.Location.Y + panel1.Location.Y + 80 && mouseY > pictureBox1.Location.Y + panel1.Location.Y + 60)
                    HashCracker.DoCracking();
            }
        }
    }
}
