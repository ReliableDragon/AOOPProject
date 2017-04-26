using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication2
{
    public partial class FileHandlingForm : BaseForm
    {
        BaseForm prevWindow;

        public FileHandlingForm(BaseForm prev, userstruct inUser)
        {
            InitializeComponent();
            prevWindow = prev;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = openFileDialog.FileName;
            }
        }

        private void FileHandlingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            prevWindow.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.StreamReader checkFile = new System.IO.StreamReader(textBox1.Text);
                switch (comboBox1.SelectedIndex)
                {
                    case -1: MessageBox.Show("Please select input type for file.");
                        break;
                    case 0:
                        AddUsers();
                        break;
                    case 1:
                        AddCourses();
                        break;
                    case 2:
                        AddCourseHistories();
                        break;
                    case 3:
                        AddPrereqs();
                        break;
                    default:
                        MessageBox.Show("How did you even do that?");
                        break;
                }
            }
            catch (System.IO.FileLoadException)
            {
                MessageBox.Show("Cannot access file!");
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("File does not exist!");
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("File path cannot be blank!");
            }
        }

        public static string TrimSpaces(string inString)
        {
            while (inString[inString.Length - 1] == ' ')
                inString = inString.Substring(0, inString.Length - 1);
            return inString;
        }

        private void AddUsers()
        {
            System.IO.StreamReader inFile = new System.IO.StreamReader(textBox1.Text);
            int lineNum = 0;
            int numInserted = 0;
            string inLine;
            Regex reg = new Regex(@"[a-zA-Z ]{10}\ [a-zA-Z ]{10}\ [a-zA-Z ]{15}\ [a-zA-Z ]{15}\ [a-zA-Z ]{15}\ [a-zA-Z ]{10}");
            bool keepGoing = true;

            while (!inFile.EndOfStream && keepGoing)
            {
                inLine = inFile.ReadLine();
                lineNum++;
                if (!reg.IsMatch(inLine))
                {
                    DialogResult result = MessageBox.Show("Error - inproper input format on line " + lineNum.ToString() + ". Continue?", "Error", MessageBoxButtons.YesNo);
                    if (result == System.Windows.Forms.DialogResult.No)
                        keepGoing = false;
                }
                else
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                    string usernameIn, firstNameIn, lastNameIn, passwordIn, statusIn;
                    usernameIn = TrimSpaces(inLine.Substring(0, 10));
                    passwordIn = Hasher.GetHash(TrimSpaces(inLine.Substring(11, 10))).ToString();
                    firstNameIn = TrimSpaces(inLine.Substring(22, 15));
                    lastNameIn = TrimSpaces(inLine.Substring(54, 15));
                    statusIn = TrimSpaces(inLine.Substring(70, 10));

                    var queryGetAdvisor = (from usr in db.users
                                           where usr.username == statusIn
                                           select usr.userID).FirstOrDefault();
                    user newUser = new user
                    {
                        username = usernameIn,
                        password = passwordIn,
                        firstName = firstNameIn,
                        lastName = lastNameIn,
                        advisor = (queryGetAdvisor == default(int)) ? null : (int?)queryGetAdvisor,
                        usertype = (queryGetAdvisor == default(int)) ? "student" : statusIn

                    };
                    try
                    {
                        db.users.InsertOnSubmit(newUser);
                        db.SubmitChanges();
                        numInserted++;
                    }
                    catch (Exception)
                    {
                        DialogResult result = MessageBox.Show("Error - invalid user insertion attempted. Skip user and continue?", "Error", MessageBoxButtons.YesNo);
                        if (result == System.Windows.Forms.DialogResult.No)
                            keepGoing = false;
                    }

                }
            }
            MessageBox.Show(numInserted.ToString() + " users inserted successfully.");
            inFile.Close();
        }

        private void AddCourses()
        {
            System.IO.StreamReader inFile = new System.IO.StreamReader(textBox1.Text);
            int lineNum = 0;
            int numInserted = 0;
            int tbsInserted = 0;
            string inLine;
            Regex reg = new Regex(@"[SF]\d{2}\ [A-Z]{2,3}\-\d{3}\-\d{2}\ {1,2}[a-zA-z ]{15}\ [a-zA-z ]{10}\ \d\.\d{2}\ \d{1,3}\ {1,3}\d(\ \d{5})+");
            bool keepGoing = true;

            while (!inFile.EndOfStream && keepGoing)
            {
                inLine = inFile.ReadLine();
                lineNum++;
                if (!reg.IsMatch(inLine))
                {
                    DialogResult result = MessageBox.Show("Error - inproper input format on line " + lineNum.ToString() + ". Continue?", "Error", MessageBoxButtons.YesNo);
                    if (result == System.Windows.Forms.DialogResult.No)
                        keepGoing = false;
                }
                else
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                    string semesterIn, courseNameIn, courseTitleIn, instructor;
                    decimal courseCreditIn;
                    int numSeatsIn, numTBs;
                    List<int> timeBlocksIn = new List<int>();

                    semesterIn = inLine.Substring(0, 3);
                    courseNameIn = TrimSpaces(inLine.Substring(4, 10));
                    courseTitleIn = TrimSpaces(inLine.Substring(15, 15));
                    instructor = TrimSpaces(inLine.Substring(31, 10));
                    courseCreditIn = Convert.ToDecimal(inLine.Substring(42, 4));
                    numSeatsIn = Convert.ToInt32(TrimSpaces(inLine.Substring(47, 3)));
                    numTBs = (int)char.GetNumericValue(inLine[51]);

                    for (int i = 0; i < numTBs; i++)
                    {
                        timeBlocksIn.Add(Convert.ToInt32(inLine.Substring(53 + i*6, 5)));
                    }

                    var queryGetInstructorID = (from usr in db.users
                                           where usr.username == instructor
                                           select usr.userID).FirstOrDefault();
                    course newCourse = new course
                    {
                        semester = semesterIn,
                        courseName = courseNameIn,
                        courseTitle = courseTitleIn,
                        instructor = queryGetInstructorID,
                        courseCredit = courseCreditIn,
                        numSeats = numSeatsIn
                    };
                    try
                    {
                        db.courses.InsertOnSubmit(newCourse);
                        db.SubmitChanges();
                        var queryGetCourseID = (from crs in db.courses
                                               where crs.courseName == courseNameIn
                                               select crs.courseID).FirstOrDefault();
                        foreach (var c in timeBlocksIn)
                        {
                            course_timeblock newCourseTimeblock = new course_timeblock
                            {
                                courseID = queryGetCourseID,
                                timeblock = c
                            };
                            db.course_timeblocks.InsertOnSubmit(newCourseTimeblock);
                            tbsInserted++;
                        }
                        db.SubmitChanges();
                        numInserted++;
                    }
                    catch (Exception)
                    {
                        DialogResult result = MessageBox.Show("Error - invalid course insertion attempted. Skip user and continue?", "Error", MessageBoxButtons.YesNo);
                        if (result == System.Windows.Forms.DialogResult.No)
                            keepGoing = false;
                    }


                }
            }
            MessageBox.Show(numInserted.ToString() + " courses with " + tbsInserted.ToString() + " total timeblocks inserted successfully.");
            inFile.Close();
        }

        private void AddPrereqs()
        {
            System.IO.StreamReader inFile = new System.IO.StreamReader(textBox1.Text);
            int lineNum = 0;
            int numInserted = 0;
            int numSkipped = 0;
            string inLine;
            Regex reg = new Regex(@"[a-zA-Z ]{2,3}\-\d{3}\ ?\ \d{1,2}(\ \ ?[a-zA-Z ]{2,3}\-\d{3})+");
            bool keepGoing = true;

            while (!inFile.EndOfStream && keepGoing)
            {
                inLine = inFile.ReadLine();
                lineNum++;
                if (!reg.IsMatch(inLine))
                {
                    DialogResult result = MessageBox.Show("Error - inproper input format on line " + lineNum.ToString() + ". Continue?", "Error", MessageBoxButtons.YesNo);
                    if (result == System.Windows.Forms.DialogResult.No)
                        keepGoing = false;
                }
                else
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                    string inCourseName = TrimSpaces(inLine.Substring(0, 7));
                    int howManyPrereqs = Convert.ToInt32(TrimSpaces(inLine.Substring(8, 2)));
                    List<string> inPrereqNames = new List<string>();
                    for (int i = 0; i < howManyPrereqs; i++)
                    {
                        inPrereqNames.Add(TrimSpaces(inLine.Substring(11 + 7 * i, 7)));
                    }
                    foreach (var c in inPrereqNames)
                    {
                        prereq inPrereq = new prereq
                        {
                            courseName = inCourseName + "-00",
                            prereqName = c + "-00"
                        };
                        try
                        {
                            db.prereqs.InsertOnSubmit(inPrereq);
                            db.SubmitChanges();
                            numInserted++;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            if (e.Number == 547)
                            {
                                DialogResult result = MessageBox.Show("Error - attempt to insert prerequisites for nonexistant class. Skip line and continue?", "Error", MessageBoxButtons.YesNo);
                                if (result == System.Windows.Forms.DialogResult.No)
                                    keepGoing = false;
                            }
                            else if (e.Number == 2627)
                            {
                                numSkipped++;
                            }
                            else
                            {
                                DialogResult result = MessageBox.Show("Error - attempt to insert invalid data. Skip line and attmempt to continue?", "Error", MessageBoxButtons.YesNo);
                                if (result == System.Windows.Forms.DialogResult.No)
                                    keepGoing = false;
                            }
                        }


                    }
                }
            }
            string outMessage = numInserted.ToString() + " prerequisite entries inserted successfully.";
            if (numSkipped > 0)
                outMessage += " " + numSkipped + " existing prerequisite entries skipped.";
            MessageBox.Show(outMessage);
            inFile.Close();
        }

        private void AddCourseHistories()
        {
            System.IO.StreamReader inFile = new System.IO.StreamReader(textBox1.Text);
            int lineNum = 0;
            int numInserted = 0;
            int numSkipped = 0;
            string inLine;
            Regex reg = new Regex(@"[a-zA-Z ]{10}\ \d{1}[1-9 ](\ ([A-Z]{2}\-\d{3}\-\d{2}\ {2}|[A-Z]{3}\-\d{3}\-\d{2}\ )[FS]\d{2}\ \d\.\d{2}\ [ABCDEFSUWXOIR][ABCDEFSUWXOIR+\-Q ]{2})+");
            bool keepGoing = true;

            while (!inFile.EndOfStream && keepGoing)
            {
                inLine = inFile.ReadLine();
                lineNum++;
                if (!reg.IsMatch(inLine) || (inLine.Length - 13) % 24 != 0)
                {
                    DialogResult result = MessageBox.Show("Error - inproper input format on line " + lineNum.ToString() + ". Continue?", "Error", MessageBoxButtons.YesNo);
                    if (result == System.Windows.Forms.DialogResult.No)
                        keepGoing = false;
                }
                else
                {
                    linqtoregdbDataContext db = new linqtoregdbDataContext();
                    string usernameIn, courseNameIn, semesterIn, gradeIn;
                    decimal courseCreditIn;
                    int numCourses;
                    usernameIn = TrimSpaces(inLine.Substring(0, 10));
                    numCourses = Convert.ToInt32(TrimSpaces(inLine.Substring(11, 2)));
                    for (int i = 0; i < numCourses; i++)
                    {
                        courseNameIn = TrimSpaces(inLine.Substring(14 + i*24, 10));
                        semesterIn = TrimSpaces(inLine.Substring(25 + i * 24, 3));
                        courseCreditIn = Convert.ToDecimal(inLine.Substring(29 + i * 24, 4));
                        gradeIn = TrimSpaces(inLine.Substring(34 + i * 24, 3));

                        user_course newUserCourse = new user_course
                        {
                            username = usernameIn,
                            courseName = courseNameIn,
                            grade = gradeIn,
                            semester = semesterIn,
                            courseCredit = courseCreditIn
                        };
                        try
                        {
                            db.user_courses.InsertOnSubmit(newUserCourse);
                            db.SubmitChanges();
                            numInserted++;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            if (e.Number == 2627)
                            {
                                numSkipped++;
                            }
                            else
                            {
                                DialogResult result = MessageBox.Show("Error - invalid course history insertion attempted. Skip user and continue?", "Error", MessageBoxButtons.YesNo);
                                if (result == System.Windows.Forms.DialogResult.No)
                                    keepGoing = false;
                            }
                        }
                    }

                }

            }
            string outMessage = numInserted.ToString() + " course history records inserted successfully.";
            if (numSkipped > 0)
                outMessage += " " + numSkipped + " existing records skipped.";
            MessageBox.Show(outMessage);
            inFile.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            prevWindow.Show();
            this.Close();
        }
    }
}
