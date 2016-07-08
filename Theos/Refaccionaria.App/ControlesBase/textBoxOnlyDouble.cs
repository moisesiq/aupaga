using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public class textBoxOnlyDouble : TextBox
    {
        protected sealed override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {   
            //allows 0-9, backspace, and decimal
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }            
            //checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if (this.Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
            base.OnKeyPress(e);
        }
    }
}
