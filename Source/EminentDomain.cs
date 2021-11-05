using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.UI;
using UnityEngine;
using EminentDomain.Source;
//using System.Reflection;


namespace EminentDomain
{
    public class EminentDomain : IUserMod
    {
        public string Name
        {
            get { return "Eminent Domain"; }
        }

          
        public string Description
        {
            get { return "Eminent Domain costs for demolition"; }
        }

    }

}
