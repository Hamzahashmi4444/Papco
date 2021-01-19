using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLAS.Controllers
{
    class ListItem
    {
        public string CompIdLI { get; set; }
        public string CompCodeLI { get; set; }
        public string CapacityLI { get; set; }
        public string OrderedQtyLI { get; set; }
        public string PlannedQtyLI { get; set; }
        public string ActualQtyLI { get; set; }
        public string ManualQtyLI { get; set; }

        public string PreDip { get; set; }
        public string PostDip { get; set; }
        public string Delta { get; set; }
        public bool isCreatedLI { get; set; }
    }
}
