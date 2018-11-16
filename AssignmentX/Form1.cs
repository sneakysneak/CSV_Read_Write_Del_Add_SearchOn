using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;        //important, it ensures to read and write files and data streams


namespace AssignmentX
{
    public partial class Form1 : Form
    {
        struct staffData // created a struct instead of a class, because struct has value types while class is usually reference type.
        {                // three data types
            public string Firstname;
            public string Surname;
            public string ExtenCode;
        }
        string path = "\\Desktop\\CollegeData.csv"; //path to the linked file
        public Form1()
        {
            InitializeComponent();
        }

        private staffData[] ReadFile() //array for the data
        {
            try
            {
                string[] arrayLines = File.ReadAllLines(path); //read the file, put it to a 1D array
                staffData[] staffSheet = new staffData[arrayLines.Length]; //need for another 1D array, it helps to make difference in the text based on comma ","

                for (int i = 0; i < arrayLines.Length; i++) //now the file will be read and based on this structure
                {
                    string[] info = arrayLines[i].Split(new[] { ',' }); //split lines with comma
                    staffData staff = new staffData(); //3 different data 
                    staff.Surname = info[0];
                    staff.Firstname = info[1];
                    staff.ExtenCode = info[2];

                    staffSheet[i] = staff;
                }

                return staffSheet;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); //if something goes wrong, a message appears what is the problem
                return null;
            }
        }
        private void toSaveStaffData(staffData[] SaveData) //method to be able to save data, and convert data like in the file
        {
            StreamWriter sw = new StreamWriter(path, false);

            for (int i = 0; i < SaveData.Length; i++)
            {

                sw.WriteLine(SaveData[i].Surname + "," + SaveData[i].Firstname + "," + SaveData[i].ExtenCode);
            }

            sw.Close();
        }
        void bubbleSort(staffData[] toSaveData)
        {
            bool toSwap = true;
            int toCount;

            while (toSwap)
            {
                toSwap = false;

                for (toCount = 0; toCount <= toSaveData.Length - 2; toCount++) //1 or 2?? both doesn't work
                {
                    var oldExtensionCode = Int32.Parse(toSaveData[toCount].ExtenCode);
                    var newExtensionCode = Int32.Parse(toSaveData[toCount].ExtenCode);

                    if (oldExtensionCode > newExtensionCode)
                    {
                        toSaveData[toCount].ExtenCode = newExtensionCode.ToString();
                        toSaveData[toCount + 1].ExtenCode = oldExtensionCode.ToString();
                        toSwap = true;
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void cmdAdd_Click(object sender, EventArgs e) //Add button, it has timed function too, after the user added, the boxes will be cleared
        {
            var checkIfValid = toValidateData(); //validation method

            if (checkIfValid == true)
            {
                staffData[] addStaff = ReadFile();
                var notCorrectExtension = checkExtenCodeDuplicated(addStaff, txtExt.Text);
                if (notCorrectExtension == false)
                {
                    var temporaryData = new staffData[addStaff.Length + 1];

                    for (int i = 0; i < addStaff.Length; i++)
                    {
                        temporaryData[i] = addStaff[i];
                    }
                    temporaryData[addStaff.Length] = new staffData
                    {
                        Surname = txtSurname.Text,
                        Firstname = txtFirstname.Text,
                        ExtenCode = txtExt.Text
                    };

                    addStaff = temporaryData;

                    bubbleSort(addStaff);

                    toSaveStaffData(addStaff);

                    lvConsole.Clear();
                    txtSurname.Text = "";
                    txtFirstname.Text = "";
                    txtExt.Text = "";
                    lblUpdated.Visible = true;
                    tmrUpdated.Interval = 3000;
                    tmrUpdated.Enabled = true;
                }
                else
                {
                    MessageBox.Show("This extension code is in use");
                }
              }
            }           

        private void cmdClear_Click(object sender, EventArgs e) //clear the list view display
        {
            txtSurname.Clear();
            txtFirstname.Clear();
            txtExt.Clear();
            lvConsole.Clear();
        }

        private void cmdSearch_Click(object sender, EventArgs e) //search button
        {
            lvConsole.Clear(); //clear any remaining information

            staffData[] toSaveData = ReadFile(); //it reads the file and convert the information based on the previous format
            for (int i = 0; i < toSaveData.Length; i++) //matches the array's information with the input data
            {
                if (
                    (txtSurname.Text == "" || toSaveData[i].Surname == txtSurname.Text) &&
                    (txtFirstname.Text == "" || toSaveData[i].Firstname == txtFirstname.Text) &&
                    (txtExt.Text == "" || toSaveData[i].ExtenCode == txtExt.Text)
                    )
                {
                    lvConsole.Items.Add(toSaveData[i].Firstname + ", " + toSaveData[i].Surname + ", " + toSaveData[i].ExtenCode); //adds information to the list view, which matches
                    lvConsole.Items[lvConsole.Items.Count - 1].Tag = i; //finds the index of the data and adds it to the list view
                }               
            }                     
        }
        private void cmdUpdate_Click(object sender, EventArgs e) //update button,read the data file and update the information based on the text boxes 
        {
            staffData[] toSaveData = ReadFile();            
            var checkIfValid = toValidateData(); // avoid duplicate extension codes 

            if (checkIfValid == true)
            {
                var index = (int)lvConsole.Items[lvConsole.SelectedIndices[0]].Tag; // it gets the index of the searched information on staffdata array, based on the searched information

                toSaveData[index].Surname = txtSurname.Text;
                toSaveData[index].Firstname = txtFirstname.Text;

                toSaveStaffData(toSaveData); //Method that save the information into the CSV file

                lvConsole.Clear(); //clear remaining information
                txtSurname.Text = "";
                txtFirstname.Text = "";
                txtExt.Text = "";
                lblUpdated.Visible = true; //timer method to display updated for 3 secs
                tmrUpdated.Interval = 3000;
                tmrUpdated.Enabled = true;
            }
        }
        private void lvConsole_MouseClick(object sender, MouseEventArgs e)
        {
            staffData[] toSaveData = ReadFile();
            var index = (int)lvConsole.Items[lvConsole.SelectedIndices[0]].Tag; //This statement takes the index, on the staff array, of the selected item on the list view. 

            //Then copy the information into the text boxes
            txtSurname.Text = toSaveData[index].Surname;
            txtFirstname.Text = toSaveData[index].Firstname;
            txtExt.Text = toSaveData[index].ExtenCode;
        }

        private void txtSurname_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdUpdate_Click_1(object sender, EventArgs e)
        {
            
            
        }
        bool checkExtenCodeDuplicated(staffData[] addStaff, string extensionToAdd) //method against duplicate extension code data
        {
            foreach (var staffData in addStaff)
            {
                if (staffData.ExtenCode.Equals(extensionToAdd))
                {
                    return true;
                }
            }
            return false;
        }
        bool validateNumber (string value) //method to validate numbers
        {
            try
            {
                Int32.Parse(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        bool validateText(string value) //method to validate texts
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (!(
                value[i] ==' '||
                (value[i] >= 'a' && value[i] <= 'z')||
                (value[i] >= 'A' && value[i] <= 'Z')))
                {
                    return false;
                }
            }
            return true;
        }
        bool toValidateData() //method to validate between text and numbers on the relevant fields
        {
            if(string.IsNullOrEmpty(txtExt.Text) || string.IsNullOrEmpty(txtSurname.Text) || string.IsNullOrEmpty(txtFirstname.Text))
            {
                MessageBox.Show("Check fields, one or more is empty");
                return false;
            }
            else
            {
                if (!validateNumber(txtExt.Text))
                {
                    MessageBox.Show("Extension must be number");
                    return false;
                }
                else if (!validateText(txtSurname.Text))
                {
                    MessageBox.Show("Surname can be letters");
                    return false;
                }
                else if (!validateText(txtFirstname.Text))
                {
                    MessageBox.Show("Firstname can be letters");
                    return false;
                }
            }

            return true;
        }
        

        private void tmrAdded_Tick(object sender, EventArgs e)
        {
            lblAdded.Visible = false; //timed text doesn't show up, just when it's called
        }

        private void tmrUpdated_Tick(object sender, EventArgs e)
        {
            lblUpdated.Visible = false; //timed text doesn't show up, just when it's called
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblUpdated_Click(object sender, EventArgs e)
        {

        }

        private void cmdSort_Click(object sender, EventArgs e)
        {
            //bubbleSort();
        }

        private void lblBanner_Click(object sender, EventArgs e)
        {

        }
    }
}
