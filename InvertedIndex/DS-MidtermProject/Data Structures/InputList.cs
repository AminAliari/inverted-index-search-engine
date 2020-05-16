using System.Collections.Generic;

namespace DS_MidtermProject {
    class InputList <T> {
    
        int pointer = 0;
        List<T> s = new List<T>();

        public void add (T data) {
            s.Add(data);
            pointer = s.Count;
        }

        public T up() {
            if (pointer > 0) {
                --pointer;
                return s[pointer];
            }
            return default(T);
        }

        public T down() {
            if (pointer + 1 < s.Count) {
                ++pointer;
                return s[pointer];
            }
            return default(T);
        }
    }
}
