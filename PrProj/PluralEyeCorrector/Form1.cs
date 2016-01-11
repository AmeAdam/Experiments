using System;
using System.Windows.Forms;
using MyXml;
using MyXml.PrProj;
using MyXml.PrProjXmlCommands;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private readonly PremiereProjectLoader _premiereLoader;
        private PremiereProject _project;
        public EffectRequest EffectRequest;
        private PrProjFile prProj;

        public Form1(PremiereProjectLoader premiereLoader)
        {
            _premiereLoader = premiereLoader;
            InitializeComponent();
        }

        private void BtnSelectFileClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                AddFile(openFileDialog1.FileName);
        }

        private void AddFile(string file)
        {
            if (file.EndsWith(".prproj", StringComparison.InvariantCultureIgnoreCase))
            {
                tbPrProj.Text = file;
                //prProj = new PrProjFile(file);
                //cbEffectName.Items.Clear();
                _project = _premiereLoader.Load(file);
                EffectRequest = new EffectRequest(_project);

                cbEffectName.DataSource = EffectRequest.GetAllEffectsNames();
                var sequneces = _project.GetAll<Sequence>();
                sequneces.Insert(0, new Sequence{Key = null, Name = "[All Sequences]"});
                cbSequences.DataSource = sequneces;
                cbSequences.DisplayMember = "Name";
                cbSequences.ValueMember = "Key";
            }
            else if (file.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                tbXml.Text = file;
        }

        private void BtnAnalizujClick(object sender, EventArgs e)
        {
            var proj = new PrProjFile(tbPrProj.Text);
            proj.Analize();

            var xml = new XmlFile(tbXml.Text);
            xml.Analize();

            listBox1.Items.Clear();
            foreach (var c in proj.Clips)
                listBox1.Items.Add(c.Key);

            listBox2.Items.Clear();
            foreach (var c in xml.Clips)
            {
                listBox2.Items.Add(c.Name);
                proj.Update(c.Name, c.Start, c.End);
            }
            proj.Save();
        }

        private void Form1DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) 
                AddFile(file);
        }

        private void Form1DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void BtnDisableEffectClick(object sender, EventArgs e)
        {
            EffectRequest.ChangeEnabledEffectState(cbEffectName.Text, true, GetSelectedSequence());
            _project.Save(tbPrProj.Text);
        }

        private void BtnEnableEffectClick(object sender, EventArgs e)
        {
            EffectRequest.ChangeEnabledEffectState(cbEffectName.Text, false, GetSelectedSequence());
            _project.Save(tbPrProj.Text);
        }

        private void bRemoveEffect_Click(object sender, EventArgs e)
        {
            EffectRequest.RemoveEffects(cbEffectName.Text, GetSelectedSequence());
            _project.Save(tbPrProj.Text);
        }

        private void bSetTop_Click(object sender, EventArgs e)
        {
            EffectRequest.SetEffectTop(cbEffectName.Text, GetSelectedSequence());
            _project.Save(tbPrProj.Text);
        }

        private int? GetSelectedSequence()
        {
            var key = (PremiereObjectKey) cbSequences.SelectedValue;
            if (key == null)
                return null;
            return key.ObjectID;
        }
    }
}
