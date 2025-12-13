using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgramLab
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void DichotomyButton_Click(object sender, EventArgs e)
        {
          Dichotomy secondForm = new Dichotomy();
          secondForm.Show();
        }

        private void Gold_Click(object sender, EventArgs e)
        {
          
           GoldenRatio secondForm = new GoldenRatio();
           secondForm.Show();
            
        }

        private void SLAU_Click(object sender, EventArgs e)
        {
            SLAU secondForm = new SLAU();
            secondForm.Show();  
        }

        private void Sort_Click(object sender, EventArgs e)
        {
            Sort secondForm = new Sort();
            secondForm.Show();
        }

        private void Newton_Click(object sender, EventArgs e)
        {
            Newton secondForm = new Newton();   
            secondForm.Show();  
        }

        private void MNK_Click(object sender, EventArgs e)
        {
            MNK secondForm = new MNK();
            secondForm.Show();
        }

        private void Intedral_Click(object sender, EventArgs e)
        {
            Integral secondForm = new Integral(); 
            secondForm.Show();
        }
    }
}
