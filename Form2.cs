namespace EsiCrypto3
{
    public partial class Form2 : Form
    {
        private Form1 mainForm;

        public Form2(Form1 form1)
        {
            InitializeComponent();
            mainForm = form1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string selectedComponent = comboBoxComponents.SelectedItem.ToString();
            int x = int.Parse(textBoxX.Text);
            int y = int.Parse(textBoxY.Text);

            Control control = null;
            switch (selectedComponent)
            {
                case "Button":
                    control = new System.Windows.Forms.Button();
                    control.Text = "Button";
                    break;
                case "Label":
                    control = new Label();
                    control.Text = "Label";
                    break;
                case "DataGrid":
                    control = new DataGridView();
                    control.Text = "DataGridView";
                    break;

            }

            if (control != null)
            {
                control.Location = new System.Drawing.Point(x, y);
                mainForm.AddControlToForm(control);
            }
        }
    }
}
