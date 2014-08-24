using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vocals
{
    [Serializable]
    public class Profile
    {
        public string name;
        public List<Command> commandList;

        public Profile() {
        }

        public Profile(string name){
            this.name = name;
            commandList = new List<Command>();
        }

        public void addCommand(Command c){
            commandList.Add(c);
        }

        ~Profile(){
        }

        public override string ToString()
        {
            return this.name;
        }
    
      
    }
}
