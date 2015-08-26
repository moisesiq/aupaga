using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Refaccionaria.App
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
                                       ToolStripItemDesignerAvailability.ContextMenuStrip |
                                       ToolStripItemDesignerAvailability.StatusStrip)]
    public class ButtonStripItem : ToolStripControlHost
    {
        private Button button;

        public ButtonStripItem()
            : base(new Button())
        {
            this.button = this.Control as Button;
        }

        // Add properties, events etc. you want to expose...

        public FlatStyle FlatStyle { get { return this.button.FlatStyle; } set { this.button.FlatStyle = value; } }
        public FlatButtonAppearance FlatAppearance { get { return this.button.FlatAppearance; } }
        // public bool TabStop { get { return this.button.TabStop; } set { this.button.TabStop = value; } }
        // public int TabIndex { get { return this.button.TabIndex; } set { this.button.TabIndex = value; } }
    }
}
