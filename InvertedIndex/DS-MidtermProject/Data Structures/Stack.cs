using System.Collections.Generic;

namespace DS_MidtermProject {
    class Stack<T> {

        List<T> s = new List<T>();
        T temp;

        public void push (T data) {
            s.Add(data);
        }

        public T pop () {
            if (s.Count > 0) {
                temp = s[s.Count - 1];
                s.RemoveAt(s.Count - 1);
                return temp;
            }
            return default(T);
        }

        public T peek () {
            if (s.Count > 0) return s[s.Count - 1];
            return default(T);
        }

        public bool isEmpty () {
            return s.Count == 0;
        }
    }
}
