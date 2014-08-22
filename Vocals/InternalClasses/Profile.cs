using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voice_Defense
{
    [Serializable]
    public class Profile
    {
        public string name;
        public List<Command> commandList;

        public Profile(string name){
            this.name = name;
            commandList = new List<Command>();
        }

        public void addCommand(string commandString, List<Actions> actionList){
            commandList.Add(new Command(commandString, actionList));
        }

        ~Profile(){
        }

        public override string ToString()
        {
            return this.name;
        }
    
      
    }
}
