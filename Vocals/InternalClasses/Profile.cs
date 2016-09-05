using System;
using System.Collections.Generic;

namespace Vocals
{
    [Serializable]
    public class Profile
    {
        public string Name;
        public List<Command> CommandList;

        public Profile() {
        }

        public Profile(string name){
            this.Name = name;
            CommandList = new List<Command>();
        }

        public void AddCommand(Command c){
            CommandList.Add(c);
        }

        ~Profile(){
        }

        public override string ToString()
        {
            return this.Name;
        }
    
      
    }
}
