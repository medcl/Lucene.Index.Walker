using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Lucene.Index.Walker
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			result = new Result();
			result.Dock = DockStyle.Fill;
			this.splitContainer1.Panel2.Controls.Add(result);

			this.folderBrowserDialog1.SelectedPath = Application.StartupPath;
		}

		private Result result;
		private Directory directory;
		
		private void OToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				directory = FSDirectory.GetDirectory(folderBrowserDialog1.SelectedPath,false);
			}

			if (directory != null)
				if(Lucene.Net.Index.IndexReader.IndexExists(directory))
				{
					Lucene.Net.Index.IndexReader indexReader = Lucene.Net.Index.IndexReader.Open(directory);

					var docRootNode = new TreeNode(folderBrowserDialog1.SelectedPath); //async loading

					treeView1.Nodes.Clear();

					treeView1.Nodes.Add(docRootNode);

					var docNum= indexReader.NumDocs();
					toolStripLabel3.Text="Version:"+indexReader.GetVersion()+" DocNum:" + docNum.ToString();

				
					var fieldNames = indexReader.GetFieldNames(Lucene.Net.Index.IndexReader.FieldOption.ALL);


					var docRoot = docRootNode.Nodes.Add("Docs (" + docNum + ")");

					for (int i = 0; i < docNum; i++)
					{
						var doc=indexReader.Document(i);
						var tempDoc= docRoot.Nodes.Add("Index"+i+" ("+doc.GetFieldsCount().ToString()+")");
						IEnumerable fields = doc.Fields();
						tempDoc.Tag = doc;
						foreach (object field in fields)
						{
							tempDoc.Nodes.Add(field.ToString());
						}
					}


					var fieldRoot = docRootNode.Nodes.Add("Fields (" + fieldNames.Count + ")");
					foreach (System.Collections.DictionaryEntry name in fieldNames)
					{
						fieldRoot.Nodes.Add(name.Key.ToString());
					}
					//	listBox1.Items.Add("最大文档数:"+);
			
				}
		}

		private void searchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (directory != null)
			{
				IndexSearcher indexSearcher=new IndexSearcher(directory);
				QueryParser queryParser = new QueryParser("Name",new StandardAnalyzer());
				if (!string.IsNullOrEmpty(toolStripTextBox1.Text))
				{
					var q = queryParser.Parse(toolStripTextBox1.Text);
					var docs= indexSearcher.Search(q);

					result.AppendLine(string.Format("search result,query:{0},total hits:{1}",q,docs.Length()));

					for (int i = 0; i < docs.Length(); i++)
					{
						result.AppendLine(string.Format("hit:{0},doc:{1}",i+1,docs.Doc(i)));
					}
				}
			}
		}

		private void toolStripTextBox1_Click(object sender, EventArgs e)
		{
			toolStripTextBox1.Text = string.Empty;
		}

		
		private void getDuplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SortedDictionary<string, int> _numDocOfFileds;
			var duplicateName = new HashSet<string>();
			var fieldName = Microsoft.VisualBasic.Interaction.InputBox("please input the field to be compared with.", "Before Comparing", "", 50, 50);
			if(!string.IsNullOrEmpty(fieldName) )
			{

				_numDocOfFileds=new SortedDictionary< string,int>();
				if (treeView1.Nodes != null&&treeView1.Nodes.Count>0)
					foreach (TreeNode variable in treeView1.Nodes[0].Nodes)
					{
						foreach (TreeNode var in variable.Nodes)
						{
							var document = var.Tag as Document;

							if (document != null)
							{
								var field = document.GetField(fieldName);
								int value;
								if (field != null)
									if (_numDocOfFileds.TryGetValue(field.StringValue(),out value))
									{
										_numDocOfFileds[field.StringValue()] = value+ 1;
										duplicateName.Add(field.StringValue());//add to duplicate list
									}
									else
									{
										_numDocOfFileds[field.StringValue()] =  1;
									}

//							foreach (Lucene.Net.Documents.Field field in document.Fields())
//							{
//								if(field.StringValue()==varField)
//								{
//									
//								}
//							}
							}
							//	_numDocOfFileds[value.]
						}
					}
				
				if(duplicateName.Count>0)
				{
					result.AppendLine(string.Format("{0} duplicate index with filed:{1} was found", duplicateName.Count,fieldName));
					foreach (var variable in duplicateName)
				{
					result.AppendLine(string.Format("field value:{0},doc num:{1}", variable,_numDocOfFileds[variable].ToString()));					
				}}
				else
				{
					result.AppendLine(string.Format("no duplicate index with filed:{0} was found", fieldName));
				}

			}
		}
	}
}
