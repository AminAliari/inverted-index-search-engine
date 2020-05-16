using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_MidtermProject {

    public class IndexNode {

        public string word;
        public List<string> files = new List<string>();

        public IndexNode(string word,string filename) {
            this.word = word;
            addFile(filename);
        }

        public void addFile(string filename) {
            if (!files.Contains(filename)) {
                files.Add(filename);
            }
        }

        internal int CompareTo(IndexNode s2) {
            return word.CompareTo(s2.word);
        }
    }
}
