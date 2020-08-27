using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using CarPlayList;

namespace FlashList
{
	public partial class MainForm : Form
	{
        private List<FileItem> items = new List<FileItem>();
        private BindingSource filesBinding = new BindingSource();

		public MainForm()
		{
			InitializeComponent();
		}

		void ListBox1MouseDown(object sender, MouseEventArgs e)
		{
            if (lbFileList.SelectedItem != null)
                lbFileList.DoDragDrop((/*FileInfo*/FileItem)lbFileList.SelectedItem, DragDropEffects.All);
		}
		
		void ListBox1DragDrop(object sender, DragEventArgs e)
		{
            int index = lbFileList.IndexFromPoint(lbFileList.PointToClient(new Point(e.X, e.Y)));
            FileItem drag_item = (FileItem)e.Data.GetData(typeof(FileItem));

            if (index >= 0)
            {
                filesBinding.Remove(drag_item);
                filesBinding.Insert(index, drag_item);
            }
        }

        private void lbFileList_DragOver(object sender, DragEventArgs e)
        {
            lbFileList.SelectedIndex = lbFileList.IndexFromPoint(lbFileList.PointToClient(new Point(e.X, e.Y)));
        }
		
		void ListBox1DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(FileItem))) 
				e.Effect = DragDropEffects.Move;
			else
				e.Effect = DragDropEffects.None;
		}

        private void CreateFileList(string[] files)
        {
            items.Clear();
            Array.Sort(files);

            foreach (String fname in files)
                items.Add(new FileItem { FileName = Path.GetFileName(fname), FullPath = fname });
            filesBinding.ResetBindings(false);
        }

        private void bClose_Click(object sender, EventArgs e) => Application.Exit();

        private void bSelectDirectory_Click(object sender, EventArgs e)
        {
            filesBinding.DataSource = items;
            lbFileList.DataSource = filesBinding;
            lbFileList.DisplayMember = "FileName";
            lbFileList.ValueMember = "FileName";

            if (fbgDialog.ShowDialog() == DialogResult.OK)
            {
                CreateFileList(Directory.GetFiles(fbgDialog.SelectedPath));
                bSaveNames.Enabled = lbFileList.Items.Count > 0;
            }
        }

        private void bSaveNames_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= items.Count - 1; i++)
            {
                FileItem item = items[i];

                string OldPath = item.FullPath;
                string Dir = Path.GetDirectoryName(OldPath);

                item.FileName = item.FileName.TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '.', ',', ' ', '_');
                item.FileName = $"{i + 1:D2}_{item.FileName}";
                item.FullPath = $"{Dir}{Path.DirectorySeparatorChar}{item.FileName}";

                File.Move(OldPath, item.FullPath);
            }
            filesBinding.ResetBindings(false);
        }
    }
}
