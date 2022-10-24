using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace JSONExtrator
{
    public partial class jsonForm : Form
    {
        public jsonForm()
        {
            InitializeComponent();
        }

        private void FormatBtn_Click(object sender, EventArgs e)
        {
            try
            {
                JToken parsedJson = JToken.Parse(entryRichTextBox.Text);
                var beautified = parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);
                entryRichTextBox.Text = beautified;
            }
            catch
            {
                MessageBox.Show("Invalid Json");
            }
            
        }

        private void Minimise_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                JToken parsedJson = JToken.Parse(entryRichTextBox.Text);
                var minified = parsedJson.ToString(Newtonsoft.Json.Formatting.None);
                entryRichTextBox.Text = minified;
            }
            catch
            {
                MessageBox.Show("Invalid Json");
            }
        }

        private sealed class IndexContainer
        {
            private int _n;
            public int Inc() => _n++;
        }

        private void FillTreeView(TreeNode node, JToken tok, Stack<IndexContainer> s)
        {
            if (tok.Type == JTokenType.Object)
            {
                TreeNode n = node;
                if (tok.Parent != null)
                {
                    if (tok.Parent.Type == JTokenType.Property)
                    {
                        n = node.Nodes.Add($"{((JProperty)tok.Parent).Name} <{tok.Type.ToString()}>");
                    }
                    else
                    {
                        n = node.Nodes.Add($"[{s.Peek().Inc()}] <{tok.Type.ToString()}>");
                    }
                }
                s.Push(new IndexContainer());
                foreach (var p in tok.Children<JProperty>())
                {
                    FillTreeView(n, p.Value, s);
                }
                s.Pop();
            }
            else if (tok.Type == JTokenType.Array)
            {
                TreeNode n = node;
                if (tok.Parent != null)
                {
                    if (tok.Parent.Type == JTokenType.Property)
                    {
                        n = node.Nodes.Add($"{((JProperty)tok.Parent).Name} <{tok.Type.ToString()}>");
                    }
                    else
                    {
                        n = node.Nodes.Add($"[{s.Peek().Inc()}] <{tok.Type.ToString()}>");
                    }
                }
                s.Push(new IndexContainer());
                foreach (var p in tok)
                {
                    FillTreeView(n, p, s);
                }
                s.Pop();
            }
            else
            {
                var name = string.Empty;
                var value = JsonConvert.SerializeObject(((JValue)tok).Value);

                if (tok.Parent.Type == JTokenType.Property)
                {
                    name = $"{((JProperty)tok.Parent).Name} : {value}";
                }
                else
                {
                    name = $"[{s.Peek().Inc()}] : {value}";
                }

                node.Nodes.Add(name);
            }
        }





        private void Populate_btn_Click(object sender, EventArgs e)
        {
            jsonTreeView.Nodes.Clear();
            FillTreeView(jsonTreeView.Nodes.Add("ROOT"), JToken.Parse(entryRichTextBox.Text), new Stack<IndexContainer>());
        }

        private void Extract_Btn_Click(object sender, EventArgs e)
        {
            extractedRichTextBox.Text = "";
            
            foreach(var entry in JToken.Parse(entryRichTextBox.Text))
            {
                try
                {
                    extractedRichTextBox.Text += entry[extractRichTextBox.Text] + "\n";
                }
                catch
                {

                }
            }    
            
        }
    }
}