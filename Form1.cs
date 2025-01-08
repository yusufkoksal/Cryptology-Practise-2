namespace EsiCrypto3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.Show();
        }

        public void AddControlToForm(Control control)
        {
            this.Controls.Add(control);
        }
    }
}

