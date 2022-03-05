using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
namespace Split_Any_Files
{
    public partial class mp : Form
    {

        LinkLabel link;
        ProgressBar progerss;
        MyConfiguration config;
        FileStream fileRead;
        FileStream fileWrite;
        bool pause = false;
        byte[] octets;
        List<string> liste;
        int percent, index, part, number;
        long onePart, sumLengthOfParts, total;

        public mp()
        {
            InitializeComponent();
            config = new MyConfiguration();
        }

        #region ********************* The Events **************************

        private void mp_Load(object sender, EventArgs e)
        {
            this.calculing_binary.DisplayStyle = ToolStripItemDisplayStyle.Text;
            config.loadConfiguration();
            
            foreach (string c in config.getPaths())
            {
                if (!c.Equals(""))
                    this.ComboBox_browse.Items.Add(c);
            }
            this.ComboBox_browse.SelectedItem = config.getPathSelected();
        }

        private void linkLabel_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel link = ((LinkLabel)sender);
            if (link.Name.Contains("browse_3"))
            {
                this.openFileDialog1.Multiselect = false;
                this.openFileDialog1.Filter = "Files (*.ng0001)|*.ng0001";
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    link.Text = this.openFileDialog1.FileName; //item.SubItems.Add("1 of " + numberOfPartsFileEncrypted(filesNames[i]).ToString())

                    foreach (EXListViewItem element in listViewOwn2.Items)
                    {
                        if (((EXControlListViewSubItem)element.SubItems[1]).MyControl.Name.Equals(link.Name) == true)
                        {
                            if (this.ComboBox_browse.Text.Equals("") == true)
                            {
                                ((EXControlListViewSubItem)element.SubItems[2]).MyControl.Text = link.Text.Substring(0, link.Text.LastIndexOf("."));
                                element.SubItems[5].Text = capacity(new FileInfo(link.Text).Length);
                                element.SubItems[7].Text = "1 of " + numberOfPartsFileEncrypted(link.Text).ToString();
                                break;
                            }
                            else
                            {
                                string s = this.ComboBox_browse.Text;
                                string fileName = link.Text.Substring(link.Text.LastIndexOf("\\") + 1);
                                fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                                s = s + "\\" + fileName;
                                s = s.Replace("\\\\", "\\");
                                ((EXControlListViewSubItem)element.SubItems[2]).MyControl.Text = s;
                                element.SubItems[5].Text = capacity(new FileInfo(link.Text).Length);
                                element.SubItems[7].Text = "1 of " + numberOfPartsFileEncrypted(link.Text).ToString();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (EXListViewItem element in listViewOwn2.Items)
                    {
                        if (((EXControlListViewSubItem)element.SubItems[2]).MyControl.Name.Equals(link.Name))
                        {
                            LinkLabel control = (LinkLabel)((EXControlListViewSubItem)element.SubItems[1]).MyControl;
                            string s = this.folderBrowserDialog1.SelectedPath;
                            string fileName = control.Text.Substring(control.Text.LastIndexOf("\\") + 1);
                            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                            s = s + "\\" + fileName;
                            link.Text = s.Replace("\\\\", "\\");
                            break;
                        }

                    }
                }
            }
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel link = ((LinkLabel)sender);
            if (link.Name.Contains("browse_1"))
            {
                this.openFileDialog1.Multiselect = false;
                this.openFileDialog1.Filter = "All Files (*.*)|*.*";
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    link.Text = this.openFileDialog1.FileName;

                    foreach (EXListViewItem element in listViewOwn1.Items)
                    {
                        if (((EXControlListViewSubItem)element.SubItems[1]).MyControl.Name.Equals(link.Name) == true)
                        {
                            if (this.ComboBox_browse.Text.Equals("") == true)
                            {
                                ((EXControlListViewSubItem)element.SubItems[2]).MyControl.Text = link.Text + ".ng0001";
                                element.SubItems[5].Text = capacity(new FileInfo(link.Text).Length);
                                break;
                            }
                            else
                            {
                                string s = this.ComboBox_browse.Text;
                                s = s + link.Text.Substring(link.Text.LastIndexOf("\\")) + ".ng0001";
                                s = s.Replace("\\\\", "\\");
                                ((EXControlListViewSubItem)element.SubItems[2]).MyControl.Text = s;

                                element.SubItems[5].Text = capacity(new FileInfo(link.Text).Length);
                                break;
                            }
                        }

                    }
                }
            }
            else
            {
                if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (EXListViewItem element in listViewOwn1.Items)
                    {
                        if (((EXControlListViewSubItem)element.SubItems[2]).MyControl.Name.Equals(link.Name))
                        {
                            LinkLabel control = (LinkLabel)((EXControlListViewSubItem)element.SubItems[1]).MyControl;
                            link.Text = this.folderBrowserDialog1.SelectedPath + control.Text.Substring(control.Text.LastIndexOf("\\")) + ".ng0001";
                            link.Text = link.Text.Replace("\\\\", "\\");
                            break;
                        }
                    }
                }
            }
        }

        private void doing_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(this.ComboBox_browse.Text))
                Directory.CreateDirectory(this.ComboBox_browse.Text);
            
            if (!pause)
            {
                index = -1;
                if (this.tabControl1.SelectedIndex == 0)
                    this.timer1_Encrypt.Start();
                else if (this.tabControl1.SelectedIndex == 1)
                    this.timer1_Decrypt.Start();
            }
            else
            {
                if (this.tabControl1.SelectedIndex == 0)
                {
                    timer3_Encrypt.Start();
                    ((PictureBox)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[9]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\loading3.gif");
                }
                else
                {
                    timer3_Decrypt.Start();
                    ((PictureBox)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[8]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\loading3.gif");
                }
            }
            this.calculing_binary.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            this.doing.Enabled = false;
            this.pause_button.Enabled = true;
            this.stop.Enabled = true;
            this.Button_browse.Enabled = false;
            this.ComboBox_browse.Enabled = false;
            this.Button_Refresh.Enabled = false;
        }

        private void openFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Multiselect = true;

            if (this.tabControl1.SelectedIndex == 0)
            {
                this.openFileDialog1.Filter = "All Files (*.*)|*.*";
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                    addFiles(this.openFileDialog1.FileNames, listViewOwn1);
            }
            else if (this.tabControl1.SelectedIndex == 1)
            {
                this.openFileDialog1.Filter = "Files (*.ng0001)|*.ng0001";
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                    addFiles(this.openFileDialog1.FileNames, listViewOwn2);
            }
            Button_Refresh_Click(sender, e);

        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.tabControl1.SelectedIndex == 0)
                    addFiles(Directory.GetFiles(this.folderBrowserDialog1.SelectedPath), listViewOwn1);

                else if (this.tabControl1.SelectedIndex == 1)
                    addFiles(Directory.GetFiles(this.folderBrowserDialog1.SelectedPath, "*.ng0001"), listViewOwn2);
            }
            Button_Refresh_Click(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            index++;

            if (listViewOwn1.Items.Count > 0 && index < listViewOwn1.Items.Count)
            {

                if (index > 0)
                {
                    listViewOwn1.Items[index - 1].Selected = false;
                    ((CheckBox)((EXControlListViewSubItem)listViewOwn1.Items[index - 1].SubItems[6]).MyControl).Checked = false;
                }

                EXListViewItem item = (EXListViewItem)listViewOwn1.Items[index];



                if (((CheckBox)((EXControlListViewSubItem)item.SubItems[6]).MyControl).Checked)
                {
                    if (!((LinkLabel)((EXControlListViewSubItem)item.SubItems[2]).MyControl).Text.Equals("Open Output File"))
                    {
                        percent = 0;
                        part = 0;
                        numberOfPartsFileEncrypting(((ComboBox)((EXControlListViewSubItem)item.SubItems[7]).MyControl).SelectedIndex, ((LinkLabel)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[2]).MyControl).Text);

                        EncryptFile(((LinkLabel)((EXControlListViewSubItem)item.SubItems[1]).MyControl).Text, liste[part]);

                        if (fileRead.Length - sumLengthOfParts < onePart)
                            onePart = fileRead.Length - sumLengthOfParts;
                        //this.Text = "part :" + part.ToString();
                        this.timer1_Encrypt.Stop();
                        this.timer3_Encrypt.Start();
                        ((PictureBox)((EXControlListViewSubItem)item.SubItems[9]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\loading3.gif");
                        ((PictureBox)((EXControlListViewSubItem)item.SubItems[10]).MyControl).Image = null;
                    }

                    item.Selected = true;
                    item.EnsureVisible();
                    listViewOwn1.Select();

                }

                ((LinkLabel)((EXControlListViewSubItem)item.SubItems[1]).MyControl).Enabled = false;
                ((LinkLabel)((EXControlListViewSubItem)item.SubItems[2]).MyControl).Enabled = false;
                ((CheckBox)((EXControlListViewSubItem)item.SubItems[6]).MyControl).Enabled = false;
                ((ComboBox)((EXControlListViewSubItem)item.SubItems[7]).MyControl).Enabled = false;


            }
            else
            {
                foreach (EXListViewItem c in listViewOwn1.Items)
                {

                    ((LinkLabel)((EXControlListViewSubItem)c.SubItems[1]).MyControl).Enabled = true;
                    ((LinkLabel)((EXControlListViewSubItem)c.SubItems[2]).MyControl).Enabled = true;
                    ((CheckBox)((EXControlListViewSubItem)c.SubItems[6]).MyControl).Enabled = true;
                    ((CheckBox)((EXControlListViewSubItem)c.SubItems[6]).MyControl).Checked = false;
                    ((ComboBox)((EXControlListViewSubItem)c.SubItems[7]).MyControl).Enabled = true;
                }

                timer1_Encrypt.Stop();
                this.calculing_binary.DisplayStyle = ToolStripItemDisplayStyle.Text;
                if (index > 0)
                {
                    listViewOwn1.Items[index - 1].Selected = false;
                    ((CheckBox)((EXControlListViewSubItem)listViewOwn1.Items[index - 1].SubItems[6]).MyControl).Checked = false;
                }

                this.doing.Enabled = true;
                this.pause_button.Enabled = false;
                this.stop.Enabled = false;
                pause = false;
                this.Button_browse.Enabled = true;
                this.ComboBox_browse.Enabled = true;
                this.Button_Refresh.Enabled = true;
                if (fileWrite != null)
                    fileWrite.Close();
                fileWrite = null;
                if (fileRead != null)
                    fileRead.Close();
                fileRead = null;
                
                onePart = 0; sumLengthOfParts = 0; total = 0;
                MessageBox.Show("Spliting files is finished", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }


        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            part++;
            if (part < liste.Count)
            {

                EncryptFile(((LinkLabel)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[1]).MyControl).Text, liste[part]);
                if (fileRead.Length - sumLengthOfParts < onePart)
                    onePart = fileRead.Length - sumLengthOfParts;
                timer2_Encrypt.Stop();
                timer3_Encrypt.Start();
            }
            else
            {
                percent = 100;
                ((ProgressBar)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[4]).MyControl).Value = 100;
                ((ProgressBar)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[4]).MyControl).Refresh();
                listViewOwn1.Items[index].SubItems[3].Text = percent.ToString() + "%";
                ((PictureBox)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[10]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\button_ok.png");
                ((PictureBox)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[9]).MyControl).Image = null;
                timer2_Encrypt.Stop();
                timer1_Encrypt.Start();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            bool finish = false;
            long longLen = sumLengthOfParts;
            if (fileWrite.Length  <= onePart )
            {
                if (fileRead.Read(octets, 0, octets.Length) != 0)
                {
                    fileWrite.Write(octets, 0, octets.Length);
                   
                    percent = (int)((sumLengthOfParts + fileWrite.Length) * 100 / fileRead.Length);
                    long cpt = lengthLoading(onePart, fileWrite.Length);
                    octets = new byte[cpt];
                }
                else
                {
                    finish = true;
                    sumLengthOfParts += fileWrite.Length;
                }
            }
            else
            {
                finish = true;
                sumLengthOfParts += fileWrite.Length;

            }

            this.calculing_binary.Text = "-&Capacity Spliting :" + capacity(longLen + fileWrite.Length) + " / " + capacity(fileRead.Length) + "\n" +
                                         "-Capacity of part (" + listViewOwn1.Items[index].SubItems[8].Text + ") Spliting :" + capacity(fileWrite.Length) + " / " + capacity(onePart);
            if (percent > 100)
                percent = 100;
            listViewOwn1.Items[index].SubItems[3].Text = percent.ToString() + "%";
            ((ProgressBar)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[4]).MyControl).Value = percent;
            if (finish)
            {
                if (fileWrite != null)
                    fileWrite.Close();
                fileWrite = null;

                octets = null;
                timer3_Encrypt.Stop();
                timer2_Encrypt.Start();
            }
        }

        private void timer1_Decrypt_Tick(object sender, EventArgs e)
        {
            index++;

            if (listViewOwn2.Items.Count > 0 && index < listViewOwn2.Items.Count)
            {
                if (index > 0)
                {
                    listViewOwn2.Items[index - 1].Selected = false;
                    ((CheckBox)((EXControlListViewSubItem)listViewOwn2.Items[index - 1].SubItems[6]).MyControl).Checked = false;
                }

                EXListViewItem item = (EXListViewItem)listViewOwn2.Items[index];

                if (((CheckBox)((EXControlListViewSubItem)item.SubItems[6]).MyControl).Checked)
                {
                    if (!((LinkLabel)((EXControlListViewSubItem)item.SubItems[2]).MyControl).Text.Equals("Open Output File"))
                    {
                        percent = 0;
                        part = 0;
                        numberOfPartsFileEncrypted(((LinkLabel)((EXControlListViewSubItem)item.SubItems[1]).MyControl).Text);

                        DecryptFile(liste[part], ((LinkLabel)((EXControlListViewSubItem)item.SubItems[2]).MyControl).Text);

                        this.timer1_Decrypt.Stop();
                        this.timer3_Decrypt.Start();
                        ((PictureBox)((EXControlListViewSubItem)item.SubItems[8]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\loading3.gif");
                        ((PictureBox)((EXControlListViewSubItem)item.SubItems[9]).MyControl).Image = null;
                    }
                    item.Selected = true;
                    item.EnsureVisible();
                    listViewOwn2.Select();
                }

            }
            else
            {
                foreach (EXListViewItem c in listViewOwn2.Items)
                {

                    ((LinkLabel)((EXControlListViewSubItem)c.SubItems[1]).MyControl).Enabled = true;
                    ((LinkLabel)((EXControlListViewSubItem)c.SubItems[2]).MyControl).Enabled = true;
                    ((CheckBox)((EXControlListViewSubItem)c.SubItems[6]).MyControl).Enabled = true;
                    ((CheckBox)((EXControlListViewSubItem)c.SubItems[6]).MyControl).Checked = false;
                }

                timer1_Decrypt.Stop();
                this.calculing_binary.DisplayStyle = ToolStripItemDisplayStyle.Text;
                if (index > 0)
                {
                    listViewOwn2.Items[index - 1].Selected = false;
                    ((CheckBox)((EXControlListViewSubItem)listViewOwn2.Items[index - 1].SubItems[6]).MyControl).Checked = false;
                }

                this.doing.Enabled = true;
                this.pause_button.Enabled = false;
                this.stop.Enabled = false;
                pause = false;
                this.Button_browse.Enabled = true;
                this.ComboBox_browse.Enabled = true;
                this.Button_Refresh.Enabled = true;
                MessageBox.Show("Joining files is finished", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (fileWrite != null)
                    fileWrite.Close();
                fileWrite = null;
                if (fileRead != null)
                    fileRead.Close();
                fileRead = null;
               
                octets = null;
               
                liste = null;
                onePart = 0; sumLengthOfParts = 0; total = 0;
            }
        }

        private void timer2_Decrypt_Tick(object sender, EventArgs e)
        {
            part++;
            if (part < liste.Count)
            {
                DecryptFile(liste[part], ((LinkLabel)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[2]).MyControl).Text);
                timer2_Decrypt.Stop();
                timer3_Decrypt.Start();
            }
            else
            {
                percent = 100;
                listViewOwn2.Items[index].SubItems[3].Text = percent.ToString() + "%";
                ((PictureBox)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[9]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\button_ok.png");
                ((PictureBox)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[8]).MyControl).Image = null;
                timer2_Decrypt.Stop();
                timer1_Decrypt.Start();
            }
        }

        private void timer3_Decrypt_Tick(object sender, EventArgs e)
        {
            bool finish = false;

            if (fileRead.Read(octets, 0, octets.Length) != 0)
            {
                fileWrite.Write(octets, 0, octets.Length);
                percent = (int)((fileWrite.Length) * 100 / total);
                long dcpt = lengthLoading(sumLengthOfParts, fileWrite.Length);//3678982;
                octets = new byte[dcpt];
            }
            else
            {
                finish = true;

            }
            this.calculing_binary.Text = "&Capacity Joining :" + capacity(fileWrite.Length) + " / " + capacity(total) + "\n" +
                "Capacity of part (" + listViewOwn2.Items[index].SubItems[7].Text + ") Joining :" + capacity(fileWrite.Length - (sumLengthOfParts - fileRead.Length)) + " / " + capacity(fileRead.Length);
            listViewOwn2.Items[index].SubItems[3].Text = percent.ToString() + "%";
            ((ProgressBar)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[4]).MyControl).Value = percent;

            if (finish)
            {

                if (fileRead != null)
                    fileRead.Close();
                fileRead = null;

                timer3_Decrypt.Stop();
                timer2_Decrypt.Start();
            }
        }

        private void Button_browse_Click(object sender, EventArgs e)
        {

            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                ComboBox_browse.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void pause_button_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                timer3_Encrypt.Stop();
                this.calculing_binary.DisplayStyle = ToolStripItemDisplayStyle.Text;
                ((PictureBox)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[9]).MyControl).Image = null;
            }
            else
            {
                timer3_Decrypt.Stop();
                this.calculing_binary.DisplayStyle = ToolStripItemDisplayStyle.Text;
                ((PictureBox)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[8]).MyControl).Image = null;
            }
            pause = true;
            this.doing.Enabled = true;
            this.pause_button.Enabled = false;

        }

        private void stop_ButtonClick(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                timer3_Encrypt.Stop();
                ((PictureBox)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[9]).MyControl).Image = null;
                ((PictureBox)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[10]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\cancel.png"); ;
                listViewOwn1.Items[index].Selected = false;
               
                timer1_Encrypt.Start();
            }
            else
            {
                timer3_Decrypt.Stop();
                ((PictureBox)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[8]).MyControl).Image = null;
                ((PictureBox)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[9]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\cancel.png"); ;
                timer1_Decrypt.Start();
                listViewOwn2.Items[index].Selected = false;
            }
            if (fileWrite != null)
                fileWrite.Close();
            fileWrite = null;
            if (fileRead != null)
                fileRead.Close();
            fileRead = null;
            this.doing.Enabled = true;
            this.pause_button.Enabled = false;

        }

        private void stop_all_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                timer3_Encrypt.Stop();
                timer1_Encrypt.Stop();

                ((PictureBox)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[9]).MyControl).Image = null;
                ((PictureBox)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[10]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\cancel.png"); ;
                listViewOwn1.Items[index].Selected = false;
                ((CheckBox)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[6]).MyControl).Checked = false;

                foreach (EXListViewItem c in listViewOwn1.Items)
                {
                    ((LinkLabel)((EXControlListViewSubItem)c.SubItems[1]).MyControl).Enabled = true;
                    ((LinkLabel)((EXControlListViewSubItem)c.SubItems[2]).MyControl).Enabled = true;
                    ((CheckBox)((EXControlListViewSubItem)c.SubItems[6]).MyControl).Enabled = true;
                }
               
            }
            else
            {
                timer3_Decrypt.Stop();
                timer1_Decrypt.Stop();
                ((PictureBox)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[8]).MyControl).Image = null;
                ((PictureBox)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[9]).MyControl).Image = Image.FromFile(Application.StartupPath + "\\cancel.png"); ;
                listViewOwn2.Items[index].Selected = false;
                ((CheckBox)((EXControlListViewSubItem)listViewOwn2.Items[index].SubItems[6]).MyControl).Checked = false;

                foreach (EXListViewItem c in listViewOwn2.Items)
                {
                    ((LinkLabel)((EXControlListViewSubItem)c.SubItems[1]).MyControl).Enabled = true;
                    ((LinkLabel)((EXControlListViewSubItem)c.SubItems[2]).MyControl).Enabled = true;
                    ((CheckBox)((EXControlListViewSubItem)c.SubItems[6]).MyControl).Enabled = true;
                }
            }

            this.doing.Enabled = true;
            this.pause_button.Enabled = false;
            this.stop.Enabled = false;
            this.calculing_binary.DisplayStyle = ToolStripItemDisplayStyle.Text;
            if (fileWrite != null)
                fileWrite.Close();
            fileWrite = null;
            if (fileRead != null)
                fileRead.Close();
            fileRead = null;
     
            octets = null;
       
            liste = null;
            onePart = 0; sumLengthOfParts = 0; total = 0;
          
            pause = false;
            index = -1;
            this.Button_browse.Enabled = true;
        }

        private void deleteNoCheckedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewOwn lvo;
            if (this.tabControl1.SelectedIndex == 0)
                lvo = listViewOwn1;
            else
                lvo = listViewOwn2;
            foreach (EXListViewItem item in lvo.Items)
            {
                if (!((CheckBox)((EXControlListViewSubItem)item.SubItems[6]).MyControl).Checked)
                {
                    item.Remove();
                }

            }
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
                listViewOwn1.Items.Clear();
            else
                listViewOwn2.Items.Clear();
        }

        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewOwn lvo;
            if (this.tabControl1.SelectedIndex == 0)
                lvo = listViewOwn1;
            else
                lvo = listViewOwn2;
            foreach (EXListViewItem item in lvo.Items)
            {
                ((CheckBox)((EXControlListViewSubItem)item.SubItems[6]).MyControl).Checked = true;
            }
        }

        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewOwn lvo;
            if (this.tabControl1.SelectedIndex == 0)
                lvo = listViewOwn1;
            else
                lvo = listViewOwn2;
            foreach (EXListViewItem item in lvo.Items)
            {
                ((CheckBox)((EXControlListViewSubItem)item.SubItems[6]).MyControl).Checked = false;
            }
        }

        private void ComboBox_browse_TextChanged(object sender, EventArgs e)
        {
            if (!ComboBox_browse.Text.Equals(""))
            {
                string[] listePaths = new string[ComboBox_browse.Items.Count];
                ComboBox_browse.Items.CopyTo(listePaths, 0);
                config.setPaths(listePaths);
                config.setPathSelected(this.ComboBox_browse.Text);

                if (!ComboBox_browse.Items.Contains(ComboBox_browse.Text))
                {
                    ComboBox_browse.Items.Add(ComboBox_browse.Text);
                }


                if (this.tabControl1.SelectedIndex == 0)
                {
                    foreach (EXListViewItem item in listViewOwn1.Items)
                    {
                        EXControlListViewSubItem subItem1 = (EXControlListViewSubItem)item.SubItems[1];
                        EXControlListViewSubItem subItem2 = (EXControlListViewSubItem)item.SubItems[2];
                        ((LinkLabel)(subItem2).MyControl).Text = ComboBox_browse.Text + ((LinkLabel)subItem1.MyControl).Text.Substring(((LinkLabel)subItem1.MyControl).Text.LastIndexOf("\\")) + ".ng0001";
                        ((LinkLabel)(subItem2).MyControl).Text = ((LinkLabel)(subItem2).MyControl).Text.Replace("\\\\", "\\");

                    }
                }
                else if (this.tabControl1.SelectedIndex == 1)
                {
                    foreach (EXListViewItem item in listViewOwn2.Items)
                    {
                        EXControlListViewSubItem subItem1 = (EXControlListViewSubItem)item.SubItems[1];
                        EXControlListViewSubItem subItem2 = (EXControlListViewSubItem)item.SubItems[2];

                        string fileName = ((LinkLabel)subItem1.MyControl).Text.Substring(((LinkLabel)subItem1.MyControl).Text.LastIndexOf("\\") + 1);
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        ((LinkLabel)(subItem2).MyControl).Text = ComboBox_browse.Text + "\\" + fileName;
                        ((LinkLabel)(subItem2).MyControl).Text = ((LinkLabel)(subItem2).MyControl).Text.Replace("\\\\", "\\");

                    }
                }

            }
        }

        // private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ComboBox cbox = ((ComboBox)sender);

        //    MessageBox.Show(cbox.SelectedIndex+"----"+cbox.SelectedIndex.ToString() + "---------- " + onePart.ToString() + "-----" + number.ToString() + "----------" + chemin);
        //}

        private void ComboBox_browse_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.Handled = true;
        }

        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            if (!ComboBox_browse.Text.Equals(""))
            {
                string myPath = ComboBox_browse.Text;
                ComboBox_browse.Text = "";
                ComboBox_browse.Text = myPath;
            }
        }


        private void mp_FormClosing(object sender, FormClosingEventArgs e)
        {
            config.saveConfiguration();
        }

        #endregion

        #region ****************** subs and functions *********************

        private void numberOfPartsFileEncrypting(int choix, string outPutFile)
        {
            string chemin = ((LinkLabel)((EXControlListViewSubItem)listViewOwn1.Items[index].SubItems[1]).MyControl).Text;
            FileInfo fr = new FileInfo(chemin);
            switch (choix)
            {
                case 0: onePart = 3678982; break;
                case 1: onePart = 52428800; break;
                case 2: onePart = 104857600; break;
                case 3: onePart = 209715200; break;
                case 4: onePart = 524288000; break;
                case 5: onePart = 734003200; break;
                case 6: onePart = fr.Length; break;
            }
            number = (int)(fr.Length / onePart);
            if (fr.Length % onePart != 0)
                number += 1;
            fr = null;

            string ext;
            liste = new List<string>();

            for (int i = 0; i < number; i++)
            {
                if (i < 9)
                    ext = "ng000" + (i + 1).ToString();
                else if (i >= 9 && i < 99)
                    ext = "ng00" + (i + 1).ToString();
                else if (i >= 99 && i < 999)
                    ext = "ng0" + (i + 1).ToString();
                else
                    ext = "ng" + (i + 1).ToString();

                liste.Add(outPutFile.Replace("ng0001", ext));
            }
        }

        private int numberOfPartsFileEncrypted(string path)
        {
            string fileName = path.Substring(path.LastIndexOf("\\") + 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
            liste = Directory.GetFiles(path.Substring(0, path.LastIndexOf("\\")), fileName + ".ng*").ToList<string>();
            total = 0;
            foreach (string pathFile in liste)
            {
                FileInfo info = new FileInfo(pathFile);
                total += info.Length;
            }
            return liste.Count;
        }

        private void EncryptFile(string inPutFile, string outPutFile)
        {

            try
            {
                if (part == 0)
                {
                    fileRead = new FileStream(inPutFile, FileMode.Open);
                    this.Text = fileRead.Name.Substring(fileRead.Name.LastIndexOf("\\") + 1);
                    sumLengthOfParts = 0;
                }
                   
                listViewOwn1.Items[index].SubItems[8].Text = (part + 1).ToString() + " of " + number.ToString();
                fileWrite = new FileStream(outPutFile, FileMode.Create);
                long cpt = lengthLoading(onePart, fileWrite.Length);
                octets = new byte[cpt];

                percent = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void DecryptFile(string inPutFile, string outPutFile)
        {

            try
            {
                if (part == 0)
                {
                    if (fileWrite != null)
                        fileWrite.Close();
                    fileWrite = new FileStream(outPutFile, FileMode.Create);
                    sumLengthOfParts = 0;
                }


                listViewOwn2.Items[index].SubItems[7].Text = (part + 1).ToString() + " of " + liste.Count.ToString();
                fileRead = new FileStream(inPutFile, FileMode.Open);
                this.Text = fileRead.Name.Substring(fileRead.Name.LastIndexOf("\\") + 1);
                sumLengthOfParts += fileRead.Length;

                long dcpt = lengthLoading(sumLengthOfParts, fileWrite.Length);//3678982;

                octets = new byte[dcpt];

                percent = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private long lengthLoading(long onePart1, long lengthFileNow)
        {
            long loading = onePart1 - lengthFileNow;
            if (loading < 0)
                loading = -(loading);
            if (loading > 3678982)
                loading = 3678982;
            return loading;
        }

        private void addFiles(string[] filesNames, ListViewOwn list_view)
        {
            int nbr = 0;
            if (list_view.Items.Count > 0)
            {
                nbr = int.Parse(list_view.Items[list_view.Items.Count - 1].Text);
            }
            for (int i = 0; i < filesNames.Length; i++)
            {
                EXListViewItem item = new EXListViewItem((nbr + 1).ToString());

                /************************ 1 *****************************/
                EXControlListViewSubItem cs = new EXControlListViewSubItem();
                link = new LinkLabel();

                link.Text = filesNames[i];
                link.AutoSize = false;
                link.LinkColor = Color.Yellow;
                link.TextAlign = ContentAlignment.MiddleLeft;
                if (list_view.Equals(this.listViewOwn1))
                {
                    link.Name = "browse_1_" + (2 * nbr).ToString();
                    link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
                }
                else
                {
                    link.Name = "browse_3_" + (2 * nbr).ToString();
                    link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked_1);
                }

                item.SubItems.Add(cs);
                list_view.AddControlToSubItem(link, cs);

                /************************ 2 *****************************/
                cs = new EXControlListViewSubItem();
                link = new LinkLabel();
                link.Text = "Open Output File";
                link.AutoSize = false;
                link.LinkColor = Color.Yellow;
                link.TextAlign = ContentAlignment.MiddleLeft;
                if (list_view.Equals(this.listViewOwn1))
                {
                    link.Name = "browse_2_" + ((2 * nbr) + 1).ToString();
                    link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
                }
                else
                {
                    link.Name = "browse_4_" + ((2 * nbr) + 1).ToString();
                    link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked_1);
                }

                item.SubItems.Add(cs);
                list_view.AddControlToSubItem(link, cs);

                /************************ 3 *****************************/
                item.SubItems.Add("0%");
                /************************ 4 *****************************/
                cs = new EXControlListViewSubItem();
                progerss = new ProgressBar();

                progerss.Minimum = 0;
                progerss.Maximum = 100;
                item.SubItems.Add(cs);
                list_view.AddControlToSubItem(progerss, cs);

                /************************ 5 *****************************/
                FileInfo info = new FileInfo(filesNames[i]);
                item.SubItems.Add(capacity(info.Length));

                /************************ 6 *****************************/
                cs = new EXControlListViewSubItem();
                CheckBox box = new CheckBox();
                box.Name = "Box_6_" + (nbr + 1).ToString();
                box.AutoSize = false;
                box.Checked = true;
                box.CheckAlign = ContentAlignment.MiddleCenter;
                item.SubItems.Add(cs);
                list_view.AddControlToSubItem(box, cs);


                /************************ 7 *****************************/
                if (list_view.Equals(this.listViewOwn1))
                {
                    cs = new EXControlListViewSubItem();
                    ComboBox cbox = new ComboBox();
                    cbox.Name = "ComboBox_7_" + (nbr + 1).ToString();
                    cbox.AutoSize = false;
                    cbox.Items.Add("Lowest 3.5 Mo");
                    cbox.Items.Add("Lower 50 Mo");
                    cbox.Items.Add("Semi-Medium 100 Mo");
                    cbox.Items.Add("Medium 200 Mo");
                    cbox.Items.Add("Big 500 Mo");
                    cbox.Items.Add("Large 700 Mo");
                    cbox.Items.Add("No parts");
                    cbox.DropDownStyle = ComboBoxStyle.DropDownList;
                    cbox.SelectedIndex = 6;
                    // cbox.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
                    item.SubItems.Add(cs);
                    list_view.AddControlToSubItem(cbox, cs);
                }
                else
                {
                    item.SubItems.Add("1 of " + numberOfPartsFileEncrypted(filesNames[i]).ToString());
                }
                /************************ 8 *****************************/
                if (list_view.Equals(this.listViewOwn1))
                    item.SubItems.Add("");
                /************************ 9 *****************************/
                /************************ 8 *****************************/
                cs = new EXControlListViewSubItem();
                PictureBox pc = new PictureBox();
                pc.Name = "Picture_9_" + (nbr + 1).ToString();
                pc.SizeMode = PictureBoxSizeMode.StretchImage;
                item.SubItems.Add(cs);
                list_view.AddControlToSubItem(pc, cs);

                /************************ 10 *****************************/
                /************************ 9 *****************************/
                cs = new EXControlListViewSubItem();
                pc = new PictureBox();
                pc.Name = "Picture_10_" + (nbr + 1).ToString();
                pc.SizeMode = PictureBoxSizeMode.StretchImage;
                item.SubItems.Add(cs);
                list_view.AddControlToSubItem(pc, cs);

                list_view.Items.Add(item);
                nbr++;
            }
        }

        private string capacity(long length)
        {
            short Ko = 1024;
            int Mo = Ko * 1024;
            long Go = Mo * 1024;
            string s = null;

            if (length >= Go)
            {
                s = String.Format("{0:0.##}", ((double)length / (double)Go)) + " Go";
            }
            else if (length >= Mo)
            {
                s = String.Format("{0:0.##}", ((double)length / (double)Mo)) + " Mo";
            }
            else if (length >= Ko)
            {
                s = String.Format("{0:0.##}", ((double)length / (double)Ko)) + " ko";
            }
            else
                s = length.ToString() + " Octects";
            return s;
        }

       
        #endregion
    }
}


// 1.
//    Public Function ListView_AddProgressBar(ByRef pListView As System.Windows.Forms.ListView, ByVal ListViewItemIndex As Integer, ByVal ColumnIndex As Integer) As System.Windows.Forms.ProgressBar
// 2.
//    Dim r As Rectangle
// 3.
//    Dim pb As New System.Windows.Forms.ProgressBar
// 4.

// 5.
//    r = pListView.Items(ListViewItemIndex).Bounds()
// 6.
//    r.Width = pListView.Columns(ColumnIndex).Width
// 7.
//    If ColumnIndex > 0 Then
// 8.
//    r.X = r.X + pListView.Columns(ColumnIndex - 1).Width
// 9.
//    End If
//10.
//    pb.Parent = pListView
//11.
//    pb.SetBounds(r.X, r.Y, r.Width, r.Height)
//12.
//    pb.Visible = True
//13.

//14.
//    Return pb
//15.
//    End Function