using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Net;
using System.Threading;

namespace Downloader
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox TxtURL;
		private System.Windows.Forms.Button BtnGo;
		private string Document;
		private string PresentWorkingDirectory;
		private string FolderDirectory;
		private string CurrentAddress;
		private System.Windows.Forms.ListView ListPage;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView ListFiles;
		private class MyPage
		{
			internal string URL;
			internal bool visited;
			internal string RefPage;
			public MyPage(string address, string reference)
			{
				URL = address;
				visited = false;
				RefPage = reference;
			}
		}
		private ArrayList PageList;
		private System.Windows.Forms.StatusBar StatusURL;
		private System.Windows.Forms.Button BtnDownload;
		private Thread ThreadPage;
		private Thread ThreadDownload;
		private Thread ThreadImport;
		private ArrayList LogFileRead;
		private System.IO.StreamWriter LogFileWrite;
		private System.IO.StreamWriter BriefURLs;
		private System.Windows.Forms.Button BtnAbortURL;
		private System.Windows.Forms.Button BtnAbortDL;
		private System.Windows.Forms.CheckBox chkLinks;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			if (!System.IO.Directory.Exists(@"C:\H-Pics"))
			{
				Directory.CreateDirectory(@"C:\H-Pics\");
			}
			if (!Directory.Exists(@"C:\H-Pics\Repository"))
			{
				Directory.CreateDirectory(@"C:\H-Pics\Repository");
			}
			ThreadPage = new Thread(new ThreadStart(GrabLinks));
			ThreadDownload = new Thread(new ThreadStart(DownloadFile));
			ThreadImport = new Thread(new ThreadStart(Importing));
			FolderDirectory = @"C:\H-Pics\";
			try
			{
                CheckForIllegalCrossThreadCalls = false;
				StreamReader LogFile = null;
				try
				{
					LogFile = new StreamReader(Application.StartupPath + "\\files.log");
				}
				catch (System.IO.FileNotFoundException ex)
				{
					LogFile = null;
					Console.WriteLine(ex.Message);
				}
				LogFileRead = new ArrayList();
				if (LogFile != null)
				{
					string [] Temp = LogFile.ReadToEnd().Replace("\n", "").Split('\r');
					foreach (string s in Temp)
					{
						if (s.Length > 5)
							LogFileRead.Add(s);
					}
					LogFile.Close();
				}
				LogFileWrite = new StreamWriter(Application.StartupPath + "\\files.log", true);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Log file creation error");
				LogFileRead = null;
				Console.WriteLine(ex.Message);
			}
			BriefURLs = null;
			CheckForEmptyFolders(FolderDirectory);
			PageList = new ArrayList();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TxtURL = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.BtnGo = new System.Windows.Forms.Button();
			this.ListPage = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.ListFiles = new System.Windows.Forms.ListView();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.StatusURL = new System.Windows.Forms.StatusBar();
			this.BtnDownload = new System.Windows.Forms.Button();
			this.BtnAbortURL = new System.Windows.Forms.Button();
			this.BtnAbortDL = new System.Windows.Forms.Button();
			this.chkLinks = new System.Windows.Forms.CheckBox();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.button4 = new System.Windows.Forms.Button();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// TxtURL
			// 
			this.TxtURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TxtURL.Location = new System.Drawing.Point(56, 16);
			this.TxtURL.Name = "TxtURL";
			this.TxtURL.Size = new System.Drawing.Size(358, 20);
			this.TxtURL.TabIndex = 0;
			this.TxtURL.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "URL:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// BtnGo
			// 
			this.BtnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnGo.Location = new System.Drawing.Point(422, 16);
			this.BtnGo.Name = "BtnGo";
			this.BtnGo.Size = new System.Drawing.Size(56, 23);
			this.BtnGo.TabIndex = 2;
			this.BtnGo.Text = "Go!";
			this.BtnGo.Click += new System.EventHandler(this.button1_Click);
			// 
			// ListPage
			// 
			this.ListPage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.columnHeader1});
			this.ListPage.FullRowSelect = true;
			this.ListPage.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.ListPage.Location = new System.Drawing.Point(10, 48);
			this.ListPage.Name = "ListPage";
			this.ListPage.Size = new System.Drawing.Size(300, 304);
			this.ListPage.TabIndex = 3;
			this.ListPage.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 560;
			// 
			// ListFiles
			// 
			this.ListFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ListFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader2});
			this.ListFiles.FullRowSelect = true;
			this.ListFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.ListFiles.Location = new System.Drawing.Point(318, 48);
			this.ListFiles.MultiSelect = false;
			this.ListFiles.Name = "ListFiles";
			this.ListFiles.Size = new System.Drawing.Size(300, 304);
			this.ListFiles.TabIndex = 4;
			this.ListFiles.View = System.Windows.Forms.View.Details;
			this.ListFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListFiles_KeyDown);
			this.ListFiles.SelectedIndexChanged += new System.EventHandler(this.ListFiles_SelectedIndexChanged);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Width = 560;
			// 
			// StatusURL
			// 
			this.StatusURL.Location = new System.Drawing.Point(0, 401);
			this.StatusURL.Name = "StatusURL";
			this.StatusURL.Size = new System.Drawing.Size(630, 22);
			this.StatusURL.TabIndex = 5;
			this.StatusURL.Text = "Done.";
			// 
			// BtnDownload
			// 
			this.BtnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnDownload.Location = new System.Drawing.Point(438, 366);
			this.BtnDownload.Name = "BtnDownload";
			this.BtnDownload.Size = new System.Drawing.Size(64, 23);
			this.BtnDownload.TabIndex = 6;
			this.BtnDownload.Text = "Download";
			this.BtnDownload.Click += new System.EventHandler(this.BtnDownload_Click);
			// 
			// BtnAbortURL
			// 
			this.BtnAbortURL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnAbortURL.Location = new System.Drawing.Point(486, 16);
			this.BtnAbortURL.Name = "BtnAbortURL";
			this.BtnAbortURL.Size = new System.Drawing.Size(56, 23);
			this.BtnAbortURL.TabIndex = 7;
			this.BtnAbortURL.Text = "Abort";
			this.BtnAbortURL.Click += new System.EventHandler(this.BtnAbortURL_Click);
			// 
			// BtnAbortDL
			// 
			this.BtnAbortDL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnAbortDL.Location = new System.Drawing.Point(510, 366);
			this.BtnAbortDL.Name = "BtnAbortDL";
			this.BtnAbortDL.Size = new System.Drawing.Size(104, 23);
			this.BtnAbortDL.TabIndex = 8;
			this.BtnAbortDL.Text = "Stop Download";
			this.BtnAbortDL.Click += new System.EventHandler(this.BtnAbortDL_Click);
			// 
			// chkLinks
			// 
			this.chkLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkLinks.Location = new System.Drawing.Point(16, 366);
			this.chkLinks.Name = "chkLinks";
			this.chkLinks.TabIndex = 9;
			this.chkLinks.Text = "Grab Links Only";
			// 
			// button4
			// 
			this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button4.Location = new System.Drawing.Point(550, 16);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(64, 23);
			this.button4.TabIndex = 13;
			this.button4.Text = "Abort All";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2,
																					  this.menuItem5,
																					  this.menuItem6,
																					  this.menuItem7});
			this.menuItem1.Text = "File";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem3,
																					  this.menuItem4});
			this.menuItem2.Text = "Import";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.Text = "URL List...";
			this.menuItem3.Click += new System.EventHandler(this.button2_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "Files...";
			this.menuItem4.Click += new System.EventHandler(this.button3_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 1;
			this.menuItem5.Text = "Export...";
			this.menuItem5.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 2;
			this.menuItem6.Text = "-";
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 3;
			this.menuItem7.Text = "Exit";
			this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(630, 423);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.chkLinks);
			this.Controls.Add(this.BtnAbortDL);
			this.Controls.Add(this.BtnAbortURL);
			this.Controls.Add(this.BtnDownload);
			this.Controls.Add(this.StatusURL);
			this.Controls.Add(this.ListFiles);
			this.Controls.Add(this.ListPage);
			this.Controls.Add(this.BtnGo);
			this.Controls.Add(this.TxtURL);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Menu = this.mainMenu1;
			this.MinimumSize = new System.Drawing.Size(640, 432);
			this.Name = "Form1";
			this.Text = "Downloader";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private string GetWorkingDirectory(string address)
		{
			if (address.EndsWith("/"))
			{
				return address;
			}
			else if (address.LastIndexOf('/') < 7)
			{
				return address + "/";
			}
			else
			{
				return address.Substring(0, address.LastIndexOf('/') + 1);
			}
		}

		private void CheckForEmptyFolders(string folderName)
		{
			string [] Folders = Directory.GetDirectories(folderName);
			if (Folders.Length > 0)
			{
				foreach (string f in Folders)
				{
					if (f.ToLower().TrimEnd('\\') != @"c:\h-pics\repository")
						CheckForEmptyFolders(f);
				}
			}
			Folders = Directory.GetDirectories(folderName);
			if (Folders.Length == 0)
			{
				string [] Files = Directory.GetFiles(folderName);
				if (Files.Length > 0)
				{
					foreach (string FileName in Files)
					{
						if (FileName.ToUpper() == (folderName.ToUpper() + "\\THUMBS.DB"))
						{
							Console.WriteLine("Deleting " + FileName);
							File.Delete(FileName);
						}
						else if (!File.Exists(FolderDirectory + @"Repository\" + Path.GetFileName(FileName)))
						{
							Console.WriteLine("Moving " + FileName);
							File.Move(FileName, FolderDirectory + @"Repository\" + Path.GetFileName(FileName));
						}
						else
							for (int lcv = 1; lcv <= 20000; lcv++)
							{
								if (!File.Exists(FolderDirectory + @"Repository\" + Path.GetFileNameWithoutExtension(FileName) + lcv.ToString() + Path.GetExtension(FileName)))
								{
									File.Move(FileName, FolderDirectory + @"Repository\" + Path.GetFileNameWithoutExtension(FileName) + lcv.ToString() + Path.GetExtension(FileName));
									break;
								}
							}
					}
				}
				if (folderName.ToLower().StartsWith(@"c:\h-pics"))
					if (Directory.GetFiles(folderName).Length == 0)
						Directory.Delete(folderName, true);
			}
		}
		private string MakeDirectory(string address)
		{
			string [] temp = address.Split('/');
			string NewDirectory = this.FolderDirectory;
			try
			{
				for (int lcv = 2; lcv < temp.Length - 1; lcv++)
				{
					temp[lcv] = temp[lcv].Replace(":", "").Replace(".com", ".com.FOLDER");
					NewDirectory += temp[lcv] + @"\";
					if (File.Exists(NewDirectory))
					{
						try
						{
							File.Delete(NewDirectory);
						}
						catch
						{
						}
					}
					if (!Directory.Exists(NewDirectory))
						Directory.CreateDirectory(NewDirectory);
				}
			}
			catch (Exception ex)
			{
				try
				{
					NewDirectory.Remove(NewDirectory.LastIndexOf("\\") + 1, NewDirectory.Length - NewDirectory.LastIndexOf("\\"));
				}
				catch
				{
					NewDirectory = FolderDirectory;
				}
			}
			return NewDirectory;
		}

		private string GetFileName(string address)
		{
			return address.Remove(0, address.LastIndexOf('/'));
		}

		private void GrabLinks()
		{
			string address = CurrentAddress;
			StatusURL.Text = "Checking " + address + "...";
			this.Refresh();
			WebClient MyClient = new WebClient();
			string MyDirectory = this.GetWorkingDirectory(address);
			byte [] temp;
			Document = "";
			string [] lines;
			try
			{
				temp = MyClient.DownloadData(address);
				if (temp.Length > 0)
				{
					foreach (byte b in temp)
					{
						Document += (char)b;
					}
					lines = Document.Replace('\n', ' ').Split('<');
					lines[0] = ">";
					for (int lcv = 0; lcv < lines.Length; lcv++)
					{
						lines[lcv] = "<" + lines[lcv];
					}
					if (lines.Length > 1 && !chkLinks.Checked)
					{
						foreach (string s in lines)
						{
							try
							{
								if (s.ToLower().StartsWith("<a"))
								{

									bool parent = false;
									string [] MyStrings = s.Split('=');
									string S2 = null;
									for (int i = 0; i < MyStrings.Length; i++)
										if (MyStrings[i].Trim().ToLower().EndsWith("href"))
										{
											S2 = MyStrings[i+1].Trim();
											break;
										}
									S2 = S2.Trim('\"');
									S2 = S2.Split('\"')[0].Trim().Replace(" ","%20").Split('#')[0];
									while(S2.LastIndexOf("//") > 6)
									{
										S2 = S2.Remove(S2.LastIndexOf("//"),1);
									}
									while (S2.IndexOf("./") >= 0 && !S2.StartsWith("http"))
									{
										if (S2.IndexOf("../") == S2.IndexOf("./") - 1 && S2.IndexOf("../") >= 0 )
										{
											string tempDir = "";
											while (S2.IndexOf("../") >= 0)
											{
												tempDir = MyDirectory.TrimEnd('/');
												int loc = tempDir.LastIndexOf('/');
												int count = tempDir.Length - loc;
												tempDir = tempDir.Remove(loc, count) + '/';
												S2 = S2.TrimStart('\'');
												S2 = S2.TrimStart('.');
												S2 = S2.TrimStart('/');
											}

											S2 = tempDir + S2;
											parent = true;
										}
										else
										{
											S2 = S2.Replace("./", "");
										}
									}
									if (S2.ToLower().StartsWith("http"))
									{
										bool found = false;
										foreach (ListViewItem item in ListPage.Items)
										{
											if (S2 == item.Text)
												found = true;
										}
										if (!found)
										{
											string TheFile = S2.Substring(S2.LastIndexOf('/'), S2.Length - S2.LastIndexOf('/')).TrimStart('/');
											string TheFullExtension = Path.GetExtension(TheFile);
											string TheExtension = TheFullExtension.Split('#')[0].ToLower().Split('?')[0].TrimStart('.');
											if (((parent) || (S2.StartsWith(MyDirectory))) && (TheExtension == "shtml" || TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "asp"))
											{
												MyPage MP = new MyPage(S2, address);
												PageList.Add(MP);
												ListPage.Items.Add(S2);
											}
											else if (!S2.EndsWith("/") && !(TheExtension == "shtml" || TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi"|| TheExtension == "com" || TheExtension == "asp"))
												if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
													if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
                                                        if (ListFiles.FindItemWithText(S2) == null)
														    ListFiles.Items.Add(S2);
										}
									}
									else if (!(S2.ToLower().StartsWith("http")) && !(S2.ToLower().IndexOf(':') >= 0))
									{
										if (S2.StartsWith("/"))
										{
											S2 = address.Split('/')[0] + "//" + address.Split('/')[2] + S2;
										}
										else
											S2 = MyDirectory + S2;
										bool found = false;
										foreach (ListViewItem item in ListPage.Items)
										{
											if (S2 == item.Text)
												found = true;
										}
										if (!found)
										{
											string TheFile = S2.Substring(S2.LastIndexOf('/'), S2.Length - S2.LastIndexOf('/')).TrimStart('/');
											string TheFullExtension = Path.GetExtension(TheFile);
											string TheExtension = TheFullExtension.Split('#')[0].ToLower().Split('?')[0].TrimStart('.');
											if (((parent) || (S2.StartsWith(MyDirectory))) && (TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "asp"))
											{
												MyPage MP = new MyPage(S2, address);
												PageList.Add(MP);
												ListPage.Items.Add(S2);
											}
											else if (!S2.EndsWith("/") && !(TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "com" || TheExtension == "asp"))
												if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
													if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
													{
														
														if (ListFiles.FindItemWithText(S2) == null)
															ListFiles.Items.Add(S2);
													}
										}
									}
								}
								else if (s.ToLower().StartsWith("<img"))
								{
									string S2 = "";
									string [] S = s.Split('=');
									for (int LCV = 0; LCV < S.Length; LCV++)
									{
										if (S[LCV].Trim().ToLower().EndsWith("src"))
										{
											S2 = S[LCV + 1];
											break;
										}
									}								
									S2 = S2.Trim('\"');
									S2 = S2.Split('\"')[0].Trim().Split('#')[0];
									while(S2.LastIndexOf("//") > 6)
									{
										S2 = S2.Remove(S2.LastIndexOf("//"),1);
									}
									while (S2.IndexOf("./") >= 0)
									{
										if (S2.IndexOf("../") == S2.IndexOf("./") - 1 && S2.IndexOf("../") >= 0 )
										{
											string tempDir = "";
											while (S2.IndexOf("../") >= 0)
											{
												tempDir = MyDirectory.TrimEnd('/');
												int loc = tempDir.LastIndexOf('/');
												int count = tempDir.Length - loc;
												tempDir = tempDir.Remove(loc, count) + '/';
												S2 = S2.TrimStart('\'');
												S2 = S2.TrimStart('.');
												S2 = S2.TrimStart('/');
											}

											S2 = tempDir + S2;
										}
										else
										{
											S2 = S2.Replace("./", "");
										}
									}
									if (S2.StartsWith("http"))
									{
										bool found = false;
										foreach (ListViewItem item in ListFiles.Items)
										{
											if (S2 == item.Text)
												found = true;
										}
										if (!found && !S2.EndsWith("/"))
											if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
												if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
												{
                                                    if (ListFiles.FindItemWithText(S2) == null)
                                                        ListFiles.Items.Add(S2);
                                                }
									}
									else
									{
										S2 = S2.TrimStart('/');
										S2 = MyDirectory + S2;
										bool found = false;
										foreach (ListViewItem item in ListFiles.Items)
										{
											if (S2 == item.Text)
												found = true;
										}
										if (!found && !S2.EndsWith("/"))
											if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
												if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
												{
                                                    if (ListFiles.FindItemWithText(S2) == null)
                                                        ListFiles.Items.Add(S2);
                                                }
									} // end else
								}
								else if (s.ToLower().StartsWith("<frame "))
								{

									bool parent = false;
									string [] MyStrings = s.Split('=');
									string S2 = null;
									for (int i = 0; i < MyStrings.Length; i++)
										if (MyStrings[i].Trim().ToLower().EndsWith("src"))
										{
											S2 = MyStrings[i+1].Trim();
											break;
										}

									S2 = S2.Trim('\"');
									S2 = S2.Split('\"')[0].Trim().Replace(" ","%20").Split('#')[0];
									while(S2.LastIndexOf("//") > 6)
									{
										S2 = S2.Remove(S2.LastIndexOf("//"),1);
									}
									while (S2.IndexOf("./") >= 0)
									{
										if (S2.IndexOf("../") == S2.IndexOf("./") - 1 && S2.IndexOf("../") >= 0 )
										{
											string tempDir = "";
											while (S2.IndexOf("../") >= 0)
											{
												tempDir = MyDirectory.TrimEnd('/');
												int loc = tempDir.LastIndexOf('/');
												int count = tempDir.Length - loc;
												tempDir = tempDir.Remove(loc, count) + '/';
												S2 = S2.TrimStart('.');
												S2 = S2.TrimStart('/');
											}

											S2 = tempDir + S2;
											parent = true;
										}
										else
										{
											S2 = S2.Replace("./", "");
										}
									}
									if (S2.ToLower().StartsWith("http"))
									{
										bool found = false;
										foreach (ListViewItem item in ListPage.Items)
										{
											if (S2 == item.Text)
												found = true;
										}
										if (!found)
										{
											string TheFile = S2.Substring(S2.LastIndexOf('/'), S2.Length - S2.LastIndexOf('/')).TrimStart('/');
											string TheFullExtension = Path.GetExtension(TheFile);
											string TheExtension = TheFullExtension.Split('#')[0].ToLower().Split('?')[0].TrimStart('.');
											if (((parent) || (S2.StartsWith(MyDirectory))) && (TheExtension == "shtml" || TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "asp"))
											{
												MyPage MP = new MyPage(S2, address);
												PageList.Add(MP);
												ListPage.Items.Add(S2);
											}
											else if (!S2.EndsWith("/") && !(TheExtension == "shtml" || TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi"|| TheExtension == "com" || TheExtension == "asp"))
												if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
													if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
													{
                                                        if (ListFiles.FindItemWithText(S2) == null)
                                                            ListFiles.Items.Add(S2);
                                                    }
										}
									}
									else if (!(S2.ToLower().StartsWith("http")) && !(S2.ToLower().IndexOf(':') >= 0))
									{
										S2 = S2.TrimStart('/');
										S2 = MyDirectory + S2;
										bool found = false;
										foreach (ListViewItem item in ListPage.Items)
										{
											if (S2 == item.Text)
												found = true;
										}
										if (!found)
										{
											string TheFile = S2.Substring(S2.LastIndexOf('/'), S2.Length - S2.LastIndexOf('/')).TrimStart('/');
											string TheFullExtension = Path.GetExtension(TheFile);
											string TheExtension = TheFullExtension.Split('#')[0].ToLower().Split('?')[0].TrimStart('.');
											if (((parent) || (S2.StartsWith(MyDirectory))) && (TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "asp"))
											{
												MyPage MP = new MyPage(S2, address);
												PageList.Add(MP);
												ListPage.Items.Add(S2);
											}
											else if (!S2.EndsWith("/") && !(TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "com" || TheExtension == "asp"))
												if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
													if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
													{
                                                        if (ListFiles.FindItemWithText(S2) == null)
                                                            ListFiles.Items.Add(S2);
                                                    }
										}
									}
								}
							}
							catch
							{
							}
						}
					}
					if (lines.Length > 1 && chkLinks.Checked)
					{
						foreach (string s in lines)
						{
							if (s.ToLower().StartsWith("<a"))
							{

								bool parent = false;
								string [] MyStrings = s.Split('=');
								string S2 = null;
								for (int i = 0; i < MyStrings.Length; i++)
									if (MyStrings[i].Trim().ToLower().EndsWith("href"))
									{
										S2 = MyStrings[i+1].Trim();
										break;
									}
								S2 = S2.Trim('\"');
								S2 = S2.Split('\"')[0].Trim().Replace(" ","%20");
								while(S2.LastIndexOf("//") > 6)
								{
									S2 = S2.Remove(S2.LastIndexOf("//"),1);
								}
								while (S2.IndexOf("./") >= 0)
								{
									if (S2.IndexOf("../") == S2.IndexOf("./") - 1 && S2.IndexOf("../") >= 0 )
									{
										string tempDir = "";
										while (S2.IndexOf("../") >= 0)
										{
											tempDir = MyDirectory.TrimEnd('/');
											int loc = tempDir.LastIndexOf('/');
											int count = tempDir.Length - loc;
											tempDir = tempDir.Remove(loc, count) + '/';
											S2 = S2.TrimStart('.');
											S2 = S2.TrimStart('/');
										}

										S2 = tempDir + S2;
										parent = true;
									}
									else
									{
										S2 = S2.Replace("./", "");
									}
								}
								if (S2.ToLower().StartsWith("http"))
								{
									bool found = false;
									foreach (ListViewItem item in ListPage.Items)
									{
										if (S2 == item.Text)
										{
											found = true;
											break;
										}
									}
									foreach (ListViewItem item in ListFiles.Items)
									{
										if (S2 == item.Text)
										{
											found = true;
											break;
										}
									}
									if (!found)
									{
										string TheFile = S2.Substring(S2.LastIndexOf('/'), S2.Length - S2.LastIndexOf('/')).TrimStart('/');
										string TheFullExtension = Path.GetExtension(TheFile);
										string TheExtension = TheFullExtension.Split('#')[0].ToLower().Split('?')[0].TrimStart('.');
										if (((parent) || (S2.StartsWith(MyDirectory))) && (TheExtension == "shtml" || TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "php"))
										{
											MyPage MP = new MyPage(S2, address);
											PageList.Add(MP);
											ListPage.Items.Add(S2);
										}
											/*else if (!S2.EndsWith("/") && !(TheExtension == "shtml" || TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi"|| TheExtension == "com"))
												if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
													if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
														ListFiles.Items.Add(S2);*/
										else if (((!parent) && (!S2.StartsWith(MyDirectory))))
										{
                                            if (ListFiles.FindItemWithText(S2) == null)
                                                ListFiles.Items.Add(S2);
                                        }
									}
								}
								else if (!(S2.ToLower().StartsWith("http")) && !(S2.ToLower().IndexOf(':') >= 0))
								{
									S2 = S2.TrimStart('/');
									S2 = MyDirectory + S2;
									bool found = false;
									foreach (ListViewItem item in ListPage.Items)
									{
										if (S2 == item.Text)
											found = true;
									}
									if (!found)
									{
										string TheFile = S2.Substring(S2.LastIndexOf('/'), S2.Length - S2.LastIndexOf('/')).TrimStart('/');
										string TheFullExtension = Path.GetExtension(TheFile);
										string TheExtension = TheFullExtension.Split('#')[0].ToLower().Split('?')[0].TrimStart('.');
										if (((parent) || (S2.StartsWith(MyDirectory))) && (TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "asp"))
										{
											MyPage MP = new MyPage(S2, address);
											PageList.Add(MP);
											ListPage.Items.Add(S2);
										}
										/*else if (!S2.EndsWith("/") && !(TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "com"))
											if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
												if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
													ListFiles.Items.Add(S2);*/
									}
								}
							}
							/*else if (s.ToLower().StartsWith("<img"))
							{
								string S2 = "";
								string [] S = s.Split('=');
								for (int LCV = 0; LCV < S.Length; LCV++)
								{
									if (S[LCV].ToLower().EndsWith("src"))
									{
										S2 = S[LCV + 1];
										break;
									}
								}								
								S2 = S2.Trim('\"');
								S2 = S2.Split('\"')[0].Trim();
								while(S2.LastIndexOf("//") > 6)
								{
									S2 = S2.Remove(S2.LastIndexOf("//"),1);
								}
								if (S2.IndexOf("../") >= 0)
								{
									string tempDir = "";
									while (S2.IndexOf("../") >= 0)
									{
										tempDir = MyDirectory.TrimEnd('/');
										int loc = tempDir.LastIndexOf('/');
										int count = tempDir.Length - loc;
										tempDir = tempDir.Remove(loc, count) + '/';
										S2 = S2.TrimStart('.');
										S2 = S2.TrimStart('/');
									}

									S2 = tempDir + S2;
								}
								if (S2.StartsWith("http"))
								{
									bool found = false;
									foreach (ListViewItem item in ListFiles.Items)
									{
										if (S2 == item.Text)
											found = true;
									}
									if (!found && !S2.EndsWith("/"))
										if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
											if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
												ListFiles.Items.Add(S2);
								}
								else
								{
									S2 = MyDirectory + S2;
									bool found = false;
									foreach (ListViewItem item in ListFiles.Items)
									{
										if (S2 == item.Text)
											found = true;
									}
									if (!found && !S2.EndsWith("/"))
										if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
											if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
												ListFiles.Items.Add(S2);
								} // end else
							}
							else if (s.ToLower().StartsWith("<frame"))
							{

								bool parent = false;
								string [] MyStrings = s.Split('=');
								string S2 = null;
								for (int i = 0; i < MyStrings.Length; i++)
									if (MyStrings[i].ToLower().EndsWith("src"))
									{
										S2 = MyStrings[i+1].Trim();
										break;
									}

								S2 = S2.Trim('\"');
								S2 = S2.Split('\"')[0].Trim().Replace(" ","%20");
								while(S2.LastIndexOf("//") > 6)
								{
									S2 = S2.Remove(S2.LastIndexOf("//"),1);
								}
								while (S2.IndexOf("./") >= 0)
								{
									if (S2.IndexOf("../") == S2.IndexOf("./") - 1 && S2.IndexOf("../") >= 0 )
									{
										string tempDir = "";
										while (S2.IndexOf("../") >= 0)
										{
											tempDir = MyDirectory.TrimEnd('/');
											int loc = tempDir.LastIndexOf('/');
											int count = tempDir.Length - loc;
											tempDir = tempDir.Remove(loc, count) + '/';
											S2 = S2.TrimStart('.');
											S2 = S2.TrimStart('/');
										}

										S2 = tempDir + S2;
										parent = true;
									}
									else
									{
										S2 = S2.Replace("./", "");
									}
								}
								if (S2.ToLower().StartsWith("http"))
								{
									bool found = false;
									foreach (ListViewItem item in ListPage.Items)
									{
										if (S2 == item.Text)
											found = true;
									}
									if (!found)
									{
										string TheFile = S2.Substring(S2.LastIndexOf('/'), S2.Length - S2.LastIndexOf('/')).TrimStart('/');
										string TheFullExtension = Path.GetExtension(TheFile);
										string TheExtension = TheFullExtension.Split('#')[0].ToLower().Split('?')[0].TrimStart('.');
										if (((parent) || (S2.StartsWith(MyDirectory))) && (TheExtension == "shtml" || TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi"))
										{
											MyPage MP = new MyPage(S2, address);
											PageList.Add(MP);
											ListPage.Items.Add(S2);
										}
										else if (!S2.EndsWith("/") && !(TheExtension == "shtml" || TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi"|| TheExtension == "com"))
											if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
												if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
													ListFiles.Items.Add(S2);
									}
								}
								else if (!(S2.ToLower().StartsWith("http")) && !(S2.ToLower().IndexOf(':') >= 0))
								{
									S2 = MyDirectory + S2;
									bool found = false;
									foreach (ListViewItem item in ListPage.Items)
									{
										if (S2 == item.Text)
											found = true;
									}
									if (!found)
									{
										string TheFile = S2.Substring(S2.LastIndexOf('/'), S2.Length - S2.LastIndexOf('/')).TrimStart('/');
										string TheFullExtension = Path.GetExtension(TheFile);
										string TheExtension = TheFullExtension.Split('#')[0].ToLower().Split('?')[0].TrimStart('.');
										if (((parent) || (S2.StartsWith(MyDirectory))) && (TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi"))
										{
											MyPage MP = new MyPage(S2, address);
											PageList.Add(MP);
											ListPage.Items.Add(S2);
										}
										else if (!S2.EndsWith("/") && !(TheExtension == "htm" || TheExtension == "html" || TheExtension == "php" || TheExtension == "cgi" || TheExtension == "com"))
											if (S2.LastIndexOf("/") < S2.LastIndexOf("."))
												if (S2.LastIndexOf("#") < S2.LastIndexOf("/"))
													ListFiles.Items.Add(S2);
									}
								}
							}*/
						}
					}
				}
				ArrayList PageCopy = new ArrayList();
				foreach (ListViewItem lvi in ListPage.Items)
				{
					if (lvi.Text == CurrentAddress)
					{
						lvi.BackColor = Color.White;
						lvi.ForeColor = Color.Navy;
						break;
					}
				}
				foreach (MyPage item in PageList)
				{
					if (item.RefPage == address && !item.visited)
					{
						item.visited = true;
						PageCopy.Add(item);
					}
					else
					{
					}
				}
				foreach (MyPage item in PageCopy)
				{
					CurrentAddress = item.URL;
					foreach (ListViewItem item2 in ListPage.Items)
						if (item2.Text == CurrentAddress)
						{
							ListPage.SelectedItems.Clear();
							item2.BackColor = Color.Gray;
							break;
						}
					this.GrabLinks();
				}
			}
			catch (Exception ex)
			{
			}
			finally
			{
				StatusURL.Text = "Done! (" + ListPage.Items.Count.ToString() + " items)";
				Thread.Sleep(0);
			}
		}


		private void Importing()
		{
			openFileDialog1 = new OpenFileDialog();
			openFileDialog1.FileName = "*.txt";
			openFileDialog1.Filter = "Text Document (*.txt)|*.txt|All Files(*.*)|*.*";
			openFileDialog1.DefaultExt = "txt";
			chkLinks.Enabled = false;
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				System.IO.StreamReader sr = new StreamReader(openFileDialog1.FileName.Trim());
				BriefURLs = new StreamWriter(openFileDialog1.FileName.Trim() + ".log", true);
				string s = sr.ReadToEnd();
				sr.Close();
				string [] addresses = s.Split('\n');
				if (addresses.Length > 0)
				{
					TxtURL.Enabled = false;
					this.ListFiles.Items.Clear();
					foreach(string S2 in addresses)
					{
						
						TxtURL.Text = S2.Trim();
						PresentWorkingDirectory = GetWorkingDirectory(TxtURL.Text.Trim());

						this.ListPage.Items.Clear();
					
						MyPage MainPage = new MyPage(TxtURL.Text.Trim(), TxtURL.Text.Trim());
						MainPage.visited = true;
						PageList.Add(MainPage);
						ListPage.Items.Add(TxtURL.Text.Trim());
						CurrentAddress = TxtURL.Text.Trim();
						try
						{
							if (ThreadPage.ThreadState == (ThreadPage.ThreadState | ThreadState.Suspended) 
								|| ThreadPage.ThreadState == (ThreadPage.ThreadState | ThreadState.Running)
								|| ThreadPage.ThreadState == (ThreadPage.ThreadState | ThreadState.WaitSleepJoin))
								ThreadPage = new Thread (new ThreadStart(GrabLinks));
							ThreadPage.Start();
						}
						catch (ThreadStateException ex)
						{
							StatusURL.Text = "Suspended";
						}
						ThreadPage.Join();
						if (ListPage.Items.Count <= 5)
						{
							BriefURLs.WriteLine(S2.Trim());
						}
					}
					TxtURL.Enabled = true;
				}
			}
			chkLinks.Enabled = true;
		}

		private bool AddToDownloadedLog(string address)
		{
			if (LogFileRead.Contains(address))
				return false;
			else
			{
				return true;
			}
		}
		private void button1_Click(object sender, System.EventArgs e)
		{
			PresentWorkingDirectory = GetWorkingDirectory(TxtURL.Text.Trim());

			this.ListPage.Items.Clear();
			this.ListFiles.Items.Clear();
			MyPage MainPage = new MyPage(TxtURL.Text.Trim(), TxtURL.Text.Trim());
			MainPage.visited = true;
			PageList.Add(MainPage);
			ListPage.Items.Add(TxtURL.Text.Trim());
			CurrentAddress = TxtURL.Text.Trim();
			try
			{
				if (ThreadPage.ThreadState == (ThreadPage.ThreadState | ThreadState.Suspended) 
					|| ThreadPage.ThreadState == (ThreadPage.ThreadState | ThreadState.Running)
					|| ThreadPage.ThreadState == (ThreadPage.ThreadState | ThreadState.WaitSleepJoin)
					|| ThreadPage.ThreadState == (ThreadPage.ThreadState | ThreadState.Aborted))
					ThreadPage = new Thread (new ThreadStart(GrabLinks));
				ThreadPage.Start();
			}
			catch (ThreadStateException ex)
			{
				StatusURL.Text = "Suspended";
			}
			Thread.Sleep(0);
		}

		private void DownloadFile()
		{
			WebClient MyClient = new WebClient();
			int Counter = 1;
			foreach( ListViewItem item in ListFiles.Items)
			{
				item.BackColor = Color.Gray;
				if (this.AddToDownloadedLog(item.Text))
				{
					StatusURL.Text = "Downloading " + item.Text + " (" + Counter + " of " + ListFiles.Items.Count + ")...";
					this.Refresh();
					string FileName = GetFileName(item.Text);
					string DownloadDirectory = MakeDirectory(item.Text);
					try
					{
						if (!File.Exists(DownloadDirectory + FileName))
							MyClient.DownloadFile(item.Text, DownloadDirectory + FileName);
					}
					catch
					{
					}
					finally
					{
					}
					LogFileRead.Add(item.Text);
					LogFileWrite.WriteLine(item.Text);
				}
				Counter++;
				item.ForeColor = Color.Navy;
				item.BackColor = Color.White;
			}
			StatusURL.Text = "Done!";
			this.CheckForEmptyFolders(FolderDirectory);
			Thread.Sleep(0);
		}

		private void BtnDownload_Click(object sender, System.EventArgs e)
		{
			if (ThreadDownload.ThreadState == (ThreadDownload.ThreadState | ThreadState.Suspended) 
				|| ThreadDownload.ThreadState == (ThreadDownload.ThreadState | ThreadState.Running)
				|| ThreadDownload.ThreadState == (ThreadDownload.ThreadState | ThreadState.WaitSleepJoin)
				|| ThreadDownload.ThreadState == (ThreadDownload.ThreadState | ThreadState.Aborted))
				ThreadDownload = new Thread(new ThreadStart(this.DownloadFile));
			ThreadDownload.Start();
			Thread.Sleep(0);
		}

		private void BtnAbortURL_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.ThreadPage.Abort();
			}
			catch
			{
			}
			finally
			{
				StatusURL.Text = "Aborted.";
				TxtURL.Enabled = true;
				chkLinks.Enabled = true;
			}
		}

		private void BtnAbortDL_Click(object sender, System.EventArgs e)
		{
			this.ThreadDownload.Abort();
			StatusURL.Text = "Download aborted.";
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{	
				if (ThreadDownload != null)
					ThreadDownload.Abort();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			try
			{
				if (ThreadPage != null)
				{
					this.ThreadPage.Abort();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			try
			{
				if (ThreadImport != null)
					this.ThreadImport.Abort();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			LogFileWrite.Close();
			if (BriefURLs != null)
			{
				BriefURLs.Close();
			}
		}

		private void button1_Click_1(object sender, System.EventArgs e)
		{
			saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.FileName = "*.txt";
			saveFileDialog1.Filter = "Text Document (*.txt)|*.txt|All Files(*.*)|*.*";
			saveFileDialog1.DefaultExt = "txt";

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				try
				{
					if (File.Exists(saveFileDialog1.FileName.Trim()))
						File.Delete(saveFileDialog1.FileName.Trim());

					System.IO.StreamWriter sw = new StreamWriter(saveFileDialog1.FileName.Trim(), true);
					foreach (ListViewItem lvi in ListFiles.Items)
					{
						sw.WriteLine(lvi.Text);
					}
					sw.Close();
				}
				catch
				{
					MessageBox.Show("File Failed to write", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			if (ThreadImport.ThreadState == (ThreadImport.ThreadState | ThreadState.Suspended) 
				|| ThreadImport.ThreadState == (ThreadImport.ThreadState | ThreadState.Running)
				|| ThreadImport.ThreadState == (ThreadImport.ThreadState | ThreadState.WaitSleepJoin))
				ThreadImport = new Thread (new ThreadStart(Importing));
			ThreadImport.Start();
			Thread.Sleep(0);
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			openFileDialog1 = new OpenFileDialog();
			openFileDialog1.FileName = "*.txt";
			openFileDialog1.Filter = "Text Document (*.txt)|*.txt|All Files(*.*)|*.*";
			openFileDialog1.DefaultExt = "txt";
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				System.IO.StreamReader sr = new StreamReader(openFileDialog1.FileName.Trim());
				string s = sr.ReadToEnd();
				sr.Close();
				string [] addresses = s.Replace("\r", "").Split('\n');
				if (addresses.Length > 0)
				{
					this.ListFiles.Items.Clear();
					foreach(string S2 in addresses)
					{
                        if (S2.Length > 4)
                        {
                            
                            if (ListFiles.FindItemWithText(S2) == null)
                                ListFiles.Items.Add(S2);
                        }
					}
					StatusURL.Text = "Done! (" + ListFiles.Items.Count.ToString() + " items)";
				}
			}
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.ThreadImport.Abort();
			}
			catch
			{
				MessageBox.Show("Could not abort Import Thread", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			try
			{
				this.ThreadPage.Abort();
			}
			catch
			{
				MessageBox.Show("Could not abort Page Thread", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			try
			{
				this.ThreadDownload.Abort();
			}
			catch
			{
				MessageBox.Show("Could not abort Download Thread", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			if (BriefURLs != null)
			{
				BriefURLs.Close();
			}
			TxtURL.Enabled = true;
			chkLinks.Enabled = true;
		}

		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void ListFiles_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			int Location;
			if (e.KeyData == Keys.Delete)
			{
				if (ListFiles.SelectedItems.Count == 1)
				{
					Location = ListFiles.SelectedIndices[0];
				}
				else
					Location = -1;
				if (Location >= 0)
				{
					ListFiles.Items.Remove(ListFiles.SelectedItems[0]);
					if (Location == ListFiles.Items.Count && ListFiles.Items.Count > 0)
						ListFiles.Items[Location - 1].Selected = true;
					else if (ListFiles.Items.Count > 0)
						ListFiles.Items[Location].Selected = true;
				}
			}
		}

		private void ListFiles_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
