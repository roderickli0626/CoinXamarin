﻿using mycoin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.ViewModels
{
    public class MySubstanceViewModel
    {
        private MySubstance _substance;

        public MySubstanceViewModel(MySubstance substance)
        {
            this._substance = substance;
        }

        public string SubstanceName { get { return _substance.SubstanceName; } }
        public string SubstanceImageUrl { get { return _substance.SubstanceImageUrl; } }
        public int ID { get { return _substance.ID; } }
        
    }
}
