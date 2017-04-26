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
    public partial class Home : BaseForm
    {
        private Login login;
        private userstruct User;

        public Home()
        {
            InitializeComponent();
        }

        public Home(Login loginPage, userstruct person)
        {
            InitializeComponent();
            login = loginPage;
            User = person;
            if (User.type == "admin" || User.type == "manager")
            {
                //this.Height -= 45;
                button4.Visible = false;
                button4.Enabled = false;
                button5.Visible = false;
                button5.Enabled = false;
                button7.Show();
            }
            else if (User.type == "faculty")
            {
                button6.Show();
                button4.Visible = false;
                button4.Enabled = false;
            }
            else
            {
                button5.Visible = true;
                button5.Enabled = true;
                button4.Visible = false;
                button4.Enabled = false;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Classes screen = new Classes(this, User);
            screen.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            login.Close();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            login.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Schedule schedule = new Schedule(this);
            schedule.Show();
            this.Hide();
        }

        // Calculates GPA for student taking in total credits and course totals
        //public double calc_GPA(double total, double num)
        //{
        //    double solution = total / num;

        //    return solution;
        //}
        //// it takes a grade from history.in and returns it with the appropiate value. In addition it takes in course credits 
        //// and returns as a double
        //public double credit(string grade)
        //{
        //    double x = 0.0;
        //    if (grade.Equals("A"))
        //    {
        //        x = 4.0;
        //    }
        //    else if (grade.Equals("B-"))
        //    {
        //        x = 3.7;
        //    }
        //    else if (grade.Equals("B+"))
        //    {
        //        x = 3.3;
        //    }
        //    else if (grade.Equals("B"))
        //    {
        //        x = 3.0;
        //    }
        //    else if (grade.Equals("B-"))
        //    {
        //        x = 2.7;
        //    }
        //    else if (grade.Equals("C+"))
        //    {
        //        x = 2.3;
        //    }
        //    else if (grade.Equals("C"))
        //    {
        //        x = 2.0;
        //    }
        //    else if (grade.Equals("C-"))
        //    {
        //        x = 1.7;
        //    }
        //    else if (grade.Equals("D+"))
        //    {
        //        x = 1.3;
        //    }
        //    else if (grade.Equals("D "))
        //    {
        //        x = 1.0;
        //    }
        //    else if (grade.Equals("D-"))
        //    {
        //        x = 0.7;
        //    }
        //    else if (grade.Equals("F"))
        //    {
        //        x = 0.0;
        //    }
        //    else if (grade.Equals("WF"))
        //    {
        //        x = 0.0;
        //    }
        //    else if (grade.Equals("S"))
        //    {
        //        x = 0.0;
        //    }
        //    else if (grade.Equals("U"))
        //    {
        //        x = 0.0;
        //    }
        //    else
        //    {
        //        //x = Convert.ToDouble(grade);
        //        x = 0.0;
        //    }
        //    return x;
        //}

        private void button5_Click(object sender, EventArgs e)
        {


            //linqtoregdbDataContext db = new linqtoregdbDataContext();

            //var queryCourseHistory = from uc in db.user_courses
            //                         where uc.username == User.username
            //                         select uc;

            //var queryUserCourses = from uc in db.user_courses
            //                       join crs in db.courses on uc.courseID equals crs.courseID
            //                       where uc.username == User.username && uc.semester == "S14"
            //                       select crs;
            
                history form = new history(this, User, false);
                //double counter = 0.0;
                //double sum = 0.0;

                //foreach (var c in queryCourseHistory)
                //{
                //    form.dataGridView1.Rows.Add(c.courseName, c.semester, (decimal)c.courseCredit, c.grade);
                //    if (c.grade != "N" && c.grade != "S")
                //    {
                //        sum += credit(c.grade) * (double)c.courseCredit;
                //        counter += (double)c.courseCredit;
                //    }
                //}
                //form.dataGridView1.Rows.Add("", "", "", "Total GPA:", calc_GPA(sum, counter));


            //var queryGetCourses = from crs in db.courses
            //                      join 


            //System.IO.StreamReader history_db = new System.IO.StreamReader("classList.in");

            //string line;

            ////A while loop that goes through the entire file 
            //while ((line = history_db.ReadLine()) != null)
            //{


            //    //An if statement that checks the users name and displays the history form otherwise nothing happens
            //    if (line.Contains(User.username))
            //    {
            //        //a new form to display the users history is created
            //        //history form = new history(this);

            //        //a for loop that inserts student data into a table.
            //        for (int i = 0; i < Convert.ToInt32(line.Substring(11, 2)); i++)
            //        {

            //            form.dataGridView1.Rows.Add(line.Substring(14 + i * 24, 9), line.Substring(25 + i * 24, 3), line.Substring(29 + i * 24, 5), (line.Substring(34 + i * 24, 2) == "N ") ? " " : line.Substring(34 + i * 24, 2), (line.Substring(34 + i * 24, 2) == "N ") ? " " : Convert.ToString(credit(line.Substring(34 + i * 24, 2))));
            //            if (line.Substring(34 + i * 24, 2) != "N " && line.Substring(34 + i * 24, 2) != "S ")
            //            {
            //                sum += credit(line.Substring(34 + i * 24, 2)) * credit(line.Substring(29 + i * 24, 5));
            //                counter += credit(line.Substring(29 + i * 24, 5));
            //            }
            //        }


                    //form.dataGridView1.Rows.Add("", "", "", "Total GPA:", calc_GPA(sum, counter));
                    form.Show();
                    this.Hide();
            //    }
            //}
            //history_db.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FacultyHome facultyWindow = new FacultyHome(User, this);
            facultyWindow.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (User.type == "manager")
            {
                AdminWindow adminWindow = new AdminWindow(this, User);
                adminWindow.Show();
                this.Hide();
            }
            else
            {
                EditUsersForm limWindow = new EditUsersForm(this, User);
                limWindow.Show();
                this.Hide();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ChangePassword changePasswordWindow = new ChangePassword(User, this);
            this.Hide();
            changePasswordWindow.Show();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point((this.Width - panel1.Width) / 2-5, (this.Height - panel1.Height) / 2-15);
        }

        private void Home_Load(object sender, EventArgs e)
        {

        }

        private void Home_FormClosed(object sender, FormClosedEventArgs e)
        {
            login.Show();
        }
    }
}
