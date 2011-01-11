using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lucene.Index.Walker
{
	public partial class Result : UserControl
	{
		private SortedDictionary<string, int> _dataSource;

		private Inspector _inspector=new Inspector();

		public Result()
		{
			InitializeComponent();
		}

		public void AppendLine(string line)
		{
			listBox1.Items.Add(line);
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listBox1.Items.Clear();
		}

		private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			_inspector.SendMessage(listBox1.SelectedItem.ToString());
			_inspector.ShowDialog();
		}
	}
}
