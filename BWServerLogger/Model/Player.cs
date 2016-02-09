using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BWServerLogger.Util;

namespace BWServerLogger.Model
{
    public class Player : BaseDatabase
    {
        private string playerName;

        public string Name
        {
            get
            {
                return playerName;
            }
            set
            {
                if (TagUtil.HasClanTag(value))
                {
                    HasClanTag = true;
                }
               playerName = TagUtil.StripTags(value);
            }
        }

        public bool HasClanTag
        {
            get;
            set;
        }

        public Player() : base()
        {
            HasClanTag = false;
        }

        public Player(string name) : this()
        {
            this.Name = name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
